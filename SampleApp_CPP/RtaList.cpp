/*******************************************************************************************
*
* ©2024 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#include "stdafx.h"
#include "RtaList.h"
#include <vector>
#include <string>

/***
    Synopsis	: A class derived from CScannerListCtrl used to simplify operations
                  incorporating editable cell feature for RTA events get/set methods.

    Author		: NM2652

***/

// Implementation of the dynamic class CRtaListCtrl, which inherits from CListCtrl
IMPLEMENT_DYNAMIC(CRtaListCtrl, CListCtrl)

// Static members initialization for CRtaListCtrl

// Default data for the grid, represented as a 2D vector of strings
vector<vector<wstring>> CRtaListCtrl::defaultData;
vector<vector<CString>> CRtaListCtrl::defaulttooltips;

// Reference to the grid data, initialized to point to the default data
vector<vector<wstring>>& CRtaListCtrl::gridData = CRtaListCtrl::defaultData;
vector<vector<CString>>& CRtaListCtrl::tooltips = CRtaListCtrl::defaulttooltips;

// Set to store indices of restricted columns where editing or other operations are restricted
set<int> CRtaListCtrl::restrictedCols = {};

// Constructor for CRtaListCtrl
CRtaListCtrl::CRtaListCtrl() 
{
    // Create the tooltip control
    if (!m_toolTip.Create(this))
    {
        TRACE0("Failed to create tooltip control\n");
    }

    // Add the tool (the entire control in this case)
    m_toolTip.AddTool(this, _T(""));

    // Activate the tooltip control
    m_toolTip.Activate(TRUE);
}

// Destructor for CRtaListCtrl
CRtaListCtrl::~CRtaListCtrl() {}

// Message map for CRtaListCtrl, which maps Windows messages to their handler functions
BEGIN_MESSAGE_MAP(CRtaListCtrl, CListCtrl)
    // Message handler for the WM_PAINT message, which is called when the control needs to be repainted
    ON_WM_PAINT()

    // Message handler for the WM_LBUTTONDOWN message, which is called when the left mouse button is clicked
    ON_WM_LBUTTONDOWN()

    // Message handler for the NM_CUSTOMDRAW notification, which allows custom drawing of items and subitems
    ON_NOTIFY_REFLECT(NM_CUSTOMDRAW, &CRtaListCtrl::OnCustomDraw)

    // Message handler for the EN_KILLFOCUS notification, which is sent when the edit control loses focus
    // The ID '1' corresponds to the ID used when creating the edit control
    ON_EN_KILLFOCUS(1, &CRtaListCtrl::OnEndEdit)

    ON_NOTIFY_EX(TTN_NEEDTEXTA, 0, OnToolNeedText)
    ON_NOTIFY_EX(TTN_NEEDTEXTW, 0, OnToolNeedText)
END_MESSAGE_MAP()

void CRtaListCtrl::SetHeader(int colNum, LPCTSTR header, int width, BOOL restricted)
{
    // Call the base class method to set the header text and column width
    CScannerListCtrl::SetHeader(header, width);

    // If the 'restricted' flag is TRUE, add the column number to the restrictedCols set
    // This set is used to determine which columns should be non-editable
    if (restricted)
    {
        restrictedCols.insert(colNum);
    }
}

void CRtaListCtrl::PreSubclassWindow()
{
    CListCtrl::PreSubclassWindow();

    // Disable the CToolTipCtrl of CListCtrl so it won't disturb the CWnd tooltip
    GetToolTips()->Activate(FALSE);

    // Activate the standard CWnd tooltip functionality
    VERIFY(EnableToolTips(TRUE));
}

void CRtaListCtrl::CellHitTest(const CPoint& pt, int& nRow, int& nCol) const
{
    nRow = -1;
    nCol = -1;

    LVHITTESTINFO lvhti = { 0 };
    lvhti.pt = pt;
    nRow = ListView_SubItemHitTest(m_hWnd, &lvhti);	// SubItemHitTest is non-const
    nCol = lvhti.iSubItem;
    if (!(lvhti.flags & LVHT_ONITEMLABEL))
        nRow = -1;
}

