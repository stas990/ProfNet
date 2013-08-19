#include "PipeMessanger.h"
#include <cstdlib>
#include <iostream>
#include <fstream>

#include <sstream>
#include <cstring>

#include <windows.h>
#include <tchar.h>
using namespace std;

void PipeMessanger::Send(string message)
{
	HANDLE hPipe = CreateNamedPipe(PipeName, PIPE_ACCESS_DUPLEX, PIPE_TYPE_MESSAGE | PIPE_READMODE_MESSAGE, PIPE_UNLIMITED_INSTANCES, 4096, 4096, 1000, NULL);

	ConnectNamedPipe(hPipe, NULL);

	LPTSTR data = const_cast<LPTSTR>(message.c_str());
	DWORD bytesWritten = 0;
	WriteFile(hPipe, data, _tcslen(data) * sizeof(TCHAR), &bytesWritten, NULL);
	CloseHandle(hPipe);
}