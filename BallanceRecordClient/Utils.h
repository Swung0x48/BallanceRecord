#pragma once
#include <fstream>

class Utils
{
public:
	static std::string Hash(std::ifstream& fs);
};