BOOL CRtaListCtrl::OnToolNeedText(UINT id, NMHDR* pNMHDR, LRESULT* pResult)
{
    CPoint pt(GetMessagePos());
    ScreenToClient(&pt);

    int nRow, nCol;
    CellHitTest(pt, nRow, nCol);

    CString tooltip = GetToolTipText(nRow, nCol);
    if (tooltip.IsEmpty())
        return FALSE;

    // Non-unicode applications can receive requests for tooltip-text in unicode
    TOOLTIPTEXTA* pTTTA = (TOOLTIPTEXTA*)pNMHDR;
    TOOLTIPTEXTW* pTTTW = (TOOLTIPTEXTW*)pNMHDR;
#ifndef _UNICODE
    if (pNMHDR->code == TTN_NEEDTEXTA)
        lstrcpyn(pTTTA->szText, static_cast<LPCTSTR>(tooltip), sizeof(pTTTA->szText));
    else
        _mbstowcs(pTTTW->szText, static_cast<LPCTSTR>(tooltip), sizeof(pTTTW->szText) / sizeof(WCHAR));
#else
    if (pNMHDR->code == TTN_NEEDTEXTA)
        _wcstombsz(pTTTA->szText, static_cast<LPCTSTR>(tooltip), sizeof(pTTTA->szText));
    else
        lstrcpyn(pTTTW->szText, static_cast<LPCTSTR>(tooltip), sizeof(pTTTW->szText) / sizeof(WCHAR));
#endif
    // If wanting to display a tooltip which is longer than 80 characters,
    // then one must allocate the needed text-buffer instead of using szText,
    // and point the TOOLTIPTEXT::lpszText to this text-buffer.
    // When doing this, then one is required to release this text-buffer again
    return TRUE;
}

bool CRtaListCtrl::ShowToolTip(const CPoint& pt) const
{
    // Lookup up the cell
    int nRow, nCol;
    CellHitTest(pt, nRow, nCol);

    if (nRow != -1 && nCol != -1)
        return true;
    else
        return false;
}

CString CRtaListCtrl::GetToolTipText(int nRow, int nCol)
{
    try {

        if (nRow != -1 && nCol != -1 && !IsCheckboxColumn(nRow, nCol) && nCol != 0) {
            for (const auto& tooltip : tooltips) {
                // Check if the row's item texts match the current tooltip's criteria
                if (
                    (GetItemText(nRow, 2) == tooltip[0] && GetItemText(nRow, 3) == tooltip[1]) || 
                    (GetItemText(nRow, 1) == tooltip[0] && GetItemText(nRow, 2) == tooltip[1]) ||
                    (GetItemText(nRow, 3) == tooltip[0] && GetItemText(nRow, 4) == tooltip[1])
                    ) {
                    // Return the tooltip text corresponding to the column
                    if (nCol >= 1 && nCol < tooltip.size()) {
                        if (GetItemText(nRow, 1) == tooltip[0] && GetItemText(nRow, 2)) return tooltip[nCol + 1];
                        if ((GetItemText(nRow, 3) == tooltip[0] && GetItemText(nRow, 4) == tooltip[1])) {
                            if (nCol < 3 || nCol > 4) return L"";
                            else return tooltip[nCol - 1];
                        }
                        else return tooltip[nCol];
                    }
                    else {
                        // If nCol is out of range, return the cell text
                        return GetItemText(nRow, nCol);
                    }
                }
            }
            // If no matching tooltip is found, return the cell text
            return GetItemText(nRow, nCol);
        }
        // Handle invalid row or column
        return _T("");
    }
    catch (exception ex){
        throw ex;
        return _T("");
    }
}

INT_PTR CRtaListCtrl::OnToolHitTest(CPoint point, TOOLINFO* pTI) const
{
    CPoint pt(GetMessagePos());
    ScreenToClient(&pt);
    if (!ShowToolTip(pt))
        return -1;

    int nRow, nCol;
    CellHitTest(pt, nRow, nCol);

    //Get the client (area occupied by this control
    RECT rcClient;
    GetClientRect(&rcClient);

    //Fill in the TOOLINFO structure
    pTI->hwnd = m_hWnd;
    pTI->uId = (UINT)(nRow * 1000 + nCol);
    pTI->lpszText = LPSTR_TEXTCALLBACK;	// Send TTN_NEEDTEXT when tooltip should be shown
    pTI->rect = rcClient;

    return pTI->uId; // Must return a unique value for each cell (Marks a new tooltip)
}


