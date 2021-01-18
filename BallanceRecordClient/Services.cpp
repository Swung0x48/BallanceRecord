#include "Services.h"
#include "sha256.h"
#include <fstream>
#include <filesystem>
#include <cpr/cpr.h>
#include <nlohmann/json.hpp>

Services::Services(const std::string& remoteAddress, const std::string& refreshToken):
	_remoteAddress(remoteAddress),
	_refreshToken(refreshToken)
{}

std::string Services::Login()
{
	nlohmann::json request;
	request["refreshToken"] = this->_refreshToken;

	cpr::Response rawResponse = Put(
		cpr::Url{ this->_remoteAddress + USER_SESSION },
		cpr::Header{{"Content-Type", "application/json"}},
		cpr::Body{ request.dump() });

	if (rawResponse.status_code != 200) return "";
	
	nlohmann::json response = nlohmann::json::parse(rawResponse.text);
	this->_username = response["username"];
	this->_jwt = response["token"];
	this->_refreshToken = response["refreshToken"];
	return this->_refreshToken;
}


std::string Services::Hash(std::ifstream& fs)
{
	const int BUF_SIZE = 256;
	SHA256 sha256;
	std::vector<char> buffer(BUF_SIZE, 0);
	while (!fs.eof())
	{
		fs.read(buffer.data(), buffer.size());
		std::streamsize readSize = fs.gcount();
		sha256.add(buffer.data(), readSize);
	}
	return sha256.getHash();
}

bool Services::UploadRecord(std::string name, int score, double time, std::string mapHash)
{
	nlohmann::json request;
	request["name"] = name;
	request["score"] = score;
	request["time"] = time;
	request["mapHash"] = mapHash;
	cpr::Response rawResponse = Post(
		cpr::Url{ this->_remoteAddress + RECORDS },
		cpr::Header{
			{"Content-Type", "application/json"},
			{"Authorization", "bearer " + this->_jwt}
		},
		cpr::Body{ request.dump() });

	if (rawResponse.status_code == 401)
	{
		Login();
		rawResponse = Post(
			cpr::Url{ this->_remoteAddress + RECORDS },
			cpr::Header{
				{"Content-Type", "application/json"},
				{"Authorization", this->_jwt}
			},
			cpr::Body{ request.dump() });
		if (rawResponse.status_code != 201)
			return false;
	}
	
	if (rawResponse.status_code == 201) return true;

	return false;
}
