#include "Services.h"
#include <fstream>
#include <filesystem>
#include <nlohmann/json.hpp>

Services::Services(const std::string& remoteAddress, const std::string& refreshToken):
	_remoteAddress(remoteAddress),
	_refreshToken(refreshToken)
{}

Services* Services::Create(IConfig* config, IProperty** props) {
	config->SetCategoryComment("Remote", "Remote settings");
	props[0] = config->GetProperty("Remote", "Address");
	props[0]->SetComment("Remote server address");
	props[0]->SetDefaultString("REMOTE_ADDR_HERE");

	config->SetCategoryComment("Account", "Account settings");
	props[1] = config->GetProperty("Account", "APIKey");
	props[1]->SetComment("This is the token from the website");
	props[1]->SetDefaultString("YOUR_KEY_HERE");

	return new Services(props[0]->GetString(), props[1]->GetString());
}

std::string Services::Login()
{
	nlohmann::json request;
	request["refreshToken"] = this->_refreshToken;

	cpr::Response rawResponse = Put(
		cpr::Url{ this->_remoteAddress + USER_SESSION },
		cpr::Header{{"Content-Type", "application/json"}},
		cpr::Body{ request.dump() });

	if (rawResponse.status_code != 200) return "";
	
	std::unique_lock<std::mutex> lk(mtx_);
	nlohmann::json response = nlohmann::json::parse(rawResponse.text);
	this->_username = response["username"];
	this->_jwt = response["token"];
	this->_refreshToken = response["refreshToken"];
	return this->_refreshToken;
}

std::future<cpr::Response> Services::UploadRecord(std::string remark, int score, double duration, std::string mapHash)
{
	nlohmann::json request;
	request["remark"] = remark;
	request["score"] = score;
	request["duration"] = duration;
	request["mapHash"] = mapHash;
	cpr::Url url = this->_remoteAddress + this->RECORDS;
	cpr::Header header = {
			{ "Content-Type", "application/json" },
			{ "Authorization", "bearer " + this->_jwt }
	};
	cpr::Body body = request.dump();
	auto timeout = cpr::Timeout{ 5000 };
	
	return PostAsync(url, header, body, timeout);

	/*if (rawResponse.status_code == 401)
	{
		Login();
		cpr::Header header = {
			{ "Content-Type", "application/json" },
			{ "Authorization", "bearer " + this->_jwt }
		};
		rawResponse = Post(url, header, body, timeout);
		if (rawResponse.status_code != 201)
			return false;
	}
	
	if (rawResponse.status_code == 201) return true;
	return false;*/
}
