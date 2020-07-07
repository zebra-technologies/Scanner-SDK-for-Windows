/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#pragma once

/***
	Synopsis:		A class used to simplify operations with Tabbed Windows and factor out Windows
					specific stuff which might get in the way of understanding CoreScanner API
					usage.

	Assumptions:	Each Dialog window that constitutes a tab should expose:
					1. It's resource ID with enum name IDD
					2. A class method GetClassString() which returns the Tab Heading
					3. A GetTabManager method which returns a reference to this instance

	Author:			VRQW74
	Date:			13th July 2010
***/

class CTabWindowManager : public CTabCtrl
{
	DECLARE_MESSAGE_MAP()

public:
	CTabWindowManager() : m_TabIndex(0), m_pParentWnd(0)
	{
	}
	~CTabWindowManager();

	template <typename T> void SetTabClass(CWnd* pParentWindow, T* DlgClass = 0)
	{
		m_pParentWnd = pParentWindow;
		if(DlgClass == 0)
			DlgClass = new T;
		DlgClass->Create(T::IDD, pParentWindow);
		TCITEM tabItem;
		tabItem.mask = TCIF_TEXT;
		tabItem.pszText = DlgClass->GetClassString();
		InsertItem(m_TabIndex, &tabItem);
		TABINFO info;
		info.TabDlg = (CDialog*)DlgClass;
		info.TabDlgIndex = m_TabIndex++;
		m_DlgMap[T::IDD] = info;
	}
	//Show tab by tab index
	void ShowTab(int TabIndex)
	{
		int TabDlgID;
		TABINFO TabInfo;
		POSITION pos = m_DlgMap.GetStartPosition();
		while (pos != NULL)
		{
			m_DlgMap.GetNextAssoc( pos, TabDlgID, TabInfo );
			if(TabInfo.TabDlgIndex == TabIndex)
			{
				CPoint pt;
				CalculatePagePosition(pt);
				TabInfo.TabDlg->SetWindowPos(&wndTop, pt.x,	pt.y, 0, 0, SWP_NOSIZE|SWP_SHOWWINDOW);
				SetCurSel(TabInfo.TabDlgIndex);
			}
			else
				TabInfo.TabDlg->ShowWindow(SW_HIDE);
		}
	}
	//Show tab by Underlying dialog class
	template <typename T> void ShowTab(T* TabClass = 0)
	{
		int TabDlgID;
		TABINFO TabInfo;
		POSITION pos = m_DlgMap.GetStartPosition();
		while (pos != NULL)
		{
			m_DlgMap.GetNextAssoc( pos, TabDlgID, TabInfo );
			T* p = (T*)TabInfo.TabDlg;
			if(TabDlgID == T::IDD)
			{
				CPoint pt;
				CalculatePagePosition(pt);
				p->SetWindowPos(&wndTop, pt.x,	pt.y, 0, 0, SWP_NOSIZE|SWP_SHOWWINDOW);
				SetCurSel(TabInfo.TabDlgIndex);
			}
			else
				p->ShowWindow(SW_HIDE);
		}
	}

	template <typename T> T& GetTabDlg(T* TabClass = 0)
	{
		return *((T*)(m_DlgMap[T::IDD].TabDlg));
	}

	afx_msg BOOL OnTabChange(NMHDR *pNMHDR, LRESULT *pResult);

private:

	void CalculatePagePosition(CPoint& pt);

private:

	typedef struct _TABINFO
	{
		int TabDlgIndex;
		CDialog* TabDlg;

	} TABINFO;

	CMap<int, int, TABINFO, TABINFO> m_DlgMap;
	int m_TabIndex;
	CWnd *m_pParentWnd;
};
