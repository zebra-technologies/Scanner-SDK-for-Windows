/*******************************************************************************************
*
* ©2016 Symbol Technologies LLC. All rights reserved.
*
********************************************************************************************/
#include "stdafx.h"
#include "CommonFunctions.h"
#include "ScannerSDKSampleAppDlg.h"
#include "EventSink.h"

#include "ScannerActionDlg.h"
#include "ScaleDlg.h"
#include "IntelDocCap.h"
#include "BarcodeDlg.h"
#include "ConfigurationDlg.h"
#include "ImageVideoDlg.h"
#include "MiscellaneousDlg.h"
#include "RSMDlg.h"
#include "LogsDlg.h"
#include <exception>
#include "QuickXmlParser.h"
#include "ScanToConnectDlg.h"

#include "ScannerCommands.h"
#include <sstream>

#ifdef _DEBUG
	#define new DEBUG_NEW
#endif

CScannerSDKSampleAppDlg::CScannerSDKSampleAppDlg(CWnd* pParent /*=NULL*/)
: CDialog(CScannerSDKSampleAppDlg::IDD, pParent)

{
	
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	m_ScannerCommands = 0;
	m_OpenAndRegistered = false;
}

CScannerSDKSampleAppDlg::~CScannerSDKSampleAppDlg()
{
	if(m_ScannerCommands)
	{
		delete m_ScannerCommands;
		m_ScannerCommands = 0;
	}
}

void CScannerSDKSampleAppDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX,	IDC_TAB1,		m_TabWndMgr					);
	DDX_Control(pDX,	IDC_LIST1,		m_ScannerListControl		);
	DDX_Control(pDX,	IDC_EDIT1,		txtStatus					);
	DDX_Control(pDX,	IDC_BUTTON3,	btnDiscover					);
	DDX_Control(pDX,	IDC_BUTTON6,	btnPullTri					);
	DDX_Control(pDX,	IDC_BUTTON7,	btnRelTri					);
	DDX_Control(pDX,	IDC_COMBO1,		cmbScanner					);
	DDX_Text   (pDX,	IDC_EDIT2,		txtScannerSummary			);
}

BEGIN_MESSAGE_MAP(CScannerSDKSampleAppDlg, CDialog)
	//Windows message map
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_WM_CTLCOLOR()
	ON_NOTIFY(LVN_ITEMCHANGED, IDC_LIST1, &CScannerSDKSampleAppDlg::OnListItemchanged)
	//Command maps
	ON_BN_CLICKED(IDC_BUTTON3, &CScannerSDKSampleAppDlg::OnDiscoverScanners)
	ON_BN_CLICKED(IDC_BUTTON6, &CScannerSDKSampleAppDlg::OnPullTrigger)
	ON_BN_CLICKED(IDC_BUTTON7, &CScannerSDKSampleAppDlg::OnReleaseTrigger)

	ON_CBN_SELCHANGE(IDC_COMBO1, &CScannerSDKSampleAppDlg::OnSelectChangeScannerCombo)
	ON_NOTIFY(TCN_SELCHANGE, IDC_TAB1, &CScannerSDKSampleAppDlg::OnTcnSelchangeTab1)
END_MESSAGE_MAP()

BOOL CScannerSDKSampleAppDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	/**
		Create the CoreScanner COM object before creating the Tab dialogs below. This is
		necessary to ensure that any tab dialog init routines that depend on the COM object
		are executed without fail (eg: SetLocaleConfigInfo() in CBarcodeDlg)
	**/
	m_ScannerCommands = new CScannerCommands(this);
	//Create the Tab Dialogs and fire their OnInitDialog
	m_TabWndMgr.SetTabClass<CBarcodeDlg>(this);
	m_TabWndMgr.SetTabClass<CImageVideoDlg>(this);
	m_TabWndMgr.SetTabClass<CIntelDocCap>(this);
	m_TabWndMgr.SetTabClass<CScannerActionDlg>(this);
	m_TabWndMgr.SetTabClass<CRSMDlg>(this);
	m_TabWndMgr.SetTabClass<CConfigurationDlg>(this);
	m_TabWndMgr.SetTabClass<CScanToConnectDlg>(this);
	m_TabWndMgr.SetTabClass<CMiscellaneousDlg>(this);
	m_TabWndMgr.SetTabClass<CScaleDlg>(this);
	m_TabWndMgr.SetTabClass<CLogsDlg>(this);

	GetTabManager().ShowTab<CBarcodeDlg>();

	InitScannerListControl();
	Async = 0;

	return TRUE; 
}

