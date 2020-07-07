/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#include "stdafx.h"
#include "ScannerSDKSampleApp.h"
#include "RSMDlg.h"
#include "LogsDlg.h"

#include "QuickXmlParser.h"

IMPLEMENT_DYNAMIC(CRSMDlg, CDialog)

CRSMDlg::CRSMDlg(CWnd* pParent ) : CDialog(CRSMDlg::IDD, pParent)
{
	Async = 0;
}

CRSMDlg::~CRSMDlg()
{
}

void CRSMDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LIST1, RsmGridControl);
}

void CRSMDlg::SetScannerID(std::wstring *ScannerID)
{
	SelectedScannerID = *ScannerID;
}

void CRSMDlg::SetAsync(int *ParaA)
{
	Async = *ParaA;
}
/***
	Standard MFC message map
***/
BEGIN_MESSAGE_MAP(CRSMDlg, CDialog)
	ON_BN_CLICKED(IDC_BUTTON1, &CRSMDlg::OnGetAllAttributeIDs)
	ON_BN_CLICKED(IDC_BUTTON2, &CRSMDlg::OnGetAttributeInfo)
	ON_BN_CLICKED(IDC_BUTTON3, &CRSMDlg::OnGetNextAttributeInfo)
	ON_BN_CLICKED(IDC_BUTTON4, &CRSMDlg::OnSetAttribute)
	ON_BN_CLICKED(IDC_BUTTON5, &CRSMDlg::OnStoreAttribute)
	ON_BN_CLICKED(IDC_BUTTON6, &CRSMDlg::OnSelectAll)
	ON_BN_CLICKED(IDC_BUTTON7, &CRSMDlg::OnClearAll)
	ON_WM_CTLCOLOR()
END_MESSAGE_MAP()

/***
	Retrieves all the RSM attribute IDs into the "ID" column of the RSM list control.
***/
void CRSMDlg::OnGetAllAttributeIDs()
{
	CHECK_CMD0;

	long status = 1;
	CComBSTR outXml(L"");
	//Issue the RSM command RSM_ATTR_GETALL.
	if ( !SC->cmdGetAll(SelectedScannerID, &outXml, Async, &status) )
	{
		if ( !Async )
		{
			//Update the log tab and set the RSM grid entries if it's a synchronous command.
			GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);
			SetGridIDs(outXml);
		}
	}
	LOG(status, "ATTR_GETALL");
}

/***
	Initializes the RSM List control header column text. This is called from OnInitDialog().
***/
void CRSMDlg::InitAttribList()
{
	RsmGridControl.SetHeader(_T("ID"), 100);
	RsmGridControl.SetHeader(_T("Type"), 50);
	RsmGridControl.SetHeader(_T("Property"), 60);
	RsmGridControl.SetHeader(_T("Value"), 200);
}
/***
	Retrieves the RSM  Type, Property, value records for each comma serperated attribute ID 
***/
void CRSMDlg::OnGetAttributeInfo()
{
	CHECK_CMD0;
	//Get the comma serperated string of attribute IDs
	wstring AttributeList;
	MakeSelectedRsmIdList(AttributeList);
	long status = 1;
	CComBSTR outXml(L"");
	//Issue the RSM command RSM_ATTR_GET.
	if ( !SC->cmdGet(SelectedScannerID, AttributeList, &outXml, Async, &status) )
	{
		if ( !Async )
		{
			//Update the log tab and set the RSM grid entries  if it's a synchronous command.
			GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);
			SetGridAttributes(outXml);
		}
	}
	LOG(status, "ATTR_GET");
}
/***
	Walks the RSM grid control and retrieves all the attribute IDs corresponding to
	the selected rows. It then creates a comma serperated list in AttributeList.
***/
void CRSMDlg::MakeSelectedRsmIdList(wstring &AttributeList)
{
	AttributeList = L"";
	POSITION p = RsmGridControl.GetFirstSelectedItemPosition();
	while ( p )
	{
		int nSelected = RsmGridControl.GetNextSelectedItem(p);
		AttributeList.append(RsmGridControl.GetItemText(nSelected,0));

		if ( p ) AttributeList.append(L",");
	}
}

