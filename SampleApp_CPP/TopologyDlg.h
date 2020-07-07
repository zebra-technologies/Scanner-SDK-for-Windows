/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#pragma once
#include "afxcmn.h"
#include "resource.h"	

/***
	Synopsis:		A class used to simplify operations with a Tree control and Image List used to
					describe Device Topology information. To support more scanners add the related
					icons to the project in the format "scannermodel.ico" and include them into the
					m_ResNames map.

	Author:			VRQW74
	Date:			13th July 2010
***/

class CDeviceTree : public CTreeCtrl
{
public:

	#define UNKNOWN_SCANNER_INDEX	0
	#define UNKNOWN_CRADLE_INDEX	1
	#define CRADLE_INDEX			2

	CDeviceTree()
	{
	}

	void CreateDeviceTree()
	{
		m_ResNames.SetSize(13);

		m_ResNames[UNKNOWN_SCANNER_INDEX]	=  _T("Unknown"); 
		m_ResNames[UNKNOWN_CRADLE_INDEX]	=  _T("UnknownC");
		m_ResNames[CRADLE_INDEX]			=  _T("STB");
		m_ResNames[3]						=  _T("LS2208");
		m_ResNames[4]						=  _T("LS3408");
		m_ResNames[5]						=  _T("DS9808");
		m_ResNames[6]						=  _T("LS9203");
		m_ResNames[7]						=  _T("LS4278");
		m_ResNames[8]						=  _T("LS3008");
		m_ResNames[9]						=  _T("LS3578");
		m_ResNames[10]						=  _T("LS1203");
		m_ResNames[11]						=  _T("DS6707");
		m_ResNames[12]						=  _T("LS4208");
		// SetSize on m_ResNames and Add other scanner models here
		SetDeviceIcons();
	}


	void AddLeaf(int depth, LPCTSTR LeafText)
	{
		HTREEITEM hParent, hTreeNode;
		int ImageIndex = GetDeviceImageIndex(LeafText);
		
		BOOL status = m_TreeMap.Lookup(depth - 1, hParent);
		if(status == 0)
		{
			hTreeNode = InsertItem(LeafText, ImageIndex, ImageIndex, TVI_ROOT);
			if(m_hRoot == 0) m_hRoot = hTreeNode;
		}
		else
		{
			hTreeNode = InsertItem(LeafText, ImageIndex, ImageIndex, hParent, TVI_LAST);
			int x, y;
			GetItemImage(hParent, x, y);
			if((x == UNKNOWN_SCANNER_INDEX && y == UNKNOWN_SCANNER_INDEX) ||
				(x == UNKNOWN_CRADLE_INDEX && y == UNKNOWN_CRADLE_INDEX))

				SetItemImage(hParent, UNKNOWN_CRADLE_INDEX, UNKNOWN_CRADLE_INDEX);
			else
				SetItemImage(hParent, CRADLE_INDEX, CRADLE_INDEX);
		}
		m_TreeMap[depth] = hTreeNode;
	}

	void SetDeviceIcons()
	{
		int Count = (int)m_ResNames.GetCount();
		m_ImageList.Create(32, 32, ILC_COLOR32, 0, Count);
		for(int i = 0; i < Count; ++i)
		{
			m_ImageList.Add(AfxGetApp()->LoadIcon(m_ResNames[i]));
		}
		SetImageList(&m_ImageList, TVSIL_NORMAL);
	}

	int GetDeviceImageIndex(LPCTSTR LeafText)
	{
		int count = (int) m_ResNames.GetCount();
		CString leaf = LeafText;
		for(int i = 0; i < count; ++i)
		{
			if( leaf.Find(m_ResNames[i]) != -1)
				return i;
		}
		return UNKNOWN_SCANNER_INDEX;
	}

private:

	HTREEITEM m_hRoot;
	CImageList m_ImageList;
	CMap<int, int, HTREEITEM, HTREEITEM> m_TreeMap;
	CStringArray m_ResNames;

};


// CTopologyDlg dialog
class CTopologyDlg : public CDialog
{
	DECLARE_DYNAMIC(CTopologyDlg)

public:
	CTopologyDlg(CWnd* pParent = NULL);   
	virtual ~CTopologyDlg();

	void SetXML(BSTR xml)
	{
		m_XML = SysAllocString(xml);
	}

	enum { IDD = IDD_TOPOLOGY };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    

	DECLARE_MESSAGE_MAP()
public:
	virtual BOOL OnInitDialog();

private:
	CDeviceTree m_Tree;
	BSTR m_XML;
	afx_msg void OnClose();
	void Process();
};
