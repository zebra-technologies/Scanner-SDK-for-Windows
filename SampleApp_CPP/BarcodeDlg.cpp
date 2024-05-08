/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#include "stdafx.h"
#include "ScannerSDKSampleApp.h"
#include "BarcodeDlg.h"
#include "LogsDlg.h"
#include "QuickXmlParser.h"
#include "DadfScriptDlg.h"


IMPLEMENT_DYNAMIC(CBarcodeDlg, CDialog)

CBarcodeDlg::CBarcodeDlg(CWnd* pParent /*=NULL*/)
: CDialog(CBarcodeDlg::IDD, pParent)
{
    Async = 0;
}

CBarcodeDlg::~CBarcodeDlg()
{
}

void CBarcodeDlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);

    DDX_Control(pDX, IDC_EDIT2, txtDecodedData);
    DDX_Control(pDX, IDC_EDIT3, txtSymbology);
    //DDX_Control(pDX, IDC_EXPLORER1, m_EmbeddedBrowser);
    DDX_Control(pDX, IDC_ENABLELANG, m_chkEnableLocaleSelection);
    DDX_Control(pDX, IDC_DRIVERADF, m_chkDriverADF);
    DDX_Control(pDX, IDC_CMBLANG, m_cmbSelectLocale);
    DDX_Control(pDX, IDC_EDIT_Barcode, m_editBarcode);
}

