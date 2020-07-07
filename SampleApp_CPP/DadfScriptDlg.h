/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/

#pragma once
#include "afxcmn.h"
#include "resource.h"	


class CDadfScriptDlg : public CDialog
{
	DECLARE_DYNAMIC(CDadfScriptDlg)

public:

	CDadfScriptDlg(CStringW& rSource, CWnd* pParent = NULL);   
	virtual ~CDadfScriptDlg();

	enum { IDD = IDD_DadfScript };

protected:

	virtual void DoDataExchange(CDataExchange* pDX);    
	DECLARE_MESSAGE_MAP()

public:

	virtual BOOL OnInitDialog();
	afx_msg void OnClickCancel();
	afx_msg void OnClickApply();

protected:

	CRichEditCtrl m_edit;
	CStringW&	  m_Source;

};
