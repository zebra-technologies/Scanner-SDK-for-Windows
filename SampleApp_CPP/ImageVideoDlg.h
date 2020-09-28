/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#pragma once
#include "ScannerCommands.h"
#include "afxwin.h"

#include <GdiPlus.h>
using namespace Gdiplus;

/***
Class:		 CRender
Description: Image and Video Rendering helper class based on GDI+
Author:		 VRQW74
***/
class CRender
{
	static const long GLOBAL_MEM = 2*1024*1024; //Global memory = 2MB (Guessestimate!)

public:

	CRender() : m_State(true), m_pGraphics(0)
	{
		m_LargeInt.QuadPart = 0;
		
		m_hGlobal = GlobalAlloc(GMEM_MOVEABLE|GMEM_NODISCARD, GLOBAL_MEM);
		if(m_hGlobal == NULL)
		{
			m_State = false;
			return;
		}
		HRESULT hr = CreateStreamOnHGlobal(m_hGlobal, TRUE, &m_pStrm);
		if(hr != S_OK || m_pStrm == 0)
		{
			m_State = false;
			m_pStrm = 0;
			return;
		}
		GdiplusStartupInput gdiplusStartupInput;
		Status sta = GdiplusStartup(&m_gdiplusToken, &gdiplusStartupInput, NULL);
		if(sta != Ok)
		{
			m_State = false;
			return;
		}
	}

	~CRender()
	{
		if(m_pGraphics)
		{
			delete m_pGraphics;
			m_pGraphics = 0;
		}
		
		GdiplusShutdown(m_gdiplusToken);

		if(m_pStrm)
		{
			m_pStrm->Release();
			m_pStrm = 0;
		}
		if(m_hGlobal)
			GlobalFree(m_hGlobal);
	}

	void Attach(CStatic &ctrl)
	{
		ctrl.GetClientRect(&m_rcStatic);
		m_pGraphics = new Graphics(ctrl.GetDC()->m_hDC);
		if(m_pGraphics == 0)
			m_State = false;
	}

	void Render(LPBYTE MediaBuffer, LONG BufferSize)
	{
		if(m_State)
		{
			HRESULT hr = m_pStrm->Seek(m_LargeInt, STREAM_SEEK_SET, NULL);
			if(hr != S_OK) return;
			hr = m_pStrm->Write(MediaBuffer, (ULONG)BufferSize, NULL);
			if(hr != S_OK) return;
			hr = m_pStrm->Seek(m_LargeInt, STREAM_SEEK_SET, NULL);
			if(hr != S_OK) return;

			Image *pImage = new Image(m_pStrm);
			if(pImage == 0) return;
			if(pImage->GetHeight() < UINT(m_rcStatic.Height()) && pImage->GetWidth() < UINT(m_rcStatic.Width()))
			{
				Status sta = m_pGraphics->DrawImage(pImage, 0, 0);
			}
			else
			{
				Status sta = m_pGraphics->DrawImage(pImage, 0, 0, m_rcStatic.Width(), m_rcStatic.Height());
			}
			
			delete pImage;
		}
	}

	void ClearImage()
	{
		Color cc(0xFF,0xFF,0xFF);
		m_pGraphics->Clear(cc);
	}

private: 

	bool m_State{ true };
	HGLOBAL m_hGlobal;
	ULONG_PTR m_gdiplusToken ;
	IStream *m_pStrm ;
	CRect m_rcStatic;
	Graphics* m_pGraphics{ 0 };
	LARGE_INTEGER m_LargeInt;

};

//Simple struct to store image data for saving cached images.
typedef struct _IMAGE_DATA
{
	LPBYTE ImageData;
	DWORD  ImageSize;

} IMAGE_DATA;


//Image Video Dialog Handling
class CImageVideoDlg : public CDialog
{
	DECLARE_DYNAMIC(CImageVideoDlg)

public:

	CImageVideoDlg(CWnd* pParent = NULL);   
	virtual ~CImageVideoDlg();

	void SetScannerID(wstring * ScannerID);
	void SetAsync(int *ParaAsync);
	void SetAbortFirmwareState(BOOL bState);


	enum { IDD = IDD_ImageVideo };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);

	void ClearImageCache()
	{
		if(m_ImageData.ImageData)
		{
			delete[] m_ImageData.ImageData;
			m_ImageData.ImageData = 0;
		}
		m_ImageData.ImageSize = 0;
	}

	void GetImageType(CString &Ext, CString &Filter);

	DECLARE_MESSAGE_MAP()
	DECLARE_TAB_WINDOW(Image/Video)

public:
	afx_msg void OnImage();
	afx_msg void OnVideo();
	afx_msg void OnAbortTransfer();
	afx_msg void OnSelectJPG();
	afx_msg void OnSelectTIFF();
	afx_msg void OnSelectBMP();
	afx_msg void OnSelectViewFinder();
	afx_msg void OnClickSaveImage();
	afx_msg void OnClickSetBarcodeMode();

public:
	virtual BOOL OnInitDialog();
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
	void OnImageCapture(LPBYTE MediaBuffer, LONG BufferSize);
	void OnVideoCapture(LPBYTE MediaBuffer, LONG BufferSize);

private:
	CStatic m_PicControl;
	CRender m_RenderEngine;
	CBrush  m_brush;
	CButton RdTIFF;
	CButton RdBMP;
	CButton RdJPG;
	CButton chkVideoViewFinderEnable;
	wstring SelectedScannerID;
	int Async;

	IMAGE_DATA m_ImageData;


};
