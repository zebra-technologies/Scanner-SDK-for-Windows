/*******************************************************************************************
*
* ©2024 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/

#pragma once
#include "stdafx.h"
#include "ScannerList.h"
#include <map>
#include <vector>
#include <string>
#include <set>


using namespace std;

/***
	Synopsis	: A class derived from CScannerListCtrl used to simplify operations
				  incorporating editable cell feature for RTA events get/set methods.

	Author		: NM2652

***/

class CRtaListCtrl : public CScannerListCtrl
{
	DECLARE_DYNAMIC(CRtaListCtrl)

public:
	CRtaListCtrl();
	virtual ~CRtaListCtrl();

	void ClearList(); // Method to clear all items and columns
	void SetHeader(int colNum, LPCTSTR header, int width, BOOL restricted);
	static vector<vector<wstring>>& gridData;
	static vector<vector<CString>>& tooltips;
	static set<int> restrictedCols;

	afx_msg BOOL OnToolNeedText(UINT id, NMHDR* pNMHDR, LRESULT* pResult);

	void CellHitTest(const CPoint& pt, int& nRow, int& nCol) const;
	bool ShowToolTip(const CPoint& pt) const;
	CString GetToolTipText(int nRow, int nCol);

protected:
	DECLARE_MESSAGE_MAP()
	afx_msg void OnPaint();
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnCustomDraw(NMHDR* pNMHDR, LRESULT* pResult);

	virtual INT_PTR OnToolHitTest(CPoint point, TOOLINFO* pTI) const;
	virtual void PreSubclassWindow();

private:
	void OnEndEdit();
	void DrawCheckbox(CDC* pDC, CListCtrl* pListCtrl, int nItem, int nSubItem, CRect& rect);
	BOOL IsCheckboxChecked(int nItem, int nSubItem);
	void ToggleCheckbox(int nItem, int nSubItem);
	BOOL IsCheckboxColumn(int nItem, int nSubItem);

	static vector<vector<wstring>> CRtaListCtrl::defaultData;
	static vector<vector<CString>> CRtaListCtrl::defaulttooltips;

	map<pair<int, int>, BOOL> m_CheckboxStates;
	// Edit control for in-place editing of list items
	CEdit m_editControl;

	// Tooltip control for displaying dynamic tooltips
	CToolTipCtrl m_toolTip;

	// Index of the item being edited
	int m_nEditingItem;

	// Index of the subitem being edited
	int m_nEditingSubItem;

	// Last item and subitem for which the tooltip was displayed
	int m_nLastItem;
	int m_nLastSubItem;

};