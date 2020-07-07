/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#include "stdafx.h"
#include "ScannerSDKSampleApp.h"
#include "LogsDlg.h"

IMPLEMENT_DYNAMIC(CLogsDlg, CDialog)

CLogsDlg::CLogsDlg(CWnd* pParent /*=NULL*/)
: CDialog(CLogsDlg::IDD, pParent)
{
    m_Count = 0;
}

CLogsDlg::~CLogsDlg()
{
}

void CLogsDlg::ShowOutXml(BSTR outXml)
{
    this->EnableWindow(true);
    m_editLogs.SetWindowText(outXml);
}

void CLogsDlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_EDIT1, txtResultLog);
    DDX_Control(pDX, IDC_EDIT_Logs, m_editLogs);
}

BEGIN_MESSAGE_MAP(CLogsDlg, CDialog)
    ON_BN_CLICKED(IDC_BUTTON1, &CLogsDlg::OnClearEventLog)
    ON_BN_CLICKED(IDC_BUTTON2, &CLogsDlg::OnClearXmlLog)
    ON_WM_CTLCOLOR()
END_MESSAGE_MAP()

void CLogsDlg::ShowResult(std::wstring result)
{
    txtResultLog.SetWindowTextW(result.c_str());
}

void CLogsDlg::OnClearEventLog()
{
    txtResultLog.SetWindowTextW(L"");
    m_Count = 0;
}

BOOL CLogsDlg::OnInitDialog()
{
    CDialog::OnInitDialog();
    return TRUE;  
}

HBRUSH CLogsDlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
    CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
    if(!m_brush.m_hObject)
        m_brush.CreateSolidBrush(RGB(247, 250, 253));
    return m_brush;
}

void CLogsDlg::OnClearXmlLog()
{
    m_editLogs.SetWindowText(L"");
}


void CLogsDlg::DisplayResult(long status, wchar_t* Cmd)
{
    CString s(Cmd);

    switch ( status )
    {
        case -1:
            s.Append(L"");
            break;

        case 0:
            s.Append(L" - Command Success.");
            break;
            
        case 10:
            s.Append(L" - Command Failed. Device is Locked by Another Application.");
            break;

        case 150:
            s.Append(L" - No CDC device found. Error: 150");
            break;

        case 151:
            s.Append(L" - Unable to open CDC port. Error: 151");
            break;

        default:
            wchar_t buf[8];
            _ltow_s(status, buf, 8, 10);
            s.Append(L" - Command Failed. Error:");
            s.Append(buf);
            break;
    }
    UpdateResults(s);
}

void CLogsDlg::UpdateResults(CString& result)
{
    GetMainDlg()->txtStatus.SetWindowTextW(result);
    CString ExistingText;
    txtResultLog.GetWindowTextW(ExistingText);

    wchar_t buf[8];
    _itow_s(++m_Count, buf, 8, 10);

    ExistingText.Append(buf);
    ExistingText.Append(L". ");
    ExistingText.Append(result);
    ExistingText.Append(L"\r\n");
    txtResultLog.SetWindowTextW(ExistingText);
}