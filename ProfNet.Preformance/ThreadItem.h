#include "MethodsStack.h"

class ThreadItem
{
public:
	ThreadItem(UINT_PTR threadId, double timeStamp);
	~ThreadItem();

	double CreatedTimeStamp;
	double DestroydTimeStamp;
	double OsThreadAssignedTimeStamp;
	MethodsStack *CallStack;
	UINT_PTR ThreadId;
	UINT_PTR OsThreadId;
	DWORD* ID;
};