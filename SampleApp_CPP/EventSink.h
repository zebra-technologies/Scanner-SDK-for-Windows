/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#pragma once
#include "ScannerSDKSampleAppDlg.h"


class CEventSink : public CCmdTarget
{
	DECLARE_DYNAMIC(CEventSink)

public:
	CScannerSDKSampleAppDlg * AppBridge;
	CEventSink();
	CEventSink(CScannerSDKSampleAppDlg *ParaAppBridge);
	virtual ~CEventSink();
	

	virtual void OnFinalRelease();
	void onScanCmdResponseEvent(short status, BSTR scanCmdResponse);
	void onScanDataEvent(short reserved, BSTR scanData);
	
	void onVideoEvent(SHORT eventType, LONG size, VARIANT *sfvideoData, BSTR* pScannerData);
	void onImageEvent(SHORT eventType, LONG size, SHORT imageFormat, VARIANT *sfImageData, BSTR* pScannerData);
    void onParameterBarcodeEvent(SHORT eventType, LONG size,SHORT imageFormat,VARIANT *sfImageData,BSTR* pData);
	
	void onPNPEvents(short eventType, BSTR ppnpData); 
	void onScannerNotificationEvent(short scannerMode, BSTR pScannerData); 
	void onScanRMDEvent(short eventType, BSTR eventData);
	void onIONotificationEvent(short type, unsigned char data);
	void OnBinaryDataEvent(SHORT eventType, LONG size, SHORT dataFormat, VARIANT *sfImageData, BSTR* pScannerData);

protected:
	DECLARE_MESSAGE_MAP()
	DECLARE_DISPATCH_MAP()
	DECLARE_INTERFACE_MAP()
};


