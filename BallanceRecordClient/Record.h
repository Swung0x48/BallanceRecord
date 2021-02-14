#pragma once
#include <string>

struct Record
{
	Record(const std::string& name, const int score, const double time, const std::string& mapHash)
		: name(name), score(score), time(time), mapHash(mapHash)
	{}
	std::string name;
	int score;
	double time;
	std::string mapHash;
};

