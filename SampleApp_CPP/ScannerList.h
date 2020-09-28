/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/

#pragma once

/***
	Synopsis	: A class used to simplify operations with the List Control
	Author		: VRQW74

***/
class CScannerListCtrl : public CListCtrl
{
public:

	CScannerListCtrl()
	{
		ColumnCount = 0;
		m_nItem = 0;
		SelIndicesArray = 0;
	}

	~CScannerListCtrl()
	{
		if(SelIndicesArray)
		{
			delete [] SelIndicesArray;
			SelIndicesArray = 0;
		}
	}

	void SetHeader(LPCTSTR header, int width)
	{
		InsertColumn(++ColumnCount, header, 10, width);
		SetExtendedStyle(LVS_EX_FULLROWSELECT| LVS_EX_GRIDLINES);
	}

	void SetField(int column, LPTSTR value)
	{
		if(column == 0)
		{
			LVITEM lvItem;
			lvItem.mask = LVIF_TEXT;
			lvItem.iItem = 0;
			lvItem.iSubItem = 0;
			lvItem.lParam = _ttoi(value);
			lvItem.pszText = value;
			m_nItem = InsertItem(&lvItem);
		}
		else
			SetItemText(m_nItem, column, value);
	}

	int* GetSelectedIndices()
	{
		int  nItem = -1;
		if(SelIndicesArray)
		{
			delete [] SelIndicesArray;
			SelIndicesArray = 0;
		}

		UINT u = GetSelectedCount();
		UINT i;
		if (u > 0)
		{
			std::size_t indices_array_size = u + (std::size_t)1;
			SelIndicesArray = new int[indices_array_size];
			for (i = 0; i < u; i++)
			{
				nItem = GetNextItem(nItem, LVNI_SELECTED);
				SelIndicesArray[i] = nItem;
			}
			SelIndicesArray[i] = -1;
		}
		return SelIndicesArray;

	}

	static int CALLBACK SortComp(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort)
	{
		return (int)(lParam2 - lParam1);
	}

	void Sort()
	{
		ListView_SortItemsEx(this->m_hWnd, CScannerListCtrl::SortComp, 0);
	}


private:
	int ColumnCount;
	int m_nItem;
	int *SelIndicesArray;
};
