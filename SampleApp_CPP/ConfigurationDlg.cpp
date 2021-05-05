/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#include "stdafx.h"
#include "ScannerSDKSampleApp.h"
#include "ConfigurationDlg.h"
#include "LogsDlg.h"
#include "shlwapi.h"
#include "QuickXmlParser.h"

IMPLEMENT_DYNAMIC(CConfigurationDlg, CDialog)

CConfigurationDlg::CConfigurationDlg(CWnd* pParent )
: CDialog(CConfigurationDlg::IDD, pParent)
{
	Async = 0;
}

CConfigurationDlg::~CConfigurationDlg()
{
}

void CConfigurationDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_EDIT1, txtFWPath);
	DDX_Control(pDX, IDC_PROGRESS1, FWprogress);
	DDX_Control(pDX, IDC_CHECK2, m_chkBulk);
	DDX_Control(pDX, IDC_CHECK3, ChkClaim);
	DDX_Control(pDX, IDC_COMBO1, m_cmbProtocol);
	DDX_Control(pDX, IDC_EDIT_DecodeToneFilePath, txtDecodeToneFilePath);
	DDX_Control(pDX, IDC_EDIT_ElectricFenceCustomTonePath, txtElectricFenceCustomToneWaveFileName);
}


BEGIN_MESSAGE_MAP(CConfigurationDlg, CDialog)
	ON_BN_CLICKED(IDC_BUTTON1, &CConfigurationDlg::OnBrowse)
	ON_BN_CLICKED(IDC_BUTTON2, &CConfigurationDlg::OnUpdateFirmware)
	ON_BN_CLICKED(IDC_BUTTON3, &CConfigurationDlg::OnAbortFirmwareUpdate)
	ON_BN_CLICKED(IDC_BUTTON4, &CConfigurationDlg::OnLaunchFirmware)
	ON_WM_CTLCOLOR()
	ON_BN_CLICKED(IDC_CHECK3, &CConfigurationDlg::OnClaimScanner)
	ON_CBN_SELCHANGE(IDC_COMBO1, &CConfigurationDlg::OnChangeProtocol)
	ON_BN_CLICKED(IDC_BUTTON_DecodeToneBrrowse, &CConfigurationDlg::OnBnClickedButtonDecodetonebrrowse)
	ON_BN_CLICKED(IDC_BUTTON_DecodeToneErase, &CConfigurationDlg::OnBnClickedButtonDecodetoneerase)
	ON_BN_CLICKED(IDC_BUTTON_DecodeToneUpload, &CConfigurationDlg::OnBnClickedButtonDecodetoneupload)
	ON_BN_CLICKED(IDC_BUTTON_ElectricFenceCustomToneBrowse, &CConfigurationDlg::OnBnClickedButtonElectricFenceCustomToneBrowse)
	ON_BN_CLICKED(IDC_BUTTON_ElectricFenceCustomToneUpload, &CConfigurationDlg::OnBnClickedButtonElectricFenceCustomToneUpload)
	ON_BN_CLICKED(IDC_BUTTON_ElectricFenceCustomToneErase, &CConfigurationDlg::OnBnClickedButtonElectricFenceCustomToneErase)
END_MESSAGE_MAP()


void CConfigurationDlg::OnBrowse()
{
	CFileDialog fOpenDlg(TRUE, L"dat", L"", OFN_HIDEREADONLY|OFN_FILEMUSTEXIST, 
						 L"Firmware files (*.dat)|*.dat|Plugin files (*.SCNPLG)|*.SCNPLG||", this);

	fOpenDlg.m_pOFN->lpstrTitle = L"FirmwareUpdateFiles";

	if ( fOpenDlg.DoModal() == IDOK )
	{
		txtFWPath.SetWindowTextW(fOpenDlg.GetPathName());
	}
}


void CConfigurationDlg::SetScannerID(std::wstring *ScannerID)
{
	SelectedScannerID=*ScannerID;
}

void CConfigurationDlg::SetAsync(int *ParaA)
{
	Async = *ParaA;
}