LPCTSTR CBarcodeDlg::GetSymbology(int Code)
{
    switch(Code)
    {
    case ST_NOT_APP:
        return (_T("NOT APPLICABLE"));
    case ST_CODE_39:
        return (_T("CODE 39"));
    case ST_CODABAR:
        return (_T("CODABAR"));
    case ST_CODE_128:
        return (_T("CODE 128"));
    case ST_D2OF5:
        return (_T("DISCRETE 2 OF 5"));
    case ST_IATA:
        return (_T("IATA"));
    case ST_I2OF5:
        return (_T("INTERLEAVED 2 OF 5"));
    case ST_CODE93:
        return (_T("CODE 93"));
    case ST_UPCA:
        return (_T("UPC-A"));
    case ST_UPCE0:
        return (_T("UPC-E0"));
    case ST_EAN8:
        return (_T("EAN-8"));
    case ST_EAN13:
        return (_T("EAN-13"));
    case ST_CODE11:
        return (_T("CODE 11"));
    case ST_CODE49:
        return (_T("CODE 49"));
    case ST_MSI:
        return (_T("MSI"));
    case ST_EAN128:
        return (_T("EAN-128"));
    case ST_UPCE1:
        return (_T("UPC-E1"));
    case ST_PDF417:
        return (_T("PDF-417"));
    case ST_CODE16K:
        return (_T("CODE 16K"));
    case ST_C39FULL:
        return (_T("CODE 39 FULL ASCII"));
    case ST_UPCD:
        return (_T("UPC-D"));
    case ST_TRIOPTIC:
        return (_T("CODE 39 TRIOPTIC"));
    case ST_BOOKLAND:
        return (_T("BOOKLAND"));
    case ST_UPCA_W_CODE128:
        //return (_T("UPC-A w/Code 128 Supplemental"));
        // 0x17 was the old style coupon code that was a specific UPC-A that could have or not have a Code 128 Supplemental. Switched to GS1 Databar.
        return (_T("Coupon Code"));
    case ST_JAN13_W_CODE128:
        return (_T("EAN/JAN-13 w/Code 128 Supplemental"));
    case ST_NW7:
        return (_T("NW-7"));
    case ST_ISBT128:
        return (_T("ISBT-128"));
    case ST_MICRO_PDF:
        return (_T("MICRO PDF"));
    case ST_DATAMATRIX:
        return (_T("DATAMATRIX"));
    case ST_QR_CODE:
        return (_T("QR CODE"));
    case ST_MICRO_PDF_CCA:
        return (_T("MICRO PDF CCA"));
    case ST_POSTNET_US:
        return (_T("POSTNET US"));
    case ST_PLANET_CODE:
        return (_T("PLANET CODE"));
    case ST_CODE_32:
        return (_T("CODE 32"));
    case ST_ISBT128_CON:
        return (_T("ISBT-128 CON"));
    case ST_JAPAN_POSTAL:
        return (_T("JAPAN POSTAL"));
    case ST_AUS_POSTAL:
        return (_T("AUS POSTAL"));
    case ST_DUTCH_POSTAL:
        return (_T("DUTCH POSTAL"));
    case ST_MAXICODE:
        return (_T("MAXICODE"));
    case ST_CANADIN_POSTAL:
        return (_T("CANADIAN POSTAL"));
    case ST_UK_POSTAL:
        return (_T("UK POSTAL"));
    case ST_MACRO_PDF:
        return (_T("MACRO PDF"));
    case ST_MACRO_QR_CODE:
        return (_T("MACRO QR CODE"));
    case ST_MICRO_QR_CODE:
        return (_T("MICRO QR CODE"));
    case ST_AZTEC:
        return (_T("AZTEC"));
    case ST_AZTEC_RUNE:
        return (_T("AZTEC RUNE"));
    case ST_DISTANCE:
        return (_T("DISTANCE"));
    case ST_RSS14:
        return (_T("GS1 DATABAR"));
    case ST_RSS_LIMITET:
        return (_T("GS1 DATABAR LIMITED"));
    case ST_RSS_EXPANDED:
        return (_T("GS1 DATABAR EXPANDED"));
    case ST_PARAMETER:
        return (_T("PARAMETER"));
    case ST_USPS_4CB:
        return (_T("USPS 4CB"));
    case ST_UPU_FICS_POSTAL:
        return (_T("UPU FICS POSTAL"));
    case ST_ISSN:
        return (_T("ISSN"));
    case ST_SCANLET:
        return (_T("SCANLET"));
    case ST_CUECODE:
        return (_T("CUECODE"));
    case ST_MATRIX2OF5:
        return (_T("MATRIX 2 OF 5"));
    case ST_UPCA_2:
        return (_T("UPC-A + 2 SUPPLEMENTAL"));
    case ST_UPCE0_2:
        return (_T("UPC-E0 + 2 SUPPLEMENTAL"));
    case ST_EAN8_2:
        return (_T("EAN-8 + 2 SUPPLEMENTAL"));
    case ST_EAN13_2:
        return (_T("EAN-13 + 2 SUPPLEMENTAL"));
    case ST_UPCE1_2:
        return (_T("UPC-E1 + 2 SUPPLEMENTAL"));
    case ST_CCA_EAN128:
        return (_T("CCA EAN-128"));
    case ST_CCA_EAN13:
        return (_T("CCA EAN-13"));
    case ST_CCA_EAN8:
        return (_T("CCA EAN-8"));
    case ST_CCA_RSS_EXPANDED:
        return (_T("GS1 DATABAR EXPANDED COMPOSITE (CCA)"));
    case ST_CCA_RSS_LIMITED:
        return (_T("GS1 DATABAR LIMITED COMPOSITE (CCA)"));
    case ST_CCA_RSS14:
        return (_T("GS1 DATABAR COMPOSITE (CCA)"));
    case ST_CCA_UPCA:
        return (_T("CCA UPC-A"));
    case ST_CCA_UPCE:
        return (_T("CCA UPC-E"));
    case ST_CCC_EAN128:
        return (_T("CCC EAN-128"));
    case ST_TLC39:
        return (_T("TLC-39"));
    case ST_CCB_EAN128:
        return (_T("CCB EAN-128"));
    case ST_CCB_EAN13:
        return (_T("CCB EAN-13"));
    case ST_CCB_EAN8:
        return (_T("CCB EAN-8"));
    case ST_CCB_RSS_EXPANDED:
        return (_T("GS1 DATABAR EXPANDED COMPOSITE (CCB)"));
    case ST_CCB_RSS_LIMITED:
        return (_T("GS1 DATABAR LIMITED COMPOSITE (CCB)"));
    case ST_CCB_RSS14:
        return (_T("GS1 DATABAR COMPOSITE (CCB)"));
    case ST_CCB_UPCA:
        return (_T("CCB UPC-A"));
    case ST_CCB_UPCE:
        return (_T("CCB UPC-E"));
    case ST_SIGNATURE_CAPTURE:
        return (_T("SIGNATURE CAPTUREE"));
    case ST_MOA:
        return (_T("MOA"));
    case ST_PDF417_PARAMETER:
        return (_T("PDF417 PARAMETER"));
    case ST_CHINESE2OF5:
        return (_T("CHINESE 2 OF 5"));
    case ST_KOREAN_3_OF_5:
        return (_T("KOREAN 3 OF 5"));
    case ST_DATAMATRIX_PARAM:
        return (_T("DATAMATRIX PARAM"));
    case ST_CODE_Z:
        return (_T("CODE Z"));
    case ST_UPCA_5:
        return (_T("UPC-A + 5 SUPPLEMENTAL"));
    case ST_UPCE0_5:
        return (_T("UPC-E0 + 5 SUPPLEMENTAL"));
    case ST_EAN8_5:
        return (_T("EAN-8 + 5 SUPPLEMENTAL"));
    case ST_EAN13_5:
        return (_T("EAN-13 + 5 SUPPLEMENTAL"));
    case ST_UPCE1_5:    
        return (_T("UPC-E1 + 5 SUPPLEMENTAL"));
    case ST_MACRO_MICRO_PDF:
        return (_T("MACRO MICRO PDF"));
    case ST_OCRB:
        return (_T("OCRB"));
    case ST_OCRA:
        return (_T("OCRA"));
    case ST_PARSED_DRIVER_LICENSE:
        return (_T("PARSED DRIVER LICENSE"));
    case ST_PARSED_UID:
        return (_T("PARSED UID"));
    case ST_PARSED_NDC:
        return (_T("PARSED NDC"));
    case ST_DATABAR_COUPON:
        return (_T("DATABAR COUPON"));
    case ST_PARSED_XML:
        return (_T("PARSED XML"));
    case ST_HAN_XIN_CODE:
        return (_T("HAN XIN CODE"));
    case ST_CALIBRATION:
        return (_T("CALIBRATION"));
    case ST_GS1_DATAMATRIX:
        return (_T("GS1 DATA MATRIX"));
    case ST_GS1_QR:
        return (_T("GS1 QR"));
    case BT_MAINMARK:
        return (_T("MAINMARK"));
    case BT_DOTCODE:
        return (_T("DOTCODE"));
    case BT_GRID_MATRIX:
        return (_T("GRID MATRIX"));
    case BT_UDI_CODE:
        return (_T("UDI CODE"));
    default:
        return (_T(""));
    }
}

