/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
// IntelDocCap.cpp : implementation file
//

#include "stdafx.h"
#include "IntelDocCap.h"
#include "ISO15434.h"
#include "QuickXmlParser.h"
#include "LogsDlg.h"
// CIntelDocCap dialog



IMPLEMENT_DYNAMIC(CIntelDocCap, CDialog)

CIntelDocCap::CIntelDocCap(CWnd* pParent /*=NULL*/)
	: CDialog(CIntelDocCap::IDD, pParent)
{
	m_ImageData.ImageData = 0;
	m_ImageData.ImageSize = 0;

	Async = 0;
}


CIntelDocCap::~CIntelDocCap()
{
}

void CIntelDocCap::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_EDITI_IDC_DECODE_DATA, m_edit_DecodData);
	DDX_Control(pDX, IDC_STATIC_IDC_PIC_CTRL, m_PicControl);
	DDX_Control(pDX, IDC_COMBO_IDC_PARAM, m_cmbDocCapConf);
	DDX_Control(pDX, IDC_EDIT_IDC_PARAM_VAL, m_edit_DocCapConfValue);
	DDX_Control(pDX, IDC_EDIT_IDC_SYMBOLOGY, m_edit_DocCapBarcodeSymbology);
	DDX_Control(pDX, IDC_CHECK_USE_HID, m_chkUseHID);
}


BEGIN_MESSAGE_MAP(CIntelDocCap, CDialog)
	ON_WM_CTLCOLOR()
	ON_CBN_SELCHANGE(IDC_COMBO_IDC_PARAM, &CIntelDocCap::OnCbnSelchangeComboIdcParam)
	ON_BN_CLICKED(IDC_BUTTON_IDC_PARAM_SET, &CIntelDocCap::OnBnClickedButtonIdcParamSet)
	ON_BN_CLICKED(IDC_BUTTON_IDC_PARAM_GET, &CIntelDocCap::OnBnClickedButtonIdcParamGet)
	ON_BN_CLICKED(IDC_BUTTON_IDC_PARAM_STORE, &CIntelDocCap::OnBnClickedButtonIdcParamStore)
	ON_BN_CLICKED(IDC_BUTTON_IDC_CLEAR, &CIntelDocCap::OnBnClickedButtonIdcClear)
	ON_BN_CLICKED(IDC_CHECK_USE_HID, &CIntelDocCap::OnBnClickedCheckUseHid)
	ON_BN_CLICKED(IDC_BUTTON_IDC_SAVE_IMAGE, &CIntelDocCap::OnBnClickedButtonIdcSaveImage)
END_MESSAGE_MAP()


void CIntelDocCap::OnBinaryDataEventCapture(LPBYTE MediaBuffer,LONG BufferSize, SHORT DataFormat, BSTR* bstrScannerData)
{

	//CComBSTR outXml(*bstrScannerData);
	BSTR outXML = SysAllocString((OLECHAR*)bstrScannerData);
	CQuickXmlParser xml(outXML);
	CQuickXmlParser::TAGDATA tag[1] = {0};
	tag[0].Tag.Name			= L"channel";

	xml.Configure(tag, 1);
	CQuickXmlParser::xptr p = 0;
	int ParseCount = 0;
	
	PWCHAR pchUsbChannel;

	wstring reValue;
	do
	{
		p = xml.Parse(p);

		pchUsbChannel = xml.Translate(tag[0].Value);
		if(pchUsbChannel) 
		{
			reValue.assign(pchUsbChannel);
			if(0 == reValue.compare(L"usb_BULK"))
			{
				m_chkUseHID.SetCheck(BST_UNCHECKED);
				LOG(-1, "Intelligent Doc Cap Event Fired : Transfered through USB Bulk Channel");
			}
			else
			{
				m_chkUseHID.SetCheck(BST_CHECKED);
				LOG(-1, "Intelligent Doc Cap Event Fired : Transfered through USB HID Channel");
			}
		}

		xml.ClearValues(tag);
		
	} while(p != 0);

	OnBnClickedButtonIdcClear();
	
	if(DataFormat == ST_SIGNATURE_CAPTURE)
	{
		ClearImageCache();
		m_ImageData.ImageData = new BYTE[BufferSize + (std::size_t)2];
		memcpy(m_ImageData.ImageData, &MediaBuffer[6], BufferSize);
		m_ImageData.ImageSize = BufferSize;

		m_RenderEngine.Render(&MediaBuffer[6], BufferSize);
	}
	else
	{
		ISO15434* iso15434Evelop = ISO15434::GetEnvelope(MediaBuffer,BufferSize);
		PISOIMAGEDATA pDataFormat = NULL;
		while(pDataFormat = iso15434Evelop->GetNextFormat())
		{
			switch(pDataFormat->DataType)
			{
			case 0:
				{
					CString a(pDataFormat->decodeData.pbData);
					m_edit_DecodData.SetWindowText(a);
					m_edit_DocCapBarcodeSymbology.SetWindowText(GetSymbology(pDataFormat->decodeData.bSymbology));
				}
				break;
			case 1:
			case 2:
			case 3:
				{
					ClearImageCache();
					std::size_t image_data_array_size = pDataFormat->ImageData.bImageDataLen + (std::size_t)2;
					m_ImageData.ImageData = new BYTE[image_data_array_size];
					memcpy(m_ImageData.ImageData, pDataFormat->ImageData.pbImageData, pDataFormat->ImageData.bImageDataLen);
					m_ImageData.ImageSize = pDataFormat->ImageData.bImageDataLen;

					m_RenderEngine.Render(pDataFormat->ImageData.pbImageData, pDataFormat->ImageData.bImageDataLen);
				}
				break;
			}
		}
		delete iso15434Evelop;
	}
}

BOOL CIntelDocCap::OnInitDialog()
{
	CDialog::OnInitDialog();
	m_RenderEngine.Attach(m_PicControl);
	InitDocCapConfCombo();
	return TRUE; 
}

HBRUSH CIntelDocCap::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
	if(!m_brush.m_hObject)
		m_brush.CreateSolidBrush(RGB(247, 250, 253));

	SetLabelBkg(pDC, pWnd, IDC_STATIC);
	return m_brush;
}

void CIntelDocCap::InitDocCapConfCombo()
{
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"DocCap_MODE"),594);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"PICKLIST_EN"),402);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"DocCap_X"),596);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"DocCap_Y"),597);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"DocCap_WIDTH"),598);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"DocCap_HEIGHT"),599);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"DocCap_FMT"),601);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"DocCap_BPP"),602);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"DocCap_JPEG_Qual"),603);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"DocCap_SYMBOLOGY"),655);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"Sig_FINDBOX"),727);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"PDF_EN"),15);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"EAN128_EN"),14);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"C39_EN"),0);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"C128_EN"),8);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"DocCap_MIN_TEXT"),656);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"DocCap_MAX_TEXT"),657);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"D25_EN"),5);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"I25_EN"),6);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"CBAR_EN"),7);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"DATAMATRIX_EN"),292);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"Sig_BRIGHTEN"),654);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"Sig_SHARPEN"),658);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"Sig_DESKEW"),653);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"DocCap_BORDER"),829);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"DocCap_DELAY"),830);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"Sig_MIN_PERCENT"),651);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"Sig_MAX_ROT"),652);
	m_cmbDocCapConf.SetItemData(m_cmbDocCapConf.AddString(L"DocCap_HIBLUR"),659);
	m_cmbDocCapConf.SetCurSel(0);
}

void CIntelDocCap::SetScannerID(std::wstring *ScannerID)
{
	SelectedScannerID=*ScannerID;
}

void CIntelDocCap::SetAsync(int *ParaA)
{
	Async=*ParaA;
}

void CIntelDocCap::OnCbnSelchangeComboIdcParam()
{
	CHECK_CMD0;
	m_edit_DocCapConfValue.SetWindowText(RSMGet((int)m_cmbDocCapConf.GetItemData(m_cmbDocCapConf.GetCurSel())).c_str()); 
}

