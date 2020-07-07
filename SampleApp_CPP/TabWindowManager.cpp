/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#include "StdAfx.h"
#include "TabWindowManager.h"

////////////////////////////////////////////////////////////////////////////////////////////
//CTabWindowManager Implementation
BEGIN_MESSAGE_MAP(CTabWindowManager, CTabCtrl)
	ON_NOTIFY_REFLECT_EX(TCN_SELCHANGE, OnTabChange)
END_MESSAGE_MAP()

CTabWindowManager::~CTabWindowManager()
{
	int TabDlgID;
	TABINFO TabInfo;
	POSITION pos = m_DlgMap.GetStartPosition();
	while (pos != NULL)
	{
		m_DlgMap.GetNextAssoc( pos, TabDlgID, TabInfo );
		if(TabInfo.TabDlg != 0)
		{
			delete TabInfo.TabDlg;			//Call virtual destructor
			TabInfo.TabDlg = 0;
		}
	}
}

void CTabWindowManager::CalculatePagePosition(CPoint& pt)
{
	CRect rectTabCtrl, rectDlg;
	m_pParentWnd->GetWindowRect(&rectDlg);	//w.r.t screen coords
	this->GetWindowRect(&rectTabCtrl);		//w.r.t screen coords
	pt.x = rectTabCtrl.left - rectDlg.left;
	pt.y = rectTabCtrl.top - rectDlg.top;
}

BOOL CTabWindowManager::OnTabChange(NMHDR *pNMHDR, LRESULT *pResult)
{
	ShowTab(GetCurFocus());
	*pResult = 0;
	return FALSE;
}
