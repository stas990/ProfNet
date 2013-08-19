#include "ProfilerImplementation.h"
#include <fstream>
#include <iostream>
#include <sstream>
#include "PipeMessanger.h"

#include "JsonSerializer.h"

#define NAME_BUFFER_SIZE 2048

ProfilerImplementation *_profilerCallback = NULL;
// ----  CALLBACK FUNCTIONS ------------------
//
// The functions EnterStub, LeaveStub and TailcallStub are wrappers. The use of 
// of the extended attribute "__declspec( naked )" does not allow a direct call
// to a profiler callback (e.g., ProfilerCallback::Enter( functionID )).
//
// The enter/leave function hooks must necessarily use the extended attribute
// "__declspec( naked )". Please read the corprof.idl for more details. 
//

EXTERN_C void __stdcall EnterStub( FunctionID functionID )
{
	ProfilerImplementation::Enter( functionID );

} // EnterStub

EXTERN_C void __stdcall LeaveStub( FunctionID functionID )
{
	ProfilerImplementation::Leave( functionID );

} // LeaveStub

EXTERN_C void __stdcall TailcallStub( FunctionID functionID )
{
	ProfilerImplementation::Tailcall( functionID );

} // TailcallStub

#ifdef _X86_
void __declspec( naked ) EnterNaked()
{
	__asm
	{
		push eax
			push ecx
			push edx
			push [esp + 16]
		call EnterStub
			pop edx
			pop ecx
			pop eax
			ret 4
	}
} // EnterNaked

void __declspec( naked ) LeaveNaked()
{
	__asm
	{
		push eax
			push ecx
			push edx
			push [esp + 16]
		call LeaveStub
			pop edx
			pop ecx
			pop eax
			ret 4
	}
} // LeaveNaked

void __declspec( naked ) TailcallNaked()
{
	__asm
	{
		push eax
			push ecx
			push edx
			push [esp + 16]
		call TailcallStub
			pop edx
			pop ecx
			pop eax
			ret 4
	}
} // TailcallNaked

void __declspec( naked ) __stdcall EnterNaked2(FunctionID funcId, 
	UINT_PTR clientData, 
	COR_PRF_FRAME_INFO func, 
	COR_PRF_FUNCTION_ARGUMENT_INFO *argumentInfo)
{
	__asm
	{
		push eax
			push ecx
			push edx
			push [esp + 16]
		call EnterStub
			pop edx
			pop ecx
			pop eax
			ret 16
	}
} // EnterNaked

void __declspec( naked ) __stdcall LeaveNaked2(FunctionID funcId, 
	UINT_PTR clientData, 
	COR_PRF_FRAME_INFO func, 
	COR_PRF_FUNCTION_ARGUMENT_RANGE *retvalRange)
{
	__asm
	{
		push eax
			push ecx
			push edx
			push [esp + 16]
		call LeaveStub
			pop edx
			pop ecx
			pop eax
			ret 16
	}
} // LeaveNaked

void __declspec( naked ) __stdcall TailcallNaked2(FunctionID funcId, 
	UINT_PTR clientData, 
	COR_PRF_FRAME_INFO func)
{
	__asm
	{
		push eax
			push ecx
			push edx
			push [esp + 16]
		call TailcallStub
			pop edx
			pop ecx
			pop eax
			ret 12
	}
} // TailcallNaked

void __declspec( naked ) __stdcall EnterNaked3(FunctionIDOrClientID functionIDOrClientID)
{
	__asm
	{
		push eax
			push ecx
			push edx
			push [esp + 16]
		call EnterStub
			pop edx
			pop ecx
			pop eax
			ret 4
	}
} // EnterNaked

void __declspec( naked ) __stdcall LeaveNaked3(FunctionIDOrClientID functionIDOrClientID)
{
	__asm
	{
		push eax
			push ecx
			push edx
			push [esp + 16]
		call LeaveStub
			pop edx
			pop ecx
			pop eax
			ret 4
	}
} // LeaveNaked

