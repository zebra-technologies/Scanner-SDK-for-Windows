// ScanToConnectDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ScanToConnectDlg.h"
#include "afxdialogex.h"
#include "ScannerSDKSampleApp.h"
#include "LogsDlg.h"

#define PAIRE_BC_TYPE_LEGCY		0
#define PAIRE_BC_TYPE_NEW		1

#define PAIRE_BC_PROT_SSI		0
#define PAIRE_BC_PROT_SPP		1
#define PAIRE_BC_PROT_HID		2

#define PAIRE_BC_HOST_SSI_BT		0
#define PAIRE_BC_HOST_SSI_BT_LE		1
#define PAIRE_BC_HOST_SSI_BT_MFI	2

#define PIARE_BC_DEFAULT_NONE			0
#define PIARE_BC_DEFAULT_SET_FACTORY	1
#define PIARE_BC_DEFAULT_RESET_FACTORY	2

#define PAIR_BC_SIZE_SMALL		1
#define PAIR_BC_SIZE_MEDIUM		2
#define PAIR_BC_SIZE_LARGE		3




// CScanToConnectDlg dialog

IMPLEMENT_DYNAMIC(CScanToConnectDlg, CDialog)


BEGIN_MESSAGE_MAP(CScanToConnectDlg, CDialog)
	ON_CBN_SELCHANGE(IDC_COMBO_ScannerType, &CScanToConnectDlg::OnCbnSelchangeComboScannertype)
	ON_CBN_SELCHANGE(IDC_COMBO_DefaultOption, &CScanToConnectDlg::OnCbnSelchangeComboScannertype)
	ON_CBN_SELCHANGE(IDC_COMBO_HostName, &CScanToConnectDlg::OnCbnSelchangeComboScannertype)
	ON_CBN_SELCHANGE(IDC_COMBO_ImageSize, &CScanToConnectDlg::OnCbnSelchangeComboScannertype)
	ON_CBN_SELCHANGE(IDC_COMBO_ProtocolName, &CScanToConnectDlg::OnCbnSelchangeComboScannertype)
	ON_WM_CTLCOLOR()
	ON_BN_CLICKED(IDC_BUTTON_SaveBarcode, &CScanToConnectDlg::OnBnClickedButtonSavebarcode)
END_MESSAGE_MAP()

CScanToConnectDlg::CScanToConnectDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CScanToConnectDlg::IDD, pParent)
{
	Async = 0;
	m_ImageData.ImageData=0;
	m_ImageData.ImageSize=0;
}

CScanToConnectDlg::~CScanToConnectDlg()
{
}

void CScanToConnectDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_COMBO_ScannerType, m_cmbScannerType);
	DDX_Control(pDX, IDC_COMBO_ProtocolName, m_cmbProtocolName);
	DDX_Control(pDX, IDC_COMBO_HostName, m_cmbHostName);
	DDX_Control(pDX, IDC_COMBO_DefaultOption, m_cmbDefaultOption);
	DDX_Control(pDX, IDC_COMBO_ImageSize, m_cmbImageSize);
	DDX_Control(pDX, IDC_STATIC_ScanToConnectBarcode, m_picPairingBarcode);
	DDX_Control(pDX, IDC_BUTTON_SaveBarcode, m_btnSaveBarcode);
}

void CScanToConnectDlg::SetScannerID(std::wstring *ScannerID)
{
	SelectedScannerID=*ScannerID;
}


HBRUSH CScanToConnectDlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
	if(!m_brush.m_hObject)
		m_brush.CreateSolidBrush(RGB(247, 250, 253));

	SetLabelBkg(pDC, pWnd, IDC_STATIC);
	return m_brush;
}
// CScanToConnectDlg message handlers


void CScanToConnectDlg::OnCbnSelchangeComboScannertype()
{
	RequestPairingBarcode();
}

