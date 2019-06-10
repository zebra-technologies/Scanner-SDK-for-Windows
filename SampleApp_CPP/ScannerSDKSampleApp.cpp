/*******************************************************************************************
*
* ©2016 Symbol Technologies LLC. All rights reserved.
*
********************************************************************************************/
// ScannerSDKSampleApp.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include "ScannerSDKSampleApp.h"
#include "ScannerSDKSampleAppDlg.h"

#ifdef _DEBUG
	#define new DEBUG_NEW
#endif

BEGIN_MESSAGE_MAP(CScannerSDKSampleAppApp, CWinApp)
	ON_COMMAND(ID_HELP, &CWinApp::OnHelp)
END_MESSAGE_MAP()

CScannerSDKSampleAppApp::CScannerSDKSampleAppApp()
{
}

CScannerSDKSampleAppApp theApp;

BOOL CScannerSDKSampleAppApp::InitInstance()
{
	INITCOMMONCONTROLSEX InitCtrls;
	InitCtrls.dwSize = sizeof(InitCtrls);
	InitCtrls.dwICC = ICC_WIN95_CLASSES;
	InitCommonControlsEx(&InitCtrls);

	AfxInitRichEdit2();

	//Assign custom App Name
	free((void*)CWinApp::m_pszAppName);
	CWinApp::m_pszAppName=_tcsdup(_T("Scanner SDK C++/MFC Sample Application"));

	CWinApp::InitInstance();

	AfxEnableControlContainer();
	CScannerSDKSampleAppDlg dlg;

	HRESULT hr = S_FALSE;
	//Initialize COM lbrary for apartment threaded
	hr =  CoInitialize(0);
	COM_CHECK(hr);

	m_pMainWnd = &dlg;
	int nResponse = dlg.DoModal();

	CoUninitialize();

Error:
	return FALSE;
}
