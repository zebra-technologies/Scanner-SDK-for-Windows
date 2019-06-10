/*******************************************************************************************
*
* ©2016 Symbol Technologies LLC. All rights reserved.
*
********************************************************************************************/
#pragma once


#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		
#include <string>
using namespace std;


class CScannerSDKSampleAppApp : public CWinApp
{
public:
	CScannerSDKSampleAppApp();

	public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};

extern CScannerSDKSampleAppApp theApp;