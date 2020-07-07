/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#pragma once
#include "ScannerCommands.h"
#include "afxwin.h"
#include "ImageVideoDlg.h"


//Simple struct to store image data for saving cached images.
typedef struct _DOCCAP_IMAGE
{
	LPBYTE ImageData;
	DWORD  ImageSize;

} DOCCAP_IMAGE_DATA;

// CIntelDocCap dialog

class CIntelDocCap : public CDialog
{
	DECLARE_DYNAMIC(CIntelDocCap)

public:
	CIntelDocCap(CWnd* pParent = NULL);   // standard constructor
	virtual ~CIntelDocCap();
	void OnBinaryDataEventCapture(LPBYTE MediaBuffer, LONG BufferSize, SHORT DataFormat, BSTR* bstrScannerData);

// Dialog Data
	enum { IDD = IDD_IntelDocCap };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()
	DECLARE_TAB_WINDOW(IDC)
	
	
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);

	void ClearImageCache()
	{
		if(m_ImageData.ImageData)
		{
			delete[] m_ImageData.ImageData;
			m_ImageData.ImageData = 0;
		}
		m_ImageData.ImageSize = 0;
	}

private:
	CBrush  m_brush;
	DOCCAP_IMAGE_DATA m_ImageData;
	CRender m_RenderEngine;
	void InitDocCapConfCombo();
	int Async;
	std::wstring RSMGet(int opcode);
	int RSMSet(int opcode, wstring value, wstring dataType, bool isStore = false);
	::wstring wsSelectedDataType;
	LPCTSTR GetSymbology(int Code);
	VOID GetImageType(CString &Ext, CString &Filter);

public:
	CEdit m_edit_DecodData;
	virtual BOOL OnInitDialog();
	wstring SelectedScannerID;

	CStatic m_PicControl;
	// DocCap Configurations
	CComboBox m_cmbDocCapConf;
	afx_msg void OnCbnSelchangeComboIdcParam();
	// DOcCap configuration values
	CEdit m_edit_DocCapConfValue;
	void SetScannerID(std::wstring *ScannerID);
	void SetAsync(int *ParaA);
	afx_msg void OnBnClickedButtonIdcParamSet();
	afx_msg void OnBnClickedButtonIdcParamGet();
	afx_msg void OnBnClickedButtonIdcParamStore();
	// Doc Cap Barode Symbology Text Box
	CEdit m_edit_DocCapBarcodeSymbology;
	afx_msg void OnBnClickedButtonIdcClear();
	afx_msg void OnBnClickedCheckUseHid();
	// USE HID Check box
	CButton m_chkUseHID;
	afx_msg void OnBnClickedButtonIdcSaveImage();
};
