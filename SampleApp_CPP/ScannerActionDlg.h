/*******************************************************************************************
*
* ©2016 Symbol Technologies LLC. All rights reserved.
*
********************************************************************************************/
#pragma once
#include "ScannerCommands.h"
#include "afxwin.h"

static std::wstring ScannerLEDTypes[] = { L"43", L"42", L"45", L"46", L"47", L"48"};

typedef struct _SCANNER_HOSTTYPES
{
	wchar_t*	m_HostLabel;
	wchar_t*	m_HostValue;

}SCANNER_HOSTTYPES, *PSCANNER_HOSTTYPES;


static SCANNER_HOSTTYPES ScannerHostTypes[] = {

	{L"USB-IBMHID", L"XUA-45001-1"}, //0
	{L"USB-HIDKB", L"XUA-45001-3"},	//1
	{L"USB-OPOS", L"XUA-45001-8"}, //2
	{L"USB-SNAPI with Imaging", L"XUA-45001-9"}, //3
	{L"USB-SNAPI without Imaging", L"XUA-45001-10"}, //4
	{L"USB-CDC Serial Emulation", L"XUA-45001-11"}, //5
	{L"USB-SSI over CDC", L"XUA-45001-14"}, //6
	{L"USB-IBMTT", L"XUA-45001-2"} //7

};


class CScannerActionDlg : public CDialog
{
	DECLARE_DYNAMIC(CScannerActionDlg)

public:
	CScannerActionDlg(CWnd* pParent = NULL);   
	virtual ~CScannerActionDlg();

	void InitBeepList();
	void InitLEDCombo();
	void InitHostCombo(int CurrentHostProtocol);

	void SetScannerID(wstring * ScannerID);
	void SetAsync(int *ParaAsync);
    void UpdateScannerDisabledStatus(int scnID);

	enum { IDD = IDD_ScannerActions };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    

	DECLARE_MESSAGE_MAP()
	DECLARE_TAB_WINDOW(ScannerActions)

public:
	afx_msg void OnReboot();
	afx_msg void OnDisableScanner();
	afx_msg void OnAimOn();
	afx_msg void OnAimOff();
	afx_msg void OnLEDOn();
	afx_msg void OnLEDOff();
	afx_msg void OnBeep();
	afx_msg void OnSwitchHostMode();
	afx_msg void OnDisconnect();

public:

	virtual BOOL OnInitDialog();
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);

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

};
