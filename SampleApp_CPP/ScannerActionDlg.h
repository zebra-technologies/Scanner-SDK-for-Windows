/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#pragma once
#include "ScannerCommands.h"
#include "afxwin.h"

static std::wstring ScannerLEDTypes[] = { L"43", L"42", L"45", L"46", L"47", L"48"};


class CScannerActionDlg : public CDialog
{
    DECLARE_DYNAMIC(CScannerActionDlg)
    const std::wstring defaultVibrationDuration = L"10";

public:
    CScannerActionDlg(CWnd* pParent = NULL);   
    virtual ~CScannerActionDlg();

    void InitBeepList();
    void InitLEDCombo();
    void InitHostCombo(int CurrentHostProtocol);
    void InitPagerMotor();
    void SetScannerID(wstring * ScannerID);
    void SetAsync(int *ParaAsync);

    bool IsDigit(const std::string str);

    enum { IDD = IDD_ScannerActions };

    afx_msg void OnReboot();
    afx_msg void OnDisableScanner();
    afx_msg void OnEnableScanner();
    afx_msg void OnAimOn();
    afx_msg void OnAimOff();
    afx_msg void OnLEDOn();
    afx_msg void OnLEDOff();
    afx_msg void OnBeep();
    afx_msg void OnSwitchHostMode();
    afx_msg void OnDisconnect();
    virtual BOOL OnInitDialog();
    afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
    afx_msg void OnPagerMotor();

protected:
    virtual void DoDataExchange(CDataExchange* pDX);    

    DECLARE_MESSAGE_MAP()
    DECLARE_TAB_WINDOW(ScannerActions)

 
private:

    CBrush  m_brush;
    CComboBox CmbLED;
    CButton ChkEnableScanner;
    CButton ChkSilence;
    wstring SelectedScannerID;
    int Async;

    CComboBox m_cmbBeeps;
    CComboBox m_cmbHostVariants;
    CButton chkSilentSwitch;
    CButton chkPermanantChange;

    CButton m_btnScannerEnable;
    CButton m_btnScannerDisable;
    CEdit m_editPMDuration;
};
