/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#include "stdafx.h"
#include "ScannerSDKSampleApp.h"
#include "ImageVideoDlg.h"
#include "LogsDlg.h"

const int IMAGE_FILETYPE_PARAMNUM = 0x0130;   
const int BMP_FILE_SELECTION = 0x0003;   
const int TIFF_FILE_SELECTION = 0x0004;   
const int JPEG_FILE_SELECTION = 0x0001;

IMPLEMENT_DYNAMIC(CImageVideoDlg, CDialog)

CImageVideoDlg::CImageVideoDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CImageVideoDlg::IDD, pParent)
{
	m_ImageData.ImageData = 0;
	m_ImageData.ImageSize = 0;

	Async = 0;
}

CImageVideoDlg::~CImageVideoDlg()
{
	ClearImageCache();
}

void CImageVideoDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_CHECK1, chkVideoViewFinderEnable);
	DDX_Control(pDX,IDC_RADIO1,RdJPG);
	DDX_Control(pDX,IDC_RADIO2,RdTIFF);
	DDX_Control(pDX,IDC_RADIO3,RdBMP);
	DDX_Control(pDX, IDC_STATIC_PICTURE_CONTROL, m_PicControl);
}

void CImageVideoDlg::SetScannerID(std::wstring *ScannerID)
{
	SelectedScannerID = *ScannerID;
}

void CImageVideoDlg::SetAsync(int *ParaA)
{
	Async = *ParaA;
}


BEGIN_MESSAGE_MAP(CImageVideoDlg, CDialog)
	ON_BN_CLICKED(IDC_BUTTON1, &CImageVideoDlg::OnImage)
	ON_BN_CLICKED(IDC_BUTTON2, &CImageVideoDlg::OnVideo)
	ON_BN_CLICKED(IDC_BUTTON3, &CImageVideoDlg::OnAbortTransfer)
	ON_BN_CLICKED(IDC_RADIO1, &CImageVideoDlg::OnSelectJPG)
	ON_BN_CLICKED(IDC_RADIO2, &CImageVideoDlg::OnSelectTIFF)
	ON_BN_CLICKED(IDC_RADIO3, &CImageVideoDlg::OnSelectBMP)
	ON_WM_CTLCOLOR()
	ON_BN_CLICKED(IDC_CHECK1, &CImageVideoDlg::OnSelectViewFinder)
	ON_BN_CLICKED(IDC_BUTTON11, &CImageVideoDlg::OnClickSaveImage)
	ON_BN_CLICKED(IDC_BUTTON13, &CImageVideoDlg::OnClickSetBarcodeMode)
END_MESSAGE_MAP()


void CImageVideoDlg::OnImage()
{
	CHECK_CMD0;

	long status=1;	
	SC->cmdCaptureImage(SelectedScannerID, Async, &status);
	LOG(status, "SET_IMAGE_MODE");
}

void CImageVideoDlg::OnVideo()
{
	CHECK_CMD0;

	long status=1;	
	SC->cmdCaptureVideo(SelectedScannerID, Async, &status);
	LOG(status, "SET_VIDEO_MODE");
}

void CImageVideoDlg::OnAbortTransfer()
{
	CHECK_CMD0;

	long status=1;	
	SC->cmdAbortImageXfer(SelectedScannerID,Async,&status);
	LOG(status, "ABORT_IMAGE_XFER");
}

void CImageVideoDlg::OnSelectJPG()
{
	CHECK_CMD0;

	if(RdJPG.GetCheck())
	{
		long status=1;
		wstring ID;
		wstring Value;

		wchar_t buf[8];
		int a = 10;
		_itow_s(IMAGE_FILETYPE_PARAMNUM, buf, 8, 10);
		ID.append(buf);
		_itow_s(JPEG_FILE_SELECTION, buf, 8, 10);
		Value.append(buf);
		SC->cmdSetParametersEx(SelectedScannerID, ID, Value, 0, &status);
		LOG(status, "SET_PARAMETERS_IMAGE_JPG");
	}
}

void CImageVideoDlg::OnSelectTIFF()
{
	CHECK_CMD0;

	if(RdTIFF.GetCheck())
	{
		long status=1;
		wstring ID;
		wstring Value;

		wchar_t buf[8];
		int a = 10;
		_itow_s(IMAGE_FILETYPE_PARAMNUM, buf, 8, 10);
		ID.append(buf);
		_itow_s(TIFF_FILE_SELECTION, buf, 8, 10);
		Value.append(buf);
		SC->cmdSetParametersEx(SelectedScannerID, ID, Value, 0, &status);
		LOG(status, "SET_PARAMETERS_IMAGE_TIFF");
	}
}