void CScannerSDKSampleAppDlg::OnPaint()
{
	if ( IsIconic() )
	{
		CPaintDC dc(this); 
		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

HCURSOR CScannerSDKSampleAppDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

void CScannerSDKSampleAppDlg::OnScanData(short reserved, BSTR scanData)
{
	GetTabManager().GetTabDlg<CBarcodeDlg>().SetBarcode(scanData);
	LOG(-1, "Barcode Event Fired");
	GetTabManager().ShowTab<CBarcodeDlg>();
}

void CScannerSDKSampleAppDlg::OnImageEvent(LPBYTE MediaBuffer, LONG BufferSize)
{
	GetTabManager().GetTabDlg<CImageVideoDlg>().OnImageCapture(MediaBuffer, BufferSize);
	LOG(-1, "Image Event Fired");
}

void CScannerSDKSampleAppDlg::ParameterBarcodeEvent(LPBYTE MediaBuffer, LONG BufferSize)
{
	GetTabManager().GetTabDlg<CScanToConnectDlg>().OnParameterBarcode(MediaBuffer, BufferSize);
	LOG(-1, "Image Event Fired");
}

void CScannerSDKSampleAppDlg::OnBinaryDataEvent(LPBYTE MediaBuffer, LONG BufferSize, SHORT DataFormat,  BSTR* bstrScannerData)
{
	GetTabManager().GetTabDlg<CIntelDocCap>().OnBinaryDataEventCapture(MediaBuffer, BufferSize, DataFormat, bstrScannerData);
}

void CScannerSDKSampleAppDlg::OnVideoEvent(LPBYTE MediaBuffer, LONG BufferSize)
{
	GetTabManager().GetTabDlg<CImageVideoDlg>().OnVideoCapture(MediaBuffer, BufferSize);
}

void CScannerSDKSampleAppDlg::OnCmdResponse(short status, BSTR scanCmdResponse)
{
	GetTabManager().GetTabDlg<CRSMDlg>().SetGridFromAsyncResponse(scanCmdResponse);
}

void CScannerSDKSampleAppDlg::OnPNP(short eventType, BSTR PnpXml)
{
	if ( eventType == 0 )
	{
		AddScannersOnPnP(PnpXml);
		LOG(-1, "Scanner Attached Event Fired");
	}
	else
	{
		RemoveScannerOnPnP(PnpXml);
		LOG(-1, "Scanner Detached Event Fired");
	}

	GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(PnpXml);
	UpdateScannerStatus();
	GetTabManager().GetTabDlg<CRSMDlg>().OnClearAll();
}

void CScannerSDKSampleAppDlg::OnNotificationEvent(short notificationType,BSTR pScannerData)
{
	switch(notificationType)
	{
		case BARCODE_MODE:
			LOG(-1, "Scanner Notification : Barcode Mode");
			break;
		case IMAGE_MODE:
			LOG(-1, "Scanner Notification : Image Mode");
			break;
		case VIDEO_MODE:
			LOG(-1, "Scanner Notification : Video Mode");
			break;
		case DEVICE_ENABLED:
			LOG(-1, "Scanner Notification : Device Enabled");
			break;
		case DEVICE_DISABLED:
			LOG(-1, "Scanner Notification : Divice Disabled");
			break;
	}
	GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(pScannerData);
}

void CScannerSDKSampleAppDlg::OnRMD(short eventType, BSTR eventData)
{
	GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(eventData);
	GetTabManager().GetTabDlg<CConfigurationDlg>().OnRMDEvent(eventType, eventData);
}

void CScannerSDKSampleAppDlg::OnIOResponse(short type, unsigned char data)
{
	LOG(-1, "Scanner IO Event Fired.");
}

void CScannerSDKSampleAppDlg::ClearAll()
{
	m_ScannerListControl.DeleteAllItems();
}

SCANNER* CScannerSDKSampleAppDlg::GetScannerInfo(std::wstring ScannerID)
{
	int ID = _wtoi(ScannerID.c_str());
	SCANNER scanner;
	if(m_ScannerMap.Lookup(ID, scanner))
		return &(m_ScannerMap[ID]);
	return 0;
}


void CScannerSDKSampleAppDlg::UpdateScannerStatus()
{
#pragma warning(disable:4311; disable:4312)

	int ScannerCount = (int)m_ScannerMap.GetCount();
	txtScannerSummary.Format(_T("Scanner Count=%d"), ScannerCount);
	CMapStringToOb ScannerMap;

	CObject *count = 0;
	int val;
	int ID;
	SCANNER sc;
	POSITION pos = m_ScannerMap.GetStartPosition();
	while(pos != NULL)
	{
		m_ScannerMap.GetNextAssoc(pos, ID, sc);
		if(ScannerMap.Lookup(sc.Type, count) > 0)
		{
			val = (int)count;
			ScannerMap.SetAt(sc.Type, (CObject*)(val + 1));
		}
		else
			ScannerMap.SetAt(sc.Type, (CObject*)(1));
	}

	if(ScannerMap.GetCount() > 0)
	{
		CString Status, Temp;
		pos = ScannerMap.GetStartPosition();
		while(pos != NULL)
		{
			ScannerMap.GetNextAssoc( pos, sc.Type, count );
			Temp.Format(_T("   |   %s=%d"), sc.Type, (int)count);
			Status += Temp;
		}
		txtScannerSummary += Status;
	}
	UpdateData(0);

#pragma warning(default:4311; disable:4312)
}

bool CScannerSDKSampleAppDlg::TranslateProtocolNames(SCANNER& Scanner, CString& TranslatedName, wchar_t *Name)
{
	Scanner.HostMode = MODE_ALL;

	if(*Name == 0) return false;
	int Protocol = GetTabManager().GetTabDlg<CConfigurationDlg>().QueryScannerProtocol();
	
	if(wcsncmp(Name, L"USBIBMHID", 9) == 0)
	{
		if(Protocol == SCANNER_TYPES_IBMHID || Protocol == SCANNER_TYPES_ALL)
		{
			TranslatedName = L"IBM HANDHELD";
			Scanner.HostMode = MODE_IBMHID;
			return true;
		}
		return false;
	}
	if(wcsncmp(Name, L"USBIBMTT", 8) == 0)
	{
		if(Protocol == SCANNER_TYPES_IBMTT || Protocol == SCANNER_TYPES_ALL)
		{
			TranslatedName = L"IBM TABLETOP";
			Scanner.HostMode = MODE_IBMTT;
			return true;
		}
		return false;
	}

	if(wcsncmp(Name, L"USBHIDKB", 8) == 0)
	{
		if(Protocol == SCANNER_TYPES_HIDKB || Protocol == SCANNER_TYPES_ALL)
		{
			TranslatedName = L"HID KEYBOARD";
			Scanner.HostMode = MODE_HIDKB;
			return true;
		}
		return false;
	}

	if(wcsncmp(Name, L"SNAPI", 5) == 0)
	{
		if(Protocol == SCANNER_TYPES_SNAPI || Protocol == SCANNER_TYPES_ALL)
		{
			TranslatedName = L"SNAPI";
			Scanner.HostMode = MODE_SNAPI_IMG;
			return true;
		}
		return false;
	}

	if(wcsncmp(Name, L"SSI_BT", 6) == 0)
	{
		if(Protocol == SCANNER_TYPES_SSI_BT || Protocol == SCANNER_TYPES_ALL)
		{
			TranslatedName = L"SSI_BT";
			Scanner.HostMode = MODE_SSI_BT;
			return true;
		}
		return false;
	}

	if(wcsncmp(Name, L"SSI", 3) == 0)
	{
		if(Protocol == SCANNER_TYPES_SSI || Protocol == SCANNER_TYPES_ALL)
		{
			TranslatedName = L"SSI";
			Scanner.HostMode = MODE_SSI;
			return true;
		}
		return false;
	}

	if(wcsncmp(Name, L"USBOPOS", 3) == 0)
	{
		if(Protocol == SCANNER_TYPES_OPOS || Protocol == SCANNER_TYPES_ALL)
		{
			TranslatedName = L"USBOPOS";
			Scanner.HostMode = MODE_USB_OPOS;
			return true;
		}
		return false;
	}

	TranslatedName = Name;
	return true;
}

bool CScannerSDKSampleAppDlg::ShowScanners(BSTR outXml)
{
	CQuickXmlParser x(outXml);

	CQuickXmlParser::TAGDATA tag[9] = {0};
	tag[0].Tag.Name = L"scanner";
	tag[0].Attribs[0].Name = L"type";
	tag[1].Tag.Name = L"scannerID";
	tag[2].Tag.Name = L"serialnumber";
	tag[3].Tag.Name = L"GUID";
	tag[4].Tag.Name = L"VID";
	tag[5].Tag.Name = L"PID";
	tag[6].Tag.Name = L"modelnumber";
	tag[7].Tag.Name = L"firmware";
	tag[8].Tag.Name = L"DoM";

	x.Configure(tag, 9);

	CQuickXmlParser::xptr p = 0;
	m_ScannerListControl.DeleteAllItems();
	m_ScannerMap.RemoveAll();

	wchar_t *val = 0;
	while(1)
	{
		p = x.Parse(p);
		SCANNER sc;

		val = x.Translate(tag[0].AttribValues[0]); //Scanner Type
		CString sType;
		if(!TranslateProtocolNames(sc, sType, val)) 
		{
			if(p == 0) break;
			x.ClearValues(tag);
			continue;
		}
		
		val = x.Translate(tag[1].Value); //Scanner ID
		sc.ID = _ttoi(val);
		m_ScannerListControl.SetField(0, val);

		m_ScannerListControl.SetField(1, (LPTSTR)sType.GetBuffer()); //Set Scanner type here
		sc.Type = sType;

		val = x.Translate(tag[6].Value); //Scanner Model Number
		sc.Model = val;
		m_ScannerListControl.SetField( 2, val); 

 		val = x.Translate(tag[7].Value); //Firmware
		sc.Firmware = val;
		m_ScannerListControl.SetField( 3, val);

		val = x.Translate(tag[8].Value); //Date of manufacture
		sc.DoM = val;
		m_ScannerListControl.SetField( 4, val);

		val =  x.Translate(tag[2].Value); //Serial Number
		sc.Serial = val;
		m_ScannerListControl.SetField( 5, val);

		val = x.Translate(tag[3].Value); //GUID
		sc.GUID = val;
		m_ScannerListControl.SetField( 6, val); 
		m_ScannerMap[sc.ID] = sc;
		
		if(p == 0) break;
		x.ClearValues(tag);
	}

	m_ScannerListControl.SetItemState(0, LVIS_SELECTED, LVIS_SELECTED | LVIS_FOCUSED);
	m_ScannerListControl.EnsureVisible(0, TRUE);

	SynchScannerCombo();
	return true;

}
void CScannerSDKSampleAppDlg::RefreshScannerList()
{
	m_ScannerListControl.DeleteAllItems();

	TCHAR Buf[256];
	int ID;
	SCANNER sc;
	POSITION pos = m_ScannerMap.GetStartPosition();
	while(pos != NULL)
	{
		m_ScannerMap.GetNextAssoc(pos, ID, sc);

		_itot_s(sc.ID, Buf, 256, 10);
		m_ScannerListControl.SetField( 0, Buf);							//#
		m_ScannerListControl.SetField( 1, (LPTSTR)sc.Type.GetBuffer()); //Com Interface
		m_ScannerListControl.SetField( 2, sc.Model.GetBuffer());		//Model #
		m_ScannerListControl.SetField( 3, sc.Firmware.GetBuffer());		//Firmware
		m_ScannerListControl.SetField( 4, sc.DoM.GetBuffer());			//Built
		m_ScannerListControl.SetField( 5, sc.Serial.GetBuffer());		//Serial # or Port #
		m_ScannerListControl.SetField( 6, sc.GUID.GetBuffer());			//GUID
	}

	m_ScannerListControl.SetItemState(0, LVIS_SELECTED, LVIS_SELECTED | LVIS_FOCUSED);
	m_ScannerListControl.EnsureVisible(0, TRUE);

	UpdateScannerClaimedStatus();
	UpdateDisabledScannerStatus();
}

bool CScannerSDKSampleAppDlg::AddScannersOnPnP(BSTR outXml)
{
	CQuickXmlParser x(outXml);

	CQuickXmlParser::TAGDATA tag[7] = {0};
	tag[0].Tag.Name = L"scanner";
	tag[0].Attribs[0].Name = L"type";
	tag[1].Tag.Name = L"scannerID";
	tag[2].Tag.Name = L"serialnumber";
	tag[3].Tag.Name = L"GUID";
	tag[4].Tag.Name = L"VID";
	tag[5].Tag.Name = L"PID";
	tag[6].Tag.Name = L"modelnumber";

	x.Configure(tag, 7);
	CQuickXmlParser::xptr p = 0;

	wchar_t *val = 0;
	while(1)
	{
		p = x.Parse(p);
		SCANNER sc;

		val = x.Translate(tag[0].AttribValues[0]); //Scanner Type
		CString sType;
		if(!TranslateProtocolNames(sc, sType, val)) 
		{
			if(p == 0) break;
			x.ClearValues(tag);
			continue;
		}
		
		wchar_t* IdVal = x.Translate(tag[1].Value);
		sc.ID = _ttoi(IdVal);
		RsmMap map;
		map[535] = L"";
		map[20004] = L"";

		QueryRsmValues(IdVal, map);

		sc.Type = sType;
		sc.Model = x.Translate(tag[6].Value);
		sc.Serial = x.Translate(tag[2].Value);
		sc.GUID = x.Translate(tag[3].Value);
		sc.Firmware = map[20004].c_str();
		sc.DoM = map[535].c_str();

		m_ScannerMap[sc.ID] = sc;
		
		if(p == 0) break;
		x.ClearValues(tag);
	}

	RefreshScannerList();
	SynchScannerCombo();
	return true;
}

bool CScannerSDKSampleAppDlg::QueryRsmValues(std::wstring ScannerID, RsmMap &RsmValueMap)
{
	//Create a comma serperated list of attribute IDs
	std::wstringstream sAttribList;
	POSITION startpos = RsmValueMap.GetStartPosition();
	POSITION pos = startpos;

	if(startpos == NULL) return false;

	int Key;
	std::wstring Value;
	while (pos != NULL)
	{
		if(pos != startpos) sAttribList << L",";
		RsmValueMap.GetNextAssoc( pos, Key, Value );
		
		sAttribList << Key;
	}

	long status = 1;
	BSTR outXml = ::SysAllocString(L"");

	if(SC->cmdGet(ScannerID, sAttribList.str(), &outXml, 0, &status)) return false;

	CQuickXmlParser xml(outXml);
	CQuickXmlParser::TAGDATA tag[2] = {0};

	tag[0].Tag.Name = L"id";
	tag[1].Tag.Name = L"value";

	xml.Configure(tag, 2);
	CQuickXmlParser::xptr p = 0;

	while(1)
	{
		p = xml.Parse(p);

		int AttributeID = _wtoi(xml.Translate(tag[0].Value));
		RsmValueMap[AttributeID] = xml.Translate(tag[1].Value);

		if(p == 0) break;
		xml.ClearValues(tag);
	}
	return true;
}

bool CScannerSDKSampleAppDlg::RemoveScannerOnPnP(BSTR outXml)
{
	CQuickXmlParser x(outXml);
	CQuickXmlParser::TAGDATA tag[2] = {0};

	tag[0].Tag.Name = L"scannerID";
	tag[1].Tag.Name = L"pnp";

	x.Configure(tag, 2);
	CQuickXmlParser::xptr p = 0;

	wchar_t *val = 0;
	while(1)
	{
		p = x.Parse(p);
		SCANNER sc;

		int ScannerID = _ttoi(x.Translate(tag[0].Value));
		int ID;
		POSITION pos = m_ScannerMap.GetStartPosition();
		while(pos != NULL)
		{
			m_ScannerMap.GetNextAssoc(pos, ID, sc);
			if(sc.ID == ScannerID)
			{
				wchar_t* PnP = tag[1].Value.Name;
				if(PnP)
				{
					if(PnP[0] == L'0' ) break;
				}

				m_ScannerMap.RemoveKey(ID);
				break;
			}
		}
		if(p == 0) break;
		x.ClearValues(tag);
	}

	RefreshScannerList();
	SynchScannerCombo();
	return true;
}
/*
bool CScannerSDKSampleAppDlg::RemoveScannerOnPnP(BSTR outXml)
{
	CQuickXmlParser x(outXml);
	CQuickXmlParser::TAGDATA tag[1] = {0};

	tag[0].Tag.Name = L"scannerID";

	x.Configure(tag, 1);
	CQuickXmlParser::xptr p = 0;

	wchar_t *val = 0;
	while(1)
	{
		p = x.Parse(p);
		SCANNER sc;

		int ScannerID = _ttoi(x.Translate(tag[0].Value));
		int ID;
		POSITION pos = m_ScannerMap.GetStartPosition();
		while(pos != NULL)
		{
			m_ScannerMap.GetNextAssoc(pos, ID, sc);
			if(sc.ID == ScannerID)
			{
				m_ScannerMap.RemoveKey(ID);
				break;
			}
		}
		if(p == 0) break;
		x.ClearValues(tag);
	}

	RefreshScannerList();
	SynchScannerCombo();
	return true;
}
*/
void CScannerSDKSampleAppDlg::SynchScannerCombo()
{
	cmbScanner.ResetContent();

	CString comboline;
	int ID, N;
	SCANNER sc;
	POSITION pos = m_ScannerMap.GetStartPosition();
	while(pos != NULL)
	{
		m_ScannerMap.GetNextAssoc(pos, ID, sc);
		CString model = (sc.Model.GetLength() == 0 ? _T("Model Unavailable") : sc.Model);
		N = model.Find(_T('-'));
		if(N != -1)
			comboline.Format(_T("%d  -  %s"), sc.ID, model.Left(N));
		else
			comboline.Format(_T("%d  -  %s"), sc.ID, model);

		cmbScanner.SetItemData(cmbScanner.AddString(comboline), sc.ID);
	}

	cmbScanner.SetCurSel(0);
}

void CScannerSDKSampleAppDlg::OnDiscoverScanners()
{
	long status = 1;
	if(m_ScannerCommands == 0) return;

	if(m_OpenAndRegistered == true)
	{
		//Unregister and close existing connections
		status = 1;
		m_ScannerCommands->cmdUnRegisterEvents(6, L"1,2,4,8,16,32", &status);
		LOG(status, "UNREGISTER_FOR_ALL_EVENTS");

		m_ScannerCommands->cmdClose(&status);
		LOG(status, "CLOSE SCANNERS");
		m_OpenAndRegistered = false;

		m_ScannerMap.RemoveAll();
		cmbScanner.ResetContent();
	}
	
	//Open Scanners of all types or a particular type specified by the selected protocol(Note: In 
	//this particular use case, either scanners with all protocols or one specific
	//protocol will be opened.)
	SHORT ScannerTypesArray[TOTAL_SCANNER_TYPES] = {0, 0, 0, 0, 0, 0, 0, 0,0};
	int Protocol = GetTabManager().GetTabDlg<CConfigurationDlg>().QueryScannerProtocol();
	ScannerTypesArray[0] = Protocol;

	m_ScannerCommands->cmdOpen(ScannerTypesArray, 1, &status);
	LOG(status, "OPEN SCANNERS");

	//Register for all possible events
	status = 1;
	m_ScannerCommands->cmdRegisterEvents(6, L"1,2,4,8,16,32", &status);
	LOG(status, "REGISTER_FOR_ALL_EVENTS");
	//Initiate scanner discovery
	status = 1;
	BSTR outXml = 0;
	if ( m_ScannerCommands->cmdDiscover(&outXml, &status) == S_OK )
	{
		GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);
		ShowScanners(outXml);
		UpdateScannerStatus();
		m_OpenAndRegistered = true;
	}

	LOG(status, "GET_SCANNERS");

	GetTabManager().GetTabDlg<CRSMDlg>().OnClearAll();
	GetTabManager().GetTabDlg<CScanToConnectDlg>().RequestPairingBarcode();
	GetTabManager().GetTabDlg<CScanToConnectDlg>().EnableUIControllers(TRUE);
}

void CScannerSDKSampleAppDlg::InitScannerListControl()
{
	m_ScannerListControl.SetHeader(_T("#"), 25);
	m_ScannerListControl.SetHeader(_T("Com Interface"), 100);
	m_ScannerListControl.SetHeader(_T("Model #"), 125);
	m_ScannerListControl.SetHeader(_T("Firmware"), 125);
	m_ScannerListControl.SetHeader(_T("Built"), 75);
	m_ScannerListControl.SetHeader(_T("Serial # or Port #"), 125);
	m_ScannerListControl.SetHeader(_T("GUID"), 200);
}

void CScannerSDKSampleAppDlg::OnListItemchanged(NMHDR *pNMHDR, LRESULT *pResult)
{
	LPNMLISTVIEW pNMLV = reinterpret_cast<LPNMLISTVIEW>(pNMHDR);
	POSITION p = m_ScannerListControl.GetFirstSelectedItemPosition();
	while ( p )
	{
		int nSelected = m_ScannerListControl.GetNextSelectedItem(p);

		SelectedScannerID = m_ScannerListControl.GetItemText(nSelected, 0);

		CString s = m_ScannerListControl.GetItemText(nSelected, 0);
	    int scnID = _ttoi(s);
		for(int i = 0; i < cmbScanner.GetCount(); ++i)
		{
			int ID = (int)cmbScanner.GetItemData(i);
			if(ID == scnID)
			{
				cmbScanner.SetCurSel(i);
				break;
			}
		}

		GetTabManager().GetTabDlg<CRSMDlg>().SetScannerID(&SelectedScannerID);
		GetTabManager().GetTabDlg<CBarcodeDlg>().SetScannerID(&SelectedScannerID);
		GetTabManager().GetTabDlg<CScannerActionDlg>().SetScannerID(&SelectedScannerID);
		GetTabManager().GetTabDlg<CConfigurationDlg>().SetScannerID(&SelectedScannerID);
		GetTabManager().GetTabDlg<CMiscellaneousDlg>().SetScannerID(&SelectedScannerID);
		GetTabManager().GetTabDlg<CImageVideoDlg>().SetScannerID(&SelectedScannerID);
		GetTabManager().GetTabDlg<CScaleDlg>().SetScannerID(&SelectedScannerID);
		GetTabManager().GetTabDlg<CIntelDocCap>().SetScannerID(&SelectedScannerID);

		GetTabManager().GetTabDlg<CConfigurationDlg>().UpdateClaimedStatus(scnID);
	    GetTabManager().GetTabDlg<CScannerActionDlg>().UpdateScannerDisabledStatus(scnID);

		SCANNER* p = GetScannerInfo(SelectedScannerID);
		if(p->HostMode == MODE_SNAPI_IMG || p->HostMode == MODE_SSI|| p->HostMode == MODE_SSI_BT)
		{
			btnPullTri.EnableWindow(1);
			btnRelTri.EnableWindow(1);
			GetTabManager().GetTabDlg<CImageVideoDlg>().SetAbortFirmwareState(1);
		}
		else
		{
			btnPullTri.EnableWindow(0);
			btnRelTri.EnableWindow(0);
			GetTabManager().GetTabDlg<CImageVideoDlg>().SetAbortFirmwareState(0);
		}
	}
	*pResult = 0;
}


void CScannerSDKSampleAppDlg::UpdateScannerClaimedStatus()
{
  
	
	CList<int,int> tempLst;
 	POSITION pos = m_ClaimedScannerList.GetHeadPosition();
    while(pos != NULL)
	{
		int ID= m_ClaimedScannerList.GetNext(pos);
		tempLst.AddHead(ID);
	}	

	pos = tempLst.GetHeadPosition();
    while(pos != NULL)
	{
		int ID= tempLst.GetNext(pos);
		if (!ScannerListContains(ID))
		{
			 POSITION loc=m_ClaimedScannerList.Find(ID);
		     if (loc!=NULL)
			    m_ClaimedScannerList.RemoveAt(loc);
		}
	}	

}


void CScannerSDKSampleAppDlg::UpdateDisabledScannerStatus()
{
  
	
	CList<int,int> tempLst;
 	POSITION pos = m_DisabledScannerList.GetHeadPosition();
    while(pos != NULL)
	{
		int ID= m_DisabledScannerList.GetNext(pos);
		tempLst.AddHead(ID);
	}	

	pos = tempLst.GetHeadPosition();
    while(pos != NULL)
	{
		int ID= tempLst.GetNext(pos);
		if (!ScannerListContains(ID))
		{
			 POSITION loc=m_DisabledScannerList.Find(ID);
		     if (loc!=NULL)
			    m_DisabledScannerList.RemoveAt(loc);
		}
	}	

}

bool CScannerSDKSampleAppDlg::ScannerListContains(int scnID)
{
  
	SCANNER sc;
	int ID;

	POSITION pos = m_ScannerMap.GetStartPosition();
	while(pos != NULL)
	{
		m_ScannerMap.GetNextAssoc(pos, ID, sc);
		if (ID==scnID)
			return true;
	}
	return false;

}

void CScannerSDKSampleAppDlg::SetCommandMode(int Async)
{
	GetTabManager().GetTabDlg<CRSMDlg>().SetAsync(&Async);
	GetTabManager().GetTabDlg<CBarcodeDlg>().SetAsync(&Async);
	GetTabManager().GetTabDlg<CScannerActionDlg>().SetAsync(&Async);
	GetTabManager().GetTabDlg<CConfigurationDlg>().SetAsync(&Async);
	GetTabManager().GetTabDlg<CMiscellaneousDlg>().SetAsync(&Async);
	GetTabManager().GetTabDlg<CImageVideoDlg>().SetAsync(&Async);
	GetTabManager().GetTabDlg<CScaleDlg>().SetAsync(&Async);

	if(Async == 1)
		LOG(0, "COMMAND SET TO ASYNC MODE")
	else
		LOG(0, "COMMAND RESET TO SYNC MODE")
}

void CScannerSDKSampleAppDlg::OnPullTrigger()
{
	if(m_ScannerCommands == 0) return;
	long status =1;
	m_ScannerCommands->cmdPullTrigger(SelectedScannerID,Async,&status);
	LOG(status, "PULL_TRIGGER");
}

void CScannerSDKSampleAppDlg::OnReleaseTrigger()
{
	if(m_ScannerCommands == 0) return;
	long status =1;
	m_ScannerCommands->cmdReleaseTrigger(SelectedScannerID,Async,&status);
	LOG(status, "RELEASE_TRIGGER");
}

HBRUSH CScannerSDKSampleAppDlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
	if(!m_brush.m_hObject)
		m_brush.CreateSolidBrush(RGB(233, 238, 239));

	SetLabelBkg(pDC, pWnd, IDC_EDIT1);
	SetLabelBkg(pDC, pWnd, IDC_EDIT2);
	SetLabelBkg(pDC, pWnd, IDC_STATIC);

	return m_brush;
}

