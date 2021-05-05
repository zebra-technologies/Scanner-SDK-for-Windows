

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


 /* File created by MIDL compiler version 8.00.0595 */
/* at Wed Nov 07 10:25:29 2018
 */
/* Compiler settings for _CoreScanner.idl:
    Oicf, W1, Zp8, env=Win64 (32b run), target_arch=AMD64 8.00.0595 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#pragma warning( disable: 4049 )  /* more than 64k source lines */


#ifdef __cplusplus
extern "C"{
#endif 


#include <rpc.h>
#include <rpcndr.h>

#ifdef _MIDL_USE_GUIDDEF_

#ifndef INITGUID
#define INITGUID
#include <guiddef.h>
#undef INITGUID
#else
#include <guiddef.h>
#endif

#define MIDL_DEFINE_GUID(type,name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8) \
        DEFINE_GUID(name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8)

#else // !_MIDL_USE_GUIDDEF_

#ifndef __IID_DEFINED__
#define __IID_DEFINED__

typedef struct _IID
{
    unsigned long x;
    unsigned short s1;
    unsigned short s2;
    unsigned char  c[8];
} IID;

#endif // __IID_DEFINED__

#ifndef CLSID_DEFINED
#define CLSID_DEFINED
typedef IID CLSID;
#endif // CLSID_DEFINED

#define MIDL_DEFINE_GUID(type,name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8) \
        const type name = {l,w1,w2,{b1,b2,b3,b4,b5,b6,b7,b8}}

#endif !_MIDL_USE_GUIDDEF_

MIDL_DEFINE_GUID(IID, IID_ICoreScanner,0x2105896C,0x2B38,0x4031,0xBD,0x0B,0x7A,0x9C,0x4A,0x39,0xFB,0x93);


MIDL_DEFINE_GUID(IID, LIBID_CoreScanner,0xDB07B9FC,0x18B0,0x4B55,0x9A,0x44,0x31,0xD2,0xC2,0xF8,0x78,0x75);


MIDL_DEFINE_GUID(IID, DIID__ICoreScannerEvents,0x981E3D8B,0xC756,0x4195,0xA7,0x02,0xF1,0x98,0x96,0x50,0x31,0xC6);


MIDL_DEFINE_GUID(CLSID, CLSID_CCoreScanner,0x9F8D4F16,0x0F61,0x4A38,0x98,0xB3,0x1F,0x6F,0x80,0xF1,0x1C,0x87);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif



