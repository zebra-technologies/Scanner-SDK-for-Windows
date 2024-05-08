/*******************************************************************************************
*
*  ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
// ScannerCommands.cpp : implementation file
//

#include "stdafx.h"
#include "ScannerSDKSampleApp.h"
#include "ScannerSDKSampleAppDlg.h"
#include "ScannerCommands.h"
#include "afxctl.h"
#include "afxdlgs.h"
#include <sstream>
#include "QuickXmlParser.h"
#include "shlwapi.h"
#include "ScannerActionDlg.h"

// CScannerCommands

IMPLEMENT_DYNAMIC(CScannerCommands, CCmdTarget)

    CScannerCommands *CScannerCommands::m_pThis = 0;

CScannerCommands::CScannerCommands()
{
    ScannerInterface = NULL;
    m_dwCookie = 0;
    cmdCreateInstance();
}

CScannerCommands::CScannerCommands(CScannerSDKSampleAppDlg * ParaAppDialog)
{
    m_AppDialog = ParaAppDialog;
    ScannerInterface = NULL;
    m_dwCookie = 0;
    cmdCreateInstance();
}

CScannerCommands::~CScannerCommands()
{
    cmdDestroyInstance();
}


BEGIN_MESSAGE_MAP(CScannerCommands, CCmdTarget)
END_MESSAGE_MAP()

// CScannerCommands message handlers
long CScannerCommands::cmdCreateInstance(void)
{
    HRESULT hr = S_FALSE;
    //Create the Scanner COM object
    hr = CoCreateInstance(CLSID_CCoreScanner, NULL, CLSCTX_ALL, IID_ICoreScanner, ((void**)&ScannerInterface));
    COM_CHECK(hr);

    if(ScannerInterface)
    {
        //Create an instance of the sink object to sink Core Scanner Events
        ScannerEventSink = new CEventSink(m_AppDialog);
        ScannerEventSinkUnknown = ScannerEventSink->GetIDispatch(FALSE);
        //Advice or make a connection
        BOOL stat = AfxConnectionAdvise(ScannerInterface, DIID__ICoreScannerEvents, ScannerEventSinkUnknown, FALSE, &m_dwCookie);
    }
    m_pThis = this;
    return hr;

Error:
    m_pThis = 0;
    return hr;

}

void CScannerCommands::cmdDestroyInstance()
{
    /*** If ever the CoreScanner Service becomes unresponsive when the application is
    closed, a STATUS_ACCESS_VIOLATION is raised on this thread which is handled
    by a Win32 exception handler. - VRQW74
    ***/
    _try
    {
        if(ScannerInterface)
        {
            if(m_dwCookie != 0 && ScannerEventSink)
            {
                BOOL stat = AfxConnectionUnadvise(ScannerInterface, DIID__ICoreScannerEvents, ScannerEventSinkUnknown, FALSE, m_dwCookie);
                delete ScannerEventSink;
                ScannerEventSink = 0;
                m_dwCookie = 0;
            }
            ScannerInterface->Release();
            ScannerInterface = 0;
        }
    }
    _except( EXCEPTION_EXECUTE_HANDLER )
    {
    }
}

HRESULT CScannerCommands::Execute(LONG opcode, BSTR* inXML, BSTR* outXML, int Async, LONG* status)
{
    if(ScannerInterface == 0) return S_FALSE;
    HRESULT hr = S_FALSE;
    
    if (Async)
        hr = ScannerInterface->ExecCommandAsync(opcode, inXML, status);
    else
        hr = ScannerInterface->ExecCommand(opcode, inXML, outXML, status); //Force call this version if outXML has to be returned.
    return hr;
}

