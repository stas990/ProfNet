#include "Stopwatch.h"

Stopwatch::Stopwatch()
{
	m_time1.QuadPart = 0;
	m_time2.QuadPart = 0;
	m_elapsed.QuadPart = 0;
	m_isRunning = false;
	m_frequency = (double)Frequency();
}

void Stopwatch::Start()
{
	if(!m_isRunning)
	{
		DWORD_PTR oldmask = ::SetThreadAffinityMask(::GetCurrentThread(), 0);
		QueryPerformanceCounter(&m_time1);
		::SetThreadAffinityMask(::GetCurrentThread(), oldmask);
		m_isRunning = true;
	}
}
void Stopwatch::Stop()
{
	if(m_isRunning)
	{
		DWORD_PTR oldmask = ::SetThreadAffinityMask(::GetCurrentThread(), 0);
		QueryPerformanceCounter(&m_time2);
		::SetThreadAffinityMask(::GetCurrentThread(), oldmask);
		m_elapsed.QuadPart += (m_time2.QuadPart - m_time1.QuadPart);
		m_isRunning = false;
	}
}
void Stopwatch::Reset()
{
	m_isRunning = false;
	m_time1.QuadPart = 0;
	m_time2.QuadPart = 0;
	m_elapsed.QuadPart = 0;
}
void Stopwatch::Restart()
{
	Reset();
	Start();
}
bool Stopwatch::IsRunning()
{
	return m_isRunning;
}
long long Stopwatch::ElapsedTicks() const
{
	if(m_isRunning)
	{
		LARGE_INTEGER now;
		DWORD_PTR oldmask = ::SetThreadAffinityMask(::GetCurrentThread(), 0);
		QueryPerformanceCounter(&now);
		::SetThreadAffinityMask(::GetCurrentThread(), oldmask);
		return (long long)(m_elapsed.QuadPart + (now.QuadPart - m_time1.QuadPart));
	}
	return (long long)m_elapsed.QuadPart;
}
double Stopwatch::ElapsedSeconds() const
{
	return (double)ElapsedTicks() / m_frequency;
}
double Stopwatch::ElapsedMilliseconds() const
{
	return ElapsedSeconds() * 1000;
}

long long Stopwatch::Frequency()
{
	LARGE_INTEGER frequency;
	DWORD_PTR oldmask = ::SetThreadAffinityMask(::GetCurrentThread(), 0);
	QueryPerformanceFrequency(&frequency);
	::SetThreadAffinityMask(::GetCurrentThread(), oldmask);
	return (long long)frequency.QuadPart;
}
bool Stopwatch::IsHighResolution()
{
	LARGE_INTEGER tester;
	return QueryPerformanceFrequency(&tester);
}
Stopwatch Stopwatch::StartNew()
{
	Stopwatch result = Stopwatch();
	result.Start();
	return result;
}