void CConfigurationDlg::OnUpdateFirmware()
{
	CHECK_CMD0;

	long status = 1;
	CString CSFilePath;
	wstring FilePath;
	txtFWPath.GetWindowTextW(CSFilePath);
	if(!PathFileExists(CSFilePath)) return;

	FilePath = LPCTSTR(CSFilePath);
	SC->cmdUpdateFW(SelectedScannerID, FilePath, Async, &status, (m_chkBulk.GetCheck() == BST_CHECKED));
	LOG(status, "UPDATE_FIRMWARE");
}

/***
	Parses the XML response for the RMD events(Firmware notification events). Please refer to
	the "Firmware Upgrade Procedure" section in the SDK Developers guide to understand the different
	XML reponses for events: SCANNER_UF_SESS_START, SCANNER_UF_DL_START, SCANNER_UF_DL_PROGRESS,
	SCANNER_UF_DL_END, SCANNER_UF_SESS_END, SCANNER_UF_STATUS.

	Note: CQuickXmlParser does not care if the specified tag is non applicable - it simply returns NULL.
***/
bool CConfigurationDlg::ReadFirmwareResponseEventXML(BSTR RespXml, int *nMax, int *nProgress, wstring *sStatus, wstring *csScannerID)
{
	CQuickXmlParser xml(RespXml);
	CQuickXmlParser::TAGDATA tag[4] = {0};
	tag[0].Tag.Name = L"scannerID"; 
	tag[1].Tag.Name = L"maxcount"; 
	tag[2].Tag.Name = L"status"; 
	tag[3].Tag.Name = L"progress"; 
	xml.Configure(tag, 4);
	CQuickXmlParser::xptr p = 0;

	p = xml.Parse(p);
	
	*csScannerID = xml.Translate(tag[0].Value);
	*nMax		 = _wtoi(xml.Translate(tag[1].Value));
	*sStatus	 = xml.Translate(tag[2].Value);
	*nProgress	 = _wtoi(xml.Translate(tag[3].Value));

	return true;
}
/***
	Updates the progress bar control according to the firmware upload status events.
***/
void CConfigurationDlg::OnRMDEvent(short eventType, BSTR prmdData)
{
	if(eventType < SCANNER_UF_SESS_START || eventType > SCANNER_UF_STATUS) return;

	int nMax			= 0;
	int nProgress		= 0;
	wstring sStatus		= L"";
	wstring sScannerID	= L"";

	ReadFirmwareResponseEventXML(prmdData, &nMax, &nProgress, &sStatus, &sScannerID);

	if(SelectedScannerID != sScannerID) return;

	//Trivial download state indicator: 0 = intialized, 1 = Download Started, 2 = Download finished. 
	static int DownloadState = 0; 
	
	switch(eventType)
	{
		case SCANNER_UF_SESS_START:		
				//Flash download session has started.
				DownloadState = 0;
				FWprogress.SetRange32(0, nMax);
				LOG(-1, "ScanRMD Event fired - SCANNER_UF_SESS_START ");
				break;

		case SCANNER_UF_DL_START:	
				//A firmware component download has started. There could be more than one
				//component in the firmware. When all the components have finished downloading,
				//the firmware download completes.
				DownloadState = 1;
				FWprogress.SetPos(0);
				LOG(-1, "ScanRMD Event fired - SCANNER_UF_DL_START");
				break;

		case SCANNER_UF_DL_PROGRESS:		
				//Blocks of data corresponding to a specific component have started downloading.
				//When all the blocks are downloadedm, the component download is said to have completed.
				//Not all blocks downloads are logged since this can fill up the log quite quickly.
				if(DownloadState == 1)
				{
					LOG(-1, "ScanRMD Events fired consecutively - SCANNER_UF_DL_PROGRESS");
					DownloadState = 2;
				}
				FWprogress.SetPos(nProgress);
				break;

		case SCANNER_UF_DL_END:
				//The final block corresponding to a component has been downloaded.
				LOG(-1, "ScanRMD Event fired - SCANNER_UF_DL_END");
				break;

		case SCANNER_UF_SESS_END:
				//The flash update session has completed.
				FWprogress.SetPos(0);
				LOG(-1, "ScanRMD Event fired - SCANNER_UF_SESS_END");
				if(DownloadState == 2)
				{
					DownloadState = 0;
				}
				break;

		case SCANNER_UF_STATUS:
				//Some error has occurred or the update was aborted. Reset the the progress bar.
				DownloadState = 0;
				FWprogress.SetPos(0);
				LOG(-1, "ScanRMD Event fired - SCANNER_UF_STATUS ");
				break;
	}
}
/***
	Aborts the firmware upload process
***/
void CConfigurationDlg::OnAbortFirmwareUpdate()
{
	CHECK_CMD0;

	long status = 1;
	SC->cmdUpdateFWAbort(SelectedScannerID,Async,&status);
	LOG(status, "ABORT_UPDATE_FIRMWARE");
}
/***
	Launches the firmware in the Scanner. This is where the ROM is actually flashed.
***/
void CConfigurationDlg::OnLaunchFirmware()
{
	CHECK_CMD0;

	long status = 1;
	SC->cmdStartNewFW(SelectedScannerID,Async,&status);
	LOG(status, "START_NEW_FIRMWARE");

}
/***
	Initializes the  Select protocol combo box.
***/
void CConfigurationDlg::InitProtocolCombo()
{
	m_cmbProtocol.SetItemData(m_cmbProtocol.AddString(_T("ALL")), SCANNER_TYPES_ALL);
	m_cmbProtocol.SetItemData(m_cmbProtocol.AddString(_T("SNAPI")), SCANNER_TYPES_SNAPI);
	m_cmbProtocol.SetItemData(m_cmbProtocol.AddString(_T("IBM HANDHELD")), SCANNER_TYPES_IBMHID);
	m_cmbProtocol.SetItemData(m_cmbProtocol.AddString(_T("HID KEYBOARD")), SCANNER_TYPES_HIDKB);
	m_cmbProtocol.SetCurSel(0);
}