void __declspec( naked ) __stdcall TailcallNaked3(FunctionIDOrClientID functionIDOrClientID)
{
	__asm
	{
		push eax
			push ecx
			push edx
			push [esp + 16]
		call TailcallStub
			pop edx
			pop ecx
			pop eax
			ret 4
	}
} // TailcallNaked

#elif defined(_WIN64)
// these are linked in AMD64 assembly (amd64\asmhelpers.asm)
EXTERN_C void EnterNaked2(FunctionID funcId, 
	UINT_PTR clientData, 
	COR_PRF_FRAME_INFO func, 
	COR_PRF_FUNCTION_ARGUMENT_INFO *argumentInfo);
EXTERN_C void LeaveNaked2(FunctionID funcId, 
	UINT_PTR clientData, 
	COR_PRF_FRAME_INFO func, 
	COR_PRF_FUNCTION_ARGUMENT_RANGE *retvalRange);
EXTERN_C void TailcallNaked2(FunctionID funcId, 
	UINT_PTR clientData, 
	COR_PRF_FRAME_INFO func);

EXTERN_C void EnterNaked3(FunctionIDOrClientID functionIDOrClientID);
EXTERN_C void LeaveNaked3(FunctionIDOrClientID functionIDOrClientID);
EXTERN_C void TailcallNaked3(FunctionIDOrClientID functionIDOrClientID);
#endif // _X86_    

BOOL SetPrivilege(
	HANDLE hToken,          // access token handle
	LPCTSTR lpszPrivilege,  // name of privilege to enable/disable
	BOOL bEnablePrivilege   // to enable or disable privilege
	) 
{
	TOKEN_PRIVILEGES tp;
	LUID luid;

	if ( !LookupPrivilegeValue( 
		NULL,            // lookup privilege on local system
		lpszPrivilege,   // privilege to lookup 
		&luid ) )        // receives LUID of privilege
	{
		printf("LookupPrivilegeValue error: %u\n", GetLastError() ); 
		return FALSE; 
	}

	tp.PrivilegeCount = 1;
	tp.Privileges[0].Luid = luid;
	if (bEnablePrivilege)
		tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
	else
		tp.Privileges[0].Attributes = 0;

	// Enable the privilege or disable all privileges.

	if ( !AdjustTokenPrivileges(
		hToken, 
		FALSE, 
		&tp, 
		sizeof(TOKEN_PRIVILEGES), 
		(PTOKEN_PRIVILEGES) NULL, 
		(PDWORD) NULL) )
	{ 
		printf("AdjustTokenPrivileges error: %u\n", GetLastError() ); 
		return FALSE; 
	} 

	if (GetLastError() == ERROR_NOT_ALL_ASSIGNED)

	{
		printf("The token does not have the specified privilege. \n");
		return FALSE;
	} 

	return TRUE;
}


ProfilerImplementation::ProfilerImplementation() : PrfInfo(), m_bTargetV2CLR(true)
{
	_threads = map<UINT_PTR,ThreadItem*>();
	_stopwatch = Stopwatch::StartNew();
	_allMethods = list<MethodItem*>();
	ReadSettings();
}

ProfilerImplementation::~ProfilerImplementation()
{

}


HRESULT ProfilerImplementation::QueryInterface(REFIID riid, void **ppInterface )
{
	Logger::Log("QueryInterface called");
	if(riid == IID_IUnknown)
		*ppInterface = static_cast<ICorProfilerCallback*>(this);
	else if(riid == IID_ICorProfilerCallback)
		*ppInterface = static_cast<ICorProfilerCallback*>(this);
	else if(riid == IID_ICorProfilerCallback2)
		*ppInterface = static_cast<ICorProfilerCallback2*>(this);
	else if(riid == IID_ICorProfilerCallback3)
		*ppInterface = static_cast<ICorProfilerCallback3*>(this);
	else
	{
		Logger::Log("QueryInterface does not interface");
		*ppInterface = NULL;
		return E_NOINTERFACE;
	}

	reinterpret_cast<IUnknown *>( *ppInterface )->AddRef();

	return S_OK;
}

