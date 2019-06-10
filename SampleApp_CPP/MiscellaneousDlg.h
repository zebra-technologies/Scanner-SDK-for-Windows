/*******************************************************************************************
*
* ©2016 Symbol Technologies LLC. All rights reserved.
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
	void InitBaudCombo();
	void InitDatabitsCombo();
	void InitParitybitsCombo();
	void InitStopbitsCombo();


	enum { IDD = IDD_Miscellaneous };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    

	DECLARE_MESSAGE_MAP()
	DECLARE_TAB_WINDOW(Miscellaneous)

public:

	afx_msg void OnSDKVersion();
	afx_msg void OnGetDeviceTopology();
	afx_msg void OnSetSerialInterface();
	afx_msg void OnAsyncMode();

public:
	virtual BOOL OnInitDialog();
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);

private:

	CBrush  m_brush;

	CButton chkAsync;
	CButton m_chkFlow;
	CComboBox m_cmbBaud;
	CComboBox m_cmbDatabits;
	CComboBox m_cmbParityBits;
	CComboBox m_cmbStopBits;

	wstring SelectedScannerID;
	int Async;

};