/***
	MFC specific dialog initialization.
***/
BOOL CConfigurationDlg::OnInitDialog()
{
	CDialog::OnInitDialog();
	InitProtocolCombo();

	//Default Bulk update enabled. Use this setting for maximum performance during the update
	//process when in SNAPI mode.
	m_chkBulk.SetCheck(BST_CHECKED);	
	return TRUE;  
}

HBRUSH CConfigurationDlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
	if(!m_brush.m_hObject)
		m_brush.CreateSolidBrush(RGB(247, 250, 253));
	return m_brush;
}

void CConfigurationDlg::OnClaimScanner()
{
	CHECK_CMD0;

	long status =1;
	
	CList<int,int> *pClaimedList = &(((CScannerSDKSampleAppDlg*)(AfxGetApp()->m_pMainWnd))->m_ClaimedScannerList);
	int scnID= _wtoi(SelectedScannerID.c_str());

	if ( ChkClaim.GetCheck() )
	{
		SC->cmdClaim(SelectedScannerID, Async, &status);
		LOG(status, "CLAIM_DEVICE");
		
		if (status==STATUS_SUCCESS)
		{
			POSITION pos=pClaimedList->Find(scnID);
			if (pos==NULL)
			{
			     pClaimedList->AddHead(scnID);
			}
		}
	}
	else
	{
		SC->cmdRelease(SelectedScannerID, Async, &status);
		LOG(status, "RELEASE_DEVICE");
		
		if (status==STATUS_SUCCESS)
		{
			POSITION pos=pClaimedList->Find(scnID);
			if (pos!=NULL)
			{
				pClaimedList->RemoveAt(pos);
			}
		}
	}

}

