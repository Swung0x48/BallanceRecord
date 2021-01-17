#pragma once
#define _SILENCE_CXX17_C_HEADER_DEPRECATION_WARNING

#include <string>

class IBML;

class Services
{
private:
	std::string _refreshToken;
	std::string _jwt = "";
	std::string _remoteAddress = "";
	std::string _username = "";
	const std::string REMOTE_VER = "v1";
	const std::string USER_SESSION = "/api/" + REMOTE_VER + "/user/session";
public:
	std::string Login();
	Services(const std::string& remoteAddress, const std::string& refreshToken);
	std::string Hash(std::ifstream& fs);
	std::string GetUsername() { return this->_username; }
};

