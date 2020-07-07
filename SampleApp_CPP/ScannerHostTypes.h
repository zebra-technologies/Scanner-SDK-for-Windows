/*******************************************************************************************
*
* ©2020 Zebra Technologies Corp. and/or its affiliates.
*
********************************************************************************************/
#pragma once


typedef struct _SCANNER_HOSTTYPES
{
    wchar_t*    m_HostLabel;
    wchar_t*    m_HostValue;

}SCANNER_HOSTTYPES, *PSCANNER_HOSTTYPES;

static SCANNER_HOSTTYPES ScannerHostTypes[] = {

    {L"USB-IBMHID", L"XUA-45001-1"}, //0
    {L"USB-HIDKB", L"XUA-45001-3"}, //1
    {L"USB-OPOS", L"XUA-45001-8"}, //2
    {L"USB-SNAPI with Imaging", L"XUA-45001-9"}, //3
    {L"USB-SNAPI without Imaging", L"XUA-45001-10"}, //4
    {L"USB-CDC Serial Emulation", L"XUA-45001-11"}, //5
    {L"USB-SSI over CDC", L"XUA-45001-14"}, //6
    {L"USB-IBMTT", L"XUA-45001-2"} //7
};