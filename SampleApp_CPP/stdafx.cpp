
// stdafx.cpp : source file that includes just the standard includes
// ScannerSDKSampleApp.pch will be the pre-compiled header
// stdafx.obj will contain the pre-compiled type information

#include "stdafx.h"

void SetLabelBkg(CDC* pDC, CWnd *pWnd, int ID)
{
	if(pWnd->GetDlgCtrlID() == ID)
	{
		pDC->SetBkMode(TRANSPARENT);
	}
}