void CIntelDocCap::OnBnClickedButtonIdcParamGet()
{
	CHECK_CMD0;
	m_edit_DocCapConfValue.SetWindowText(RSMGet((int)m_cmbDocCapConf.GetItemData(m_cmbDocCapConf.GetCurSel())).c_str()); 
}

std::wstring CIntelDocCap::RSMGet(int opcode)
{
	DWORD_PTR dpOpCode =  m_cmbDocCapConf.GetItemData(m_cmbDocCapConf.GetCurSel());
	wchar_t a[5];
	long status = 1;
	CComBSTR outXml(L"");
	//enable the secure methode '_itow_s' resolve the unsafe functions
	_itow_s((int)dpOpCode, a, 10);
	if ( !SC->cmdGet(SelectedScannerID,a, &outXml, Async, &status) )
	{
		if ( !Async )
		{
			//Update the log tab 
			GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);
		}
	}

	CQuickXmlParser xml(outXml);
	CQuickXmlParser::TAGDATA tag[2] = {0};
	tag[0].Tag.Name			= L"datatype";
	tag[1].Tag.Name			= L"value";

	xml.Configure(tag, 2);
	CQuickXmlParser::xptr p = 0;
	int ParseCount = 0;
	
	PWCHAR pchValue;
	PWCHAR pchDataType;

	wstring reValue;
	do
	{
		p = xml.Parse(p);

		pchValue = xml.Translate(tag[1].Value);
		if(pchValue) reValue.append(pchValue);
		
		pchDataType = xml.Translate(tag[0].Value);
		if(pchDataType) wsSelectedDataType = pchDataType;

		xml.ClearValues(tag);
		
	} while(p != 0);

	return reValue;
}

void CIntelDocCap::OnBnClickedButtonIdcParamSet()
{
	CHECK_CMD0;
	DWORD_PTR dpOpCode =  m_cmbDocCapConf.GetItemData(m_cmbDocCapConf.GetCurSel());
	CString csValue;
	m_edit_DocCapConfValue.GetWindowText(csValue);
	RSMSet((int)dpOpCode, csValue.GetBuffer(), wsSelectedDataType);
}

void CIntelDocCap::OnBnClickedButtonIdcParamStore()
{
	CHECK_CMD0;
	DWORD_PTR dpOpCode =  m_cmbDocCapConf.GetItemData(m_cmbDocCapConf.GetCurSel());
	CString csValue;
	m_edit_DocCapConfValue.GetWindowText(csValue);
	RSMSet((int)dpOpCode, csValue.GetBuffer(), wsSelectedDataType, true);
}

int CIntelDocCap::RSMSet(int attribID, std::wstring value, wstring dataType, bool isStore)
{
	std::wstring AttributeList = L"";
	CComBSTR outXml(L"");
	
	LONG status = -1;

	wchar_t buff[10];
	//enable the secure methode '_itow_s' resolve the unsafe functions
	_itow_s(attribID, buff, 10);
	CString AttribID = buff;
	
	AttributeList.append(L"<attribute><id>");
	AttributeList.append(AttribID);
	AttributeList.append(L"</id><datatype>");
	//If the attribute IDs correspond to non-actionable attributes, then add
	//them as a valid data type.
	if ( AttribID != "6000" && AttribID != "6001" && AttribID != "6003")
	{
		AttributeList.append(dataType);
	}
	//completes the tag 
	AttributeList.append(L"</datatype><value>");
	AttributeList.append(value);
	AttributeList.append(L"</value></attribute>");
	
	if(isStore)
	{
		if ( !SC->cmdStore(SelectedScannerID, AttributeList, Async, &status) )
		{
			if ( !Async )
			{
				//Update the log tab 
				GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);
			}
		}
	}
	else
	{
		if ( !SC->cmdSet(SelectedScannerID, AttributeList, Async, &status) )
		{
			if ( !Async )
			{
				//Update the log tab 
				GetTabManager().GetTabDlg<CLogsDlg>().ShowOutXml(outXml);
			}
		}
	}

	return 0;
}

LPCTSTR CIntelDocCap::GetSymbology(int Code)
{
	switch(Code)
	{
	case ST_NOT_APP: return (_T("NOT APPLICABLE"));
	case ST_CODE_39: return (_T("CODE 39"));
	case ST_CODABAR: return (_T("CODABAR"));
	case ST_CODE_128: return (_T("CODE 128"));
	case ST_D2OF5: return (_T("D 2 OF 5"));
	case ST_IATA: return (_T("IATA"));
	case ST_I2OF5: return (_T("I 2 OF 5"));
	case ST_CODE93: return (_T("CODE 93"));
	case ST_UPCA: return (_T("UPCA"));
	case ST_UPCE0: return (_T("UPCE 0"));
	case ST_EAN8: return (_T("EAN 8"));
	case ST_EAN13: return (_T("EAN 13"));
	case ST_CODE11: return (_T("CODE 11"));
	case ST_CODE49: return (_T("CODE 49"));
	case ST_MSI: return (_T("MSI"));
	case ST_EAN128: return (_T("EAN 128"));
	case ST_UPCE1: return (_T("UPCE 1"));
	case ST_PDF417: return (_T("PDF 417"));
	case ST_CODE16K: return (_T("CODE 16K"));
	case ST_C39FULL: return (_T("C39FULL"));
	case ST_UPCD: return (_T("UPCD"));
	case ST_TRIOPTIC: return (_T("TRIOPTIC"));
	case ST_BOOKLAND: return (_T("BOOKLAND"));
	case ST_UPCA_W_CODE128:return (_T("Coupon Code"));
	case ST_JAN13_W_CODE128:return (_T("EAN/JAN-13 w/Code 128 Supplemental"));
	case ST_NW7: return (_T("NW7"));
	case ST_ISBT128: return (_T("ISBT128"));
	case ST_MICRO_PDF: return (_T("MICRO PDF"));
	case ST_DATAMATRIX: return (_T("DATAMATRIX"));
	case ST_QR_CODE: return (_T("QR CODE"));
	case ST_MICRO_PDF_CCA: return (_T("MICRO PDF CCA"));
	case ST_POSTNET_US: return (_T("POSTNET US"));
	case ST_PLANET_CODE: return (_T("PLANET CODE"));
	case ST_CODE_32: return (_T("CODE 32"));
	case ST_ISBT128_CON: return (_T("ISBT 128 CON"));
	case ST_JAPAN_POSTAL: return (_T("JAPAN POSTAL"));
	case ST_AUS_POSTAL: return (_T("AUS POSTAL"));
	case ST_DUTCH_POSTAL: return (_T("DUTCH POSTAL"));
	case ST_MAXICODE: return (_T("MAXICODE"));
	case ST_CANADIN_POSTAL: return (_T("CANADA POSTAL"));
	case ST_UK_POSTAL: return (_T("UK POSTAL"));
	case ST_MACRO_PDF: return (_T("MACRO PDF"));
	case ST_RSS14: return (_T("RSS 14"));
	case ST_RSS_LIMITET: return (_T("RSS LIMITED"));
	case ST_RSS_EXPANDED: return (_T("RSS EXPANDED"));
	case ST_SCANLET: return (_T("ST SCANLET"));
	case ST_UPCA_2: return (_T("UPCA 2"));
	case ST_UPCE0_2: return (_T("UPCE0 2"));
	case ST_EAN8_2: return (_T("EAN8 2"));
	case ST_EAN13_2: return (_T("EAN13 2"));
	case ST_UPCE1_2: return (_T("UPCE1 2"));
	case ST_CCA_EAN128: return (_T("CCA EAN 128"));
	case ST_CCA_EAN13: return (_T("CCA EAN 13"));
	case ST_CCA_EAN8: return (_T("CCA EAN 8"));
	case ST_CCA_RSS_EXPANDED: return (_T("CCA RSS EXPANDED"));
	case ST_CCA_RSS_LIMITED: return (_T("CCA RSS LIMITED"));
	case ST_CCA_RSS14: return (_T("CCA RSS 14"));
	case ST_CCA_UPCA: return (_T("CCA UPCA"));
	case ST_CCA_UPCE: return (_T("CCA UPCE"));
	case ST_CCC_EAN128: return (_T("CCC EAN 128"));
	case ST_TLC39: return (_T("TLC39"));
	case ST_CCB_EAN128: return (_T("CCB EAN 128"));
	case ST_CCB_EAN13: return (_T("CCB EAN 13"));
	case ST_CCB_EAN8: return (_T("CCB EAN 8"));
	case ST_CCB_RSS_EXPANDED: return (_T("CCB RSS EXPANDED"));
	case ST_CCB_RSS_LIMITED: return (_T("CCB RSS LIMITED"));
	case ST_CCB_RSS14: return (_T("CCB RSS 14"));
	case ST_CCB_UPCA: return (_T("CCB UPCA"));
	case ST_CCB_UPCE: return (_T("CCB UPCE"));
	case ST_SIGNATURE_CAPTURE: return (_T("SIGNATURE CAPTURE"));
	case ST_MATRIX2OF5: return (_T("MATRIX 2 OF 5"));
	case ST_CHINESE2OF5: return (_T("CHINESE 2 OF 5"));
	case ST_UPCA_5: return (_T("UPCA 5"));
	case ST_UPCE0_5: return (_T("UPCE0 5"));
	case ST_EAN8_5: return (_T("EAN8 5"));
	case ST_EAN13_5: return (_T("EAN13 5"));
	case ST_UPCE1_5: return (_T("UPCE1 5"));
	case ST_MACRO_MICRO_PDF: return (_T("MACRO MICRO PDF"));
	case ST_MICRO_QR_CODE: return (_T("MICRO QR CODE"));
	case ST_AZTEC: return (_T("AZTEC"));
	case ST_HAN_XIN_CODE: return (_T("Han Xin Code"));
	default: return _T("");
	}
}
void CIntelDocCap::OnBnClickedButtonIdcClear()
{
	m_RenderEngine.ClearImage();
	m_edit_DecodData.SetWindowText(L"");
	m_edit_DocCapBarcodeSymbology.SetWindowText(L"");
	ClearImageCache();
}

