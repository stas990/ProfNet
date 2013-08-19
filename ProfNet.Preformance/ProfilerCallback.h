#pragma once

#include <cor.h>
#include <corprof.h>
#include <atlbase.h>

#define ASSERT_HR(x)		_ASSERT(SUCCEEDED(x))

class ProfilerCallback : public ICorProfilerCallback3
{
public:
// IUnknown interface implementation
    STDMETHOD_(ULONG, AddRef)();
    STDMETHOD_(ULONG, Release)();
    STDMETHOD(QueryInterface)( REFIID riid, void **ppInterface );
// End of IUnknown interface implementation

// ICorProfilerCallback interface implementation
    STDMETHOD(Initialize)(IUnknown * pICorProfilerInfoUnk);
    STDMETHOD(Shutdown)();
    STDMETHOD(AppDomainCreationStarted)(UINT_PTR appDomainId);
    STDMETHOD(AppDomainCreationFinished)(UINT_PTR appDomainId, HRESULT hrStatus);
    STDMETHOD(AppDomainShutdownStarted)(UINT_PTR appDomainId);
    STDMETHOD(AppDomainShutdownFinished)(UINT_PTR appDomainId, HRESULT hrStatus);
    STDMETHOD(AssemblyLoadStarted)(UINT_PTR assemblyId);
    STDMETHOD(AssemblyLoadFinished)(UINT_PTR assemblyId, HRESULT hrStatus);
    STDMETHOD(AssemblyUnloadStarted)(UINT_PTR assemblyId);
    STDMETHOD(AssemblyUnloadFinished)(UINT_PTR assemblyId, HRESULT hrStatus);
    STDMETHOD(ModuleLoadStarted)(UINT_PTR moduleId);
    STDMETHOD(ModuleLoadFinished)(UINT_PTR moduleId, HRESULT hrStatus);
    STDMETHOD(ModuleUnloadStarted)(UINT_PTR moduleId);
    STDMETHOD(ModuleUnloadFinished)(UINT_PTR moduleId, HRESULT hrStatus);
    STDMETHOD(ModuleAttachedToAssembly)(UINT_PTR moduleId, UINT_PTR assemblyId);
    STDMETHOD(ClassLoadStarted)(UINT_PTR classId);
    STDMETHOD(ClassLoadFinished)(UINT_PTR classId, HRESULT hrStatus);
    STDMETHOD(ClassUnloadStarted)(UINT_PTR classId);
    STDMETHOD(ClassUnloadFinished)(UINT_PTR classId, HRESULT hrStatus);
    STDMETHOD(FunctionUnloadStarted)(UINT_PTR functionId);
    STDMETHOD(JITCompilationStarted)(UINT_PTR functionId, BOOL fIsSafeToBlock);
    STDMETHOD(JITCompilationFinished)(UINT_PTR functionId, HRESULT hrStatus,
                                        BOOL fIsSafeToBlock);
    STDMETHOD(JITCachedFunctionSearchStarted)(UINT_PTR functionId,
                                                BOOL * pbUseCachedFunction);
    STDMETHOD(JITCachedFunctionSearchFinished)(UINT_PTR functionId,
                                                COR_PRF_JIT_CACHE result);
    STDMETHOD(JITFunctionPitched)(UINT_PTR functionId);
    STDMETHOD(JITInlining)(UINT_PTR callerId, UINT_PTR calleeId,
                            BOOL * pfShouldInline);
    STDMETHOD(ThreadCreated)(UINT_PTR threadId);
    STDMETHOD(ThreadDestroyed)(UINT_PTR threadId);
    STDMETHOD(ThreadAssignedToOSThread)(UINT_PTR managedThreadId,
                                            ULONG osThreadId);
    STDMETHOD(RemotingClientInvocationStarted)();
    STDMETHOD(RemotingClientSendingMessage)(GUID * pCookie, BOOL fIsAsync);
    STDMETHOD(RemotingClientReceivingReply)(GUID * pCookie, BOOL fIsAsync);
    STDMETHOD(RemotingClientInvocationFinished)();
    STDMETHOD(RemotingServerReceivingMessage)(GUID * pCookie, BOOL fIsAsync);
    STDMETHOD(RemotingServerInvocationStarted)();
    STDMETHOD(RemotingServerInvocationReturned)();
    STDMETHOD(RemotingServerSendingReply)(GUID * pCookie, BOOL fIsAsync);
    STDMETHOD(UnmanagedToManagedTransition)(UINT_PTR functionId,
                                            COR_PRF_TRANSITION_REASON reason);
    STDMETHOD(ManagedToUnmanagedTransition)(UINT_PTR functionId,
                                            COR_PRF_TRANSITION_REASON reason);
    STDMETHOD(RuntimeSuspendStarted)(COR_PRF_SUSPEND_REASON suspendReason);
    STDMETHOD(RuntimeSuspendFinished)();
    STDMETHOD(RuntimeSuspendAborted)();
    STDMETHOD(RuntimeResumeStarted)();
    STDMETHOD(RuntimeResumeFinished)();
    STDMETHOD(RuntimeThreadSuspended)(UINT_PTR threadId);
    STDMETHOD(RuntimeThreadResumed)(UINT_PTR threadId);
    STDMETHOD(MovedReferences)(ULONG cMovedObjectIDRanges,
                                UINT_PTR oldObjectIDRangeStart[],
                                UINT_PTR newObjectIDRangeStart[],
                                ULONG cObjectIDRangeLength[]);
    STDMETHOD(ObjectAllocated)(UINT_PTR objectId, UINT_PTR classId);
    STDMETHOD(ObjectsAllocatedByClass)(ULONG cClassCount, UINT_PTR classIds[],
                                        ULONG cObjects[]);
    STDMETHOD(ObjectReferences)(UINT_PTR objectId, UINT_PTR classId,
                                    ULONG cObjectRefs, UINT_PTR objectRefIds[]);
    STDMETHOD(RootReferences)(ULONG cRootRefs, UINT_PTR rootRefIds[]);
    STDMETHOD(ExceptionThrown)(UINT_PTR thrownObjectId);
    STDMETHOD(ExceptionSearchFunctionEnter)(UINT_PTR functionId);
    STDMETHOD(ExceptionSearchFunctionLeave)();
    STDMETHOD(ExceptionSearchFilterEnter)(UINT_PTR functionId);
    STDMETHOD(ExceptionSearchFilterLeave)();
    STDMETHOD(ExceptionSearchCatcherFound)(UINT_PTR functionId);
    STDMETHOD(ExceptionOSHandlerEnter)(UINT_PTR functionId);
    STDMETHOD(ExceptionOSHandlerLeave)(UINT_PTR functionId);
    STDMETHOD(ExceptionUnwindFunctionEnter)(UINT_PTR functionId);
    STDMETHOD(ExceptionUnwindFunctionLeave)();
    STDMETHOD(ExceptionUnwindFinallyEnter)(UINT_PTR functionId);
    STDMETHOD(ExceptionUnwindFinallyLeave)();
    STDMETHOD(ExceptionCatcherEnter)(UINT_PTR functionId, UINT_PTR objectId);
    STDMETHOD(ExceptionCatcherLeave)();
    STDMETHOD(COMClassicVTableCreated)(ClassID wrappedClassId,
                        REFGUID implementedIID, void *pVTable, ULONG cSlots);        
    STDMETHOD(COMClassicVTableDestroyed)(ClassID wrappedClassId,
                        REFGUID implementedIID, void *pVTable);
    STDMETHOD(ExceptionCLRCatcherFound)(void);        
    STDMETHOD(ExceptionCLRCatcherExecute)(void);
// End of ICorProfilerCallback interface implementation

