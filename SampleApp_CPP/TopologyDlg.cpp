/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
// TopologyDlg.cpp : implementation file
//

#include "stdafx.h"
#include "TopologyDlg.h"
#include "QuickXmlParser.h"

IMPLEMENT_DYNAMIC(CTopologyDlg, CDialog)

CTopologyDlg::CTopologyDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CTopologyDlg::IDD, pParent)
{
	m_XML = 0;
}

CTopologyDlg::~CTopologyDlg()
{
}

void CTopologyDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_TREE1, m_Tree);
}


BEGIN_MESSAGE_MAP(CTopologyDlg, CDialog)
	ON_BN_CLICKED(IDC_BUTTON1, &CTopologyDlg::OnClose)
END_MESSAGE_MAP()

BOOL CTopologyDlg::OnInitDialog()
{
	if(m_XML == 0) 
	{
		EndDialog(0);
		return FALSE;
	}
	CDialog::OnInitDialog();
	m_Tree.CreateDeviceTree();
	Process();
	return TRUE;  
}

void CTopologyDlg::Process()
{
	CQuickXmlParser x(m_XML);

	CQuickXmlParser::TAGDATA tag[2] = {0};
	tag[0].Tag.Name = L"scanner";
	tag[1].Tag.Name = L"modelnumber";
	tag[0].Attribs[0].Name = L"type";

	x.Configure(tag, 2);

	CQuickXmlParser::xptr p = 0;
	while(1)
	{
		p = x.Parse(p);
		wchar_t *scnType = x.Translate(tag[0].AttribValues[0]);
		int depth = tag[0].Depth;
	
		if (wcscmp(scnType,L"USBHIDKB")==0)
		{
             m_Tree.AddLeaf(depth,L"Unknown");
		}
		else 
		{
			wchar_t *Model = x.Translate(tag[1].Value);
			if(*Model == 0) return;
			if(wcslen(Model) == 0) Model = L"Model Unavailable";
			m_Tree.AddLeaf(depth, Model);
		}
		if(p == 0) break;
		x.ClearValues(tag);
		
	}

}


void CTopologyDlg::OnClose()
{
	OnCancel();
}