void CImageVideoDlg::OnSelectBMP()
{
	CHECK_CMD0;

	if(RdBMP.GetCheck())
	{
		long status=1;
		wstring ID;
		wstring Value;

		wchar_t buf[8];
		int a = 10;
		_itow_s(IMAGE_FILETYPE_PARAMNUM, buf, 8, 10);
		ID.append(buf);
		_itow_s(BMP_FILE_SELECTION, buf, 8, 10);
		Value.append(buf);
		SC->cmdSetParametersEx(SelectedScannerID, ID, Value, 0, &status);
		LOG(status, "SET_PARAMETERS_IMAGE_BMP");
	}
}

BOOL CImageVideoDlg::OnInitDialog()
{
	CDialog::OnInitDialog();
	m_RenderEngine.Attach(m_PicControl);
	return TRUE; 
}

HBRUSH CImageVideoDlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
	if(!m_brush.m_hObject)
		m_brush.CreateSolidBrush(RGB(247, 250, 253));
	return m_brush;
}

void CImageVideoDlg::OnImageCapture(LPBYTE MediaBuffer, LONG BufferSize)
{
	ClearImageCache();
	m_ImageData.ImageData = new BYTE[BufferSize + (std::size_t)2];
	memcpy(m_ImageData.ImageData, MediaBuffer, BufferSize);
	m_ImageData.ImageSize = BufferSize;

	m_RenderEngine.Render(MediaBuffer, BufferSize);
}

void CImageVideoDlg::OnVideoCapture(LPBYTE MediaBuffer, LONG BufferSize)
{
	m_RenderEngine.Render(MediaBuffer, BufferSize);
}

const short VIDEOVIEWFINDER_PARAMNUM = 324;
const short VIDEOVIEWFINDER_ON		 = 1;   /* Video view finder on   */
const short VIDEOVIEWFINDER_OFF		 = 0;   /* Video view finder off  */


void CImageVideoDlg::OnSelectViewFinder()
{
	int val = chkVideoViewFinderEnable.GetCheck();
	wstring ViewFinderEnable = ((val == 1) ? L"1" : L"0");

	long status=1;	
	wstring ViewFinderParam = L"324";
	SC->cmdViewFinderParameter(SelectedScannerID, ViewFinderParam, ViewFinderEnable, 0, &status);
	LOG(status, "SET PARAMETER VIEW-FINDER");

}

void CImageVideoDlg::GetImageType(CString &Ext, CString &Filter)
{
	LPBYTE p = m_ImageData.ImageData;
	if(p[0] == 0x42 && p[1] == 0x4D)
	{
		Ext = _T(".bmp");
		Filter = _T("BMP Images (*.bmp)|*.bmp||");
		return;
	}
	if(p[0] == 0xFF && p[1] == 0xD8 && p[2] == 0xFF && p[3] == 0xE0)
	{
		Ext = _T(".jpg");
		Filter = _T("JPEG Images (*.jpg)|*.jpg||");
		return;
	}
	if(p[0] == 0x4D && p[1] == 0x4D && p[2] == 0x00 && p[3] == 0x2A)
	{
		Ext = _T(".tif");
		Filter = _T("TIF Images (*.tif)|*.tif||");
		return;
	}
}


void CImageVideoDlg::OnClickSaveImage()
{
	if(m_ImageData.ImageData == 0) return;

	CString Ext;
	CString Filter;
	CFile file;

	GetImageType(Ext, Filter);
	CFileDialog SaveDlg(FALSE, Ext, NULL, 0, Filter);
	if(SaveDlg.DoModal() == IDOK)
	{
		CString csFileName = SaveDlg.GetFolderPath() + L"\\"+ SaveDlg.GetFileName();
		file.Open(csFileName, CFile::modeCreate | CFile::modeWrite);
		file.SeekToBegin();
		file.Write(m_ImageData.ImageData, m_ImageData.ImageSize);
		file.Close();
	}
}

void CImageVideoDlg::SetAbortFirmwareState(BOOL bState)
{
	this->GetDlgItem(IDC_BUTTON3)->EnableWindow(bState);
}

void CImageVideoDlg::OnClickSetBarcodeMode()
{
	CHECK_CMD0;

	long status = 1;	
	SC->cmdSetBarcodeMode(SelectedScannerID, Async, &status);
	LOG(status, "SET_BARCODE_MODE");

}
