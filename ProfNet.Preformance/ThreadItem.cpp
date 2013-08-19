#include "ThreadItem.h"

ThreadItem::ThreadItem(UINT_PTR threadId, double createdTimeStamp)
{
	CallStack = new MethodsStack(524);
	ThreadId = threadId;
	CreatedTimeStamp = createdTimeStamp;
}

ThreadItem::~ThreadItem()
{
	delete CallStack;
}