void CConfigurationDlg::UpdateClaimedStatus(int scnID)
{
		
	    CList<int,int> *pClaimedList = &(((CScannerSDKSampleAppDlg*)(AfxGetApp()->m_pMainWnd))->m_ClaimedScannerList);
		POSITION pos=pClaimedList->Find(scnID);
		if (pos==NULL)
		{
			ChkClaim.SetCheck(0);
		}
		else 
		{
			ChkClaim.SetCheck(1);
		}
		UpdateData(true);
}



void CConfigurationDlg::OnChangeProtocol()
{
	int index = m_cmbProtocol.GetCurSel();
	int NewProtocol = (int) m_cmbProtocol.GetItemData(index);
	int &CurrentProtocol = QueryScannerProtocol();
	if(NewProtocol != CurrentProtocol)
	{
		QueryScannerProtocol() = NewProtocol;
		CString logmsg, proto;
		m_cmbProtocol.GetLBText(index, proto);
		logmsg.Format(_T("PROTOCOL CHANGED TO %s"), proto);
		LOGV(-1, logmsg.GetBuffer());
		GetMainDlg()->OnDiscoverScanners();
	}
}

void CConfigurationDlg::OnBnClickedButtonDecodetonebrrowse()
{
	CFileDialog fOpenDlg(TRUE, L"dat", L"", OFN_HIDEREADONLY|OFN_FILEMUSTEXIST, 
						 L"Wave files (*.wav)|*.wav|Wave files (*.wave)|*.wave||", this);

	fOpenDlg.m_pOFN->lpstrTitle = L"Decode Tone Files";

	if ( fOpenDlg.DoModal() == IDOK )
	{
		txtDecodeToneFilePath.SetWindowTextW(fOpenDlg.GetPathName());
	}
}

void CConfigurationDlg::OnBnClickedButtonDecodetoneerase()
{
	CHECK_CMD0;

	long status = 1;
	SC->cmdEraseDecodeTone(SelectedScannerID, &status);
	LOG(status, "ERASE_DECODE_TONE");
}

void CConfigurationDlg::OnBnClickedButtonDecodetoneupload()
{
	CHECK_CMD0;

	long status = 1;
	CString CSFilePath;
	wstring FilePath;
	txtDecodeToneFilePath.GetWindowTextW(CSFilePath);
	if(!PathFileExists(CSFilePath)) return;

	FilePath = LPCTSTR(CSFilePath);
	SC->cmdUpdateDecodeTone(SelectedScannerID, FilePath, &status);
	LOG(status, "UPDATE_DECODE_TONE");
}


void CConfigurationDlg::OnBnClickedButtonElectricFenceCustomToneBrowse()
{
	CFileDialog fOpenDlg(TRUE, L"dat", L"", OFN_HIDEREADONLY | OFN_FILEMUSTEXIST,
		L"Wave files (*.wav)|*.wav|Wave files (*.wave)|*.wave||", this);

	fOpenDlg.m_pOFN->lpstrTitle = L"Electric Fence Custom Tone";

	if (fOpenDlg.DoModal() == IDOK)
	{
		txtElectricFenceCustomToneWaveFileName.SetWindowTextW(fOpenDlg.GetPathName());
	}
}


void CConfigurationDlg::OnBnClickedButtonElectricFenceCustomToneUpload()
{
	CHECK_CMD0;

	long status = 1;
	CString CSFilePath;
	wstring FilePath;
	txtElectricFenceCustomToneWaveFileName.GetWindowTextW(CSFilePath);
	if (!PathFileExists(CSFilePath)) return;

	FilePath = LPCTSTR(CSFilePath);
	SC->cmdUploadElectricFenceCustomTone(SelectedScannerID, FilePath, &status);
	LOG(status, "UPDATE_ELECTRIC_FENCE_CUSTOM_TONE");
}


void CConfigurationDlg::OnBnClickedButtonElectricFenceCustomToneErase()
{
	CHECK_CMD0;

	long status = 1;
	SC->cmdEraseElectricFenceCustomTone(SelectedScannerID, &status);
	LOG(status, "ERASE_ELECTRIC_FENCE_CUSTOM_TONE");
}
