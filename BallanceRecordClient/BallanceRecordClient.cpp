#include "BallanceRecordClient.h"
#include <sstream>
#include <cpr/cpr.h>
#include <nlohmann/json.hpp>
#include <filesystem>
#include <fstream>
#include "Services.h"
#include <iomanip>
#include <thread>
#include <BallanceRecordClient/Utils.h>

IMod* BMLEntry(IBML* bml) {
	return new BallanceRecordClient(bml);
}

void BallanceRecordClient::OnPreStartMenu()
{
	if (this->_isFirstDisplay) {
		this->_isFirstDisplay = false;

		if (this->_isOffline)
			m_bml->SendIngameMessage("An error occurred while logging in. Now running in offline mode.");
		else {
			std::stringstream ss;
			ss << "Welcome back, " << this->_services->GetUsername() << '.';
			m_bml->SendIngameMessage(ss.str().c_str());
		}
	}

	BGui::Gui gui;
	auto a = gui.AddTextLabel("aaa", "SdafR dsds", ExecuteBB::GAMEFONT_01, 0.03f, 0.8f, 0.2f, 0.03f);
	a->SetAlignment(ALIGN_LEFT);
	gui.SetVisible(true);
}

void BallanceRecordClient::OnLoad()
{
	// Init config entries
	_services = Services::Create(GetConfig(), this->_props);

	std::string newToken = _services->Login();
	if (newToken.empty())
	{
		return;
	}
	this->_isOffline = false;
	_props[1]->SetString(newToken.c_str());
}

void BallanceRecordClient::OnStartLevel()
{
	if (!this->_isOffline) {
		int currentLevelNumber;
		CKDataArray* array_CurrentLevel = m_bml->GetArrayByName("CurrentLevel");
		array_CurrentLevel->GetElementValue(0, 0, &currentLevelNumber);

		std::stringstream filename;
		filename << "3D Entities/Level/Level_" << std::setfill('0') << std::setw(2) << currentLevelNumber << ".nmo";
		std::ifstream fs("../" + filename.str(), std::ios::in | std::ios::binary);
		if (fs.fail())
		{
			m_bml->SendIngameMessage("Cannot identify map at this moment. This record won't be uploaded.");
			return;
		}
		/*std::thread hashThread([this, &fs]() {
			_mapHash = Utils::Hash(fs);
		});*/
		_mapHash = Utils::Hash(fs);
		/*this->_threads.insert(std::make_pair("hash", [this, &fs]() {
				_mapHash = Utils::Hash(fs);
			})
		);*/
	}
	
}

void BallanceRecordClient::OnProcess() 
{

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
	int score = points + lifes * lifebouns + levelBonus - 1;
	
	if (levelBonus != currentLevelNumber * 100)
	{
		m_bml->SendIngameMessage("The current Ballance instance may be modified.");
		m_bml->SendIngameMessage("This record will be considered invalid.");

		return;
	}
	//this->_threads["hash"].join();

	bool uploadSucceed = false;
	std::thread uploadThread([&]() {
		uploadSucceed = _services->UploadRecord("test from client", score, (1000 - points) / 2.0, this->_mapHash);
	});
	
	std::stringstream istr;
	auto print_clear = [this, &istr]() {
		m_bml->SendIngameMessage(istr.str().c_str()); istr.str("");
	};
	
	istr << "Points: " << points;
	print_clear();
	istr << "Lifes: " << lifes;
	print_clear();
	istr << "Level bouns: " << levelBonus;
	print_clear();
	istr << "Score: " << score;
	print_clear();
	istr << "Calculating map hash...";
	print_clear();
	istr << "MapHash: " << this->_mapHash;
	print_clear();
	
	m_bml->SendIngameMessage("Uploading result...");
	uploadThread.join();
	if (uploadSucceed)
		m_bml->SendIngameMessage("Record uploaded successfully.");
	else
		m_bml->SendIngameMessage("An error occurred while uploading.");
}