void CScannerSDKSampleAppDlg::OnSelectChangeScannerCombo()
{
	int ID = (int)cmbScanner.GetItemData(cmbScanner.GetCurSel());
	int ItemCount = m_ScannerListControl.GetItemCount();
	for(int i = 0; i < ItemCount; ++i)
	{
		CString s = m_ScannerListControl.GetItemText(i, 0);
		int ListID = _ttoi(s);
		if(ID == ListID)
		{
			m_ScannerListControl.SetItemState(i, LVIS_SELECTED, LVIS_SELECTED);
			m_ScannerListControl.SetFocus();
			break;
		}
	}

	GetTabManager().GetTabDlg<CConfigurationDlg>().UpdateClaimedStatus(ID);
    GetTabManager().GetTabDlg<CScannerActionDlg>().UpdateScannerDisabledStatus(ID);

}

void CScannerSDKSampleAppDlg::OnOK()
{
}


void CScannerSDKSampleAppDlg::OnTcnSelchangeTab1(NMHDR *pNMHDR, LRESULT *pResult)
{
	if(TabCtrl_GetCurSel(pNMHDR->hwndFrom) == 6)
	{
		GetTabManager().GetTabDlg<CScanToConnectDlg>().RequestPairingBarcode();
	}
	*pResult = 0;
}