long CScannerCommands::cmdOpen(SHORT ScannerTypes[TOTAL_SCANNER_TYPES],SHORT NOTypes,long *Status)
{
    HRESULT hr = S_FALSE;
    SAFEARRAY* pSA = NULL;
    SAFEARRAYBOUND bound[1];
    bound[0].lLbound = 0;
    bound[0].cElements = 8;

    pSA = SafeArrayCreate(VT_I2, 1, bound);

    for ( long i = 0; i < 8; i++ )
    {
        SafeArrayPutElement(pSA, &i, &ScannerTypes[i]);
    }
    hr = this->ScannerInterface->Open(0, pSA,NOTypes, Status);
    return hr;
}

long CScannerCommands::cmdClose(long *Status)
{
    HRESULT hr = S_FALSE;
    hr = ScannerInterface->Close(0,Status);
    return hr;
}

long CScannerCommands::cmdDiscover(BSTR * outXml,long *Status)
{
    HRESULT hr = S_FALSE;
    SHORT NumScan=0;
    SAFEARRAY* pSA = NULL;
    SAFEARRAYBOUND bound[1];
    bound[0].lLbound = 0;
    bound[0].cElements = 255;
    pSA = SafeArrayCreate(VT_I2, 1, bound);

    hr = ScannerInterface->GetScanners(&NumScan,pSA,outXml,Status);
    ////OutputDebugStringW(*outXml);
    return hr;
}

long CScannerCommands::cmdGetAll(wstring ScannerID,BSTR * outXml,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    CComBSTR input = inXml.c_str();

    return Execute(RSM_ATTR_GETALL, &input, outXml, Async, Status);
}

