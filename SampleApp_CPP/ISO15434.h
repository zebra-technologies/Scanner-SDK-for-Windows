/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/

#pragma once
#include "stdafx.h"
        
struct DecodeData_T
{
	DWORD bDecodeDataLen;
	BYTE bSymbology;
	BYTE* pbData;
};
struct DocCapImageData_T
{
	BYTE bImageFormat;
	DWORD bImageDataLen;
	BYTE* pbImageData;
};

typedef struct ISO13454Data
{
	BYTE DataType;
	union
	{
		DecodeData_T decodeData;
		DocCapImageData_T ImageData;
	};
}*PISOIMAGEDATA;



class ISO15434
{
public:
	 static const BYTE ISO_RS = 0x1E;		// ISO15454 Format Trailer Character
	 static const BYTE ISO_GS = 0x1D;		// ISO15454 Data Element Seperator
	 static const BYTE ISO_EOT = 0x04;		// ISO15454 Message Trailer Character
	 static const BYTE MSG_EASYCAP = 0;    // ISO15451 Message DocCap message number
	 
	 static const char BinaryFormat[];
	 
	 static const char FileTypeBarcode[];// = {'B','a','r','C','o','d','e'};
	 static const char FileTypeBMP[] ;//= {'B','M','P'};
	 static const char FileTypeJPEG[];// = {'J','P','E','G'};
	 static const char FileTypeTIFF[];// = {'T','I','F','F'};

public:
	static ISO15434* GetEnvelope(BYTE* pByteBuffer, LONG bufferSize);
	PISOIMAGEDATA GetNextFormat();
	~ISO15434(void);

private:
	ISO15434(BYTE* pByteBuffer, LONG bufferSize);
	BYTE* pbRawData;
	int m_iIndex;
	int SearchChar(const char* buffer, char c, int length);
	int m_iRawDataLength;
	BYTE* pbDecodeData;

};
