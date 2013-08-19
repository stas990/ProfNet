#define WIN32_LEAN_AND_MEAN

#include <windows.h>
#include <stdio.h>
#include "ProfilerCallback.h"
#include <assert.h>

#pragma intrinsic(strcmp,labs,strcpy,_rotl,memcmp,strlen,_rotr,memcpy,_lrotl,_strset,memset,_lrotr,abs,strcat)

ULONG ProfilerCallback::AddRef()
{
	return S_OK;
}
ULONG  ProfilerCallback::Release()
{
	return S_OK;
}
HRESULT ProfilerCallback::QueryInterface(REFIID riid, void **ppInterface )
{
	return S_OK;
}

//====================================================
// ICorProfiler implemented callbacks
//====================================================

HRESULT ProfilerCallback::Initialize(IUnknown * pICorProfilerInfoUnk )
{
    return S_OK;
}

//
// Shutdown
//

HRESULT ProfilerCallback::Shutdown()
{
    return S_OK;
}

//
// CLR action notifications - Runtime*
//

//
// RuntimeSuspendStarted
//

HRESULT ProfilerCallback::RuntimeSuspendStarted(
                                        COR_PRF_SUSPEND_REASON reason)
{
    return S_OK;
}

//
// RuntimeSuspendFinished
//

HRESULT ProfilerCallback::RuntimeSuspendFinished()
{
    return S_OK;
}

//
// RuntimeResumeStarted
//

HRESULT ProfilerCallback::RuntimeResumeStarted()
{
    return S_OK;
}

//
// Garbage Collector notifications
//

//
// MovedReferences
//

HRESULT ProfilerCallback::MovedReferences(ULONG cMovedObjectIDRanges,
                                            UINT_PTR oldObjectIDRangeStart[],
                                            UINT_PTR newObjectIDRangeStart[],
                                            ULONG cObjectIDRangeLength[])
{
	return S_OK;
}

//
// ObjectAllocated
//

HRESULT ProfilerCallback::ObjectAllocated(UINT_PTR objectId, UINT_PTR classId)
{
	return S_OK;
}

//
// ObjectsAllocatedByClass
//

HRESULT ProfilerCallback::ObjectsAllocatedByClass(ULONG cClassCount,
                                                    UINT_PTR classIds[],
                                                    ULONG cObjects[])
{
    return S_OK;
}

//
// ObjectReferences
//

HRESULT ProfilerCallback::ObjectReferences(UINT_PTR objectId, UINT_PTR classId,
                                        ULONG cObjectRefs, UINT_PTR objectRefIds[])
{
	return S_OK; // need to return non-failure code here or we stop getting called for this round of the GC
}

//
// RootReferences
//

HRESULT ProfilerCallback::RootReferences(ULONG cRootRefs, UINT_PTR rootRefIds[])
{
    return S_OK;
}


HRESULT ProfilerCallback::AppDomainCreationStarted(UINT_PTR appDomainId)
{
    return S_OK;
}

