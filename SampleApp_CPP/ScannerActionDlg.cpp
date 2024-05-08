/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#include "stdafx.h"
#include "ScannerSDKSampleApp.h"
#include "ScannerActionDlg.h"
#include "LogsDlg.h"
#include "ScannerHostTypes.h"
#include <iostream>
#include <sstream>

using namespace std;

IMPLEMENT_DYNAMIC(CScannerActionDlg, CDialog)

CScannerActionDlg::CScannerActionDlg(CWnd* pParent /*=NULL*/)
: CDialog(CScannerActionDlg::IDD, pParent)
{
    Async = 0;
}

CScannerActionDlg::~CScannerActionDlg()
{
}

void CScannerActionDlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_COMBO1, CmbLED);
    DDX_Control(pDX, IDC_COMBO2, m_cmbBeeps);
    DDX_Control(pDX, IDC_COMBO_HOSTS, m_cmbHostVariants);
    DDX_Control(pDX, IDC_CHECK3, chkSilentSwitch);
    DDX_Control(pDX, IDC_CHECK4, chkPermanantChange);
    DDX_Control(pDX, IDC_BUTTON_ENABLE, m_btnScannerEnable);
    DDX_Control(pDX, IDC_BUTTON_DISABLE, m_btnScannerDisable);
    DDX_Control(pDX, IDC_EDIT2, m_editPMDuration);
}

BEGIN_MESSAGE_MAP(CScannerActionDlg, CDialog)
    ON_BN_CLICKED(IDC_BUTTON1, &CScannerActionDlg::OnReboot)
    ON_BN_CLICKED(IDC_BUTTON2, &CScannerActionDlg::OnAimOn)
    ON_BN_CLICKED(IDC_BUTTON3, &CScannerActionDlg::OnAimOff)
    ON_BN_CLICKED(IDC_BUTTON6, &CScannerActionDlg::OnBeep)
    ON_BN_CLICKED(IDC_BUTTON4, &CScannerActionDlg::OnLEDOn)
    ON_BN_CLICKED(IDC_BUTTON5, &CScannerActionDlg::OnLEDOff)
    ON_BN_CLICKED(IDC_BUTTON12, &CScannerActionDlg::OnSwitchHostMode)
    ON_BN_CLICKED(IDC_BUTTON_Disconnect, &CScannerActionDlg::OnDisconnect)
    ON_BN_CLICKED(IDC_BUTTON_ENABLE, &CScannerActionDlg::OnEnableScanner)
    ON_BN_CLICKED(IDC_BUTTON_DISABLE, &CScannerActionDlg::OnDisableScanner)
    ON_WM_CTLCOLOR()
    ON_BN_CLICKED(IDC_BUTTON7, &CScannerActionDlg::OnPagerMotor)
END_MESSAGE_MAP()

void CScannerActionDlg::OnReboot()
{
    CHECK_CMD0;

    long status = 1;
    SC->cmdReboot(SelectedScannerID,Async,&status);
    LOG(status, "REBOOT_SCANNER");
}

void CScannerActionDlg::OnDisconnect()
{
    CHECK_CMD0;

    long status = 1;
    SC->cmdDisconnect(SelectedScannerID,Async,&status);
    LOG(status, "DISCONNECT_SCANNER");
}

void CScannerActionDlg::InitLEDCombo()
{
    CHECK_CMD0;
    CmbLED.SetItemData(CmbLED.AddString(L"GREEN"), 0);
    CmbLED.SetItemData(CmbLED.AddString(L"YELLOW"), 2);
    CmbLED.SetItemData(CmbLED.AddString(L"RED"), 4);
    CmbLED.SetCurSel(0);
}

void CScannerActionDlg::InitPagerMotor()
{
    m_editPMDuration.SetWindowText((LPCTSTR)defaultVibrationDuration.c_str());
}

void CScannerActionDlg::InitBeepList()
{
    //Init Beep Combo
    //CHECK_CMD0;
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"ONE SHORT HIGH"), 0);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"TWO SHORT HIGH"), 1);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"THREE SHORT HIGH"), 2);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"FOUR SHORT HIGH"), 3);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"FIVE SHORT HIGH"), 4);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L""), -1);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"ONE SHORT LOW"), 5);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"TWO SHORT LOW"), 6);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"THREE SHORT LOW"), 7);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"FOUR SHORT LOW"), 8);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"FIVE SHORT LOW"), 9);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L""), -1);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"ONE LONG HIGH"), 10);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"TWO LONG HIGH"), 11);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"THREE LONG HIGH"), 12);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"FOUR LONG HIGH"), 13);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"FIVE LONG HIGH"), 14);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L""), -1);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"ONE LONG LOW"), 15);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"TWO LONG LOW"), 16);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"THREE LONG LOW"), 17);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"FOUR LONG LOW"), 18);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"FIVE LONG LOW"), 19);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L""), -1);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"FAST HIGH LOW HIGH LOW"), 20);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"SLOW HIGH LOW HIGH LOW"), 21);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"HIGH LOW"), 22);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"LOW HIGH"), 23);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"HIGH LOW HIGH"), 24);
    m_cmbBeeps.SetItemData(m_cmbBeeps.AddString(L"LOW HIGH LOW"), 25);

    m_cmbBeeps.SetCurSel(0);
}

void CScannerActionDlg::OnDisableScanner()
{
    CHECK_CMD0;

    long status = 1;
    SC->cmdDisable(SelectedScannerID, Async, &status);
    LOG(status, "SCAN_DISABLE");
}