HRESULT ProfilerImplementation::Initialize(IUnknown *pICorProfilerInfoUnk)
{
	Logger::Log("Beginning of the initialization");
	_profilerCallback = this;

	DWORD eventMask = (DWORD)(COR_PRF_MONITOR_THREADS + COR_PRF_MONITOR_ENTERLEAVE);
	/*eventMask = eventMask | (DWORD) COR_PRF_MONITOR_ENTERLEAVE
                            | (DWORD) COR_PRF_MONITOR_EXCEPTIONS
                            | (DWORD) COR_PRF_MONITOR_CODE_TRANSITIONS;*/

	HRESULT hr = CommonInitialization(pICorProfilerInfoUnk, true, eventMask);

	if(FAILED(hr))
	{
		Logger::Log("Something is wrong. Please look at the Windows logs");
		return hr;
	}
	
	Sleep(100); // Give the threads a chance to read any signals that are already set.
	
	PipeMessanger::Send("Profiler Initialized");

	return S_OK;
}

void FillNames(list<MethodItem*> methods, map<UINT_PTR, string> *cache)
{
	list<MethodItem*>::iterator it;

	for(it = methods.begin(); it != methods.end(); it++)
	{
		MethodItem *currentItem = *it;
		
		string name = _profilerCallback->GetMethodName(currentItem->Id);
		cache->insert(pair<UINT_PTR, string>(currentItem->Id, name));

		currentItem->Name = name;

		if(currentItem->Childs.size() > 0)
		{
			FillNames(currentItem->Childs, cache);
		}
	}
}

HRESULT ProfilerImplementation::InitializeForAttach(IUnknown *pICorProfilerInfoUnk, void *pvClientData, UINT cbClientData)
{
    HRESULT hr;

	Logger::Log("Beginning of the InitializeForAttach");

    /*if (cbClientData != sizeof(ProfConfig))
    {
        Failure( "ProfilerCallback::InitializeForAttach: Client data bogus!\n" );
        return E_INVALIDARG;
    }*/

    /*m_bAttachLoaded = TRUE;
    m_bWaitingForTheFirstGC = TRUE;*/

    /*ProfConfig * pProfConfig = (ProfConfig *) pvClientData;
    _ProcessProfConfig(pProfConfig);*/

    /*if ( (m_dwEventMask & (~COR_PRF_ALLOWABLE_AFTER_ATTACH)) != 0 )
    {
		Logger::Log("ProfilerCallback::InitializeForAttach: Unsupported event mode for attach");
        return E_INVALIDARG;
    }*/

	hr = CommonInitialization(pICorProfilerInfoUnk, false, (DWORD)COR_PRF_MONITOR_THREADS);

	if(FAILED(hr))
		Logger::Log( "Allocation for Profiler Test FAILED" );

    if ( SUCCEEDED( hr ) )
    {
        //hr = m_pProfilerInfo->SetEventMask( (DWORD)(COR_PRF_MONITOR_ENTERLEAVE + COR_PRF_MONITOR_THREADS) );

        //if ( SUCCEEDED( hr ) )
        //{
        //    hr = Init(pProfConfig);
        //    if (FAILED(hr))
        //    {
        //        return hr;
        //    }

        //    hr = _InitializeThreadsAndEvents();
        //    if ( FAILED( hr ) )
        //        Failure( "Unable to initialize the threads and handles, No profiling" );
        //    Sleep(100); // Give the threads a chance to read any signals that are already set.
        //}
        //else   
        //{
        //    Failure( "SetEventMask for Profiler Test FAILED" );
        //}
    }

    return hr;
}