HRESULT ProfilerCallback::AppDomainCreationFinished(UINT_PTR appDomainId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT ProfilerCallback::AppDomainShutdownStarted(UINT_PTR appDomainId)
{
    return S_OK;
}

HRESULT ProfilerCallback::AppDomainShutdownFinished(UINT_PTR appDomainId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT ProfilerCallback::AssemblyLoadStarted(UINT_PTR assemblyId)
{   
    return S_OK;
}

HRESULT ProfilerCallback::AssemblyLoadFinished(UINT_PTR assemblyId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT ProfilerCallback::AssemblyUnloadStarted(UINT_PTR assemblyId)
{
    return S_OK;
}

HRESULT ProfilerCallback::AssemblyUnloadFinished(UINT_PTR assemblyId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT ProfilerCallback::ModuleLoadStarted(UINT_PTR moduleId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ModuleLoadFinished(UINT_PTR moduleId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT ProfilerCallback::ModuleUnloadStarted(UINT_PTR moduleId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ModuleUnloadFinished(UINT_PTR moduleId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT ProfilerCallback::ModuleAttachedToAssembly(UINT_PTR moduleId, UINT_PTR assemblyId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ClassLoadStarted(UINT_PTR classId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ClassLoadFinished(UINT_PTR classId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT ProfilerCallback::ClassUnloadStarted(UINT_PTR classId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ClassUnloadFinished(UINT_PTR classId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT ProfilerCallback::FunctionUnloadStarted(UINT_PTR functionId)
{
    return S_OK;
}

HRESULT ProfilerCallback::JITCompilationStarted(UINT_PTR functionId, BOOL fIsSafeToBlock)
{
    return S_OK;
}

HRESULT ProfilerCallback::JITCompilationFinished(UINT_PTR functionId, HRESULT hrStatus, BOOL fIsSafeToBlock)
{
    return S_OK;
}

HRESULT ProfilerCallback::JITCachedFunctionSearchStarted(UINT_PTR functionId, BOOL* pbUseCachedFunction)
{
    return S_OK;
}

HRESULT ProfilerCallback::JITCachedFunctionSearchFinished(UINT_PTR functionId, COR_PRF_JIT_CACHE result)
{
    return S_OK;
}

HRESULT ProfilerCallback::JITFunctionPitched(UINT_PTR functionId)
{
    return S_OK;
}

HRESULT ProfilerCallback::JITInlining(UINT_PTR callerId, UINT_PTR calleeId, BOOL* pfShouldInline)
{
    return S_OK;
}

HRESULT ProfilerCallback::ThreadCreated(UINT_PTR threadId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ThreadDestroyed(UINT_PTR threadId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ThreadAssignedToOSThread(UINT_PTR managedThreadId, ULONG osThreadId)
{
    return S_OK;
}

HRESULT ProfilerCallback::RemotingClientInvocationStarted()
{
    return S_OK;
}

HRESULT ProfilerCallback::RemotingClientSendingMessage(GUID* pCookie, BOOL fIsAsync)
{
    return S_OK;
}

HRESULT ProfilerCallback::RemotingClientReceivingReply(GUID* pCookie, BOOL fIsAsync)
{
    return S_OK;
}

HRESULT ProfilerCallback::RemotingClientInvocationFinished()
{
    return S_OK;
}

HRESULT ProfilerCallback::RemotingServerReceivingMessage(GUID* pCookie, BOOL fIsAsync)
{
    return S_OK;
}

HRESULT ProfilerCallback::RemotingServerInvocationStarted()
{
    return S_OK;
}

HRESULT ProfilerCallback::RemotingServerInvocationReturned()
{
    return S_OK;
}

HRESULT ProfilerCallback::RemotingServerSendingReply(GUID* pCookie, BOOL fIsAsync)
{
    return S_OK;
}

HRESULT ProfilerCallback::UnmanagedToManagedTransition(UINT_PTR functionId, COR_PRF_TRANSITION_REASON reason)
{
    return S_OK;
}

HRESULT ProfilerCallback::ManagedToUnmanagedTransition(UINT_PTR functionId, COR_PRF_TRANSITION_REASON reason)
{
    return S_OK;
}

HRESULT ProfilerCallback::RuntimeSuspendAborted()
{
    return S_OK;
}

HRESULT ProfilerCallback::RuntimeResumeFinished()
{
    return S_OK;
}

HRESULT ProfilerCallback::RuntimeThreadSuspended(UINT_PTR threadId)
{
    return S_OK;
}

HRESULT ProfilerCallback::RuntimeThreadResumed(UINT_PTR threadId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ExceptionThrown(UINT_PTR thrownObjectId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ExceptionSearchFunctionEnter(UINT_PTR functionId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ExceptionSearchFunctionLeave()
{
    return S_OK;
}

HRESULT ProfilerCallback::ExceptionSearchFilterEnter(UINT_PTR functionId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ExceptionSearchFilterLeave()
{
    return S_OK;
}

HRESULT ProfilerCallback::ExceptionSearchCatcherFound(UINT_PTR functionId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ExceptionOSHandlerEnter(UINT_PTR functionId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ExceptionOSHandlerLeave(UINT_PTR functionId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ExceptionUnwindFunctionEnter(UINT_PTR functionId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ExceptionUnwindFunctionLeave()
{
    return S_OK;
}

HRESULT ProfilerCallback::ExceptionUnwindFinallyEnter(UINT_PTR functionId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ExceptionUnwindFinallyLeave()
{
    return S_OK;
}

HRESULT ProfilerCallback::ExceptionCatcherEnter(UINT_PTR functionId,UINT_PTR objectId)
{
    return S_OK;
}

HRESULT ProfilerCallback::ExceptionCatcherLeave()
{
    return S_OK;
}

HRESULT ProfilerCallback::COMClassicVTableCreated(ClassID wrappedClassId, REFGUID implementedIID, VOID* pUnk, ULONG cSlots)
{
    return S_OK;
}

HRESULT ProfilerCallback::COMClassicVTableDestroyed(ClassID wrappedClassId, REFGUID implementedIID, VOID* pUnk)
{
    return S_OK;
}

HRESULT ProfilerCallback::ExceptionCLRCatcherFound(void)
{
    return S_OK;
}

HRESULT ProfilerCallback::ExceptionCLRCatcherExecute(void)
{
    return S_OK;
}
//====================================================
// End ICorProfilerCallback unimplemented callbacks
//====================================================


//====================================================
// Start ICorProfilerCallback2 unimplemented callbacks
//====================================================

HRESULT ProfilerCallback::ThreadNameChanged(ThreadID threadId, ULONG cchName, WCHAR name[])
{
	return S_OK;
}

HRESULT ProfilerCallback::GarbageCollectionStarted(int cGenerations, BOOL generationCollected[], COR_PRF_GC_REASON reason)
{
	return S_OK;
}

HRESULT ProfilerCallback::SurvivingReferences(ULONG cSurvivingObjectIDRanges, ObjectID objectIDRangeStart[], ULONG cObjectIDRangeLength[])
{
	return S_OK;
}

HRESULT ProfilerCallback::GarbageCollectionFinished()
{
	return S_OK;
}

HRESULT ProfilerCallback::FinalizeableObjectQueued(DWORD finalizerFlags, ObjectID objectID)
{
	return S_OK;
}

HRESULT ProfilerCallback::RootReferences2(ULONG cRootRefs, ObjectID rootRefIds[], COR_PRF_GC_ROOT_KIND rootKinds[], COR_PRF_GC_ROOT_FLAGS rootFlags[], UINT_PTR rootIds[])
{
	return S_OK;
}

HRESULT ProfilerCallback::HandleCreated(GCHandleID handleId, ObjectID initialObjectId)
{
	return S_OK;
}

HRESULT ProfilerCallback::HandleDestroyed(GCHandleID handleId)
{
	return S_OK;
}

//====================================================
// End ICorProfilerCallback2 unimplemented callbacks
//====================================================

//====================================================
// Start ICorProfilerCallback3 unimplemented callbacks
//====================================================

HRESULT ProfilerCallback::InitializeForAttach(IUnknown * pCorProfilerInfoUnk, void * pvClientData, UINT cbClientData)
{
	return S_OK;
}

HRESULT ProfilerCallback::ProfilerAttachComplete()
{
	return S_OK;
}

HRESULT ProfilerCallback::ProfilerDetachSucceeded()
{
	return S_OK;
}

//====================================================
// End ICorProfilerCallback3 unimplemented callbacks
//====================================================