void CRtaListCtrl::OnPaint()
{
    CPaintDC dc(this);
    CListCtrl::DefWindowProc(WM_PAINT, (WPARAM)dc.GetSafeHdc(), 0);
}


void CRtaListCtrl::OnLButtonDown(UINT nFlags, CPoint point)
{
    // Call the base class handler to manage default behavior
    CListCtrl::OnLButtonDown(nFlags, point);

    // Identify the item (row) that was clicked
    int nItem = HitTest(point);
    if (nItem != -1)
    {
        // Loop through all the columns in the row
        for (int i = 0; i < GetHeaderCtrl()->GetItemCount(); ++i)
        {
            CRect rect;
            // Get the rectangle area for the specific subitem (cell) in the current column
            GetSubItemRect(nItem, i, LVIR_LABEL, rect);

            // Check if the mouse click occurred within the bounds of this subitem
            if (rect.PtInRect(point))
            {
                // Check if the clicked column is not restricted
                if (restrictedCols.find(i) == restrictedCols.end() && GetItemText(nItem, i) != "Not applicable")
                {
                    // Check if the column is not a checkbox column
                    if (!IsCheckboxColumn(nItem, i))
                    {
                        // Create an edit control for the cell if it doesn't already exist
                        if (!m_editControl.GetSafeHwnd())
                        {
                            m_editControl.Create(WS_CHILD | WS_BORDER | ES_AUTOHSCROLL, rect, this, 1);
                        }
                        else
                        {
                            // Position the edit control over the clicked cell
                            m_editControl.SetWindowPos(nullptr, rect.left, rect.top, rect.Width(), rect.Height(), SWP_NOZORDER | SWP_NOACTIVATE);
                        }

                        // Set the current text of the cell in the edit control
                        m_editControl.SetWindowText(GetItemText(nItem, i));
                        m_editControl.ShowWindow(SW_SHOW);
                        m_editControl.SetFocus();
                        m_editControl.SetSel(0, -1); // Select all text within the edit control

                        // Store the item and subitem indices that are being edited
                        m_nEditingItem = nItem;
                        m_nEditingSubItem = i;
                        return;
                    }
                    else
                    {
                        // Handle checkbox toggling in checkbox columns
                        ToggleCheckbox(nItem, i - 1); // Adjust column index for checkbox
                        InvalidateRect(rect); // Redraw the cell to reflect the checkbox state change
                    }
                    break; // Exit the loop after handling the click
                }
            }
        }
    }
}

void CRtaListCtrl::OnCustomDraw(NMHDR* pNMHDR, LRESULT* pResult)
{
    // Cast the NMHDR pointer to a NMLVCUSTOMDRAW pointer to access custom draw information
    LPNMLVCUSTOMDRAW pLVCD = reinterpret_cast<LPNMLVCUSTOMDRAW>(pNMHDR);

    switch (pLVCD->nmcd.dwDrawStage)
    {
    case CDDS_PREPAINT:
        // Request notification for each item to be drawn
        *pResult = CDRF_NOTIFYITEMDRAW;
        break;

    case CDDS_ITEMPREPAINT:
        // Request notification for each subitem to be drawn
        *pResult = CDRF_NOTIFYSUBITEMDRAW;
        break;

    case CDDS_ITEMPREPAINT | CDDS_SUBITEM:
        // Check if the current subitem is in a checkbox column
        if (IsCheckboxColumn(pLVCD->nmcd.dwItemSpec, pLVCD->iSubItem))
        {
            // Get a device context (DC) from the custom draw structure
            CDC* pDC = CDC::FromHandle(pLVCD->nmcd.hdc);
            CRect rect;

            // Get the rectangle area of the subitem
            GetSubItemRect(pLVCD->nmcd.dwItemSpec, pLVCD->iSubItem, LVIR_LABEL, rect);

            // Draw the checkbox in the subitem's rectangle
            DrawCheckbox(pDC, this, pLVCD->nmcd.dwItemSpec, pLVCD->iSubItem, rect);

            // Skip the default drawing for this subitem (since we've custom drawn it)
            *pResult = CDRF_SKIPDEFAULT;
        }
        else
        {
            // Use the default drawing for subitems that aren't checkbox columns
            *pResult = CDRF_DODEFAULT;
        }
        break;

    default:
        // Use the default handling for all other stages
        *pResult = CDRF_DODEFAULT;
        break;
    }
}

