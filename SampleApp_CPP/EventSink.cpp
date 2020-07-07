/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#include "ScannerSDKSampleApp.h"
#include "stdafx.h"
#include "EventSink.h"
#include "_CoreScanner_i.c"
#include <iostream>
#include "QuickXmlParser.h"
using namespace std;

IMPLEMENT_DYNAMIC(CEventSink, CCmdTarget)

CEventSink::CEventSink()
{
	EnableAutomation();
}

CEventSink::CEventSink(CScannerSDKSampleAppDlg *ParaAppBridge)
{
	AppBridge = ParaAppBridge;
	EnableAutomation();
}

CEventSink::~CEventSink()
{
}


void CEventSink::OnFinalRelease()
{
	CCmdTarget::OnFinalRelease();
}


BEGIN_MESSAGE_MAP(CEventSink, CCmdTarget)
END_MESSAGE_MAP()

BEGIN_DISPATCH_MAP(CEventSink, CCmdTarget)
	DISP_FUNCTION_ID(CEventSink, "ScanCmdResponseEvent", 5, onScanCmdResponseEvent, VT_EMPTY,VTS_I2 VTS_BSTR)
	DISP_FUNCTION_ID(CEventSink, "onPNPEvents", 4, onPNPEvents, VT_EMPTY, VTS_I2 VTS_BSTR)
	DISP_FUNCTION_ID(CEventSink, "ScanDataEvent", 3, onScanDataEvent, VT_EMPTY,VTS_I2 VTS_BSTR)
	DISP_FUNCTION_ID(CEventSink, "ScanRMDEvent", 6, onScanRMDEvent,VT_EMPTY,VTS_I2 VTS_BSTR)
	DISP_FUNCTION_ID(CEventSink, "OnImageEvent", 1, onImageEvent, VT_EMPTY, VTS_I2 VTS_I4 VTS_I2 VTS_PVARIANT VTS_BSTR)
	DISP_FUNCTION_ID(CEventSink, "OnVideoEvent", 2, onVideoEvent, VT_EMPTY, VTS_I2 VTS_I4 VTS_PVARIANT VTS_BSTR)
	DISP_FUNCTION_ID(CEventSink, "OnScannerNotificationEvent", 8, onScannerNotificationEvent, VT_EMPTY, VTS_I2 VTS_BSTR)
	DISP_FUNCTION_ID(CEventSink, "OnIOEvent", 7, onIONotificationEvent, VT_EMPTY, VTS_I2 VTS_UI1)
	DISP_FUNCTION_ID(CEventSink, "OnBinaryDataEvent", 9, OnBinaryDataEvent, VT_EMPTY, VTS_I2 VTS_I4 VTS_I2 VTS_PVARIANT VTS_BSTR)
	DISP_FUNCTION_ID(CEventSink, "onParameterBarcodeEvent", 10,onParameterBarcodeEvent, VT_EMPTY, VTS_I2 VTS_I4 VTS_I2 VTS_PVARIANT VTS_BSTR)
END_DISPATCH_MAP()

static const IID IID_IEventSink =
{ 0xD9D7AF5A, 0x7D86, 0x423C, { 0xBF, 0x65, 0xE6, 0x86, 0x82, 0x3B, 0x32, 0x2F}};

BEGIN_INTERFACE_MAP(CEventSink, CCmdTarget)
	INTERFACE_PART(CEventSink, DIID__ICoreScannerEvents, Dispatch)
END_INTERFACE_MAP()


void CEventSink::onScanCmdResponseEvent(short status, BSTR scanCmdResponse)
{
	AppBridge->OnCmdResponse(status, scanCmdResponse);
}

void CEventSink::onIONotificationEvent(short type, unsigned char data)
{
	AppBridge->OnIOResponse(type, data);
}

void CEventSink::onScanDataEvent(short reserved, BSTR scanData)
{
	AppBridge->OnScanData(reserved, scanData);
}

void CEventSink::onImageEvent(SHORT eventType, LONG size, SHORT imageFormat, VARIANT *sfImageData, BSTR* pScannerData)
{
	LPBYTE MediaBuffer;
	SAFEARRAY* psa = sfImageData->parray;
	HRESULT hr = SafeArrayAccessData(psa, (void**)&MediaBuffer);
	hr = SafeArrayUnaccessData(psa);
	AppBridge->OnImageEvent(MediaBuffer, size);
}

void CEventSink::onParameterBarcodeEvent(SHORT eventType, LONG size,SHORT imageFormat,VARIANT *sfImageData,BSTR* pData)
{
	LPBYTE MediaBuffer;
	SAFEARRAY* psa = sfImageData->parray;
	HRESULT hr = SafeArrayAccessData(psa, (void**)&MediaBuffer);
	hr = SafeArrayUnaccessData(psa);
	AppBridge->ParameterBarcodeEvent(MediaBuffer, size);
}


void CEventSink::onVideoEvent(SHORT eventType, LONG size, VARIANT *sfvideoData, BSTR* pScannerData)
{
	
	LPBYTE MediaBuffer;
	SAFEARRAY* psa = sfvideoData->parray;
	HRESULT hr = SafeArrayAccessData(psa, (void**)&MediaBuffer);
	hr = SafeArrayUnaccessData(psa);
	AppBridge->OnVideoEvent(MediaBuffer, size);
}


void CEventSink::onPNPEvents(short eventType, BSTR ppnpData)
{
	AppBridge->OnPNP(eventType,ppnpData);
}

void CEventSink::onScannerNotificationEvent(short notificationType, BSTR pScannerData)
{
	AppBridge->OnNotificationEvent(notificationType,pScannerData);
}


void CEventSink::onScanRMDEvent(short eventType, BSTR eventData)
{
	AppBridge->OnRMD(eventType,eventData);
}

void CEventSink::OnBinaryDataEvent(SHORT eventType, LONG size, SHORT dataFormat, VARIANT *sfBinaryData, BSTR* pScannerData)
{
	LPBYTE MediaBuffer;
	SAFEARRAY* psa = sfBinaryData->parray;
	HRESULT hr = SafeArrayAccessData(psa, (void**)&MediaBuffer);
	hr = SafeArrayUnaccessData(psa);
	AppBridge->OnBinaryDataEvent(MediaBuffer, size, dataFormat, pScannerData);
}

