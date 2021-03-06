#pragma once

#include <BML/BMLAll.h>
#include <string>
#include <future>
#include <mutex>
#include <cpr/cpr.h>

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

	std::mutex mtx_;
public:
	Services(const std::string& remoteAddress, const std::string& refreshToken);
	static Services* Create(IConfig* config, IProperty** props);
	std::string GetUsername() { return this->_username; }
	std::string GetApiKey() { return this->_refreshToken; }
	std::string Login();
	std::future<cpr::Response> UploadRecord(std::string remark, int score, double time, std::string mapHash);
};