HRESULT ProfilerImplementation::CommonInitialization(IUnknown *pICorProfilerInfoUnk, bool canSetHoks, DWORD eventMask)
{
	HRESULT hr;
	
	// define in which mode you will operate
	
	DWORD dwProcessId = GetCurrentProcessId();
	char buff[100];
	sprintf(buff, "ProcessId %d", dwProcessId);
	PipeMessanger::Send(buff);

	hr = pICorProfilerInfoUnk->QueryInterface( IID_ICorProfilerInfo, (void **)&m_pProfilerInfo );
	if (FAILED(hr))
	{
		char buff[100];
		sprintf(buff, "QueryInterface Failed 0x%x, please check the event log for more details.", hr);
		std::string buffAsStdStr = buff;
		Logger::Log(buffAsStdStr);
		return hr;
	}

	hr = pICorProfilerInfoUnk->QueryInterface( IID_ICorProfilerInfo2, (void **)&m_pProfilerInfo2 );
	if (FAILED(hr))
	{
		char buff[100];
		sprintf(buff, "QueryInterface2 Failed 0x%x, please check the event log for more details.", hr);
		std::string buffAsStdStr = buff;
		Logger::Log(buffAsStdStr);
		return hr;
	}

	hr = pICorProfilerInfoUnk->QueryInterface( IID_ICorProfilerInfo3, (void **)&m_pProfilerInfo3 );
	/*if(!m_bTargetV2CLR)
	{
		if (FAILED(hr))
		{
			PipeMessanger::Send("Failed CLRProfiler that targets v4 CLR is waiting for v4 CLR.");
			char buff[100];
			sprintf(buff, "QueryInterface3 Failed 0x%x, please check the event log for more details.", hr);
			std::string buffAsStdStr = buff;
			Logger::Log(buffAsStdStr);
			return hr;
		}
	}*/

	if (m_bTargetV2CLR)
	{
		if (m_pProfilerInfo3 != NULL)
		{
			Logger::Log("v4 CLR is loaded.  CLRProfiler that targets v2 CLR is waiting for v2 CLR.");
			return CORPROF_E_PROFILER_CANCEL_ACTIVATION;
		}
	}
	else 
	{
		if (m_pProfilerInfo3 == NULL)
		{
			PipeMessanger::Send("Failed CLRProfiler that targets v4 CLR is waiting for v4 CLR.");
			Logger::Log("v2 CLR is loaded.  CLRProfiler that targets v4 CLR is waiting for v4 CLR.");
			return E_FAIL;
		}
	}

	hr = m_pProfilerInfo->SetEventMask(eventMask);
	if (FAILED(hr))
	{
		char buff[100];
		sprintf(buff, "SetEventMask for Profiler Failed 0x%x, please check the event log for more details.", hr);
		std::string buffAsStdStr = buff;
		Logger::Log(buffAsStdStr);         
		return hr;
	}

	Logger::Log("Event mask was applied successfully");

	//We can't set hooks if profiler attaching to process
	if(canSetHoks)
	{
		if (m_bTargetV2CLR)
		{
#if defined(_X86_)
			hr = m_pProfilerInfo2->SetEnterLeaveFunctionHooks2( EnterNaked2, LeaveNaked2, TailcallNaked2 );
#elif defined(_WIN64)
			hr = m_pProfilerInfo2->SetEnterLeaveFunctionHooks2( (FunctionEnter2 *)EnterNaked2, (FunctionLeave2 *)LeaveNaked2, (FunctionTailcall2 *)TailcallNaked2 );
#endif
		}
		else
		{
#if defined(_X86_)
			hr = m_pProfilerInfo3->SetEnterLeaveFunctionHooks3(EnterNaked3, LeaveNaked3, TailcallNaked3 );
#elif defined(_WIN64)
			hr = m_pProfilerInfo3->SetEnterLeaveFunctionHooks3((FunctionEnter3 *)EnterNaked3, (FunctionLeave3 *)LeaveNaked3, (FunctionTailcall3 *)TailcallNaked3);
#endif
		}

		if (FAILED(hr))
		{
			Logger::Log( "ICorProfilerInfo::SetEnterLeaveFunctionHooks() FAILED" );
			return hr;
		}

		Logger::Log("Hooks applied successfully");
	}

	return hr;
}

