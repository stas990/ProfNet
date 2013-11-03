//Entry point for the DLL application.

#define MAX_LENGTH 256

#define WIN32_LEAN_AND_MEAN // Exclude rarely-used stuff from Windows headers
#include <windows.h>
#include <stdio.h>
#include "assert.h"
#include "ProfilerImplementation.h"

#define ARRAY_SIZE( s ) (sizeof( s ) / sizeof( s[0] ))

// {626409CA-BF10-4F44-A020-B4C99D1628EE}
static const GUID CLSID_MEM_PROFILER = 
{ 0x626409ca, 0xbf10, 0x4f44, { 0xa0, 0x20, 0xb4, 0xc9, 0x9d, 0x16, 0x28, 0xee } };

#define COM_METHOD( TYPE ) TYPE STDMETHODCALLTYPE

HINSTANCE g_hInst;        // instance handle to this piece of code

BOOL WINAPI DllMain( HINSTANCE hInstance, DWORD dwReason, LPVOID lpReserved )
{    
    // save off the instance handle for later use
    switch ( dwReason )
    {
        case DLL_PROCESS_ATTACH:
            DisableThreadLibraryCalls( hInstance );
            g_hInst = hInstance;
            break;        
    } 
        
    return TRUE;
}

class CClassFactory : public IClassFactory
{
    public:
        CClassFactory( ){ m_refCount = 1; }
    
        COM_METHOD( ULONG ) AddRef()
                            {
                                return InterlockedIncrement( &m_refCount );
                            }
        COM_METHOD( ULONG ) Release()
                            {
                                return InterlockedDecrement( &m_refCount );
                            }

        COM_METHOD( HRESULT ) QueryInterface(REFIID riid, void **ppInterface );
        
        // IClassFactory methods
        COM_METHOD( HRESULT ) LockServer( BOOL fLock ) { return S_OK; }
        COM_METHOD( HRESULT ) CreateInstance( IUnknown *pUnkOuter,
                                              REFIID riid,
                                              void **ppInterface );
        
    private:
    
        long m_refCount;                        
};

CClassFactory g_MemProfilerClassFactory;

#if defined(_X86_)
	#pragma comment(linker, "/EXPORT:DllGetClassObject=_DllGetClassObject@12,PRIVATE")
#elif defined(_WIN64)
	#pragma comment(linker, "/EXPORT:DllGetClassObject,PRIVATE")
#endif
STDAPI DllGetClassObject( REFCLSID rclsid, REFIID riid, LPVOID FAR *ppv )
{    
	Logger::Log("DllGetClassObject called");
    HRESULT hr = E_OUTOFMEMORY;

    if ( rclsid == CLSID_MEM_PROFILER )
        hr = g_MemProfilerClassFactory.QueryInterface( riid, ppv );
	
    return hr;   
}

HRESULT CClassFactory::QueryInterface( REFIID riid, void **ppInterface )
{
	Logger::Log("CClassFactory::QueryInterface called");
    if ( riid == IID_IUnknown )
        *ppInterface = static_cast<IUnknown *>( this ); 
    else if ( riid == IID_IClassFactory )
        *ppInterface = static_cast<IClassFactory *>( this );
    else
    { 
        *ppInterface = NULL;                                  
        return E_NOINTERFACE;
    }
    
    reinterpret_cast<IUnknown *>( *ppInterface )->AddRef();
    
    return S_OK;
}

#if defined(_X86_)
	#pragma comment(linker, "/EXPORT:DllCanUnloadNow=_DllCanUnloadNow@0,PRIVATE")
#elif defined(_WIN64)
	#pragma comment(linker, "/EXPORT:DllCanUnloadNow,PRIVATE")
#endif
// Used to determine whether the DLL can be unloaded by COM
STDAPI DllCanUnloadNow(void)
{
    return S_OK;
}

HRESULT CClassFactory::CreateInstance( IUnknown *pUnkOuter, REFIID riid,
                                        void **ppInstance )
{       
	Logger::Log("CClassFactory::CreateInstance called");
	// aggregation is not supported by these objects
    if ( pUnkOuter != NULL )
        return CLASS_E_NOAGGREGATION;

    ProfilerImplementation * profilerCallback = new ProfilerImplementation;

    *ppInstance = (void *)profilerCallback;

    return S_OK;
}
