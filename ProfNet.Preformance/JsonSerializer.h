#pragma once

#include "profiler_global.h"
#include "MethodItem.h"

class JsonSerializer
{
public:
	static void Serialize(std::list<MethodItem*> methods, std::string path);

private:
	static std::string SerializeItem(MethodItem* item);
};