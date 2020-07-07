
#pragma once
/***
Class		: CQuickXmlParser

Synopsis	: A non-validating XML parser for fast retrieval of Tag/Attribute values

Goals		: No external dependencies
			  No DOM overhead (eg: Creating/Destroying DOM objects in COM in every method call)
			  Simplicity - After all when the XML format is known & consistent, it's simple text extraction.
			  No extra memory allocations during parsing. You only allocate for what you need when you need.
			  Uses a single parse to map memory locations

Limitations	: Has no fail-over for incorrectly formatted XML
			  This is tested for CoreScanner specific XML. (Although in theory it should handle any XML)

Author		: VRQW74

Copyrights	: ©2020 Zebra Technologies Corp. and/or its affiliates.

Revisions	: 02 July 2010 - First design & implementation
			  05 July 2010 - Added support depth-wise parsing to handle XML records
			  14 July 2010 - Added Depth indicator to convey Parent-Child Tag relationships
			  18 Aug  2010 - Added ParseWithCallback().	
***/

#include "windows.h"

class CQuickXmlParser
{
public:

	typedef wchar_t* xptr;
	typedef struct _FIELD			//Basic field representation
	{
		xptr Name;
		UINT Size;

	} FIELD;

	typedef struct _TAGDATA			//Basic tag representation
	{
		FIELD Tag;					//In:  Tagname and size
		FIELD Attribs[5];			//In:  Tag Attribute and size array
		FIELD AttribValues[5];		//Out: Tag Attrib values returned (assumption: only 5 max attribs)
		FIELD Value;				//Out: Tag value returned
		int	  Depth;

	} TAGDATA, *PTAGDATA;


	enum { PARSE_OK, PARSE_BREAK, PARSE_NEXT};

public:

	CQuickXmlParser(void);
	CQuickXmlParser(BSTR xml);
	~CQuickXmlParser(void);

	void Attach(BSTR xml);
	BSTR LoadXmlFile(LPTSTR XmlFilePath);
	PTAGDATA GetTags();
	void Configure(PTAGDATA TagArray, int ArrayLength);
	xptr Parse(xptr pxml = 0 );
	bool ParseWithCallback(int (*proc)(CQuickXmlParser&));
	void ClearValues(PTAGDATA Array);

	xptr Translate(FIELD f);
	xptr TranslateHex2Dec(FIELD f);


private:
	void Clear();
	void SkipSpaces(xptr &p); 
	UINT SkipUntil(xptr &p, wchar_t c); 
	UINT SkipChars(xptr &p); 
	int MatchTag(xptr &p);
	void MatchAttrib(xptr &p, int tag_index, int attrib_index);
	void ParseAttribs(xptr &p, int index);

private:

	BSTR m_Xml;
	xptr m_pValue;
	UINT m_XmlLength;

	PTAGDATA m_pTagArray;
	int m_ArrayLength;
	int m_Depth;

	int (*CallbackProc)(CQuickXmlParser& ref);
};

