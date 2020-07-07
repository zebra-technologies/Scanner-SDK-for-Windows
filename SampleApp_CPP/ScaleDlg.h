/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#pragma once
#include "ScannerCommands.h"
#include "afxwin.h"
#include "EmbeddedBrowser.h"

// CScaleDlg dialog

class CScaleDlg : public CDialog
{
	DECLARE_DYNAMIC(CScaleDlg)

public:
	CScaleDlg(CWnd* pParent = NULL);   
	virtual ~CScaleDlg();
	void SetScannerID(wstring * ScannerID);
	wstring SelectedScannerID;
	int Async;
	void SetAsync(int *ParaAsync);
// Dialog Data
	enum { IDD = IDD_Scale };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()
	DECLARE_TAB_WINDOW(Scale)
public:
	afx_msg void OnBnClickedButtonReadweight();
	afx_msg void OnBnClickedButtonZeroscale();
	afx_msg void OnBnClickedButtonScalereset();
	
	CStatic m_statWeight;		// Weight value in Kilograms or pounds
	CStatic m_statWeightUnit;	// Weight unit is KG or LB
	CStatic m_statStatusDesc;	// Status Description

	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);

private:
	CBrush  m_brush;
};