void CRSMDlg::OnGetNextAttributeInfo()
{
	CHECK_CMD0;

	long status = 1;
	if ( RsmGridControl.GetSelectedCount()>1 )
	{
		TCHAR str[] = _TEXT("Get Next Attribute does not Support Multiple Select");
		::MessageBox(this->m_hWnd, str, _T("Information"), MB_OK | MB_ICONINFORMATION);
	}
	else
	{
		wstring AttributeList;
		MakeSelectedRsmIdList(AttributeList);

		BSTR outXml=::SysAllocString(L"");
		if ( !SC->cmdGetNext(SelectedScannerID, AttributeList, &outXml, Async, &status) )
		{
			if ( !Async )
			{
				GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);
				SetGridAttributes(outXml);
			}
		}
		LOG(status, "ATTR_GETNEXT");
	}
}

/***
	Creates the XML fragment	<attribute>
									<id> ... </id>
									<datatype> ... </datatype>
									<value> ... </value>
								</attribute>

	by walking all the selected rows in the RSM grid. This is necessary when issuing
	RSM_ATTR_SET and RSM_ATTR_STORE commands.
***/
void CRSMDlg::MakeIdValueAttributeXml(wstring &AttributeList)
{
	AttributeList = L"";
	POSITION p = RsmGridControl.GetFirstSelectedItemPosition();
	while ( p )
	{
		//Get the row index of the selected RSM Grid row
		int nSelected = RsmGridControl.GetNextSelectedItem(p);
		//Get the RSM attribute ID
		CString AttribID = RsmGridControl.GetItemText(nSelected, 0);
		//Make the tag <attribute><id>Rsm ID</id><datatype>
		AttributeList.append(L"<attribute><id>");
		AttributeList.append(AttribID);
		AttributeList.append(L"</id><datatype>");
		//If the attribute IDs correspond to non-actionable attributes, then add
		//them as a valid data type.
		if ( AttribID != "6000" && AttribID != "6001" && AttribID != "6003")
		{
			AttributeList.append(RsmGridControl.GetItemText(nSelected, 1));
		}
		else
		{
			AttributeList.append(L"X");
		}
		//completes the tag 
		//<attribute><id>Rsm ID</id><datatype>RSM Type</datatype><value>RSM Value</value></attribute> 
		AttributeList.append(L"</datatype><value>");
		AttributeList.append(RsmGridControl.GetItemText(nSelected, 3));
		AttributeList.append(L"</value></attribute>");
	}
}
/***
	Issues the RSM_ATTR_SET command.
***/
void CRSMDlg::OnSetAttribute()
{
	CHECK_CMD0;

	long status = 1;
	wstring AttributeList;
	MakeIdValueAttributeXml(AttributeList);
	SC->cmdSet(SelectedScannerID, AttributeList, Async, &status);
	LOG(status, "RSM_ATTR_SET");
}
/***
	Issues the RSM_ATTR_STORE command.
***/
void CRSMDlg::OnStoreAttribute()
{
	CHECK_CMD0;

	long status = 1;
	wstring AttributeList;
	MakeIdValueAttributeXml(AttributeList);
	SC->cmdStore(SelectedScannerID, AttributeList, Async, &status);
	LOG(status, "RSM_ATTR_STORE");
}
/***
	Selects all the entries in the RSM Grid - equilavent to Ctrl+A.
***/
void CRSMDlg::OnSelectAll()
{
	int NumItems = RsmGridControl.GetItemCount();

	for ( int i = 0; i < NumItems; i++ )
	{
		RsmGridControl.SetItemState(i, LVIS_SELECTED, LVIS_SELECTED | LVIS_FOCUSED);
		RsmGridControl.SetFocus();
	}
}
/***
	Clears all entries from the RSM Grid after stopping any editing operation in progress.
***/
void CRSMDlg::OnClearAll()
{
	RsmGridControl.StopEdit();
	RsmGridControl.DeleteAllItems();
}
/***
	Dialog specific functions.
***/
BOOL CRSMDlg::OnInitDialog()
{
	CDialog::OnInitDialog();
	InitAttribList();
	return TRUE;  
}

