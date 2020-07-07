/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#include "stdafx.h"
#include "ScannerSDKSampleApp.h"
#include "MiscellaneousDlg.h"
#include "LogsDlg.h"
#include "QuickXmlParser.h"
#include "TopologyDlg.h"
#include "ScannerHostTypes.h"

IMPLEMENT_DYNAMIC(CMiscellaneousDlg, CDialog)

CMiscellaneousDlg::CMiscellaneousDlg(CWnd* pParent /*=NULL*/)
: CDialog(CMiscellaneousDlg::IDD, pParent)
{
    Async = 0;
}

CMiscellaneousDlg::~CMiscellaneousDlg()
{
}

void CMiscellaneousDlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_CHECK15, chkAsync);
    DDX_Control(pDX, IDC_CHECK2, m_chkFlow);
    DDX_Control(pDX, IDC_COMBO1, m_cmbBaud);
    DDX_Control(pDX, IDC_COMBO3, m_cmbDatabits);
    DDX_Control(pDX, IDC_COMBO2, m_cmbParityBits);
    DDX_Control(pDX, IDC_COMBO4, m_cmbStopBits);
    DDX_Control(pDX, IDC_COMBO5, m_cmbSCdcSHostMode);
    DDX_Control(pDX, IDC_CHECK1, m_chkSCdcSIsSilent);
    DDX_Control(pDX, IDC_CHECK3, m_chkSCdcSIsPermanent);
}

BEGIN_MESSAGE_MAP(CMiscellaneousDlg, CDialog)
    ON_BN_CLICKED(IDC_BUTTON2, &CMiscellaneousDlg::OnSDKVersion)
    ON_BN_CLICKED(IDC_BUTTON3, &CMiscellaneousDlg::OnGetDeviceTopology)
    ON_BN_CLICKED(IDC_BUTTON5, &CMiscellaneousDlg::OnSetSerialInterface)
    ON_WM_CTLCOLOR()
    ON_BN_CLICKED(IDC_CHECK15, &CMiscellaneousDlg::OnAsyncMode)
    ON_BN_CLICKED(IDC_BUTTON1, &CMiscellaneousDlg::OnSCdcSwitchHostMode)
END_MESSAGE_MAP()

void CMiscellaneousDlg::SetScannerID(std::wstring *ScannerID)
{
    SelectedScannerID = *ScannerID;
}

void CMiscellaneousDlg::SetAsync(int *ParaA)
{
    Async = *ParaA;
}

void CMiscellaneousDlg::OnSDKVersion()
{
    CHECK_CMD0;

    BSTR outXml=::SysAllocString(L"");
    long status=1;
    SC->cmdGetScanSDKV(&outXml, Async, &status);
    CQuickXmlParser x(outXml);
    CQuickXmlParser::TAGDATA tag[1] = {0};
    tag[0].Tag.Name = L"arg-string";

    x.Configure(tag, 1);
    CQuickXmlParser::xptr p = 0;

    while(1)
    {
        p = x.Parse(p);
        wchar_t *pArg = x.Translate(tag[0].Value);
        if( wcslen(pArg) > 0)
        {
            CString info;
            info.Format(L"The Selected Scanner is Running on Zebra CoreScanner Driver Version %s API.", pArg);
            ::MessageBoxW(this->m_hWnd, info, L"CoreScanner Version", MB_OK|MB_ICONINFORMATION);
        }
        if(p == 0) break;
        x.ClearValues(tag);
    }

    GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);
    LOG(status, "GET_VERSION");
}

void CMiscellaneousDlg::OnGetDeviceTopology()
{
    CHECK_CMD0;

    long status=1;
    BSTR outXml=::SysAllocString(L"");
    SC->cmdGetDevTopology(&outXml,Async,&status);
    ////OutputDebugString(outXml);
    GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);
    LOG(status, "GET_DEVICE_TOPOLOGY");

    CTopologyDlg topo;
    topo.SetXML(outXml);
    topo.DoModal();
}

void CMiscellaneousDlg::OnSetSerialInterface()
{
    CHECK_CMD0;

    int IsParity = (int) m_cmbParityBits.GetItemData(m_cmbParityBits.GetCurSel());
    int IsStop = (int) m_cmbStopBits.GetItemData(m_cmbStopBits.GetCurSel());
    int IsFlow = (m_chkFlow.GetCheck() == BST_CHECKED) ? 1 : 0;

    CString sBaud;
    m_cmbBaud.GetWindowText(sBaud);
    CString sData;
    m_cmbDatabits.GetWindowText(sData);

    CString sSerialSettings;
    sSerialSettings.Format(_T("%s,%s,%d,%d,%d"), sBaud, sData, IsParity, IsStop, IsFlow);
    wstring Settings = sSerialSettings;
    long status=1;

    SC->cmdSetSerialInterface(SelectedScannerID, Settings, Async, &status);
    LOG(status, "SERIAL_INTERFACE_SETTINGS");

}

