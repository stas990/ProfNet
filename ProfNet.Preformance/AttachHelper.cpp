#pragma once

#undef _WIN32_WINNT
#undef NTDDI_VERSION
#define _WIN32_WINNT    0x0501
#define NTDDI_VERSION	0x05010000

#include <share.h>

#include "ProfilerCallback.h"
#include "ProfilerInfo.h"
#include "metahost.h"
#include "Logger.h"

HRESULT EnableDebugPrivilege();

BOOL g_fConsoleMode = TRUE;

extern "C" __declspec( dllexport ) void AttachProfiler(int pid, LPCWSTR wszTargetVersion, LPCWSTR wszProfilerPath, BOOL fConsoleMode)
{
	Logger::Log("Start attach");

    HMODULE hModule = NULL;
    LPVOID pvClientData = NULL;
    DWORD cbClientData = 0;
    HRESULT hr;

    ICLRMetaHost * pMetaHost = NULL;
    IEnumUnknown * pEnum = NULL;
    IUnknown * pUnk = NULL;
    ICLRRuntimeInfo * pRuntime = NULL;
    ICLRProfiling * pProfiling = NULL;

    g_fConsoleMode = fConsoleMode;

    DWORD dwProfileeProcessID = (DWORD) pid;

    CLSID clsidProfiler;
	Logger::Log("CLSIDFromString");
    CLSIDFromString(L"{626409CA-BF10-4F44-A020-B4C99D1628EE}", &clsidProfiler);
	Logger::Log("CLSIDFromString finished");
    
    DWORD dwMillisecondsMax = 5000;

    bool fCLRFound = false;

    //---------------------------------------------------------------------------------------
    // GET AND CALL API
    //---------------------------------------------------------------------------------------

	Logger::Log("Trying load mscoree.dll");
    hModule = LoadLibrary("mscoree.dll");
	Logger::Log("mscoree.dll loaded");
    if (hModule == NULL)
    {
        Logger::Log("LoadLibrary mscoree.dll failed.  hr=0x%x.\n");
        goto Cleanup;
    }

    // Note: This is the ONLY C export we need out of mscoree.dll.  This enables us to
    // get started with the metahost interfaces, and it's all COM from that point
    // forward.
	Logger::Log("Trying GetProcAddress");
    CLRCreateInstanceFnPtr pfnCreateInstance = (CLRCreateInstanceFnPtr)GetProcAddress(hModule, "CLRCreateInstance");
	Logger::Log("GetProcAddress finished");
    if (pfnCreateInstance == NULL)
    {
        Logger::Log("GetProcAddress on 'CLRCreateInstance' failed.  hr=0x%x.\n");
        goto Cleanup;
    }

    hr = (*pfnCreateInstance)(CLSID_CLRMetaHost, IID_ICLRMetaHost, (LPVOID *)&pMetaHost);
    if (FAILED(hr))
    {
        Logger::Log("CLRCreateInstance IID_ICLRMetaHost' failed.  hr=0x%x.\n");
        goto Cleanup;
    }

    // Cross-user attach requires the SE_DEBUG_NAME privilege, so attempt to enable it
    // now.  Even if the privilege is not found, the CLRProfiler still continues to attach the target process.
    // We'll just fail later on if we do try to attach to a different-user target process.
	Logger::Log("Trying EnableDebugPrivilege");
    HRESULT hrEnableDebugPrivilege = EnableDebugPrivilege();
    Logger::Log("DebugPrivilege enabled");

    HANDLE hndProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, dwProfileeProcessID);
	Logger::Log("Process opened");
    if (hndProcess == NULL)
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
        //If EnableDebugPrivilege is not successful, let the customer know running CLRProfiler as administrator may solve the problem.
        if (hrEnableDebugPrivilege == E_FAIL) 
        {
            Logger::Log("CLRProfiler can not open the target process %d (error: 0x%x), probably because CLRProfiler could not enable the debug privilege (error: 0x%x).  \n"
                "Please run the CLRProfiler as administrator and try again.");
        }
        else
        {
           Logger:: Log("OpenProcess failed.  hr=0x%x.\n");
        }
        goto Cleanup;
    }

    // One process may have multiple versions of the CLR loaded.  Grab an enumerator to
    // get back all the versions currently loaded.
    hr = pMetaHost->EnumerateLoadedRuntimes(hndProcess, &pEnum);
    if (FAILED(hr))
    {
		Logger::Log("EnumerateLoadedRuntimes' failed.  hr=0x%x.\n");
        goto Cleanup;
    }

    while (pEnum->Next(1, &pUnk, NULL) == S_OK)
    {
        hr = pUnk->QueryInterface(IID_ICLRRuntimeInfo, (LPVOID *) &pRuntime);
        if (FAILED(hr))
            goto LoopCleanup;
       
        WCHAR wszVersion[30];
        DWORD cchVersion = ARRAY_LEN(wszVersion);
        hr = pRuntime->GetVersionString(wszVersion, &cchVersion);
        if (SUCCEEDED(hr) && _wcsnicmp(wszVersion, wszTargetVersion, min(cchVersion, wcslen(wszTargetVersion))) == 0)
        {
            fCLRFound = true;
            hr = pRuntime->GetInterface(CLSID_CLRProfiling, IID_ICLRProfiling, (LPVOID *)&pProfiling);
            if (FAILED(hr))
            {
                Logger::Log("Can not get the ICLRProfiling interface. Error: 0x%x.");
                break;
            }
            // Here it is!  Attach the profiler!
            // The profilee may not have access to the profler dll 
            // Give it a try.
            hr = pProfiling->AttachProfiler(dwProfileeProcessID, dwMillisecondsMax, &clsidProfiler, wszProfilerPath, NULL, NULL);

            if(FAILED(hr))
            {
                if (hr == ERROR_TIMEOUT)//ERROR_TIMEOUT 
                {
                    Logger::Log("CLRProfiler timed out to attach to the process.\nPlease check the event log to find out whether the attach succeeded or failed.");
                }
                else if (hr == COR_E_UNAUTHORIZEDACCESS)//0x80070005
                {
                    Logger::Log("CLRProfiler failed to attach to the process with error code 0x80070005(COR_E_UNAUTHORIZEDACCESS).\n"
                        "This may occur if the target process(%d) does not have access to ProfilerOBJ.dll or the directory in which ProfilerOBJ.dll is located.\n"
                        "Please check event log for more details.");
                }
                else if (hr == CORPROF_E_CONCURRENT_GC_NOT_PROFILABLE)
                {
                    Logger::Log("Profiler initialization failed because the target process is running with concurrent GC enabled. Either\n"
                        "  1) turn off concurrent GC in the application's configuration file before launching the application, or\n" 
                        "  2) simply start the application from CLRProfiler rather than trying to attach CLRProfiler after the application has already started.");
                }
                else
                {
					char buff[100];
					sprintf(buff, "Attach Profiler Failed 0x%x, please check the event log for more details.", hr);
					std::string buffAsStdStr = buff;

                    Logger::Log(buffAsStdStr);
                }
                    
            }

            pProfiling->Release();
            pProfiling = NULL;
            break;
        }

LoopCleanup:
        if (pRuntime != NULL)
        {
            pRuntime->Release();
            pRuntime = NULL;
        }

        if (pUnk != NULL)
        {
            pUnk->Release();
            pUnk = NULL;
        }
    }

    if (fCLRFound == false)
    {
        Logger::Log("No CLR Version %s is loaded into the target process.");
        hr = E_FAIL;
    }