BOOL CScanToConnectDlg::OnInitDialog()
{
	CDialog::OnInitDialog();
	m_cmbScannerType.SetItemData(m_cmbScannerType.AddString(L"Legacy"), PAIRE_BC_TYPE_LEGCY);
	m_cmbScannerType.SetItemData(m_cmbScannerType.AddString(L"New"), PAIRE_BC_TYPE_NEW);
	m_cmbScannerType.SetCurSel(0);

	m_cmbProtocolName.SetItemData(m_cmbProtocolName.AddString(L"Simple Serial Inerface (SSI)"), PAIRE_BC_PROT_SSI);
	m_cmbProtocolName.SetItemData(m_cmbProtocolName.AddString(L"Serial Port Profile (SPP)"), PAIRE_BC_PROT_SPP);
	m_cmbProtocolName.SetItemData(m_cmbProtocolName.AddString(L"Human Interface Device (HID)"), PAIRE_BC_PROT_HID);
	m_cmbProtocolName.SetCurSel(0);

	m_cmbHostName.SetItemData(m_cmbHostName.AddString(L"SSI BT Classic (Non-Discoverable)"), PAIRE_BC_HOST_SSI_BT);
	m_cmbHostName.SetCurSel(0);


	m_cmbDefaultOption.SetItemData(m_cmbDefaultOption.AddString(L"No Defaults"), PIARE_BC_DEFAULT_NONE);
	m_cmbDefaultOption.SetItemData(m_cmbDefaultOption.AddString(L"Set Factory Defaults"), PIARE_BC_DEFAULT_SET_FACTORY);
	m_cmbDefaultOption.SetItemData(m_cmbDefaultOption.AddString(L"Restore Factory Defaults"), PIARE_BC_DEFAULT_RESET_FACTORY);
	m_cmbDefaultOption.SetCurSel(0);

	m_cmbImageSize.SetItemData(m_cmbImageSize.AddString(L"Small"), PAIR_BC_SIZE_SMALL);
	m_cmbImageSize.SetItemData(m_cmbImageSize.AddString(L"Medium"), PAIR_BC_SIZE_MEDIUM);
	m_cmbImageSize.SetItemData(m_cmbImageSize.AddString(L"Large"), PAIR_BC_SIZE_LARGE);
	m_cmbImageSize.SetCurSel(1);

	m_RenderEngine.Attach(m_picPairingBarcode);

	return TRUE;
}

void CScanToConnectDlg::SetAsync(int *ParaA)
{
	Async=*ParaA;
}

void CScanToConnectDlg::OnParameterBarcode(LPBYTE MediaBuffer, LONG BufferSize)
{
	ClearImageCache();
	m_ImageData.ImageData = new BYTE[BufferSize + (std::size_t) 2];
	memcpy(m_ImageData.ImageData, MediaBuffer, BufferSize);
	m_ImageData.ImageSize = BufferSize;

	m_RenderEngine.ClearImage();
	m_RenderEngine.Render(MediaBuffer, BufferSize);
}

void CScanToConnectDlg::RequestPairingBarcode()
{
	CHECK_CMD0;
	long status=1;

	int protocol = 0;
	
	if(m_cmbScannerType.GetItemData(m_cmbScannerType.GetCurSel()) == PAIRE_BC_TYPE_LEGCY)
	{
		protocol = 1;
		m_cmbHostName.EnableWindow(FALSE);
		m_cmbProtocolName.EnableWindow(FALSE);
		m_cmbDefaultOption.EnableWindow(FALSE);
	}
	else
	{
		m_cmbHostName.EnableWindow(TRUE);
		m_cmbProtocolName.EnableWindow(TRUE);
		m_cmbDefaultOption.EnableWindow(TRUE);
		if(m_cmbProtocolName.GetItemData(m_cmbProtocolName.GetCurSel()) == PAIRE_BC_PROT_SPP)
		{
			m_cmbHostName.EnableWindow(FALSE);
			protocol = 14;
		}
		else if(m_cmbProtocolName.GetItemData(m_cmbProtocolName.GetCurSel()) == PAIRE_BC_PROT_HID)
		{
			m_cmbHostName.EnableWindow(FALSE);
			protocol = 17;
		}
		else
		{
			if( m_cmbHostName.GetItemData(m_cmbHostName.GetCurSel()) == PAIRE_BC_HOST_SSI_BT)
			{
				protocol = 22;
			}
		}
	}


	SC->cmdGetBluetoothPairingBarcode(SelectedScannerID, Async, &status, protocol,(int) m_cmbDefaultOption.GetItemData(m_cmbDefaultOption.GetCurSel()),(int) m_cmbImageSize.GetItemData(m_cmbImageSize.GetCurSel()), L"");
	LOG(status, "GET_PAIRING_BARCODE");
}


void CScanToConnectDlg::OnBnClickedButtonSavebarcode()
{
		if(m_ImageData.ImageData == 0) return;

	CString Ext(L".jpeg");
	CString Filter(L"jpeg files (*.jpeg)|*.jpeg|All files (*.*)|*.*||");
	CFile file;

	//GetImageType(Ext, Filter);
	CFileDialog SaveDlg(FALSE, Ext, NULL, OFN_OVERWRITEPROMPT, Filter);
	if(SaveDlg.DoModal() == IDOK)
	{
		CString csFileName = SaveDlg.GetFolderPath() + L"\\"+ SaveDlg.GetFileName();
		file.Open(csFileName, CFile::modeCreate | CFile::modeWrite);
		file.SeekToBegin();
		file.Write(m_ImageData.ImageData, m_ImageData.ImageSize);
		file.Close();
	}
}

void CScanToConnectDlg::EnableUIControllers(BOOL state)
{
	m_cmbScannerType.EnableWindow(state);
	//m_cmbHostName.EnableWindow(state);
	//m_cmbProtocolName.EnableWindow(state);
	//m_cmbDefaultOption.EnableWindow(state);
	m_cmbImageSize.EnableWindow(state);
	m_btnSaveBarcode.EnableWindow(state);
}