long CScannerCommands::cmdAimOn(wstring ScannerID,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    CComBSTR input = inXml.c_str();
    BSTR outXml;

    return Execute(DEVICE_AIM_ON, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdAimOff(wstring ScannerID,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    CComBSTR input = inXml.c_str();
    BSTR outXml;

    return Execute(DEVICE_AIM_OFF, &input, &outXml, Async, Status);
}


long CScannerCommands::cmdGetNext(wstring ScannerID,wstring AttribNo,BSTR * outXml,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-xml><attrib_list>");
    inXml.append(AttribNo);
    inXml.append(L"</attrib_list></arg-xml></cmdArgs></inArgs>");

    CComBSTR input = inXml.c_str();

    return Execute(RSM_ATTR_GETNEXT, &input, outXml, Async, Status);
}

long CScannerCommands::cmdExecute(DWORD nCmdID,wstring inXml,BSTR * outXml,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    CComBSTR input = inXml.c_str();
    
    return Execute(nCmdID, &input, outXml, Async, Status);
}

long CScannerCommands::cmdGet(wstring ScannerID,wstring AttribNo,BSTR * outXml,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-xml><attrib_list>");
    inXml.append(AttribNo);
    inXml.append(L"</attrib_list></arg-xml></cmdArgs></inArgs>");

    CComBSTR input = inXml.c_str();

    return Execute(RSM_ATTR_GET, &input, outXml, Async, Status);
}


long CScannerCommands::cmdGetScanCapa(wstring ScannerID,BSTR * outXml,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    
    CComBSTR input = inXml.c_str();
    
    return Execute(DEVICE_GET_SCANNER_CAPABILITIES, &input, outXml, Async, Status);
}

long CScannerCommands::cmdSwitchHostMode(wstring ScannerID, wstring HostMode, wstring SilentSwitch, wstring PermanantChange, int Async, long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-string>");
    inXml.append(HostMode);
    inXml.append(L"</arg-string><arg-bool>");
    inXml.append(SilentSwitch);
    inXml.append(L"</arg-bool><arg-bool>");
    inXml.append(PermanantChange);
    inXml.append(L"</arg-bool></cmdArgs></inArgs>");
    BSTR outXml;

    CComBSTR input = inXml.c_str();

    return Execute(DEVICE_SWITCH_HOST_MODE, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdSCdcSHostMode(wstring HostMode, wstring SilentSwitch, wstring PermanantChange, int Async, long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml = L"<inArgs>";
    inXml.append(L"<cmdArgs><arg-string>");
    inXml.append(HostMode);
    inXml.append(L"</arg-string><arg-bool>");
    inXml.append(SilentSwitch);
    inXml.append(L"</arg-bool><arg-bool>");
    inXml.append(PermanantChange);
    inXml.append(L"</arg-bool></cmdArgs></inArgs>");
    BSTR outXml;

    CComBSTR input = inXml.c_str();

    return Execute(SWITCH_CDC_DEVICES, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdUpdateFW(wstring ScannerID,wstring FilePath,int Async,long *Status, bool IsBulk)
{
    HRESULT hr = S_FALSE;

    wstring BulkArgStr;
    if(IsBulk == true)
        BulkArgStr = L"<arg-int>2</arg-int>";
    else
        BulkArgStr = L"<arg-int>1</arg-int>";

    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-string>");
    inXml.append(FilePath);
    inXml.append(L"</arg-string>");
    inXml.append(BulkArgStr);
    inXml.append(L"</cmdArgs></inArgs>");
    BSTR outXml;

    CComBSTR input = inXml.c_str();

    if(_wcsicmp(PathFindExtensionW(FilePath.c_str()), L".SCNPLG" ) == 0)
        hr = Execute(DEVICE_UPDATE_FIRMWARE_FROM_PLUGIN, &input, &outXml, Async, Status);
    else
        hr = Execute(DEVICE_UPDATE_FIRMWARE, &input, &outXml, Async, Status);

    return hr;
}

long CScannerCommands::cmdUpdateAttribMeta(wstring ScannerID,wstring MetaFilePath,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-string>");
    inXml.append(MetaFilePath);
    inXml.append(L"</arg-string></cmdArgs></inArgs>");
    BSTR outXml;

    CComBSTR input = inXml.c_str();

    return Execute(UPDATE_ATTRIB_META_FILE, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdUpdateFWAbort(wstring ScannerID,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    BSTR outXml;
    CComBSTR input = inXml.c_str();

    return Execute(DEVICE_ABORT_UPDATE_FIRMWARE, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdFlushMacroPdf(wstring ScannerID,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    BSTR outXml;
    CComBSTR input = inXml.c_str();

    return Execute(DEVICE_FLUSH_MACROPDF, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdAbortMacroPdf(wstring ScannerID,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    BSTR outXml;
    CComBSTR input = inXml.c_str();

    return Execute(DEVICE_ABORT_MACROPDF, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdPullTrigger(wstring ScannerID,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    BSTR outXml;
    CComBSTR input = inXml.c_str();

    return Execute(DEVICE_PULL_TRIGGER, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdReleaseTrigger(wstring ScannerID, int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    BSTR outXml;
    CComBSTR input = inXml.c_str();

    return Execute(DEVICE_RELEASE_TRIGGER, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdStartNewFW(wstring ScannerID,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    BSTR outXml;
    CComBSTR input = inXml.c_str();

    return Execute(START_NEW_FIRMWARE, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdCaptureImage(wstring ScannerID,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    BSTR outXml;
    CComBSTR input = inXml.c_str();
    
    return Execute(DEVICE_CAPTURE_IMAGE, &input, &outXml, Async, Status);
}


long CScannerCommands::cmdGetBluetoothPairingBarcode(wstring ScannerID,int Async,long *Status, int protocol, int defaultOption, int size, wstring FilePath)
{
    HRESULT hr = S_FALSE;
    wchar_t tempBuffer[10];
    wstring inXml=L"<inArgs><cmdArgs><arg-int>3</arg-int><arg-int>";
    //enable the secure methode '_itow_s' resolve the unsafe functions
    _itow_s(protocol, tempBuffer, 10);
    inXml.append(tempBuffer);
    inXml.append(L",");
    _itow_s(defaultOption, tempBuffer, 10);
    inXml.append(tempBuffer);
    inXml.append(L",");
    _itow_s(size, tempBuffer, 10);
    inXml.append(tempBuffer);
    inXml.append(L"</arg-int></cmdArgs></inArgs>");

    BSTR outXml;
    CComBSTR input = inXml.c_str();

    return Execute(GET_PAIRING_BARCODE, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdSetBarcodeMode(wstring ScannerID, int Async, long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml = L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    BSTR outXml;
    CComBSTR input = inXml.c_str();

    return Execute(DEVICE_CAPTURE_BARCODE, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdCaptureVideo(wstring ScannerID,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    BSTR outXml;
    CComBSTR input = inXml.c_str();

    return Execute(DEVICE_CAPTURE_VIDEO, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdAbortImageXfer(wstring ScannerID,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    BSTR outXml;
    CComBSTR input = inXml.c_str();

    return Execute(DEVICE_ABORT_IMAGE_XFER, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdGetScanSDKV(BSTR * outXml,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs></inArgs>";
    CComBSTR input = inXml.c_str();

    return Execute(GET_VERSION, &input, outXml, Async, Status);
}

long CScannerCommands::cmdGetDevTopology(BSTR * outXml,int Async,long *Status)
{
    HRESULT hr = S_FALSE;

    wstring inXml=L"<inArgs></inArgs>";
    CComBSTR input = inXml.c_str();

    return Execute(GET_DEVICE_TOPOLOGY, &input, outXml, Async, Status);
}

long CScannerCommands::cmdSetParametersEx(wstring ScannerID, wstring ID, wstring Value, int Async, long *Status)
{
    HRESULT hr = S_FALSE;
    
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-xml><attrib_list><attribute><id>");
    inXml.append(ID);
    inXml.append(L"</id><datatype>B</datatype><value>");
    inXml.append(Value);
    inXml.append(L"</value></attribute></attrib_list></arg-xml></cmdArgs></inArgs>");
    BSTR outXml;

    CComBSTR input = inXml.c_str();
    return Execute(DEVICE_SET_PARAMETERS, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdViewFinderParameter(wstring ScannerID, wstring VVFPara1, wstring VVFPara2, int Async, long *Status)
{
    HRESULT hr = S_FALSE;

    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-xml><attrib_list><attribute><id>");
    inXml.append(VVFPara1);
    inXml.append(L"</id><datatype>B</datatype><value>");
    inXml.append(VVFPara2);
    inXml.append(L"</value></attribute></attrib_list></arg-xml></cmdArgs></inArgs>");
    BSTR outXml;

    CComBSTR input = inXml.c_str();
    
    return Execute(DEVICE_SET_PARAMETERS, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdGetParameters(wstring ScannerID,wstring Parent,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-int>");
    inXml.append(Parent);
    inXml.append(L"</arg-int></cmdArgs></inArgs>");
    BSTR outXml;

    CComBSTR input = inXml.c_str();

    return Execute(DEVICE_GET_PARAMETERS, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdPagerMotor(wstring ScannerID, wstring duration, int Async, long *Status)
{
    BSTR outXml;
    wstring inXml = L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-xml><attrib_list><attribute><id>");
    inXml.append(to_wstring( PAGER_MOTOR_ACTION));
    inXml.append(L"</id><datatype>X</datatype><value>");
    inXml.append(duration);
    inXml.append(L"</value></attribute></attrib_list></arg-xml></cmdArgs></inArgs>");

    CComBSTR input = inXml.c_str();

    return Execute(RSM_ATTR_SET, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdBeep(wstring ScannerID,wstring BeepCode,int Async,long *Status)
{
    BSTR outXml;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-int>");
    inXml.append(BeepCode);
    inXml.append(L"</arg-int></cmdArgs></inArgs>");
    CComBSTR input = inXml.c_str();

    return Execute(DEVICE_BEEP_CONTROL, &input, &outXml, Async, Status);

#if 0
    BSTR outXml;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-xml><attrib_list><attribute><id>6000</id><datatype>X</datatype><value>");
    inXml.append(BeepCode);
    inXml.append(L"</value></attribute></attrib_list></arg-xml></cmdArgs></inArgs>");
    CComBSTR input = inXml.c_str();

    return Execute(RSM_ATTR_SET, &input, &outXml, Async, Status);
#endif

}

long CScannerCommands::cmdLED(wstring ScannerID,wstring LED,int Async,long *Status,SCANNER* p)
{
    HRESULT hr = S_FALSE;
    BSTR outXml;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-int>");
    inXml.append(LED);
    inXml.append(L"</arg-int></cmdArgs></inArgs>");
    CComBSTR input = inXml.c_str();
    if(NULL != p)
    {
        if(p->Model.Left(6).Compare(L"PL3300")==0)
        {
            return Execute(DEVICE_LED_ON, &input, &outXml, Async, Status);
        }
        else
        {
            return Execute(SET_ACTION, &input, &outXml, Async, Status);
        }      
    }
    return S_FALSE;
}

long CScannerCommands::cmdLEDOff(wstring ScannerID,wstring LED,int Async,long *Status,SCANNER* p)
{
    HRESULT hr = S_FALSE;
    BSTR outXml;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-int>");
    inXml.append(LED);
    inXml.append(L"</arg-int></cmdArgs></inArgs>");
    CComBSTR input = inXml.c_str();    
    if(NULL != p)
    {
        if(p->Model.Left(6).Compare(L"PL3300")==0)
        {
            return Execute(DEVICE_LED_OFF, &input, &outXml, Async, Status);            
        }
        else
        {
            return Execute(SET_ACTION, &input, &outXml, Async, Status);            
        }
    }
    return S_FALSE;    
}

long CScannerCommands::cmdRegisterEvents(int nEvents,wstring strEventsIDs,long *Status)
{
    wchar_t buf[8];
    _itow_s(nEvents, buf, 8, 10);
    HRESULT hr = S_FALSE;

    BSTR outXml;
    wstring inXml=L"<inArgs><cmdArgs><arg-int>";
    inXml.append(buf);
    inXml.append(L"</arg-int><arg-int>");
    inXml.append(strEventsIDs);
    inXml.append(L"</arg-int></cmdArgs></inArgs>");
    CComBSTR input = inXml.c_str();

    return Execute(REGISTER_FOR_EVENTS, &input, &outXml, 0, Status);
}

long CScannerCommands::cmdUnRegisterEvents(int nEvents,wstring strEventsIDs,long *Status)
{
    wchar_t buf[8];
    _itow_s(nEvents, buf, 8, 10);
    HRESULT hr = S_FALSE;

    BSTR outXml;
    wstring inXml=L"<inArgs><cmdArgs><arg-int>";
    inXml.append(buf);
    inXml.append(L"</arg-int><arg-int>");
    inXml.append(strEventsIDs);
    inXml.append(L"</arg-int></cmdArgs></inArgs>");
    CComBSTR input = inXml.c_str();

    return Execute(UNREGISTER_FOR_EVENTS, &input, &outXml, 0, Status);
}

long CScannerCommands::cmdSet(wstring ScannerID,wstring AttribList,int Async,long *Status)
{
    HRESULT hr = S_FALSE;

    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-xml><attrib_list>");
    inXml.append(AttribList);
    inXml.append(L"</attrib_list></arg-xml></cmdArgs></inArgs>");
    BSTR outXml;
    CComBSTR input = inXml.c_str();

    return Execute(RSM_ATTR_SET, &input, &outXml, Async, Status);

}

long CScannerCommands::cmdStore(wstring ScannerID,wstring AttribList,int Async,long *Status)
{
    HRESULT hr = S_FALSE;

    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-xml><attrib_list>");
    inXml.append(AttribList);
    inXml.append(L"</attrib_list></arg-xml></cmdArgs></inArgs>");
    BSTR outXml;
    CComBSTR input = inXml.c_str();

    return Execute(RSM_ATTR_STORE, &input, &outXml, Async, Status);

}

long CScannerCommands::cmdReboot(wstring ScannerID,int Async,long *Status)
{
    BSTR outXml;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    CComBSTR input = inXml.c_str();
    HRESULT hr = S_FALSE;

    return Execute(REBOOT_SCANNER, &input, &outXml, Async, Status);

}

long CScannerCommands::cmdDisconnect(wstring ScannerID,int Async,long *Status)
{
    BSTR outXml;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    CComBSTR input = inXml.c_str();
    HRESULT hr = S_FALSE;
    
    return Execute(DISCONNECT_BT_SCANNER, &input, &outXml, Async, Status);

}

long CScannerCommands::cmdClaim(wstring ScannerID,int Async,long *Status)
{
    BSTR outXml;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    CComBSTR input = inXml.c_str();
    HRESULT hr = S_FALSE;

    return Execute(CLAIM_DEVICE, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdSetParaDefault(wstring ScannerID,int Async,long *Status)
{
    BSTR outXml;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    CComBSTR input = inXml.c_str();
    HRESULT hr = S_FALSE;
    
    return Execute(DEVICE_SET_PARAMETER_DEFAULTS, &input, &outXml, Async, Status);
}


long CScannerCommands::cmdSetParaPersistanceEx(wstring ScannerID,wstring ID, wstring Value,int Async,long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-xml><attrib_list><attribute><id>");
    inXml.append(ID);
    inXml.append(L"</id><datatype>B</datatype><value>");
    inXml.append(Value);
    inXml.append(L"</value></attribute></attrib_list></arg-xml></cmdArgs></inArgs>");
    BSTR outXml;

    CComBSTR input = inXml.c_str();
    return Execute(DEVICE_SET_PARAMETER_PERSISTANCE, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdRelease(wstring ScannerID,int Async,long *Status)
{
    BSTR outXml;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    CComBSTR input = inXml.c_str();
    HRESULT hr = S_FALSE;

    return Execute(RELEASE_DEVICE, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdEnable(wstring ScannerID,int Async,long *Status)
{
    BSTR outXml;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    CComBSTR input = inXml.c_str();
    HRESULT hr = S_FALSE;

    return Execute(DEVICE_SCAN_ENABLE, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdDisable(wstring ScannerID,int Async,long *Status)
{
    BSTR outXml;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    CComBSTR input = inXml.c_str();
    HRESULT hr = S_FALSE;

    return Execute(DEVICE_SCAN_DISABLE, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdEnableKeyboardEmulator(bool bEnable, int Async, long *Status) //VRQW74
{
    BSTR outXml;

    wstring inXml = L"<inArgs><cmdArgs><arg-bool>";
    bEnable == true ? inXml.append(L"true") : inXml.append(L"false");
    inXml.append(L"</arg-bool></cmdArgs></inArgs>");

    CComBSTR input = inXml.c_str();
    return Execute(KEYBOARD_EMULATOR_ENABLE, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdSetKeyboardEmulatorLocale(int Lang, int Async, long *Status) //VRQW74
{
    BSTR outXml;
    WCHAR Buf[8];

    _itow_s(Lang, Buf, 8, 10);

    wstring inXml = L"<inArgs><cmdArgs><arg-int>";
    inXml.append(Buf);
    inXml.append(L"</arg-int></cmdArgs></inArgs>");

    CComBSTR input = inXml.c_str();
    return Execute(KEYBOARD_EMULATOR_SET_LOCALE, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdGetKeyboardEmulatorConfig(bool& bEnable, int& Lang, int Async, long *Status) //VRQW74
{
    BSTR outXml;
    wstring inXml = L"<inArgs></inArgs>";
    CComBSTR input = inXml.c_str();

    HRESULT hr = Execute(KEYBOARD_EMULATOR_GET_CONFIG, &input, &outXml, 0, Status); // Force Async = 0 since outXml returns stuff
    if(hr == S_OK && outXml != NULL)
    {
        CQuickXmlParser x(outXml);
        CQuickXmlParser::TAGDATA tag[2] = {0};
        
        tag[0].Tag.Name = L"KeyEnumState";
        tag[1].Tag.Name = L"KeyEnumLocale";

        x.Configure(tag, 2);
        
        CQuickXmlParser::xptr p = 0;
        p = x.Parse(p);

        int state = _wtoi(x.Translate(tag[0].Value));
        bEnable = (state == 1 ? true : false);
        Lang = _wtoi(x.Translate(tag[1].Value));
        
        return S_OK;
    }
    else
        return S_FALSE;

}

long CScannerCommands::cmdReadWeight(wstring ScannerID,BSTR * outXml, int Async, long *Status)
{
    HRESULT hr = S_FALSE;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    CComBSTR input = inXml.c_str();

    return Execute(SCALE_READ_WEIGHT, &input, outXml, Async, Status);
}

long CScannerCommands::cmdZeroScale(wstring ScannerID,int Async, long *Status)
{
    BSTR outXml;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    CComBSTR input = inXml.c_str();
    HRESULT hr = S_FALSE;

    return Execute(SCALE_ZERO_SCALE, &input, &outXml, Async, Status);
}

long CScannerCommands::cmdRestScale(wstring ScannerID,int Async, long *Status)
{
    BSTR outXml;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    CComBSTR input = inXml.c_str();
    HRESULT hr = S_FALSE;

    return Execute(SCALE_SYSTEM_RESET, &input, &outXml, Async, Status);
}
// Update Decode Tone
long CScannerCommands::cmdUpdateDecodeTone(wstring ScannerID, wstring FilePath, long* Status)
{
    BSTR outXml;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-string>");
    inXml.append(FilePath);
    inXml.append(L"</arg-string></cmdArgs></inArgs>");
    CComBSTR input = inXml.c_str();
    HRESULT hr = S_FALSE;

    return Execute(UPDATE_DECODE_TONE, &input, &outXml, 0, Status);
}

// Erase Decode Tone
long CScannerCommands::cmdEraseDecodeTone(wstring ScannerID, long* Status)
{
    BSTR outXml;
    wstring inXml=L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    CComBSTR input = inXml.c_str();
    HRESULT hr = S_FALSE;

    return Execute(ERASE_DECODE_TONE, &input, &outXml, 0, Status);
}

long CScannerCommands::cmdUploadElectricFenceCustomTone(wstring ScannerID, wstring FilePath, long* Status)
{
    BSTR outXml;
    wstring inXml = L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID><cmdArgs><arg-string>");
    inXml.append(FilePath);
    inXml.append(L"</arg-string></cmdArgs></inArgs>");
    CComBSTR input = inXml.c_str();
    HRESULT hr = S_FALSE;

    return Execute(UPDATE_ELECTRIC_FENCE_CUSTOM_TONE, &input, &outXml, 0, Status);
}

long CScannerCommands::cmdEraseElectricFenceCustomTone(wstring ScannerID, long* Status)
{
    BSTR outXml;
    wstring inXml = L"<inArgs><scannerID>";
    inXml.append(ScannerID);
    inXml.append(L"</scannerID></inArgs>");
    CComBSTR input = inXml.c_str();
    HRESULT hr = S_FALSE;

    return Execute(ERASE_ELECTRIC_FENCE_CUSTOM_TONE, &input, &outXml, 0, Status);
}

long CScannerCommands::cmdSetDADFSource(wstring Source, int Async, long *Status) //VRQW74
{
    BSTR outXml;
    wstring inXml = L"<inArgs><cmdArgs><arg-string>";
    inXml.append(Source);
    inXml.append(L"</arg-string></cmdArgs></inArgs>");
    CComBSTR input = inXml.c_str();

    return Execute(CONFIGURE_DADF, &input, &outXml, Async, Status); 
}

long CScannerCommands::cmdResetDADFSource(int Async, long *Status) //VRQW74
{
    BSTR outXml;
    wstring inXml = L"<inArgs></inArgs>";
    CComBSTR input = inXml.c_str();

    return Execute(RESET_DADF, &input, &outXml, Async, Status); 
}