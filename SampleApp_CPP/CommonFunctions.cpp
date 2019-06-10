#include "StdAfx.h"
#include "CommonFunctions.h"
#include <comdef.h>
#include <string>
#include <msxml2.h>
#include "QuickXmlParser.h"


using namespace std;


static int count=1;
HRESULT hr;
IXMLDOMDocument *pXMLDom=NULL;

CCommonFunctions::CCommonFunctions(void)
{
}

void CCommonFunctions::ReadRMDXml(BSTR strXml, int *nMax, int *nProgress, std::wstring *sStatus, std::wstring *csScannerID)
{
	CoInitialize(NULL);
	wstring strScannerID (_T("scannerID"));
	wstring strMax (_T("maxcount"));
	wstring strStatus (_T("status"));
	wstring strProgress (_T("progress"));

	pXMLDom = DomFromCOM();
	long NOAttribs=NULL;
	BSTR ScannerID;
	BSTR MaxCount;
	BSTR Status;
	BSTR Progress;

	VARIANT_BOOL isSuccess;
	HRESULT loadRes=pXMLDom->loadXML(strXml,&isSuccess);
	IXMLDOMNodeList *pScannerNodeList = NULL;
	IXMLDOMNode *pScannerNode = NULL;
	IXMLDOMNodeList *pMaxNodeList = NULL;
	IXMLDOMNode *pMaxNode = NULL;
	IXMLDOMNodeList *pStatusNodeList = NULL;
	IXMLDOMNode *pStatusNode = NULL;
	IXMLDOMNodeList *pProgressNodeList = NULL;
	IXMLDOMNode *pProgressNode = NULL;

	hr = pXMLDom->getElementsByTagName((TCHAR*)strScannerID.data(), &pScannerNodeList);
	hr = pScannerNodeList->get_length(&NOAttribs);
	pScannerNodeList->reset();

	hr=pScannerNodeList->get_item(0, &pScannerNode);
	if ( pScannerNode )
	{
		hr=pScannerNode->get_text(&ScannerID);
		*csScannerID=(LPCWSTR)ScannerID;
	}

	hr = pXMLDom->getElementsByTagName((TCHAR*)strMax.data(), &pMaxNodeList);
	hr = pMaxNodeList->get_length(&NOAttribs);
	pMaxNodeList->reset();

	hr=pMaxNodeList->get_item(0, &pMaxNode);
	if ( pMaxNode )
	{
		hr=pMaxNode->get_text(&MaxCount);
		*nMax=_wtoi(MaxCount);
	}

	hr = pXMLDom->getElementsByTagName((TCHAR*)strStatus.data(), &pStatusNodeList);
	hr = pStatusNodeList->get_length(&NOAttribs);
	pStatusNodeList->reset();

	hr=pStatusNodeList->get_item(0, &pStatusNode);
	if ( pStatusNode )
	{
		hr=pStatusNode->get_text(&Status);
		*sStatus=(LPCWSTR)Status;
	}

	hr = pXMLDom->getElementsByTagName((TCHAR*)strProgress.data(), &pProgressNodeList);
	hr = pProgressNodeList->get_length(&NOAttribs);
	pProgressNodeList->reset();

	hr=pProgressNodeList->get_item(0, &pProgressNode);
	if ( pProgressNode )
	{
		hr=pProgressNode->get_text(&Progress);
		*nProgress=_wtoi(Progress);
	}
}

bool CCommonFunctions::ShowOutput(BSTR outXml,CScannerListCtrl *pList)
{
	CoInitialize(NULL);
	IXMLDOMNode *OpcodeNode = NULL;
	IXMLDOMNode *outArgsNode = NULL;
	IXMLDOMNode *argxmlNode = NULL;
	IXMLDOMNode *responseNode = NULL;
	pXMLDom = DomFromCOM();

	BSTR OpcodeValue=NULL;

	VARIANT_BOOL isSuccess;
	HRESULT loadRes=pXMLDom->loadXML(outXml,&isSuccess);

	hr=pXMLDom->get_lastChild(&outArgsNode);
	hr=outArgsNode->get_lastChild(&argxmlNode);
	hr=argxmlNode->get_lastChild(&responseNode);
	hr=responseNode->get_firstChild(&OpcodeNode);
	hr=OpcodeNode->get_text(&OpcodeValue);

	std::string strOpcode = _bstr_t (OpcodeValue);

	if ( strOpcode=="5000" )
	{
		ShowAttributes(outXml,pList);
	}
	else if ( strOpcode=="5002" ||strOpcode=="5001" )
	{
		ShowAttribProperties(outXml,pList);
	}
	CoUninitialize();
	return true;
}


void CCommonFunctions::ShowAttribProperties(BSTR outXml, CScannerListCtrl *pList, int Next)
{
	int *Indices = pList->GetSelectedIndices();
	
	CQuickXmlParser x(outXml);
	CQuickXmlParser::TAGDATA tag[4] = {0};
	tag[0].Tag.Name = L"id";
	tag[1].Tag.Name = L"datatype";
	tag[2].Tag.Name = L"permission";
	tag[3].Tag.Name = L"value";

	x.Configure(tag, 4);
	CQuickXmlParser::xptr p = 0;

	while(1)
	{
		p = x.Parse(p);

		int ID = _ttoi(x.Translate(tag[0].Value));
		for(int i = 0; Indices[i] != -1; ++i)
		{
			int index = Indices[i];
			int CurrentSelID = _ttoi(pList->GetItemText(index, 0).GetBuffer());
			
			while(CurrentSelID < ID) 
			{	
				CurrentSelID = _ttoi(pList->GetItemText(++index, 0).GetBuffer());
			}
			if(CurrentSelID == ID)
			{
				pList->SetItemText(index, 1, x.Translate(tag[1].Value));
				pList->SetItemText(index, 2, x.Translate(tag[2].Value));
				pList->SetItemText(index, 3, x.Translate(tag[3].Value));
				break;
			}
		}

		if(p == 0) break;
		x.ClearValues(tag);
	}
}


bool CCommonFunctions::ShowAttributes(BSTR outXml, CScannerListCtrl *pList)
{
	
	CQuickXmlParser x(outXml);
	CQuickXmlParser::TAGDATA tag[1] = {0};
	tag[0].Tag.Name = L"attribute";
	x.Configure(tag, 1);
	CQuickXmlParser::xptr p = 0;
	pList->DeleteAllItems();

	while(1)
	{
		p = x.Parse(p);
		pList->SetField(0, x.Translate(tag[0].Value));
		if(p == 0) break;
		x.ClearValues(tag);

	}
	pList->Sort();
	pList->SetItemState(0, LVIS_SELECTED, LVIS_SELECTED | LVIS_FOCUSED);
	pList->SetFocus();
	return true;

}

IXMLDOMDocument * CCommonFunctions::DomFromCOM()
{
	IXMLDOMDocument *pxmldoc;
	pxmldoc = NULL;

	CoCreateInstance(__uuidof(DOMDocument),NULL,CLSCTX_INPROC_SERVER,__uuidof(IXMLDOMDocument),(void**)&pxmldoc);

	pxmldoc->put_async(VARIANT_FALSE);
	pxmldoc->put_validateOnParse(VARIANT_FALSE);
	pxmldoc->put_resolveExternals(VARIANT_FALSE);

	return pxmldoc;

	if ( pxmldoc )
	{
		pxmldoc->Release();
	}
	return NULL;
}

CCommonFunctions::~CCommonFunctions(void)
{
}


