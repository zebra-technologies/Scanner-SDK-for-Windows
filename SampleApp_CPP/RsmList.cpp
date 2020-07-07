/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#include "stdafx.h"
#include "RsmList.h"


BEGIN_MESSAGE_MAP(CEditText, CEdit)
	ON_WM_CTLCOLOR_REFLECT()
END_MESSAGE_MAP()

HBRUSH CEditText::CtlColor(CDC* pDC, UINT /*nCtlColor*/)
{
	if(!m_brush.m_hObject)
		m_brush.CreateSolidBrush(RGB(255, 255, 190));

	return m_brush;
}
//-------------------------------------------------------------------------------------
BEGIN_MESSAGE_MAP(CRsmListCtrl, CScannerListCtrl)
	ON_NOTIFY_REFLECT_EX(NM_CLICK, OnClickListView)
	ON_WM_VSCROLL()
	ON_WM_MOUSEWHEEL()
END_MESSAGE_MAP()


BOOL CRsmListCtrl::OnClickListView(NMHDR *pNMHDR, LRESULT *pResult)
{
	CPoint pt;
	CRect rec;
	CString sEditVal;
	LPNMLISTVIEW p = (LPNMLISTVIEW)pNMHDR;

	if(p->iItem == -1) goto End; //Clicked on a header ctrl
	if(m_EditInfo.m_pEdit != 0)
	{
		m_EditInfo.m_pEdit->GetWindowText(sEditVal);
		SetItemText(m_EditInfo.m_Item, m_EditInfo.m_SubItem, sEditVal);
		StopEdit();
	}

	if(p->iSubItem == 3)
	{
		GetSubItemRect(p->iItem, p->iSubItem, LVIR_BOUNDS, rec);
		if(m_EditInfo.m_pEdit == 0)
		{
			m_EditInfo.m_pEdit = new CEditText();
			m_EditInfo.m_pEdit->Create(WS_CHILD | WS_VISIBLE | WS_BORDER | ES_CENTER, rec, this, 999);
			m_EditInfo.m_pEdit->SetWindowText(GetItemText(p->iItem, p->iSubItem));
			m_EditInfo.m_pEdit->SetSel(0, -1);
			m_EditInfo.m_pEdit->SetFocus();
		}

		m_EditInfo.m_Item = p->iItem;
		m_EditInfo.m_SubItem = p->iSubItem;
	}

End:
	*pResult = 0;
	return FALSE;
}
void CRsmListCtrl::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
	StopEdit();
	CScannerListCtrl::OnVScroll(nSBCode, nPos, pScrollBar);
}

BOOL CRsmListCtrl::OnMouseWheel(UINT nFlags, short zDelta, CPoint pt)
{
	StopEdit();
	return CScannerListCtrl::OnMouseWheel(nFlags, zDelta, pt);
}

//-------------------------------------------------------------------------------------