HRESULT ProfilerImplementation::Shutdown()
{
	Logger::Log("Serialization starts");
	
	remove("result.txt");
		
	map<UINT_PTR, string> *cache = new map<UINT_PTR, string>();
	FillNames(_allMethods, cache);
	delete cache;
	
	string buffer(ConfigValue.WorkFolder);
	buffer.append("\\");
	JsonSerializer::Serialize(_allMethods, buffer.append("result.txt"));
	
	/*char bufferChar[1024];
	sprintf_s(bufferChar, 1024, "%s\\%s.tmp", getenv("TMP"), "profiler");*/

	//Logger::Log(bufferChar);

	Logger::Log("Result serialized");

	return S_OK;
}

HRESULT ProfilerImplementation::ThreadCreated(UINT_PTR threadId)
{
	ThreadItem *thread = new ThreadItem(threadId, _stopwatch.ElapsedMilliseconds());
	_threads.insert(pair<UINT_PTR,ThreadItem*>(threadId, thread));
	
	DWORD *DWThreadId = new DWORD();
	HRESULT hr = m_pProfilerInfo->GetThreadInfo(threadId, DWThreadId);
	if(SUCCEEDED(hr))
	{
		thread->ID = DWThreadId;
	}
	else
		Logger::Log("Could not get ThreadId");

	return S_OK;
}

HRESULT ProfilerImplementation::ThreadDestroyed(UINT_PTR threadId)
{
	ThreadItem *thread = _threads[threadId];
	if(thread != NULL)
	{
		thread->DestroydTimeStamp = _stopwatch.ElapsedMilliseconds();
	}
	return S_OK;
}

HRESULT ProfilerImplementation::ThreadAssignedToOSThread(UINT_PTR managedThreadId, ULONG osThreadId)
{
	ThreadItem *thread = _threads[managedThreadId];
	if(thread != NULL)
	{
		thread->OsThreadId = osThreadId;
		thread->OsThreadAssignedTimeStamp = _stopwatch.ElapsedMilliseconds();
	}
	return S_OK;
}

__forceinline void ProfilerImplementation::Enter(FunctionID functionID)
{
	ThreadID currentThreadID = 0;
	HRESULT hr = _profilerCallback->m_pProfilerInfo->GetCurrentThreadID(&currentThreadID);
	
	if(SUCCEEDED(hr))
	{
		ThreadItem *item = _profilerCallback->_threads[currentThreadID];
		if(item == NULL)
		{
			item = new ThreadItem(currentThreadID, 0);
			_profilerCallback->_threads.insert(pair<ThreadID,ThreadItem*>(currentThreadID, item));
		}

		MethodItem *method = new MethodItem(_profilerCallback->_stopwatch.ElapsedMilliseconds());
		method->Id = functionID;
		method->Parent = item->CallStack->Top();
		
		if(method->Parent != NULL)
			method->Parent->Childs.push_back(method);

		method->ThreadId = item->ID;

		method->StartStopwatch();
		item->CallStack->Push(method);
	}
}

__forceinline void ProfilerImplementation::Leave(FunctionID functionID)
{
	ThreadID currentThreadID = 0;
	HRESULT hr = _profilerCallback->m_pProfilerInfo->GetCurrentThreadID(&currentThreadID);

	if(SUCCEEDED(hr))
	{
		ThreadItem *item = _profilerCallback->_threads[currentThreadID];
		if(item != NULL)
		{
			MethodItem *method = item->CallStack->Top();
			if(method != NULL)
			{
				method->StopStopwatch();

				if(method->GetDuraton() > _profilerCallback->ConfigValue.ThresholdValue)
				{
					if(method->Parent == NULL)
						_profilerCallback->_allMethods.push_back(method);				
				}
				else if(method->Parent != NULL)
					method->Parent->Childs.remove(method);

			}
			item->CallStack->Pop();
		}
	}
}

__forceinline void ProfilerImplementation::Tailcall(FunctionID functionID)
{
	ThreadID currentThreadID = 0;
	HRESULT hr = _profilerCallback->m_pProfilerInfo->GetCurrentThreadID(&currentThreadID);

	if(SUCCEEDED(hr))
	{
		ThreadItem *item = _profilerCallback->_threads[currentThreadID];
		if(item != NULL)
		{
			MethodItem *method = item->CallStack->Top();
			if(method != NULL)
			{
				method->StopStopwatch();

				if(method->GetDuraton() > _profilerCallback->ConfigValue.ThresholdValue)
				{
					if(method->Parent == NULL)
						_profilerCallback->_allMethods.push_back(method);
				}
				else if(method->Parent != NULL)
					method->Parent->Childs.remove(method);
			}
			item->CallStack->Pop();
		}
	}
}

string ProfilerImplementation::GetMethodName(FunctionID methodId)
{
	WCHAR szMethodName[NAME_BUFFER_SIZE];
	HRESULT hr = _profilerCallback->GetFullMethodName(methodId, szMethodName, NAME_BUFFER_SIZE);

	if(SUCCEEDED(hr))
	{
		size_t origsize = wcslen(szMethodName) + 1;
		size_t convertedChars = 0;
		char nstring[NAME_BUFFER_SIZE];
		wcstombs_s(&convertedChars, nstring, origsize, szMethodName, _TRUNCATE);

		return nstring;
	}

	return NULL;
}

// creates the fully scoped name of the method in the provided buffer
HRESULT ProfilerImplementation::GetFullMethodName(FunctionID functionID, LPWSTR wszMethod, int cMethod)
{
	IMetaDataImport* pIMetaDataImport = 0;
	HRESULT hr = S_OK;
	mdToken funcToken = 0;
	WCHAR szFunction[NAME_BUFFER_SIZE];
	WCHAR szClass[NAME_BUFFER_SIZE];

	// get the token for the function which we will use to get its name
	hr = m_pProfilerInfo2->GetTokenAndMetaDataFromFunction(functionID, IID_IMetaDataImport, (IUnknown **) &pIMetaDataImport, &funcToken);
		
	if(SUCCEEDED(hr))
	{
		mdTypeDef classTypeDef;
		ULONG cchFunction;
		ULONG cchClass;

		// retrieve the function properties based on the token
		hr = pIMetaDataImport->GetMethodProps(funcToken, &classTypeDef, szFunction, NAME_BUFFER_SIZE, &cchFunction, 0, 0, 0, 0, 0);
		if (SUCCEEDED(hr))
		{
			// get the function name
			hr = pIMetaDataImport->GetTypeDefProps(classTypeDef, szClass, NAME_BUFFER_SIZE, &cchClass, 0, 0);
			if (SUCCEEDED(hr))
			{
				// create the fully qualified name
				_snwprintf_s(wszMethod,cMethod,cMethod,L"%s.%s",szClass,szFunction);
			}
		}
		// release our reference to the metadata
		pIMetaDataImport->Release();
	}

	return hr;
}

void ProfilerImplementation::ReadSettings()
{
	char * val;
	val = getenv("Threshold");
	if (val != NULL) 
		ConfigValue.ThresholdValue = atof(val);
	else
		ConfigValue.ThresholdValue = 1;

	val = getenv("TmpFolder");
	if (val != NULL) 
	{
		std::ostringstream os;
		os << val;
		ConfigValue.WorkFolder = os.str();
		Logger::SetDirectory(ConfigValue.WorkFolder);
	}

	val = getenv("Runtime");
	if (val != NULL) 
	{
		std::ostringstream os;
		os << val;
		ConfigValue.RuntimeVersion = os.str();
		m_bTargetV2CLR = ConfigValue.RuntimeVersion.compare("V2") == 0;
	}
}