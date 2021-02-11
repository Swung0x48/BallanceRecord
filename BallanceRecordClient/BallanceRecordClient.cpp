#include "BallanceRecordClient.h"
#include <sstream>
#include <cpr/cpr.h>
#include <nlohmann/json.hpp>
#include <filesystem>
#include <fstream>
#include "Services.h"
#include <iomanip>
#include <thread>
#include "Utils.h"
#include <future>

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
}

void BallanceRecordClient::OnLoad()
{
	// Init config entries
	_services = Services::Create(GetConfig(), this->_props);

	std::string newToken = _services->Login();
	if (newToken.empty())
	{
		this->_isOffline = true;
		return;
	}
	this->_isOffline = false;
	_props[1]->SetString(newToken.c_str());
}

void BallanceRecordClient::OnCounterActive()
{
	if (!this->_isOffline)
		this->timer_->Start();
}

void BallanceRecordClient::OnCounterInactive()
{
	if (!this->_isOffline)
		this->timer_->Stop();
}

void BallanceRecordClient::OnPauseLevel()
{
	if (!this->_isOffline)
		this->timer_->Stop();
}

void BallanceRecordClient::OnUnpauseLevel()
{
	if (!this->_isOffline)
		this->timer_->Start();
}

void BallanceRecordClient::OnLoadObject(CKSTRING filename, BOOL isMap, CKSTRING masterName,
	CK_CLASSID filterClass, BOOL addtoscene, BOOL reuseMeshes, BOOL reuseMaterials,
	BOOL dynamic, XObjectArray* objArray, CKObject* masterObj) {
	if (this->_isOffline) return;
	if (!isMap) return;

	timer_ = new Timer(m_bml->GetTimeManager());

	auto hashLambda = [&](std::string filename) {
		std::filesystem::path path = std::filesystem::current_path().parent_path().append(filename);

		std::ifstream fs(path, std::ios::in | std::ios::binary);

		if (fs.fail())
			return;

		std::string mapHash = Utils::Hash(fs);
		mtx_.lock();
		_mapHash = mapHash;
		mtx_.unlock();
	};

	thread_["hash"] = std::thread(hashLambda, std::string(filename));
}

void BallanceRecordClient::OnStartLevel()
{
	bool succeed = false;
	if (thread_["hash"].joinable())
		thread_["hash"].join();

	mtx_.lock();
	if (_mapHash.length() == 64)
		succeed = true;
	mtx_.unlock();
	
	if (!succeed)
		m_bml->SendIngameMessage("Cannot identify map at this moment or you are offline. This record won't be uploaded.");
	else
		m_bml->SendIngameMessage(_mapHash.c_str());

	if (!this->_isOffline) {
		timer_->Reset();
		timer_->Stop();
	}
}

void BallanceRecordClient::OnProcess() 
{
	if (m_bml->IsIngame() && !this->_isOffline)
		timer_->Process();
}

void BallanceRecordClient::OnPreEndLevel()
{
	if (this->_isOffline) return;

	timer_->Stop();
	
	int points, lifes, lifebouns, currentLevelNumber, levelBonus;
	m_bml->GetArrayByName("Energy")->GetElementValue(0, 0, &points);
	m_bml->GetArrayByName("Energy")->GetElementValue(0, 1, &lifes);
	m_bml->GetArrayByName("Energy")->GetElementValue(0, 5, &lifebouns);
	m_bml->GetArrayByName("CurrentLevel")->GetElementValue(0, 0, &currentLevelNumber);
	m_bml->GetArrayByName("AllLevel")->GetElementValue(currentLevelNumber - 1, 6, &levelBonus);
	int score = points + lifes * lifebouns + levelBonus - 1;
	
	if (levelBonus != currentLevelNumber * 100)
	{
		m_bml->SendIngameMessage("The current Ballance instance may be modified.");
		m_bml->SendIngameMessage("This record will be considered invalid.");

		return;
	}
	
	char buffer[50];
	sprintf(buffer, "%lf", timer_->GetTime() / 1000.0);
	m_bml->SendIngameMessage(buffer);

	m_bml->SendIngameMessage("Uploading result...");
	
	// thread_["upload"] = std::thread([&]() {
	// 	_services->UploadRecord("Empty.", score, timer_->GetTime() / 1000.0, this->_mapHash);
	// });
	thread_["upload"] = std::thread(&Services::UploadRecord, this->_services, "Empty.", score, timer_->GetTime() / 1000.0, this->_mapHash);
	timer_->Reset();
}

void BallanceRecordClient::OnPostEndLevel()
{
	if (thread_["upload"].joinable()) {
		thread_["upload"].join();
		m_bml->SendIngameMessage("Record uploaded successfully.");
	} else {
		m_bml->SendIngameMessage("An error occurred while uploading.");
	}
}
