#include "ProfilerCallback.h"
#include "winnt.h"
#include "assert.h"
#include "Logger.h"
#include "ProfilerInfo.h"
#include "ThreadItem.h"
#include "profiler_global.h"
	
struct Config
{
	double ThresholdValue;
	string WorkFolder;
	string RuntimeVersion;
};

class ProfilerImplementation : public ProfilerCallback, public PrfInfo
{
public:
	ProfilerImplementation();
	~ProfilerImplementation();

	STDMETHOD(QueryInterface)( REFIID riid, void **ppInterface );
	STDMETHOD(Initialize)(IUnknown *pICorProfilerInfoUnk);
	STDMETHOD(InitializeForAttach)(IUnknown * pCorProfilerInfoUnk, void * pvClientData, UINT cbClientData);
	STDMETHOD(Shutdown)();

	//Thread methods
	STDMETHOD(ThreadCreated)(UINT_PTR threadId);
	STDMETHOD(ThreadDestroyed)(UINT_PTR threadId);
	STDMETHOD(ThreadAssignedToOSThread)(UINT_PTR managedThreadId, ULONG osThreadId);

	// callback functions
	static void Enter(FunctionID functionID);
	static void Leave(FunctionID functionID);
	static void Tailcall(FunctionID functionID);

	// gets the full method name given a function ID
	HRESULT GetFullMethodName(FunctionID functionId, LPWSTR wszMethod, int cMethod );

	string GetMethodName(FunctionID methodId);

	Config ConfigValue;

private:
	CComQIPtr<ICorProfilerInfo> m_pICorProfilerInfo;
	CComQIPtr<ICorProfilerInfo2> m_pICorProfilerInfo2;
	CComQIPtr<ICorProfilerInfo3> m_pICorProfilerInfo3;
	bool m_bTargetV2CLR;

	map <UINT_PTR, ThreadItem*> _threads;
	Stopwatch _stopwatch;
	list<MethodItem*> _allMethods;

	void ReadSettings();
	HRESULT CommonInitialization(IUnknown *pICorProfilerInfoUnk, bool canSetHooks, DWORD eventMask);
};