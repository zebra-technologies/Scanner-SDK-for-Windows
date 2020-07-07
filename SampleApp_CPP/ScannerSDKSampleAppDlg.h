/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#pragma once

#include "afxcmn.h"
#include "afxwin.h"
#include "ScannerSDKSampleApp.h"
#include "TabWindowManager.h"
#include "ScannerList.h"

enum HostMode
{
	MODE_ALL = -1,
	MODE_IBMHID = 0,
	MODE_HIDKB = 1,
	MODE_USB_OPOS = 2,
	MODE_SNAPI_IMG = 3,
	MODE_SNAPI_NO_IMG = 4,
	MODE_USB_CDC = 5,
	MODE_USB_SSI_CDC = 6,
	MODE_IBMTT = 7,
	MODE_SSI = 8,
	MODE_SSI_BT =9
};

//This structure is the basic unit of a Scanner List which contains Information for all
//discovered scanners
typedef struct _SCANNER
{
	CString Type;
	int ID;
	CString Model;
	CString Serial;
	CString GUID;
	CString VID;
	CString PID;
	CString DoM;
	CString Firmware;
	int HostMode;

} SCANNER, *PSCANNER;

//This map stores the scanner IDs and corresponding SCANNER objects.
typedef CMap<int, int, SCANNER, SCANNER>	ScannerMap;
//This map stores RSM attribute IDs and their corresponding values.
typedef CMap<int, int, std::wstring, std::wstring> RsmMap;

class CScannerSDKSampleAppDlg : public CDialog
{
public:
	
	CScannerSDKSampleAppDlg(CWnd* pParent = NULL);	
	~CScannerSDKSampleAppDlg();

	void UpdateScannerStatus();
	void StartupControllers();
	void CheckAllScannerTypes(bool res);
	void ClearAll();
	bool ShowScanners(BSTR outXml);
	bool AddScannersOnPnP(BSTR outXml);
	bool RemoveScannerOnPnP(BSTR outXml);
	void RefreshScannerList();
	bool QueryRsmValues(std::wstring ScannerID, RsmMap &RsmValueMap);

	//Scanner event routers
	void OnScanData(short reserved, BSTR scanData);
	void OnImageEvent(LPBYTE MediaBuffer, LONG BufferSize);
	void ParameterBarcodeEvent(LPBYTE MediaBuffer, LONG BufferSize);
	void OnBinaryDataEvent(LPBYTE MediaBuffer, LONG BufferSize, SHORT DataFormat, BSTR* bstrScannerData);
	void OnVideoEvent(LPBYTE MediaBuffer, LONG BufferSize);	
	void OnCmdResponse(short status, BSTR scanCmdResponse);
	void OnPNP(short eventType, BSTR PnpXml);
	void OnRMD(short eventType, BSTR eventData);
	void OnNotificationEvent(short notificationType,BSTR pScannerData);
	void OnIOResponse(short type, unsigned char data);
    void UpdateScannerClaimedStatus();
    void UpdateDisabledScannerStatus();
    bool ScannerListContains(int scnID);
	enum { IDD = IDD_SCANNERSDKSAMPLEAPP_DIALOG };

protected:

	virtual void DoDataExchange(CDataExchange* pDX);	
	virtual BOOL OnInitDialog();
	virtual void OnOK();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnListItemchanged(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
	afx_msg void OnSelectChangeScannerCombo();

	DECLARE_MESSAGE_MAP()

public:

	CTabWindowManager& GetTabManager(){ return m_TabWndMgr;}

	void InitScannerListControl();
	void SynchScannerCombo();

	void SetCommandMode(int Async);
	bool TranslateProtocolNames(SCANNER& Scanner, CString& TranslatedName, wchar_t *Name);
	SCANNER* GetScannerInfo(std::wstring ScannerID);

	afx_msg void OnDiscoverScanners();
	afx_msg void OnPullTrigger();
	afx_msg void OnReleaseTrigger();

public:
	CEdit txtStatus;
	
	CList<int,int>	m_DisabledScannerList;
	CList<int,int>	m_ClaimedScannerList;


private:

	CBrush  m_brush;
	CString txtScannerSummary;
	CButton btnDiscover;
	CButton btnPullTri;
	CButton btnRelTri;
	CTabWindowManager m_TabWndMgr;
	CScannerListCtrl m_ScannerListControl;
	CComboBox cmbScanner;
	CRect m_rSettingsRect;
	bool m_OpenAndRegistered;
	class CScannerCommands *m_ScannerCommands;
	HICON m_hIcon;
	wstring SelectedScannerID;
	int Async;

	ScannerMap		m_ScannerMap;	//Populated by During PNP Attach/Detach and a forced discovery via command interface.
public:
	afx_msg void OnTcnSelchangeTab1(NMHDR *pNMHDR, LRESULT *pResult);
};
