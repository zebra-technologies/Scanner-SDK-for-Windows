#pragma once
#include "ScannerCommands.h"
#include "afxwin.h"
#include "EmbeddedBrowser.h"
#include "ImageVideoDlg.h"




// CScanToConnectDlg dialog

class CScanToConnectDlg : public CDialog
{
	DECLARE_DYNAMIC(CScanToConnectDlg)

public:
	CScanToConnectDlg(CWnd* pParent = NULL);   // standard constructor
	virtual ~CScanToConnectDlg();
	void SetScannerID(wstring * ScannerID);
	wstring SelectedScannerID;
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
	virtual BOOL OnInitDialog();
	void SetAsync(int *ParaAsync);
	void OnParameterBarcode(LPBYTE MediaBuffer, LONG BufferSize);
	void RequestPairingBarcode();
	void EnableUIControllers(BOOL state);

// Dialog Data
	enum { IDD = IDD_ScanToConnect };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()
	DECLARE_TAB_WINDOW(ScanToConnect)
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
	int Async;
	CBrush  m_brush;
	CRender m_RenderEngine;
	IMAGE_DATA m_ImageData;

public:
	afx_msg void OnCbnSelchangeComboScannertype();
	CComboBox m_cmbScannerType;
	CComboBox m_cmbProtocolName;
	CComboBox m_cmbHostName;
	CComboBox m_cmbDefaultOption;
	CComboBox m_cmbImageSize;
	CStatic m_picPairingBarcode;
	afx_msg void OnBnClickedButtonSavebarcode();
	CButton m_btnSaveBarcode;
};
