#include "profiler_global.h"

#define PipeName _T("\\\\.\\pipe\\Profiler")

class PipeMessanger
{
public:
	static void Send(std::string);
};