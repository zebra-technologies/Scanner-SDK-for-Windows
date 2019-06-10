

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0555 */
/* at Thu Mar 13 13:28:13 2014
 */
/* Compiler settings for _CoreScanner.idl:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=X86 7.00.0555 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef ___CoreScanner_h__
#define ___CoreScanner_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __ICoreScanner_FWD_DEFINED__
#define __ICoreScanner_FWD_DEFINED__
typedef interface ICoreScanner ICoreScanner;
#endif 	/* __ICoreScanner_FWD_DEFINED__ */


#ifndef ___ICoreScannerEvents_FWD_DEFINED__
#define ___ICoreScannerEvents_FWD_DEFINED__
typedef interface _ICoreScannerEvents _ICoreScannerEvents;
#endif 	/* ___ICoreScannerEvents_FWD_DEFINED__ */


#ifndef __CCoreScanner_FWD_DEFINED__
#define __CCoreScanner_FWD_DEFINED__

#ifdef __cplusplus
typedef class CCoreScanner CCoreScanner;
#else
typedef struct CCoreScanner CCoreScanner;
#endif /* __cplusplus */

#endif 	/* __CCoreScanner_FWD_DEFINED__ */


/* header files for imported files */
#include "prsht.h"
#include "mshtml.h"
#include "mshtmhst.h"
#include "exdisp.h"
#include "objsafe.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __ICoreScanner_INTERFACE_DEFINED__
#define __ICoreScanner_INTERFACE_DEFINED__

/* interface ICoreScanner */
/* [unique][helpstring][dual][uuid][object] */ 


EXTERN_C const IID IID_ICoreScanner;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2105896C-2B38-4031-BD0B-7A9C4A39FB93")
    ICoreScanner : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Open( 
            /* [in] */ LONG appHandle,
            /* [in] */ SAFEARRAY * sfTypes,
            /* [in] */ SHORT lengthOfTypes,
            /* [out] */ LONG *status) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE Close( 
            /* [in] */ LONG appHandle,
            /* [out] */ LONG *status) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE GetScanners( 
            /* [out] */ SHORT *numberOfScanners,
            /* [out][in] */ SAFEARRAY * sfScannerIDList,
            /* [out] */ BSTR *outXML,
            /* [out] */ LONG *status) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE ExecCommand( 
            /* [in] */ LONG opcode,
            /* [in] */ BSTR *inXML,
            /* [out] */ BSTR *outXML,
            /* [out] */ LONG *status) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE ExecCommandAsync( 
            /* [in] */ LONG opcode,
            /* [in] */ BSTR *inXML,
            /* [out] */ LONG *status) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ICoreScannerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICoreScanner * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICoreScanner * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICoreScanner * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ICoreScanner * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ICoreScanner * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ICoreScanner * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ICoreScanner * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Open )( 
            ICoreScanner * This,
            /* [in] */ LONG appHandle,
            /* [in] */ SAFEARRAY * sfTypes,
            /* [in] */ SHORT lengthOfTypes,
            /* [out] */ LONG *status);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *Close )( 
            ICoreScanner * This,
            /* [in] */ LONG appHandle,
            /* [out] */ LONG *status);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *GetScanners )( 
            ICoreScanner * This,
            /* [out] */ SHORT *numberOfScanners,
            /* [out][in] */ SAFEARRAY * sfScannerIDList,
            /* [out] */ BSTR *outXML,
            /* [out] */ LONG *status);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *ExecCommand )( 
            ICoreScanner * This,
            /* [in] */ LONG opcode,
            /* [in] */ BSTR *inXML,
            /* [out] */ BSTR *outXML,
            /* [out] */ LONG *status);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *ExecCommandAsync )( 
            ICoreScanner * This,
            /* [in] */ LONG opcode,
            /* [in] */ BSTR *inXML,
            /* [out] */ LONG *status);
        
        END_INTERFACE
    } ICoreScannerVtbl;

    interface ICoreScanner
    {
        CONST_VTBL struct ICoreScannerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICoreScanner_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICoreScanner_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICoreScanner_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICoreScanner_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ICoreScanner_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ICoreScanner_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ICoreScanner_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ICoreScanner_Open(This,appHandle,sfTypes,lengthOfTypes,status)	\
    ( (This)->lpVtbl -> Open(This,appHandle,sfTypes,lengthOfTypes,status) ) 

#define ICoreScanner_Close(This,appHandle,status)	\
    ( (This)->lpVtbl -> Close(This,appHandle,status) ) 

#define ICoreScanner_GetScanners(This,numberOfScanners,sfScannerIDList,outXML,status)	\
    ( (This)->lpVtbl -> GetScanners(This,numberOfScanners,sfScannerIDList,outXML,status) ) 

#define ICoreScanner_ExecCommand(This,opcode,inXML,outXML,status)	\
    ( (This)->lpVtbl -> ExecCommand(This,opcode,inXML,outXML,status) ) 

#define ICoreScanner_ExecCommandAsync(This,opcode,inXML,status)	\
    ( (This)->lpVtbl -> ExecCommandAsync(This,opcode,inXML,status) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICoreScanner_INTERFACE_DEFINED__ */



#ifndef __CoreScanner_LIBRARY_DEFINED__
#define __CoreScanner_LIBRARY_DEFINED__

/* library CoreScanner */
/* [helpstring][uuid][version] */ 


EXTERN_C const IID LIBID_CoreScanner;

#ifndef ___ICoreScannerEvents_DISPINTERFACE_DEFINED__
#define ___ICoreScannerEvents_DISPINTERFACE_DEFINED__

/* dispinterface _ICoreScannerEvents */
/* [helpstring][uuid] */ 


EXTERN_C const IID DIID__ICoreScannerEvents;

#if defined(__cplusplus) && !defined(CINTERFACE)

    MIDL_INTERFACE("981E3D8B-C756-4195-A702-F198965031C6")
    _ICoreScannerEvents : public IDispatch
    {
    };
    
#else 	/* C style interface */

    typedef struct _ICoreScannerEventsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            _ICoreScannerEvents * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            _ICoreScannerEvents * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            _ICoreScannerEvents * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            _ICoreScannerEvents * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            _ICoreScannerEvents * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            _ICoreScannerEvents * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            _ICoreScannerEvents * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } _ICoreScannerEventsVtbl;

    interface _ICoreScannerEvents
    {
        CONST_VTBL struct _ICoreScannerEventsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define _ICoreScannerEvents_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define _ICoreScannerEvents_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define _ICoreScannerEvents_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define _ICoreScannerEvents_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define _ICoreScannerEvents_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define _ICoreScannerEvents_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define _ICoreScannerEvents_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */


#endif 	/* ___ICoreScannerEvents_DISPINTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_CCoreScanner;

#ifdef __cplusplus

class DECLSPEC_UUID("9F8D4F16-0F61-4A38-98B3-1F6F80F11C87")
CCoreScanner;
#endif
#endif /* __CoreScanner_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  LPSAFEARRAY_UserSize(     unsigned long *, unsigned long            , LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserMarshal(  unsigned long *, unsigned char *, LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserUnmarshal(unsigned long *, unsigned char *, LPSAFEARRAY * ); 
void                      __RPC_USER  LPSAFEARRAY_UserFree(     unsigned long *, LPSAFEARRAY * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