void CIntelDocCap::OnBnClickedCheckUseHid()
{
	if(m_chkUseHID.GetCheck() == BST_CHECKED)
	{
		RSMSet(1004, L"True", L"F");
	}
	else if(m_chkUseHID.GetCheck() == BST_UNCHECKED)
	{
		RSMSet(1004, L"False", L"F");
	}
}


void CIntelDocCap::OnBnClickedButtonIdcSaveImage()
{
	if (m_ImageData.ImageData == 0) return;

	CString Ext;
	CString Filter;
	CFile file;

	GetImageType(Ext, Filter);

	CFileDialog SaveDlg(FALSE, Ext, NULL, 0, Filter);
	if (SaveDlg.DoModal() == IDOK)
	{
		CString csFileName = SaveDlg.GetFolderPath() + L"\\" + SaveDlg.GetFileName();
		file.Open(csFileName, CFile::modeCreate | CFile::modeWrite);
		file.SeekToBegin();
		file.Write(m_ImageData.ImageData, m_ImageData.ImageSize);
		file.Close();
	}
}

void CIntelDocCap::GetImageType(CString &Ext, CString &Filter)
{
	LPBYTE p = m_ImageData.ImageData;
	if (p[0] == 0x42 && p[1] == 0x4D)
	{
		Ext = _T(".bmp");
		Filter = _T("BMP Images (*.bmp)|*.bmp||");
		return;
	}
	if (p[0] == 0xFF && p[1] == 0xD8 && p[2] == 0xFF && p[3] == 0xE0)
	{
		Ext = _T(".jpg");
		Filter = _T("JPEG Images (*.jpg)|*.jpg||");
		return;
	}
	if (p[0] == 0x4D && p[1] == 0x4D && p[2] == 0x00 && p[3] == 0x2A)
	{
		Ext = _T(".tif");
		Filter = _T("TIF Images (*.tif)|*.tif||");
		return;
	}
}