void CRtaListCtrl::DrawCheckbox(CDC* pDC, CListCtrl* pListCtrl, int nItem, int nSubItem, CRect& rect)
{
    // Determine if the checkbox should be checked or unchecked
    // The checkbox state is determined based on the data in the grid
    BOOL bChecked = IsCheckboxChecked(nItem, nSubItem - 1);

    // Set the state of the checkbox
    // DFCS_CHECKED indicates a checked checkbox
    // DFCS_BUTTONCHECK indicates an unchecked checkbox
    UINT uState = bChecked ? DFCS_CHECKED : DFCS_BUTTONCHECK;

    // Draw the checkbox in the specified rectangle with a flat appearance
    pDC->DrawFrameControl(rect, DFC_BUTTON, uState | DFCS_FLAT);
}

BOOL CRtaListCtrl::IsCheckboxChecked(int nItem, int nSubItem)
{
    // Check if the item and subitem indices are within valid bounds
    if (nItem >= 0 && nItem < gridData.size() && nSubItem >= 0 && nSubItem < gridData[nItem].size())
    {
        // Retrieve the text stored in the grid at the specified item and subitem
        wstring itemText = gridData[nItem][nSubItem];

        // Return TRUE if the text is "TRUE", indicating the checkbox is checked
        // Otherwise, return FALSE, indicating the checkbox is unchecked
        return itemText == L"TRUE" ? TRUE : FALSE;
    }

    // Return FALSE if the item or subitem indices are out of bounds or invalid
    return FALSE;
}

void CRtaListCtrl::ToggleCheckbox(int nItem, int nSubItem) {
    // Check if the item and subitem indices are within valid bounds
    if (nItem >= 0 && nItem < gridData.size() && nSubItem >= 0 && nSubItem < gridData[nItem].size())
    {
        // Get a reference to the value in the grid at the specified item and subitem
        wstring& targetValue = gridData[nItem][nSubItem];

        // Toggle the value between "TRUE" and "FALSE"
        // If the current value is "TRUE", change it to "FALSE"
        targetValue = (targetValue == L"TRUE") ? L"FALSE" : L"TRUE";
    }
}

BOOL CRtaListCtrl::IsCheckboxColumn(int nItem, int nSubItem)
{
    CString itemText = GetItemText(nItem, nSubItem);

    // Determine if the column is a checkbox based on the item text
    return (itemText == _T("TRUE") || itemText == _T("FALSE"));
}

void CRtaListCtrl::ClearList()
{
    restrictedCols.clear(); //Clear the restricted columns set
    
    DeleteAllItems(); // Delete all items

    // Delete all columns
    int nColumnCount = GetHeaderCtrl()->GetItemCount();
    for (int i = nColumnCount - 1; i >= 0; --i)
    {
        DeleteColumn(i);
    }

    // Clear checkbox states
    m_CheckboxStates.clear();
}

void CRtaListCtrl::OnEndEdit()
{
    CString text;
    m_editControl.GetWindowText(text);

    if (m_nEditingItem >= 0 && m_nEditingItem < gridData.size() && m_nEditingSubItem >= 0 && m_nEditingSubItem-1 < gridData[m_nEditingItem].size())
    {
        // Update the grid data
        gridData[m_nEditingItem][m_nEditingSubItem-1] = text;

        // Update the display
        SetItemText(m_nEditingItem, m_nEditingSubItem, text);
    }

    // Hide the edit control
    m_editControl.ShowWindow(SW_HIDE);
}