void CBarcodeDlg::SetBarcode(BSTR ScanData)
{
    this->EnableWindow(true);
    //m_EmbeddedBrowser.ShowXML(ScanData);
    m_editBarcode.SetWindowText(ScanData);

    CQuickXmlParser x(ScanData);
    CQuickXmlParser::TAGDATA tag[3] = {0};
    tag[0].Tag.Name = L"datatype";
    tag[1].Tag.Name = L"datalabel";
    tag[2].Tag.Name = L"rawdata";

    x.Configure(tag, 3);
    x.Parse();
    txtDecodedData.SetWindowTextW(x.TranslateHex2Dec(tag[1].Value));
    CQuickXmlParser::xptr y = x.Translate(tag[0].Value);
    int SymbologyType = _wtoi(y);
    txtSymbology.SetWindowTextW(GetSymbology(SymbologyType));

}

void CBarcodeDlg::SetScannerID(std::wstring *ScannerID)
{
    SelectedScannerID=*ScannerID;
}

void CBarcodeDlg::SetAsync(int *ParaA)
{
    Async=*ParaA;
}

BEGIN_MESSAGE_MAP(CBarcodeDlg, CDialog)
    ON_BN_CLICKED(IDC_BUTTON1, &CBarcodeDlg::OnFlushMacroPDF)
    ON_BN_CLICKED(IDC_BUTTON2, &CBarcodeDlg::OnAbortMacroPDF)
    ON_WM_CTLCOLOR()
    ON_BN_CLICKED(IDC_BUTTON11, &CBarcodeDlg::OnClear)
    ON_BN_CLICKED(IDC_BUTTON14, &CBarcodeDlg::OnBrowseScript)
    ON_BN_CLICKED(IDC_ENABLELANG, &CBarcodeDlg::OnBnClickedEnablelang)
    ON_CBN_SELCHANGE(IDC_CMBLANG, &CBarcodeDlg::OnCbnSelchangeCmblang)
    ON_BN_CLICKED(IDC_DRIVERADF, &CBarcodeDlg::OnSelectDriverADF)
    ON_BN_CLICKED(IDC_BUTTON15, &CBarcodeDlg::OnEditScript)
