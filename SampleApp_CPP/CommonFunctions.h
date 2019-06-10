#pragma once
#include "afxwin.h"
#include "afxcmn.h"
#include <msxml.h>
#include <iostream>
#include "ScannerList.h"

using namespace std;


class CCommonFunctions
{
public:
	CCommonFunctions(void);
	static bool ShowAttributes(BSTR outXml,CScannerListCtrl *pList);
	static void ShowAttribProperties(BSTR outXml,CScannerListCtrl *pList, int Next = 0);

	static bool ShowOutput(BSTR outXml,CScannerListCtrl *pList);
	static void ReadRMDXml(BSTR strXml, int * nMax, int * nProgress, wstring * sStatus, wstring * csScannerID);
	
	static IXMLDOMDocument * DomFromCOM();
	~CCommonFunctions(void);


};
