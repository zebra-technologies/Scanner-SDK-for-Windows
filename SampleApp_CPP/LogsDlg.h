/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/

#pragma once
#include "ScannerCommands.h"
#include "afxwin.h"
#include "EmbeddedBrowser.h"


class CLogsDlg : public CDialog
{
	DECLARE_DYNAMIC(CLogsDlg)

public:
	CLogsDlg(CWnd* pParent = NULL);   
	virtual ~CLogsDlg();

	void ShowResult(wstring result);
	void ShowOutXml(BSTR outXml);

	enum { IDD = IDD_Logs };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);   

	DECLARE_MESSAGE_MAP()
	DECLARE_TAB_WINDOW(Logs)

public:
	afx_msg void OnClearEventLog();
	afx_msg void OnClearXmlLog();

	void UpdateResults(CString& result);
	void DisplayResult(long status, wchar_t* Cmd);


public:
	virtual BOOL OnInitDialog();
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);

	CEdit txtResultLog;

private:
	CBrush  m_brush;
	int m_Count;
public:
	CEdit m_editLogs;
};
