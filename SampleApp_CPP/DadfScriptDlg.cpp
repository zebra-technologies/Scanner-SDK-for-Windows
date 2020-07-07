/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates..
*
********************************************************************************************/
// TopologyDlg.cpp : implementation file
//

#include "stdafx.h"
#include "DadfScriptDlg.h"

IMPLEMENT_DYNAMIC(CDadfScriptDlg, CDialog)

CDadfScriptDlg::CDadfScriptDlg(CStringW& rSource, CWnd* pParent /*=NULL*/ )
	: CDialog(CDadfScriptDlg::IDD, pParent), m_Source(rSource)
{
}

CDadfScriptDlg::~CDadfScriptDlg()
{
}

void CDadfScriptDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_RICHEDIT21, m_edit);
}


BEGIN_MESSAGE_MAP(CDadfScriptDlg, CDialog)
	ON_BN_CLICKED(IDCANCEL, &CDadfScriptDlg::OnClickCancel)
	ON_BN_CLICKED(IDOK, &CDadfScriptDlg::OnClickApply)
END_MESSAGE_MAP()

BOOL CDadfScriptDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	CFont fo;
	fo.CreateFontW(20, 0, 0, 0, FW_BOLD, FALSE, FALSE, FALSE, ANSI_CHARSET, OUT_DEFAULT_PRECIS, CLIP_DEFAULT_PRECIS, CLEARTYPE_QUALITY, DEFAULT_PITCH | FF_SWISS, _T("Consolas") );
	m_edit.SetFont(&fo);

	m_edit.SetWindowText(m_Source);

	return TRUE;  
}


void CDadfScriptDlg::OnClickCancel()
{
	OnCancel();
}

void CDadfScriptDlg::OnClickApply()
{
	m_edit.GetWindowText(m_Source);
	OnOK();
}


