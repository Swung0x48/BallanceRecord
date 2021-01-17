#include "BallanceRecordClient.h"
#include <sstream>
#include <cpr/cpr.h>
#include <nlohmann/json.hpp>
#include <filesystem>
#include <fstream>
#include "Services.h"
#include <iomanip>

IMod* BMLEntry(IBML* bml) {
	return new BallanceRecordClient(bml);
}

void BallanceRecordClient::OnPreStartMenu()
{
	if (this->_isOffline)
		m_bml->SendIngameMessage("An error occurred while logging in. Now running in offline mode.");
	else {
		std::stringstream ss;
		ss << "Welcome back, " << this->_services->GetUsername() << '.';
		m_bml->SendIngameMessage(ss.str().c_str());
	}
}

void BallanceRecordClient::OnLoad()
{
	// Init config entries
	GetConfig()->SetCategoryComment("Remote", "Remote settings");
	_props[0] = GetConfig()->GetProperty("Remote", "Address");
	_props[0]->SetComment("Remote server address");
	_props[0]->SetDefaultString("REMOTE_ADDR_HERE");
	
	GetConfig()->SetCategoryComment("Account", "Account settings");
	_props[1] = GetConfig()->GetProperty("Account", "APIKey");
	_props[1]->SetComment("This is the token from the website");
	_props[1]->SetDefaultString("YOUR_KEY_HERE");

	const std::string remoteAddress = _props[0]->GetString();
	const std::string refreshToken = _props[1]->GetString();
	_services = new Services(remoteAddress, refreshToken);
	std::string newToken = _services->Login();
	if (newToken.empty())
	{
		return;
		//m_bml->SendIngameMessage("Login failed. Now running in offline mode.");
	}
	//m_bml->SendIngameMessage("Welcome back, .");
	this->_isOffline = false;
	_props[1]->SetString(newToken.c_str());
}

void BallanceRecordClient::OnPreEndLevel()
{
	if (this->_isOffline) return;
	
	CKDataArray* array_Energy = m_bml->GetArrayByName("Energy");
	CKDataArray* array_AllLevel = m_bml->GetArrayByName("AllLevel");
	CKDataArray* array_CurrentLevel = m_bml->GetArrayByName("CurrentLevel");
	
	int points, lifes, lifebouns, currentLevelNumber, levelBonus;
	array_Energy->GetElementValue(0, 0, &points);
	array_Energy->GetElementValue(0, 1, &lifes);
	array_Energy->GetElementValue(0, 5, &lifebouns);
	array_CurrentLevel->GetElementValue(0, 0, &currentLevelNumber);
	array_AllLevel->GetElementValue(currentLevelNumber - 1, 6, &levelBonus);

	if (levelBonus != currentLevelNumber * 100)
	{
		m_bml->SendIngameMessage("The current Ballance instance may be modified.");
		m_bml->SendIngameMessage("This record will be considered invalid.");

		return;
	}

	std::stringstream filename;
	filename << "3D Entities/Level/Level_" << std::setfill('0') << std::setw(2) << currentLevelNumber << ".nmo";
	//std::string abspath = fs::current_path().string() + "\\..\\" + filename.str();
	std::ifstream fs("../" + filename.str(), std::ios::in | std::ios::binary);
	if (fs.fail())
	{
		m_bml->SendIngameMessage("Cannot read level file. This record won't be uploaded.");
		return;
	}
	//auto abs = std::filesystem::current_path().parent_path().append(filename.str());
	//std::ifstream fs(abs, std::ios::binary);
	std::string hash;
	hash = _services->Hash(fs);
	
	std::stringstream istr;
	istr << "MapHash: " << hash;
	m_bml->SendIngameMessage(istr.str().c_str()); istr.str("");
	istr << "Points: " << points;
	m_bml->SendIngameMessage(istr.str().c_str()); istr.str("");
	istr << "Lifes: " << lifes;
	m_bml->SendIngameMessage(istr.str().c_str()); istr.str("");
	istr << "Level bouns: " << levelBonus;
	m_bml->SendIngameMessage(istr.str().c_str()); istr.str("");
	istr << "Score: " << points + lifes * lifebouns + levelBonus;
	m_bml->SendIngameMessage(istr.str().c_str()); istr.str("");

	// cpr::Response r = cpr::Post(cpr::Url{ "https://localhost:5001/api/v1/records" },
	// 					cpr::Body{R"({"name": "string","score": 123,"time": 1234})"},
	// 	cpr::Header{{"Content-Type", "application/json"}, {"Authorization", "bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ1c2VyQGV4YW1wbGUuY29tIiwianRpIjoiMTQxYzAwMmItZTA0NS00YTg2LWE5Y2MtYjgzYzJmOWQzY2YxIiwiZW1haWwiOiJ1c2VyQGV4YW1wbGUuY29tIiwiaWQiOiJhMWQwOWI4YS02NmM5LTQ2YTctODM1MC1hMjUyN2I3NjU4YWYiLCJuYmYiOjE2MDg3NzIyODAsImV4cCI6MTYwODc3MjU4MCwiaWF0IjoxNjA4NzcyMjgwfQ.iVDTgsqfwOsH7-6MTT57i2vHQrUuvrkrgqo8QqQ3FM8"}});
	// istr << r.status_code;
	// m_bml->SendIngameMessage(istr.str().c_str()); istr.str("");
	// istr << r.header["content-type"];
	// m_bml->SendIngameMessage(istr.str().c_str()); istr.str("");
	// istr << r.text; 
	// m_bml->SendIngameMessage(istr.str().c_str()); istr.str("");
	/*std::string raw = R"({"name": "string","score": 123,"time": 1234})";
	auto json = nlohmann::json::parse(raw);
	for (auto& i: json.items())
	{
		istr << i.key();
		m_bml->SendIngameMessage(istr.str().c_str()); istr.str("");
		istr << i.value();
		m_bml->SendIngameMessage(istr.str().c_str()); istr.str("");
	}*/
}