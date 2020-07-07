/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#pragma once
#include "ScannerCommands.h"
#include "afxcmn.h"
#include "RsmList.h"

class CRSMDlg : public CDialog
{
	DECLARE_DYNAMIC(CRSMDlg)

public:

	//Constructor/Destructor
	CRSMDlg(CWnd* pParent = NULL);   
	virtual ~CRSMDlg();

	void SetScannerID(wstring * ScannerID);
	void SetAsync(int *ParaAsync);

	//Utility functions to prepare RSM command parameters.
	void MakeSelectedRsmIdList(wstring &AttributeList);
	void MakeIdValueAttributeXml(wstring &AttributeList);

	//These functions directly manipulate the RSM grid control
	bool SetGridIDs(BSTR outXml);
	void SetGridAttributes(BSTR outXml);
	bool SetGridFromAsyncResponse(BSTR outXml);
	void InitAttribList();
	afx_msg void OnSelectAll();
	afx_msg void OnClearAll();


	enum { IDD = IDD_RSM };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    
		
	DECLARE_MESSAGE_MAP()
	DECLARE_TAB_WINDOW(RSM)

public:

	afx_msg void OnGetAllAttributeIDs();
	afx_msg void OnGetAttributeInfo();
	afx_msg void OnGetNextAttributeInfo();


	//Issues RSM set/store commands
	afx_msg void OnSetAttribute();
	afx_msg void OnStoreAttribute();

	//Dialog specific functions
	virtual BOOL OnInitDialog();
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);

private:

	int Async;
	wstring SelectedScannerID;
	CRsmListCtrl RsmGridControl;
	CBrush  m_brush;
};
