/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/

#pragma once
#include "ScannerCommands.h"
#include "afxwin.h"


class CMiscellaneousDlg : public CDialog
{
    DECLARE_DYNAMIC(CMiscellaneousDlg)

public:
    CMiscellaneousDlg(CWnd* pParent = NULL);   
    virtual ~CMiscellaneousDlg();

    void SetScannerID(wstring * ScannerID);
    void SetAsync(int *ParaAsync);
    void InitSCdcSHostModeCombo();

    enum { IDD = IDD_Miscellaneous };

protected:
    virtual void DoDataExchange(CDataExchange* pDX);    

    DECLARE_MESSAGE_MAP()
    DECLARE_TAB_WINDOW(Miscellaneous)

public:

    afx_msg void OnSDKVersion();
    afx_msg void OnGetDeviceTopology();
    afx_msg void OnAsyncMode();

public:
    virtual BOOL OnInitDialog();
    afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);

private:

    CBrush  m_brush;

    CButton chkAsync;

    wstring SelectedScannerID;
    int Async;
    
    CComboBox m_cmbSCdcSHostMode;

private:
    CButton m_chkSCdcSIsSilent;
    CButton m_chkSCdcSIsPermanent;
public:
    afx_msg void OnSCdcSwitchHostMode();
};