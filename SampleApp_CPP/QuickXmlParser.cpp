/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#include "stdafx.h"
#include "QuickXmlParser.h"

CQuickXmlParser::CQuickXmlParser(void) : m_pValue(0)
{
	Clear();
}

CQuickXmlParser::CQuickXmlParser(BSTR xml) : m_pValue(0)
{
	Attach(xml);
}

void CQuickXmlParser::Attach(BSTR xml)
{
	Clear();
	m_Xml = xml;
	m_XmlLength = SysStringLen(m_Xml);
}
//The BSTR returned should be freed by the Caller to avoid a memory leak!
BSTR CQuickXmlParser::LoadXmlFile(LPTSTR XmlFilePath )
{
	HANDLE	hConfigXml = NULL;
	BOOL ret = FALSE;
	BSTR bs = 0;
	DWORD dwRead, dwReadBufferSize;
	char* Buffer = 0;

	hConfigXml = CreateFile(XmlFilePath, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, 0);
	if(hConfigXml == INVALID_HANDLE_VALUE ) goto end;

	dwReadBufferSize = GetFileSize(hConfigXml, NULL);
	if(dwReadBufferSize == INVALID_FILE_SIZE) goto end;

	Buffer = new char[dwReadBufferSize + 8];

	ret = ReadFile(hConfigXml, Buffer, dwReadBufferSize + 8, &dwRead, NULL);
	if((ret && dwRead) == 0) goto end;
	

	LPWSTR pszDest = new WCHAR[dwRead];
	::MultiByteToWideChar( CP_THREAD_ACP, 0, Buffer, dwRead, pszDest, dwRead );
	bs = SysAllocStringLen(pszDest, dwRead);
	delete [] Buffer;
	delete [] pszDest;

end:
	CloseHandle(hConfigXml);
	return bs;

}

void CQuickXmlParser::Clear()
{
	if(m_pValue)
	{
		delete[] m_pValue;
		m_pValue = 0;
	}
	m_pTagArray = 0;
	m_ArrayLength = 0; 
	m_pValue = 0;
	m_XmlLength = 0;
	m_Xml = 0;
	m_Depth = -1; //Make allowance for the <?xml ...?>
	CallbackProc = 0;
}

void CQuickXmlParser::ClearValues(PTAGDATA Array)
{
	for(int i = 0; i < m_ArrayLength; ++i)
	{
		PTAGDATA pdata = &(Array[i]);
		memset(&(pdata->Value), 0, sizeof(FIELD));
		memset(&(pdata->AttribValues[0]), 0, sizeof(FIELD)*5);
		pdata->Depth = -1;
	}
}

CQuickXmlParser::PTAGDATA CQuickXmlParser::GetTags()
{
	return m_pTagArray;
}

CQuickXmlParser::~CQuickXmlParser(void)
{
	Clear();
}

void CQuickXmlParser::Configure(CQuickXmlParser::PTAGDATA TagArray, int ArrayLength)
{
	m_pTagArray = TagArray;
	m_ArrayLength = ArrayLength;
	//calculate tag name length values
	for(int i = 0; i < m_ArrayLength; ++ i)
	{
		TAGDATA &TagData = m_pTagArray[i];

		TagData.Tag.Size = (UINT)wcslen(TagData.Tag.Name);
		for(int j = 0; j < 5; ++j)
		{
			if(TagData.Attribs[j].Name != 0 )
				TagData.Attribs[j].Size = (UINT)wcslen(TagData.Attribs[j].Name);
		}
	}
}

void CQuickXmlParser::SkipSpaces(xptr &p) //skip spaces until a non-space or close tag
{
	while(*p == L' ' && *p != L'>')++p;
}
	
UINT CQuickXmlParser::SkipUntil(xptr &p, wchar_t c) //skip until a given char
{
	UINT chrcnt;
	for(chrcnt = 0; *p != c; ++p, ++chrcnt);
	return chrcnt;
}

UINT CQuickXmlParser::SkipChars(xptr &p) //skip chars until a space
{
	UINT chrcnt;
	for(chrcnt = 0; (*p != L' ' && *p != L'>'); ++p, ++chrcnt);
	return chrcnt;
}

int CQuickXmlParser::MatchTag(xptr &p)
{
	if(m_pTagArray == 0) return -1;
	PTAGDATA Arr = m_pTagArray;

	for(int i = 0; i < m_ArrayLength; ++i)
	{
		if(wcsncmp(p, Arr[i].Tag.Name, Arr[i].Tag.Size ) == 0)
		{
			xptr ptmp = p + Arr[i].Tag.Size;
			if(*ptmp == L' ' || *ptmp == L'>' || (*(ptmp + 1) == L'/' && *(ptmp + 2) == L'>'))//Make sure it's an exact match
				return i;
		}
	}
	return -1;
}
/**
	Member function:	MatchAttrib

	Arguments:			p:	Space shifted to point to the start of the attribute pairs in the form
							[name = "value" type= 'some type' reason ="dunno "]

						tag_index: Tag definition index

						attrib_index: Attribute definition index for the tag defined by tag_index
	
	Notes:				1. Attribute values maybe enclosed in single(') or double(") quotes. However these
						delimiters are not assumed to re-appear again within the value enclosed, even though
						the standard allows for this.

						2. Once a single name-value pair is parsed and added to the attribute store
						defined by attrib_index, p is adjusted to point towards the next following
						pair or	any space preceding it.
**/
void CQuickXmlParser::MatchAttrib(xptr &p, int tag_index, int attrib_index)
{
	PTAGDATA Arr = m_pTagArray;
	UINT u = 0;

	u = Arr[tag_index].Attribs[attrib_index].Size;
	if(wcsncmp(p, Arr[tag_index].Attribs[attrib_index].Name, u) == 0)//Attrib has been found
	{
		p += u;									//skip attrib name
		SkipSpaces(p);							//skip spaces before '='
		//assert(*p == L'=');
		p += 1;									//point to char after '='
		SkipSpaces(p);							//skip spaces after '='
		wchar_t QuoteType = *p;					//QuoteType can be either " or '
		if(QuoteType == L'"' || QuoteType == '\'') ++p;		//skip past single or double quotes
		Arr[tag_index].AttribValues[attrib_index].Name = p;	//set attrib Name address
		u = SkipUntil(p, QuoteType);						//skip all valid chars of attrib value including spaces
		Arr[tag_index].AttribValues[attrib_index].Size  = (*(p - 1) == QuoteType ? --u : u);
		if(*p == QuoteType) ++p;				//skip past single or double quotes which signify attrib end
	}
}