	// ICorProfilerCallback2 interface implementation
	STDMETHOD(ThreadNameChanged)(ThreadID threadId, ULONG cchName, WCHAR name[]);
	STDMETHOD(GarbageCollectionStarted)(int cGenerations, BOOL generationCollected[], COR_PRF_GC_REASON reason);
	STDMETHOD(SurvivingReferences)(ULONG cSurvivingObjectIDRanges, ObjectID objectIDRangeStart[], ULONG cObjectIDRangeLength[]);
	STDMETHOD(GarbageCollectionFinished)();
	STDMETHOD(FinalizeableObjectQueued)(DWORD finalizerFlags, ObjectID objectID);
	STDMETHOD(RootReferences2)(ULONG cRootRefs, ObjectID rootRefIds[], COR_PRF_GC_ROOT_KIND rootKinds[], COR_PRF_GC_ROOT_FLAGS rootFlags[], UINT_PTR rootIds[]);
	STDMETHOD(HandleCreated)(GCHandleID handleId, ObjectID initialObjectId);
	STDMETHOD(HandleDestroyed)(GCHandleID handleId);
	// End of ICorProfilerCallback2 interface implementation

	// ICorProfilerCallback3 interface implementation
	STDMETHOD(InitializeForAttach)(IUnknown * pCorProfilerInfoUnk, void * pvClientData, UINT cbClientData);
	STDMETHOD(ProfilerAttachComplete)();
	STDMETHOD(ProfilerDetachSucceeded)();
	// End of ICorProfilerCallback interface implementation
};