END_MESSAGE_MAP()


void CBarcodeDlg::SetLocaleConfigInfo()
{
    long status = 1;
    bool Enable = false;
    int Lang = -1;
    
    m_cmbSelectLocale.SetItemData(m_cmbSelectLocale.AddString(L"DEFAULT"), DEFAULT);
    m_cmbSelectLocale.SetItemData(m_cmbSelectLocale.AddString(L"FRENCH"), FRENCH);
    m_cmbSelectLocale.SetItemData(m_cmbSelectLocale.AddString(L"ENGLISH"), ENGLISH);
    
    m_bDisableEvents = true;
    
    if(S_OK == SC->cmdGetKeyboardEmulatorConfig(Enable, Lang, Async, &status))
    {
        m_cmbSelectLocale.SetCurSel(Lang);

        if(Enable)
        {
            m_chkEnableLocaleSelection.SetCheck(BST_CHECKED);
        }
        else
        {
            m_chkEnableLocaleSelection.SetCheck(BST_UNCHECKED);
            m_cmbSelectLocale.EnableWindow(0);
        }
    }
    
    m_bDisableEvents = false;
}


void CBarcodeDlg::OnFlushMacroPDF()
{
    CHECK_CMD0;
    long status=1;
    SC->cmdFlushMacroPdf(SelectedScannerID, Async, &status);
    LOG(status, "FLUSH_MACROPDF");
}

void CBarcodeDlg::OnAbortMacroPDF()
{
    CHECK_CMD0;
    long status=1;
    SC->cmdAbortMacroPdf(SelectedScannerID, Async, &status);
    LOG(status, "ABORT_MACROPDF");
}

BOOL CBarcodeDlg::OnInitDialog()
{
    CDialog::OnInitDialog();
    OnClear();
    SetLocaleConfigInfo();
    m_chkDriverADF.EnableWindow(FALSE);
    return TRUE;  
}

HBRUSH CBarcodeDlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
    CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
    if(!m_brush.m_hObject)
        m_brush.CreateSolidBrush(RGB(247, 250, 253));
    
    SetLabelBkg(pDC, pWnd, IDC_STATIC);
    return m_brush;
}

void CBarcodeDlg::OnClear()
{
    //m_EmbeddedBrowser.Navigate(_T("about:blank"), 0, 0, 0, 0);
    m_editBarcode.SetWindowText(L"");
    txtDecodedData.SetWindowTextW(_T(""));
    txtSymbology.SetWindowTextW(_T(""));
}


void CBarcodeDlg::OnBnClickedEnablelang()
{
    if(m_bDisableEvents) return;
    
    CHECK_CMD0;
    long status = 1;
    
    if(m_chkEnableLocaleSelection.GetCheck())
    {
        SC->cmdEnableKeyboardEmulator(true, Async, &status);
        m_cmbSelectLocale.EnableWindow(1);
        LOG(status, "ENABLED EMULATOR");
    }
    else
    {
        SC->cmdEnableKeyboardEmulator(false, Async, &status);
        m_cmbSelectLocale.EnableWindow(0);
        LOG(status, "DISABLED EMULATOR");
    }
}

