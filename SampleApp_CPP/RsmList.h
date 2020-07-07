/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/

#pragma once
#include "ScannerList.h"
/***
	Synopsis	: A class derived from CScannerListCtrl used to simplify operations
				  incorporating editable cell feature for RSM attribute get/set methods.

	Author		: VRQW74

***/

class CEditText : public CEdit
{
	DECLARE_MESSAGE_MAP()
public:
	afx_msg HBRUSH CtlColor(CDC* pDC, UINT );

protected:
	CBrush  m_brush;
};


class CRsmListCtrl : public CScannerListCtrl
{
	DECLARE_MESSAGE_MAP()

public:
	CRsmListCtrl()
	{
		m_EditInfo.m_pEdit = 0;
		m_EditInfo.m_Item = -1;
		m_EditInfo.m_SubItem = -1;
	}
	~CRsmListCtrl()
	{
		if (m_EditInfo.m_pEdit != 0) delete m_EditInfo.m_pEdit;
	}

	void StopEdit()
	{
		if(m_EditInfo.m_pEdit != 0)
		{
			m_EditInfo.m_pEdit->DestroyWindow();
			delete m_EditInfo.m_pEdit;
			m_EditInfo.m_pEdit = 0;
			m_EditInfo.m_Item = -1;
			m_EditInfo.m_SubItem = -1;
		}
	}


protected:
	afx_msg BOOL OnClickListView(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint pt);

private:
	struct EditInfo
	{
		CEditText	*m_pEdit;
		int		m_Item;
		int		m_SubItem;
	} m_EditInfo;

};