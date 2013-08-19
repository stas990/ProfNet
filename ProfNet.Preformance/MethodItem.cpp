#include "MethodItem.h"

MethodItem::MethodItem(double startTime)
{
	_stopwatch = Stopwatch();
	_startTime = startTime;
	Childs = list<MethodItem*>();
}

MethodItem::~MethodItem()
{

}

void MethodItem::StartStopwatch()
{
	_stopwatch.Start();
}

void MethodItem::StopStopwatch()
{
	_stopwatch.Stop();
}

double MethodItem::GetDuraton() const
{
	double elapsed = _stopwatch.ElapsedMilliseconds();
	return elapsed;
}

double MethodItem::GetStartTime()
{
	return _startTime;
}

MethodItem *MethodItem::GetParent()
{
	return Parent;
}

string MethodItem::GetName()
{
	return Name;
}