void CBarcodeDlg::OnCbnSelchangeCmblang()
{
    if(m_bDisableEvents) return;
    
    CHECK_CMD0;
    
    int inSel = (int) m_cmbSelectLocale.GetItemData(m_cmbSelectLocale.GetCurSel());
    long status = 1;
    
    SC->cmdSetKeyboardEmulatorLocale(inSel, Async, &status);
    switch(inSel)
    {
    case DEFAULT:
        LOG(status, "SET LANGUAGE DEFAULT");
        break;
    case ENGLISH:
        LOG(status, "SET LANGUAGE ENGLISH");
        break;
    case FRENCH:
        LOG(status, "SET LANGUAGE FRENCH");
        break;
    }
}


void CBarcodeDlg::OnSelectDriverADF()
{
    if(m_chkDriverADF.GetCheck() == BST_UNCHECKED)
    {
        ResetDriverADF();
        m_chkDriverADF.SetWindowText(L"Not Set");
        m_chkDriverADF.EnableWindow(FALSE);
    }
}

void CBarcodeDlg::SetDriverADFbyPath()
{
    long status = 1;
    SC->cmdSetDADFSource(m_DADFPath.GetString(), Async, &status);
}

void CBarcodeDlg::SetDriverADFbySource()
{
    long status = 1;

    //XML encode entities
    CString scriptSource(m_DADFSource);
    scriptSource.Replace(L"&", L"&amp;");
    scriptSource.Replace(L"<", L"&lt;");
    scriptSource.Replace(L">", L"&gt;");
    scriptSource.Replace(L"\'", L"&apos;");
    scriptSource.Replace(L"\"", L"&quot;");
    
    SC->cmdSetDADFSource(scriptSource.GetString(), Async, &status);
}

void CBarcodeDlg::ResetDriverADF()
{
    long status = 1;
    SC->cmdResetDADFSource(Async, &status);
    m_DADFSource = "";
    m_DADFPath = "";
}

void CBarcodeDlg::OnEditScript()
{
    CDadfScriptDlg dadf(m_DADFSource);
    INT_PTR ret = dadf.DoModal();

    if(ret == IDOK)
    {
        if(m_DADFSource.IsEmpty())
        {
            m_chkDriverADF.SetCheck(BST_UNCHECKED);
            m_chkDriverADF.SetWindowText(L"Not Set");
            m_chkDriverADF.EnableWindow(FALSE);
            ResetDriverADF();
        }
        else
        {
            m_chkDriverADF.EnableWindow(TRUE);
            m_chkDriverADF.SetCheck(BST_CHECKED);
            m_chkDriverADF.SetWindowText(L"Unload");
            SetDriverADFbySource();
            m_DADFPath = "";
        }
    }
}

void CBarcodeDlg::OnBrowseScript()
{
    CFileDialog fOpenDlg(TRUE, L"dadf", L"", OFN_HIDEREADONLY|OFN_FILEMUSTEXIST, 
                            L"DADF files (*.dadf)|*.dadf||", this);
    
    fOpenDlg.m_pOFN->lpstrTitle = L"Driver ADF Scripts";
    fOpenDlg.m_pOFN->lpstrInitialDir = L".";
    
    if ( fOpenDlg.DoModal() == IDOK )
    {
        m_DADFPath = fOpenDlg.GetPathName();
        if(m_DADFPath.IsEmpty()) return;
    
        SetDriverADFbyPath();
        m_chkDriverADF.EnableWindow(TRUE);
        m_chkDriverADF.SetCheck(BST_CHECKED);
        m_chkDriverADF.SetWindowText(L"Unload");
        m_DADFSource = "";
    }
}