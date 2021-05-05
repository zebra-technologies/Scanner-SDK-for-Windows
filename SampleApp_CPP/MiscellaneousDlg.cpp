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
    DDX_Control(pDX, IDC_COMBO5, m_cmbSCdcSHostMode);
    DDX_Control(pDX, IDC_CHECK1, m_chkSCdcSIsSilent);
    DDX_Control(pDX, IDC_CHECK3, m_chkSCdcSIsPermanent);
}

BEGIN_MESSAGE_MAP(CMiscellaneousDlg, CDialog)
    ON_BN_CLICKED(IDC_BUTTON2, &CMiscellaneousDlg::OnSDKVersion)
    ON_BN_CLICKED(IDC_BUTTON3, &CMiscellaneousDlg::OnGetDeviceTopology)
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