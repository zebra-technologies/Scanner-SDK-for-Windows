/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#include "StdAfx.h"
#include "ISO15434.h"


const char ISO15434::BinaryFormat[] = {'0','9'};
const char ISO15434::FileTypeBarcode[] = {'B','a','r','C','o','d','e'};
const char ISO15434::FileTypeBMP[] = {'B','M','P'};
const char ISO15434::FileTypeJPEG[] = {'J','P','E','G'};
const char ISO15434::FileTypeTIFF[] = {'T','I','F','F'};

ISO15434* ISO15434::GetEnvelope(BYTE* rawData, LONG bufferSize)
{
	int i = 0;
	int packetLength =	((rawData[0] << 24) |	// i = 0
						(rawData[1] << 16) |	// i = 1
						(rawData[2] << 8) |	// i = 2
						rawData[3]);			// i = 3
	i = 4;
	// Header: is PacketLength correct?
	// Packet length does not include itself in the count
	if((packetLength + 4) != bufferSize)
	{
		return NULL;
	}

    // Header: is Message Type correct ?
	if(rawData[i++] != MSG_EASYCAP)				// i = 4
	{
		return NULL;
	}
    
	// ISO15434 Envelope: is message header correct?
    if ((rawData[i++] != '[') || (rawData[i++] != ')') || (rawData[i++] != '>') || (rawData[i++] != ISO_RS)) //i=5,6,7,8
	{
		return NULL;
	}

    // ISO15434 Envelope: is message trailer correct?
    if (rawData[bufferSize - 1] != ISO_EOT)
	{
		return NULL;
	}

	return new ISO15434(rawData, bufferSize);
}

ISO15434::ISO15434(BYTE* rawData, LONG bufferSize)
{
	this->pbRawData = rawData;
	this->m_iIndex = 9;	// Start of format header
	this->m_iRawDataLength = bufferSize;
	this->pbDecodeData = NULL;
}
PISOIMAGEDATA ISO15434::GetNextFormat()
{
	ISO13454Data* pformatData;
	int iDataLength = 0;
	BYTE* tempbuff = &pbRawData[m_iIndex];
	if(pbRawData[m_iIndex] == ISO_EOT)
	{
		return NULL;
	}

	if(0 != memcmp(BinaryFormat,&pbRawData[m_iIndex],2))
	{
		return NULL;
	}

	m_iIndex = m_iIndex + 2;

	if(pbRawData[m_iIndex++] != ISO_GS)
		return NULL;

	int searchIndex;
	searchIndex = SearchChar((char*)&pbRawData[m_iIndex], ISO_GS, m_iRawDataLength - m_iIndex);
	if(searchIndex > -1)
	{
		pformatData = new ISO13454Data();
		if(memcmp(&pbRawData[m_iIndex], FileTypeBarcode, searchIndex) == 0)
		{
			pformatData->DataType = 0;
		}
		else if(memcmp(&pbRawData[m_iIndex], FileTypeBMP, searchIndex) == 0)
		{
			pformatData->DataType = 1;
		}
		else if(memcmp(&pbRawData[m_iIndex], FileTypeJPEG, searchIndex) == 0)
		{
			pformatData->DataType = 2;
		}
		else if(memcmp(&pbRawData[m_iIndex], FileTypeTIFF, searchIndex) == 0)
		{
			pformatData->DataType = 3;
		}
		else
		{
			return NULL;
		}
	}
	else
	{
		return NULL;
	}
	m_iIndex = m_iIndex + searchIndex + 1;
	if(pbRawData[m_iIndex++] != ISO_GS)
		return NULL;

	searchIndex = 0;

	searchIndex = SearchChar((char*)&pbRawData[m_iIndex], ISO_GS, m_iRawDataLength - m_iIndex);
	if(searchIndex > -1)
	{
		char formatlen[10];
		memcpy(formatlen, &pbRawData[m_iIndex], searchIndex);
		formatlen[searchIndex] = NULL;
		iDataLength = atoi(formatlen);
	}
	else
	{
		return NULL;
	}
	m_iIndex = m_iIndex + searchIndex + 1;
	int iDataIndex = m_iIndex;
	BYTE* pbDataTemp;
	pbDataTemp = &pbRawData[iDataIndex];
	switch(pformatData->DataType)
	{
	case 0:
		{
			pformatData->decodeData.bSymbology = pbDataTemp[0];
			if(pbDecodeData)
			{
				delete[] pbDecodeData;
			}
			pbDecodeData = new BYTE[iDataLength];
			memcpy(pbDecodeData, &pbDataTemp[1],iDataLength - 1);
			pbDecodeData[iDataLength-1] = NULL;
			pformatData->decodeData.pbData = pbDecodeData; //&pbDataTemp[1];
			pformatData->decodeData.bDecodeDataLen = iDataLength - 1;
		}
		break;
	case 1:
	case 2:
	case 3:
		{
			int i=0;
			pformatData->ImageData.bImageDataLen = (pbDataTemp[i + 2] << 24) |
												   (pbDataTemp[i + 3] << 16) |
												   (pbDataTemp[i + 4] << 8) |
													pbDataTemp[i + 5];
			pformatData->ImageData.bImageFormat = pformatData->DataType;
			pformatData->ImageData.pbImageData = &pbDataTemp[6];
		}
		break;
	}
	m_iIndex = iDataIndex + iDataLength;
	tempbuff = &pbRawData[m_iIndex];
	if(pbRawData[m_iIndex++] != ISO_RS)
		return NULL;

	return pformatData;
}

int ISO15434::SearchChar(const char* buffer, char c, int length)
{
	int index = 0;
	bool notfound = true;
	while(index < length)
	{
		if(buffer[index++] == c)
		{
			break;
		}
	}
	if(index < length)
	{
		return index - 1;
	}
	return -1;
}

ISO15434::~ISO15434(void)
{
	if(pbDecodeData)
	{
		delete[] pbDecodeData;
		pbDecodeData = NULL;
	}
}