void CScannerActionDlg::OnEnableScanner()
{
    CHECK_CMD0;

    long status = 1;
    SC->cmdEnable(SelectedScannerID, Async, &status);
    LOG(status, "SCAN_ENABLE");
}

void CScannerActionDlg::SetScannerID(std::wstring *ScannerID)
{
    SelectedScannerID = *ScannerID;

    SCANNER* p = GetMainDlg()->GetScannerInfo(SelectedScannerID);
    InitHostCombo(p->HostMode);
}

void CScannerActionDlg::SetAsync(int *ParaA)
{
    Async = *ParaA;
}

void CScannerActionDlg::OnAimOn()
{
    CHECK_CMD0;

    long status=1;
    SC->cmdAimOn(SelectedScannerID,Async,&status);
    LOG(status, "AIM_ON");
}

void CScannerActionDlg::OnAimOff()
{
    CHECK_CMD0;

    long status=1;
    SC->cmdAimOff(SelectedScannerID,Async,&status);
    LOG(status, "AIM_OFF");
}

void CScannerActionDlg::OnLEDOn()
{
    CHECK_CMD0;

    SCANNER* p = GetMainDlg()->GetScannerInfo(SelectedScannerID);
    int inSel = (int) CmbLED.GetItemData(CmbLED.GetCurSel());
    long status = 1;
    SC->cmdLED(SelectedScannerID, ScannerLEDTypes[inSel], Async, &status,p);
    LOG(status, "LED_ON");
}

void CScannerActionDlg::OnLEDOff()
{
    CHECK_CMD0;

    SCANNER* p = GetMainDlg()->GetScannerInfo(SelectedScannerID);
    int inSel = (int) CmbLED.GetItemData(CmbLED.GetCurSel());
    long status = 1;
    SC->cmdLEDOff(SelectedScannerID, ScannerLEDTypes[inSel + 1], Async, &status,p);
    LOG(status, "LED_OFF");
}

void CScannerActionDlg::OnBeep()
{
    CHECK_CMD0;

    int val = (int) m_cmbBeeps.GetItemData(m_cmbBeeps.GetCurSel());
    long status=1;

    wchar_t buf[8];
    _itow_s(val, buf, 8, 10);
    wstring str = buf;
    SC->cmdBeep(SelectedScannerID, str, Async, &status);
    LOG(status, "SOUND_BEEPER");
}

BOOL CScannerActionDlg::OnInitDialog()
{
    CDialog::OnInitDialog();
    InitLEDCombo();
    InitPagerMotor();
    InitBeepList();
    InitHostCombo(MODE_ALL);
    return TRUE;  
}

HBRUSH CScannerActionDlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
    CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
    if(!m_brush.m_hObject)
        m_brush.CreateSolidBrush(RGB(247, 250, 253));
    return m_brush;
}

void CScannerActionDlg::InitHostCombo(int CurrentHostProtocol)
{
    m_cmbHostVariants.ResetContent();

    if(CurrentHostProtocol > MODE_IBMTT || CurrentHostProtocol <= MODE_ALL || CurrentHostProtocol == MODE_SSI_BT || CurrentHostProtocol == MODE_SSI_IP) return;

    for(int i = MODE_IBMHID; i <= MODE_IBMTT; ++i)
    {
        if(!((CurrentHostProtocol == MODE_HIDKB) && (i == MODE_SNAPI_NO_IMG)))
        {
            m_cmbHostVariants.SetItemData(m_cmbHostVariants.AddString(ScannerHostTypes[i].m_HostLabel),	(DWORD_PTR)(ScannerHostTypes[i].m_HostValue));
        }
    }
    m_cmbHostVariants.SetCurSel(0);
}

void CScannerActionDlg::OnSwitchHostMode()
{
    CHECK_CMD0;

    int index = m_cmbHostVariants.GetCurSel();
    wchar_t* ReqHostType = (wchar_t*) m_cmbHostVariants.GetItemData(index);

    if(index >= 0)
    {
        wstring strSilentSwitch = chkSilentSwitch.GetCheck() ? L"TRUE" : L"FALSE";
        wstring strPermChange = chkPermanantChange.GetCheck() ? L"TRUE" : L"FALSE";
        long status = 1;
        SC->cmdSwitchHostMode(SelectedScannerID, ReqHostType, strSilentSwitch, strPermChange, Async, &status);
        LOG(status, "DEVICE_SWITCH_HOST_MODE");
    }
}

bool CScannerActionDlg::IsDigit(const std::string str)
{
    return str.find_first_not_of("0123456789") == std::string::npos;
}

void CScannerActionDlg::OnPagerMotor()
{
    CHECK_CMD0;
    
    CString dValue;
    wstring pagerMotorDuration;
    
    m_editPMDuration.GetWindowTextW(dValue);
    if (dValue == "") {
        dValue = "0";
        m_editPMDuration.SetWindowTextW((LPCTSTR) L"0");
    }
    CT2CA pszConvertedAnsiString(dValue);
    std::string s(pszConvertedAnsiString);

    if (CScannerActionDlg::IsDigit(s)) {
        pagerMotorDuration = (LPCTSTR)dValue;

        long status = -1;
        SC->cmdPagerMotor(SelectedScannerID, pagerMotorDuration, Async, &status);
        LOG(status, "START_PAGER_MOTOR");
    }
    else {
        LOG(-1, "Please enter a numeric value for pager motor duration");
    }
    

}
