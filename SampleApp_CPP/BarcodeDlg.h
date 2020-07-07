/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/

#pragma once
#include "ScannerCommands.h"
#include "afxwin.h"
#include "EmbeddedBrowser.h"


class CBarcodeDlg : public CDialog
{
	DECLARE_DYNAMIC(CBarcodeDlg)

public:
	CBarcodeDlg(CWnd* pParent = NULL);   
	virtual ~CBarcodeDlg();

	void SetBarcode(BSTR scanData);
	void SetScannerID(wstring * ScannerID);
	wstring SelectedScannerID;
	int Async;
	void SetAsync(int *ParaAsync);

	enum
	{
		IDD = IDD_Barcode
	};

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    

	DECLARE_MESSAGE_MAP()
	DECLARE_TAB_WINDOW(Barcode)

public:
	virtual BOOL OnInitDialog();
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
	afx_msg void OnFlushMacroPDF();
	afx_msg void OnAbortMacroPDF();
	afx_msg void OnClear();
	void SetLocaleConfigInfo();

private:
	LPCTSTR GetSymbology(int Code);

private:
	CEdit txtDecodedData;
	CEdit txtSymbology;
	//CEdit txtRawData;
	//CEmbeddedBrowser m_EmbeddedBrowser;
	CBrush  m_brush;
	bool m_bDisableEvents;
	

	CButton m_chkEnableLocaleSelection;
	CButton m_chkDriverADF;
	CComboBox m_cmbSelectLocale;

	afx_msg void OnBnClickedEnablelang();
	afx_msg void OnCbnSelchangeCmblang();

	afx_msg void SetDriverADFbyPath();
	afx_msg void SetDriverADFbySource();
	afx_msg void OnEditScript();
	afx_msg void OnBrowseScript();
	afx_msg void OnSelectDriverADF();
	afx_msg void ResetDriverADF();

	CStringW	m_DADFSource;
	CStringW	m_DADFPath;

public:
	CEdit m_editBarcode;
};
