#pragma once

#include <windows.h>
#include "Stopwatch.h"
#include "profiler_global.h"

using namespace std;

class MethodItem
{
public:
	MethodItem(double startTime);
	~MethodItem();

	void StartStopwatch();
	void StopStopwatch();

	//export
	double GetDuraton() const;
	double GetStartTime();
	MethodItem *GetParent();
	string GetName();

	string Name;
	UINT_PTR Id;
	MethodItem *Parent;
	list<MethodItem*> Childs;
	string Uuid;
	DWORD *ThreadId;

private:
	Stopwatch _stopwatch;
	double _startTime;
};
