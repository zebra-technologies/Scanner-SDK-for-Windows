/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
// ScaleDlg.cpp : implementation file

#include "stdafx.h"
#include "ScaleDlg.h"
#include "ScannerSDKSampleApp.h"
#include "LogsDlg.h"
#include "QuickXmlParser.h"

IMPLEMENT_DYNAMIC(CScaleDlg, CDialog)

CScaleDlg::CScaleDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CScaleDlg::IDD, pParent)
{
	Async = 0;
}

CScaleDlg::~CScaleDlg()
{
}

void CScaleDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_STATIC_WeightValue, m_statWeight);
	DDX_Control(pDX, IDC_STATIC_WeightUnit, m_statWeightUnit);
	DDX_Control(pDX, IDC_STATIC_ScaleStatus, m_statStatusDesc);
}

/***
	Standard MFC message map
***/
BEGIN_MESSAGE_MAP(CScaleDlg, CDialog)
	ON_BN_CLICKED(IDC_BUTTON_ReadWeight, &CScaleDlg::OnBnClickedButtonReadweight)
	ON_BN_CLICKED(IDC_BUTTON_ZeroScale, &CScaleDlg::OnBnClickedButtonZeroscale)
	ON_BN_CLICKED(IDC_BUTTON_ScaleReset, &CScaleDlg::OnBnClickedButtonScalereset)
	ON_WM_CTLCOLOR()
END_MESSAGE_MAP()


// CScaleDlg message handlers
void CScaleDlg::OnBnClickedButtonReadweight()
{
	CHECK_CMD0;

	long status = 1;
	CComBSTR outXml(L"");
	//Issue the RSM command SCALE_READ_WEIGHT.
	if ( !SC->cmdReadWeight(SelectedScannerID, &outXml, Async, &status) )
	{
		if ( !Async )
		{
			//Update the log tab 
			GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);
		}
	}

	static wchar_t* ScaleWeightArray[] = 
		{	L"Scale Not Enabled",			//Index = 0
			L"Scale Not Ready",
			L"Stable Weight OverLimit",
			L"Stable Weight Under Zero",
			L"Non Stable Weight",
			L"Stable Zero Weight",
			L"Stable NonZero Weight"		//Index = 6
		};

	CQuickXmlParser xml(outXml);
	CQuickXmlParser::TAGDATA tag[4] = {0};
	tag[0].Tag.Name			= L"weight";
	tag[1].Tag.Name			= L"weight_mode";
	tag[2].Tag.Name			= L"status";
	tag[3].Tag.Name			= L"rawdata";

	xml.Configure(tag, 4);
	CQuickXmlParser::xptr p = 0;
	int ParseCount = 0;
	
	PWCHAR pchWeight;
	PWCHAR pchWeightMode ;
	PWCHAR pchStatus;
	//PWCHAR pchRawData;

	do
	{
		p = xml.Parse(p);

		pchWeight = xml.Translate(tag[0].Value);
		if(pchWeight) m_statWeight.SetWindowText(pchWeight); 
		pchWeightMode = xml.Translate(tag[1].Value);
		if(pchWeightMode) m_statWeightUnit.SetWindowText(pchWeightMode); 
		pchStatus = xml.Translate(tag[2].Value);
		
		int StatusResult = (pchStatus != 0) ? _wtoi(pchStatus) : -1;
		if(StatusResult >= 0 && StatusResult <= 6)
			m_statStatusDesc.SetWindowTextW(ScaleWeightArray[StatusResult]);
		
		//pchRawData = xml.Translate(tag[3].Value);
		xml.ClearValues(tag);
		
	} while(p != 0);

	LOG(status, "SCALE_READ_WEIGHT")
}

void CScaleDlg::OnBnClickedButtonZeroscale()
{
	CHECK_CMD0;

	long status = 1;
	CComBSTR outXml(L"");
	//Issue the RSM command SCALE_ZERO_SCALE.
	if ( !SC->cmdZeroScale(SelectedScannerID, Async, &status) )
	{
		if ( !Async )
		{
			//Update the log tab and set the RSM grid entries if it's a synchronous command.
			//GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);
			//SetGridIDs(outXml);
		}
	}
	LOG(status, "SCALE_READ_WEIGHT")
}

void CScaleDlg::OnBnClickedButtonScalereset()
{
	CHECK_CMD0;

	long status = 1;
	CComBSTR outXml(L"");
	//Issue the RSM command SCALE_SYSTEM_RESET.
	if ( !SC->cmdRestScale(SelectedScannerID, Async, &status) )
	{
		if ( !Async )
		{
			//Update the log tab and set the RSM grid entries if it's a synchronous command.
			//GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);
			//SetGridIDs(outXml);
		}
	}
	LOG(status, "SCALE_SYSTEM_RESET")
}

void CScaleDlg::SetScannerID(std::wstring *ScannerID)
{
	SelectedScannerID=*ScannerID;
}

void CScaleDlg::SetAsync(int *ParaA)
{
	Async=*ParaA;
}

HBRUSH CScaleDlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
	if(!m_brush.m_hObject)
		m_brush.CreateSolidBrush(RGB(247, 250, 253));

	SetLabelBkg(pDC, pWnd, IDC_STATIC);
	return m_brush;
}
