#include <windows.h>

class Stopwatch
{
private:
	LARGE_INTEGER m_time1;      // Start of interval.
	LARGE_INTEGER m_time2;      // End of interval.
	LARGE_INTEGER m_elapsed;    // Accumulates intervals until Reset() or Restart() is called.
	double m_frequency;         // The frequency of the CPU, as reported by QueryPerformanceFrequency().
	bool m_isRunning;           // Whether or not the Stopwatch is running.

public:
	Stopwatch();
	~Stopwatch(){}

	// Controls
	void Start();      // Starts timing an interval, unless already running.
	void Stop();       // Finishes timing an interval and adds its duration to the total elapsed time (m_elapsed).
	void Reset();      // Clears all of the Stopwatch's variables; effectively re-initialising it.
	void Restart();    // Resets and then starts the timer.

	// Information
	bool IsRunning();
	long long ElapsedTicks() const;       // Note: the Elapsed*() functions will return the elapsed time of an interval
	double ElapsedSeconds() const;        // even if a call to Stop() has not been made. This saves a bit of code when
	double ElapsedMilliseconds() const;   // reporting the timings of each iteration of a loop, for example.

	// Static Methods
	static long long Frequency();     // Returns the frequency of your CPU.
	static bool IsHighResolution();   // Does your hardware support high resolution timing?
	static Stopwatch StartNew();
};