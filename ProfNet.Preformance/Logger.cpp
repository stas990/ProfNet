#include "Logger.h"
#include <cstdlib>
#include <iostream>
#include <fstream>

#include <sstream>
#include <cstring>

#include <windows.h>
#include <tchar.h>
using namespace std;

string _directory;

void Logger::Log(string logString)
{
	fstream filestr;

	string buffer(_directory);
	buffer.append("\\");
	filestr.open (buffer.append("log.txt"), std::fstream::in | std::fstream::out | std::fstream::app);
	
	filestr << logString <<endl;
	filestr.close();
}

void Logger::SetDirectory(std::string directory)
{
	_directory = directory;
}