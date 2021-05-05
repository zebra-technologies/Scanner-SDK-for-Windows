/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/

#pragma once
#include "ScannerCommands.h"

#include "afxwin.h"
#include "afxcmn.h"

const int PARAM_PERSISTANCE_ON = 0x0001;   //Parameters persistance on 
const int PARAM_PERSISTANCE_OFF = 0x0000;  //Parameters persistance off  

class CConfigurationDlg : public CDialog
{
	DECLARE_DYNAMIC(CConfigurationDlg)

public:

	CConfigurationDlg(CWnd* pParent = NULL);   
	virtual ~CConfigurationDlg();

	void SetScannerID(wstring * ScannerID);
	void SetAsync(int *ParaAsync);
	void SetAbortFirmwareState(BOOL bState);

	//void InitParaTree();
	void OnRMDEvent(short eventType, BSTR eventData);
	bool ReadFirmwareResponseEventXML(BSTR RespXml, int *nMax, int *nProgress, wstring *sStatus, wstring *csScannerID);
    void UpdateClaimedStatus(int scnID);
 	void InitProtocolCombo();

	enum { IDD = IDD_Configuration };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    

	DECLARE_MESSAGE_MAP()
	DECLARE_TAB_WINDOW(Advanced)

public:
	afx_msg void OnBrowse();
	afx_msg void OnUpdateFirmware();
	afx_msg void OnAbortFirmwareUpdate();
	afx_msg void OnLaunchFirmware();
	afx_msg void OnClaimScanner();
	afx_msg void OnChangeProtocol();

	int& QueryScannerProtocol()
	{
		static int proto = SCANNER_TYPES_ALL; //default;
		return proto;
	}

public:
	virtual BOOL OnInitDialog();
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);

private:
	CButton ChkClaim;
	CEdit txtFWPath;
	CBrush  m_brush;
	CProgressCtrl FWprogress;
	CButton m_chkBulk;
	CComboBox m_cmbProtocol;

	wstring SelectedScannerID;
	int Async;
public:
	afx_msg void OnBnClickedButtonDecodetonebrrowse();
	afx_msg void OnBnClickedButtonDecodetoneerase();
	afx_msg void OnBnClickedButtonDecodetoneupload();
	// Decode Tone File Path Text Box
	CEdit txtDecodeToneFilePath;
	afx_msg void OnBnClickedButtonElectricFenceCustomToneBrowse();
	// Electric fence custom tone wave file name
	CEdit txtElectricFenceCustomToneWaveFileName;
	afx_msg void OnBnClickedButtonElectricFenceCustomToneUpload();
	afx_msg void OnBnClickedButtonElectricFenceCustomToneErase();
};
