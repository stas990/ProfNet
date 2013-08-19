#include "JsonSerializer.h"
#include <fstream>
#include <iostream>
#include <sstream>

void JsonSerializer::Serialize(std::list<MethodItem*> methods, std::string path)
{
	std::ostringstream os;
	os << "[";

	list<MethodItem*>::iterator iter;

	string separator = "";
	bool first = true;

	for(iter = methods.begin(); iter != methods.end(); iter++)
	{
		MethodItem *item = *iter;

		os << separator << SerializeItem(item);

		if(first)
		{
			separator = ",";
			first = false;
		}
	}

	os << "]";

	fstream filestr;

	filestr.open (path, std::fstream::in | std::fstream::out | std::fstream::app);
	
	filestr << os.str();
	filestr.close();
}

std::string JsonSerializer::SerializeItem(MethodItem* item)
{
	std::ostringstream os;
	os << "{";
	os << "\"Name\":" << '"' << item->Name << "\",";
	os << "\"Duration\":" << '"' << item->GetDuraton() << "\",";
	os << "\"StartTime\":" << '"' << item->GetStartTime() << "\",";
	os << "\"MethodId\":" << '"' << item->Id << "\",";
	os << "\"ThreadId\":" << '"' << item->ThreadId << '"';

	if(item->Childs.size() > 0)
	{
		os << ",\"Childs\":[";

		list<MethodItem*>::iterator iter;

		string separator = "";
		bool first = true;

		for(iter = item->Childs.begin(); iter != item->Childs.end(); iter++)
		{
			MethodItem *child = *iter;

			os << separator << SerializeItem(child);

			if(first)
			{
				separator = ",";
				first = false;
			}
		}

		os << "]";
	}

	os << "}";

	return os.str();
}