Cleanup:

    if (pRuntime != NULL)
    {
        pRuntime->Release();
        pRuntime = NULL;
    }

    if (pUnk != NULL)
    {
        pUnk->Release();
        pUnk = NULL;
    }

    if (pEnum != NULL)
    {
        pEnum->Release();
        pEnum = NULL;
    }

    if (pMetaHost != NULL)
    {
        pMetaHost->Release();
        pMetaHost = NULL;
    }

    if (hModule != NULL)
    {
        FreeLibrary(hModule);
        hModule = NULL;
    }
}

// Attempts to enable the SE_DEBUG_NAME privilege.  If the privilege is unavailable,
// this intentionally returns S_OK, as same-user attach is still supported without need
// for the SE_DEBUG_NAME privilege
HRESULT EnableDebugPrivilege()
{
    HRESULT hr;
    HANDLE hProcessToken = NULL;
    LPBYTE pbTokenInformation = NULL;

	Logger::Log("Trying open process token");
    if (!OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, &hProcessToken))
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
        Logger::Log("Error returned from OpenProcessToken");
        goto Cleanup;
    }

    DWORD cbTokenInformationOut;
	Logger::Log("Trying get token information");
    if (!GetTokenInformation(
        hProcessToken,
        TokenPrivileges,
        NULL,               // TokenInformation
        0,                  // TokenInformationLength,
        &cbTokenInformationOut))
    {
        DWORD dwLastError = GetLastError();
        if (dwLastError != ERROR_INSUFFICIENT_BUFFER)
        {
            hr = HRESULT_FROM_WIN32(dwLastError);
            Logger::Log("Error returned from GetTokenInformation.  hr = 0x%x.\n");
            goto Cleanup;
        }
    }

    DWORD cbTokenInformation = cbTokenInformationOut;
    pbTokenInformation = new BYTE[cbTokenInformation];
    if (pbTokenInformation == NULL)
    {
        hr = E_OUTOFMEMORY;
        Logger::Log("Unable to allocate %d bytes for token information.\n");
        goto Cleanup;
    }

    if (!GetTokenInformation(
        hProcessToken,
        TokenPrivileges,
        pbTokenInformation,
        cbTokenInformation,
        &cbTokenInformationOut))
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
        Logger::Log("Error returned from GetTokenInformation.  hr = 0x%x.\n");
        goto Cleanup;
    }

    TOKEN_PRIVILEGES * pPrivileges = (TOKEN_PRIVILEGES *) pbTokenInformation;
    BOOL fFoundDebugPrivilege = TRUE;
    LUID_AND_ATTRIBUTES * pLuidAndAttrs = NULL;

    for (DWORD i=0; i < pPrivileges->PrivilegeCount; i++)
    {
		pLuidAndAttrs = &(pPrivileges->Privileges[i]);
		TCHAR szName[256];
		TCHAR szDisplayName[256];
		ULONG cbName;
		ULONG dwLangId;

		cbName = sizeof(szName) / sizeof(szName[0]);

		if (!LookupPrivilegeName(NULL, &pLuidAndAttrs->Luid, 
			szName, &cbName))
		{
			hr = HRESULT_FROM_WIN32(GetLastError());
			Logger::Log("Error returned from LookupPrivilegeName.");
			goto Cleanup;
		}

		std::string first(szName);
		std::string second(SE_DEBUG_NAME);

        if (first == second)
        {
            fFoundDebugPrivilege = TRUE;
            break;
        }
    }

    if (!fFoundDebugPrivilege)
    {
        //Unable to find SeDebugPrivilege; user may not be able to profile higher integrity proceses. 
        //return silently and give it a try.
        //if the attach failed, let the customer know they can run CLRProfiler as administrator and try again. 
        hr = E_FAIL;
        goto Cleanup;
    }

    if ((pLuidAndAttrs->Attributes & SE_PRIVILEGE_ENABLED) != 0)
    {
        // Privilege already enabled.  Nothing to do.
        hr = S_OK;
        // Log(L"SeDebugPrivilege is already enabled.\n");
        goto Cleanup;
    }

    // Log(L"SeDebugPrivilege available but disabled.  Attempting to enable it...\n");
    pLuidAndAttrs->Attributes |= SE_PRIVILEGE_ENABLED;
    if (!AdjustTokenPrivileges(
        hProcessToken,
        FALSE,              // DisableAllPrivileges,
        pPrivileges,
        cbTokenInformationOut,
        NULL,               // PreviousState,
        NULL                // ReturnLength
        ))
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
        Logger::Log("Error returned from AdjustTokenPrivileges.  hr = 0x%x.\n");
        goto Cleanup;
    }
    
    hr = S_OK;

Cleanup:
    if (hProcessToken != NULL)
    {
        CloseHandle(hProcessToken);
        hProcessToken = NULL;
    }

    if (pbTokenInformation != NULL)
    {
        delete [] pbTokenInformation;
        pbTokenInformation = NULL;
    }
	
    return hr;
}