#pragma once
#define _SILENCE_CXX17_C_HEADER_DEPRECATION_WARNING

#include <string>

class IBML;

class Services
{
private:
	const std::string REMOTE_VER = "v1";
	const std::string USER_SESSION = "/api/" + REMOTE_VER + "/user/session";
	const std::string RECORDS = "/api/" + REMOTE_VER + "/records";
	
	std::string _refreshToken;
	std::string _jwt = "";
	std::string _remoteAddress = "";
	std::string _username = "";
public:
	Services(const std::string& remoteAddress, const std::string& refreshToken);
	static std::string Hash(std::ifstream& fs);
	std::string GetUsername() { return this->_username; }
	std::string Login();
	bool UploadRecord(std::string name, int score, double time, std::string mapHash);
};