void CMiscellaneousDlg::InitBaudCombo()
{
    m_cmbBaud.AddString(L"110");
    m_cmbBaud.AddString(L"300");
    m_cmbBaud.AddString(L"1200");
    m_cmbBaud.AddString(L"2400");
    m_cmbBaud.AddString(L"4800");
    m_cmbBaud.AddString(L"9600");
    m_cmbBaud.AddString(L"19200");
    m_cmbBaud.AddString(L"38400");
    m_cmbBaud.AddString(L"57600");
    m_cmbBaud.AddString(L"115200");
    m_cmbBaud.AddString(L"230400");
    m_cmbBaud.AddString(L"460800");
    m_cmbBaud.AddString(L"921600");

    m_cmbBaud.SetCurSel(5);
}

void CMiscellaneousDlg::InitDatabitsCombo()
{
    m_cmbDatabits.AddString(L"8");
    m_cmbDatabits.AddString(L"7");
    m_cmbDatabits.AddString(L"6");
    m_cmbDatabits.AddString(L"5");
    m_cmbDatabits.AddString(L"4");
    m_cmbDatabits.AddString(L"3");
    m_cmbDatabits.AddString(L"2");
    m_cmbDatabits.AddString(L"1");

    m_cmbDatabits.SetCurSel(0);
}

void CMiscellaneousDlg::InitParitybitsCombo()
{
    m_cmbParityBits.SetItemData(m_cmbParityBits.AddString(L"NONE"),		NOPARITY);
    m_cmbParityBits.SetItemData(m_cmbParityBits.AddString(L"ODD"),		ODDPARITY);
    m_cmbParityBits.SetItemData(m_cmbParityBits.AddString(L"EVEN"),		EVENPARITY);
    m_cmbParityBits.SetItemData(m_cmbParityBits.AddString(L"MARK"),		MARKPARITY);
    m_cmbParityBits.SetItemData(m_cmbParityBits.AddString(L"SPACE"),	SPACEPARITY);
    m_cmbParityBits.SetCurSel(0);
}
void CMiscellaneousDlg::InitStopbitsCombo()
{
    m_cmbStopBits.SetItemData(m_cmbStopBits.AddString(L"1   BIT"),	ONESTOPBIT);
    m_cmbStopBits.SetItemData(m_cmbStopBits.AddString(L"1.5 BITS"), ONE5STOPBITS);
    m_cmbStopBits.SetItemData(m_cmbStopBits.AddString(L"2   BITS"),	TWOSTOPBITS);
    m_cmbStopBits.SetCurSel(0);
}

void CMiscellaneousDlg::InitSCdcSHostModeCombo()
{
    for (int i = MODE_IBMHID; i <= MODE_IBMTT; ++i)
    {	
        m_cmbSCdcSHostMode.SetItemData(m_cmbSCdcSHostMode.AddString(ScannerHostTypes[i].m_HostLabel), (DWORD_PTR)(ScannerHostTypes[i].m_HostValue));
    }
    m_cmbSCdcSHostMode.SetCurSel(0);
}


BOOL CMiscellaneousDlg::OnInitDialog()
{
    CDialog::OnInitDialog();
    InitBaudCombo();
    InitDatabitsCombo();
    InitParitybitsCombo();
    InitStopbitsCombo();
    InitSCdcSHostModeCombo();
    return TRUE;  
}

HBRUSH CMiscellaneousDlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
    CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
    if(!m_brush.m_hObject)
        m_brush.CreateSolidBrush(RGB(247, 250, 253));

    SetLabelBkg(pDC, pWnd, IDC_STATIC);
    return m_brush;
}

void CMiscellaneousDlg::OnAsyncMode()
{
    GetMainDlg()->SetCommandMode(chkAsync.GetCheck());
}

void CMiscellaneousDlg::OnSCdcSwitchHostMode()
{
    //CHECK_CMD0;

    int index = m_cmbSCdcSHostMode.GetCurSel();
    wchar_t* ReqHostType = (wchar_t*)m_cmbSCdcSHostMode.GetItemData(index);

    if (index >= 0)
    {
        wstring strSilentSwitch = m_chkSCdcSIsSilent.GetCheck() ? L"TRUE" : L"FALSE";
        wstring strPermChange = m_chkSCdcSIsPermanent.GetCheck() ? L"TRUE" : L"FALSE";
        long status = 1;
        SC->cmdSCdcSHostMode(ReqHostType, strSilentSwitch, strPermChange, Async, &status);
        LOG(status, "SWITCH_CDC_DEVICES");
    }
}