void CQuickXmlParser::ParseAttribs(xptr &p, int index)
{
	PTAGDATA Arr = m_pTagArray;
	if(Arr[index].Attribs[0].Size == 0) return; //no attribs defined

	for( int a = 0; Arr[index].Attribs[a].Size != 0 && *p != L'>'; ++a)
	{
		SkipSpaces(p);
		MatchAttrib(p, index, a);
	}
}


CQuickXmlParser::xptr CQuickXmlParser::Parse(CQuickXmlParser::xptr pxml )
{
	if(m_pTagArray == 0) return 0;
	PTAGDATA Arr = m_pTagArray;
	UINT u = 0;
	xptr pStart = (pxml == 0) ? m_Xml : pxml;
	xptr pBase = m_Xml;
	int TagIndex = -1;
	for(xptr p = pStart; p < pBase + m_XmlLength; ++p)
	{
		if(*p == L'<')
		{
			if(*(p + 1) == L'/')
			{
				--m_Depth;
				continue;
			}
			
			++m_Depth;

			SkipSpaces(++p);
			TagIndex = MatchTag(p);

			if(TagIndex >= 0)								//Found a registered tag
			{
				if(Arr[TagIndex].Value.Name != 0)			//Tag appears again
				{
					--p;									//backtrack to the start tag '<'
					--m_Depth;
					return p;
				}

				p += Arr[TagIndex].Tag.Size;				//skip past the tag name
				ParseAttribs(p, TagIndex);					//Parse attributes
				SkipUntil(p, L'>');
				if(*(p - 1) == L'/' || *(p + 1) == L'<')	//No value tag
				{
					if(*(p - 1) == L'/') --m_Depth;
					Arr[TagIndex].Value.Name = (xptr)-1;	//Indicate Tag was encountered but not set
					Arr[TagIndex].Value.Size = 0;
					Arr[TagIndex].Depth = m_Depth;
					continue;
				}

				p += 1;										//skip past ">"
				SkipSpaces(p);
				Arr[TagIndex].Value.Name = p;
				Arr[TagIndex].Value.Size = SkipUntil(p, L'<' ); //A zero here too means no Tag Value.
				if(*(p + 1) == L'/') 
					Arr[TagIndex].Depth = m_Depth - 1;
				else
					Arr[TagIndex].Depth = m_Depth;
				
				--p;										//Prepare for the next iteration
			}
		}
	}
	return 0;
}

bool CQuickXmlParser::ParseWithCallback(int (*proc)(CQuickXmlParser&))
{
	if(proc == 0) return false;
	else CallbackProc = proc;

	CQuickXmlParser::xptr p = 0;
	bool ret = false;
	while(1)
	{
		p = Parse(p);
		if(p == 0 && ret == false) return ret;
		else ret = true;
		int ret = CallbackProc(*this);
		if(ret == PARSE_BREAK) break;
		else if(ret == PARSE_NEXT) continue;

		if(p == 0) break;
		ClearValues(m_pTagArray);
	}
	return ret;
}


CQuickXmlParser::xptr CQuickXmlParser::Translate(FIELD f)
{
	if(m_pValue)
	{
		delete[] m_pValue;
		m_pValue = 0;
	}
	m_pValue = new wchar_t[f.Size + 1];
	wcsncpy_s(m_pValue, f.Size + 1, f.Name, f.Size);
	m_pValue[f.Size] = L'\0';
	return m_pValue;
}

CQuickXmlParser::xptr CQuickXmlParser::TranslateHex2Dec(FIELD f)
{
	if(m_pValue)
	{
		delete[] m_pValue;
		m_pValue = 0;
	}
	m_pValue = new wchar_t[f.Size];
	wchar_t Arr[3] = {0};
	xptr p = f.Name;
	int j = 0;
	for(UINT i = 0; i < f.Size; ++i)
	{
		if(*p == L'0' && *(p + 1) == L'x')
		{
			Arr[0] = *(p + 2);
			Arr[1] = *(p + 3);
			wchar_t C = (wchar_t)wcstoul(Arr, 0, 16);
			m_pValue[j++] = C;
		}
		++p;
	}
	m_pValue[j] = 0;
	return m_pValue;
}
