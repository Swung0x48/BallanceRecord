#include "BallanceRecordClient.h"
#include <sstream>
#include <cpr/cpr.h>
#include <nlohmann/json.hpp>
#include <filesystem>
#include <fstream>
#include "Services.h"
#include "Utils.h"
#include <future>
#include <chrono>

IMod* BMLEntry(IBML* bml) {
	return new BallanceRecordClient(bml);
}

void BallanceRecordClient::OnPreStartMenu()
{
	GetLogger()->Info("Waiting login thread to finish...");
	mtx_.lock(); mtx_.unlock();
	GetLogger()->Info("...OK");

	GetLogger()->Info("Checking if a valid token is present...");

	if (this->is_cold_boot) {
		this->is_cold_boot = false;

		if (this->is_offline_) {
			GetLogger()->Warn("...No valid token is present");
			m_bml->SendIngameMessage("An error occurred while logging in. Now running in offline mode.");
		}
		else {
			GetLogger()->Info("...OK");
			std::stringstream ss;
			ss << "Welcome back, " << this->services_->GetUsername() << '.';
			m_bml->SendIngameMessage(ss.str().c_str());
		}
	}
}

void BallanceRecordClient::OnLoad()
{
	GetLogger()->Info("Initializing BallanceRecordClient...");
	// Init config entries
	GetLogger()->Info("Reading config...");
	services_ = Services::Create(GetConfig(), this->props_);

	std::thread login_thread = std::thread([&]() {
		while (true) {
			std::unique_lock<std::mutex> lock(login_mtx_);

			GetLogger()->Info("Attempting to login...");
			std::string new_token = services_->Login();

			if (!new_token.empty()) {
				this->is_offline_ = false;
				props_[1]->SetString(new_token.c_str());
				GetLogger()->Info("Logged in successfully.");
			}
			else {
				this->is_offline_ = true;
				GetLogger()->Warn("Login failed");
			}
			need_login_ = false;
			lock.unlock();

			while (!need_login_) {
				login_signal_.wait(lock);
			}
		}
	});
	login_thread.detach();
}

void BallanceRecordClient::OnCounterActive()
{
	if (!this->is_offline_)
		this->timer_->Start();
}

void BallanceRecordClient::OnCounterInactive()
{
	if (!this->is_offline_)
		this->timer_->Stop();
}

void BallanceRecordClient::OnPauseLevel()
{
	if (!this->is_offline_)
		this->timer_->Stop();
}

void BallanceRecordClient::OnUnpauseLevel()
{
	if (!this->is_offline_)
		this->timer_->Start();
}

void BallanceRecordClient::OnLoadObject(CKSTRING filename, BOOL isMap, CKSTRING masterName,
	CK_CLASSID filterClass, BOOL addtoscene, BOOL reuseMeshes, BOOL reuseMaterials,
	BOOL dynamic, XObjectArray* objArray, CKObject* masterObj) {
	if (this->is_offline_) return;
	if (!isMap) return;

	timer_ = new Timer(m_bml->GetTimeManager());

	auto hashLambda = [&](std::string filename) {
		std::filesystem::path path = std::filesystem::current_path().parent_path().append(filename[0] == '.' ? filename.substr(3, filename.length()) : filename);

		std::ifstream fs(path, std::ios::in | std::ios::binary);

		if (fs.fail())
			return false;

		std::string mapHash = Utils::Hash(fs);
		std::unique_lock<std::mutex> lock(mtx_);
		_mapHash = mapHash;
		return true;
	};

	future_["hash"] = std::async(std::launch::async, hashLambda, std::string(filename));
}

void BallanceRecordClient::OnStartLevel()
{
	bool succeed = false;
	if (future_["hash"].valid())
		succeed = future_["hash"].get();
	else {
		if (_mapHash.length() == 64)
			succeed = true;
	}
	
	if (!succeed)
		m_bml->SendIngameMessage("Cannot identify map at this moment or you are offline. This record won't be uploaded.");
	else
		m_bml->SendIngameMessage(_mapHash.c_str());

	if (!this->is_offline_) { // Timer is not available in offline mode.
		timer_->Reset();
		timer_->Stop();
	}
}

void BallanceRecordClient::OnProcess() 
{
	if (m_bml->IsIngame() && !this->is_offline_)
		timer_->Process();

}

void BallanceRecordClient::OnPreEndLevel()
{
	if (this->is_offline_) return;

	timer_->Stop();
	
	int points, lifes, lifebouns, currentLevelNumber, levelBonus;
	m_bml->GetArrayByName("Energy")->GetElementValue(0, 0, &points);
	m_bml->GetArrayByName("Energy")->GetElementValue(0, 1, &lifes);
	m_bml->GetArrayByName("Energy")->GetElementValue(0, 5, &lifebouns);
	m_bml->GetArrayByName("CurrentLevel")->GetElementValue(0, 0, &currentLevelNumber);
	m_bml->GetArrayByName("AllLevel")->GetElementValue(currentLevelNumber - 1, 6, &levelBonus);
	int score = points + lifes * lifebouns + levelBonus;
	
	if (levelBonus != currentLevelNumber * 100)
	{
		m_bml->SendIngameMessage("The current Ballance instance may be modified.");
		m_bml->SendIngameMessage("This record will be considered invalid.");

		return;
	}
	
	char buffer[50];
	auto elapsed_time = timer_->GetTime() / 1000.0;
	sprintf(buffer, "%lf", elapsed_time);
	m_bml->SendIngameMessage(buffer);

	m_bml->SendIngameMessage("Uploading result...");
	
	// thread_["upload"] = std::thread([&]() {
	// 	_services->UploadRecord("Empty.", score, timer_->GetTime() / 1000.0, this->_mapHash);
	// });

	future_["upload"] = std::async(std::launch::async,
		[&] { 
			std::unique_lock<std::mutex> lock(mtx_);
			auto response = this->services_->UploadRecord("Empty.", score, elapsed_time, this->_mapHash);
			if (response.get().status_code == 201) {
				lock.unlock();
				upload_signal_.notify_all();
				return true;
			}
			else {
				need_login_ = true;
				login_signal_.notify_all(); // wake up login thread.
				std::unique_lock<std::mutex> login_lock(login_mtx_); // wait for login thread to complete.
				auto response = this->services_->UploadRecord("Empty.", score, elapsed_time, this->_mapHash); // retry upload after a re-login.
				if (response.get().status_code == 201) {
					lock.unlock();
					login_lock.unlock();
					upload_signal_.notify_all();
					return true;
				}
			}
			return false;
	});

	auto check_thread = std::thread([&]() {
		while (true) {
			std::unique_lock<std::mutex> lock(mtx_);
			while (is_offline_ || !future_["upload"].valid() || future_["upload"].wait_for(std::chrono::milliseconds(0)) != std::future_status::ready) {
				upload_signal_.wait(lock);
			}

			if (future_["upload"].get())
				m_bml->SendIngameMessage("Record uploaded successfully.");
			else
				m_bml->SendIngameMessage("An error occurred while uploading.");
		}
	});
	check_thread.detach();

	timer_->Reset();
}

void BallanceRecordClient::OnPostEndLevel()
{
	
}

void BallanceRecordClient::OnUnload()
{
	this->props_[1]->SetString(this->services_->GetApiKey().c_str());
}