HBRUSH CRSMDlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
	if(!m_brush.m_hObject)
		m_brush.CreateSolidBrush(RGB(247, 250, 253));
	return m_brush;
}
/***
	Called by OnGetAllAttributeIDs() and the asyn equilavent of that function
	SetGridFromAsyncResponse() to populate the first column of the RSM Grid
	with attribute IDs.
***/
bool CRSMDlg::SetGridIDs(BSTR outXml)
{
	RsmGridControl.DeleteAllItems(); //empty the RSM Grid

	CQuickXmlParser x(outXml);
	CQuickXmlParser::TAGDATA tag[1] = {0};
	tag[0].Tag.Name = L"attribute"; //Query for the attribute ID
	x.Configure(tag, 1);
	CQuickXmlParser::xptr p = 0;

	while(1)
	{
		p = x.Parse(p);
		RsmGridControl.SetField(0, x.Translate(tag[0].Value));
		if(p == 0) break;
		x.ClearValues(tag);
	}

	RsmGridControl.Sort();
	RsmGridControl.SetItemState(0, LVIS_SELECTED, LVIS_SELECTED | LVIS_FOCUSED);
	RsmGridControl.SetFocus();
	return true;
}

void CRSMDlg::SetGridAttributes(BSTR outXml)
{
	int *Indices = RsmGridControl.GetSelectedIndices();
	
	CQuickXmlParser x(outXml);
	CQuickXmlParser::TAGDATA tag[4] = {0};
	tag[0].Tag.Name = L"id";
	tag[1].Tag.Name = L"datatype";
	tag[2].Tag.Name = L"permission";
	tag[3].Tag.Name = L"value";

	x.Configure(tag, 4);
	CQuickXmlParser::xptr p = 0;

	while(1)
	{
		p = x.Parse(p);

		int ID = _ttoi(x.Translate(tag[0].Value));

		for(int i = 0; Indices[i] != -1; ++i)
		{
			//Get the current/next selected ID in the Grid.
			int index = Indices[i];
			int CurrentSelID = _ttoi(RsmGridControl.GetItemText(index, 0).GetBuffer());
			//If the selected ID in the Grid is less than the current ID found in the XML,
			//Navigate to that row in the Grid where they are equal.
			while(CurrentSelID < ID) 
			{	
				CurrentSelID = _ttoi(RsmGridControl.GetItemText(++index, 0).GetBuffer());
			}
			//Set the values to the Grid control for the matching ID in the XML 
			//and and XML values fetched.
			if(CurrentSelID == ID)
			{
				RsmGridControl.SetItemText(index, 1, x.Translate(tag[1].Value));
				RsmGridControl.SetItemText(index, 2, x.Translate(tag[2].Value));
				RsmGridControl.SetItemText(index, 3, x.Translate(tag[3].Value));
				break;
			}
		}

		if(p == 0) break;
		x.ClearValues(tag);
	}
}
/***
	This function is called from outside of this RSM dialog code. When the Corescanner
	command dispatch mode is set to Asynchronous, any dialog method that issues a RSM
	specific command (eg: OnGetAllAttributeIDs(), OnGetAttributeInfo() etc) will execute
	and return immediately. The command response/results are then asynchronously fired
	as an event CommandResponseEvent() which needs to be implemented by the client. This
	function is called from within CommandResponseEvent().
***/
bool CRSMDlg::SetGridFromAsyncResponse(BSTR outXml)
{
	GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);

	CQuickXmlParser x(outXml);
	CQuickXmlParser::TAGDATA tag[1] = {0};
	tag[0].Tag.Name = L"opcode";

	x.Configure(tag, 1);
	CQuickXmlParser::xptr p = 0;

	p = x.Parse(p);
	std::wstring OpCode = x.Translate(tag[0].Value);
	if ( OpCode == L"5000" ) //Signifies that only attribute IDs have been returned.
	{
		SetGridIDs(outXml);
	}
	else if ( OpCode == L"5002" || OpCode == L"5001" )
	{
		SetGridAttributes(outXml);
	}
	return true;
}
