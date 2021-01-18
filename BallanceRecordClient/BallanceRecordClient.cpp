#include "BallanceRecordClient.h"
#include <sstream>
#include <cpr/cpr.h>
#include <nlohmann/json.hpp>
#include <filesystem>
#include <fstream>
#include "Services.h"
#include <iomanip>
#include <thread>

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
	}
	this->_isOffline = false;
	_props[1]->SetString(newToken.c_str());
}

void BallanceRecordClient::OnPreEndLevel()
{
	if (this->_isOffline) return;
	
	int points, lifes, lifebouns, currentLevelNumber, levelBonus;
	CKDataArray* array_Energy = m_bml->GetArrayByName("Energy");
	array_Energy->GetElementValue(0, 0, &points);

	CKDataArray* array_AllLevel = m_bml->GetArrayByName("AllLevel");
	CKDataArray* array_CurrentLevel = m_bml->GetArrayByName("CurrentLevel");
	
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
		m_bml->SendIngameMessage("Cannot identify map at this moment. This record won't be uploaded.");
		return;
	}
	//auto abs = std::filesystem::current_path().parent_path().append(filename.str());
	//std::ifstream fs(abs, std::ios::binary);
	std::string hash;
	std::thread hashThread([&]() {
			hash = _services->Hash(fs);
	});
	
	std::stringstream istr;
	auto print_clear = [&]()
	{
		m_bml->SendIngameMessage(istr.str().c_str()); istr.str("");
	};
	
	istr << "Points: " << points;
	print_clear();
	istr << "Lifes: " << lifes;
	print_clear();
	istr << "Level bouns: " << levelBonus;
	print_clear();
	istr << "Score: " << points + lifes * lifebouns + levelBonus;
	print_clear();
	istr << "Calculating map hash...";
	print_clear();
	hashThread.join();
	istr << "MapHash: " << hash;
	print_clear();
	
	_services->UploadRecord("test from client", points + lifes * lifebouns + levelBonus - 1, (1000 - points) / 2.0, hash);
}