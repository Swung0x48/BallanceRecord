#pragma once
#define _SILENCE_CXX17_C_HEADER_DEPRECATION_WARNING
#include <BML/BMLAll.h>
#include <BML/Gui.h>
#include <unordered_map>
#include <future>
#include "Services.h"
#include "Timer.h"

constexpr int BRC_MAJOR_VER = 0;
constexpr int BRC_MINOR_VER = 3;
constexpr int BRC_PATCH_VER = 1;
//constexpr char BRC_VERSION[] = { BRC_MAJOR_VER + '0', '.', BRC_MINOR_VER + '0', '.', BRC_PATCH_VER + '0' };

extern "C" {
	__declspec(dllexport) IMod* BMLEntry(IBML* bml);
}

class BallanceRecordClient: public IMod
{
private:
	char BRC_VERSION[50];

	std::mutex mtx_;
	std::mutex login_mtx_;
	std::mutex upload_mtx_;
	bool is_offline_ = true;
	bool is_cold_boot = true;
	bool need_login_ = true;
	std::condition_variable login_signal_;
	std::condition_variable upload_signal_;
	std::string _mapHash;
	IProperty* props_[2];
	std::unordered_map<std::string, std::future<bool>> future_;
	Services* services_ = nullptr;
	Timer* timer_ = nullptr;
	BGui::Gui* gui_ = nullptr;
public:
	BallanceRecordClient(IBML* bml): IMod(bml) {}
	virtual CKSTRING GetID() override { return "RecordClient"; }
	virtual CKSTRING GetVersion() override {
		sprintf(BRC_VERSION, "%d.%d.%d", BRC_MAJOR_VER, BRC_MINOR_VER, BRC_PATCH_VER);
		return BRC_VERSION; 
	}
	virtual CKSTRING GetName() override { return "Ballance Record Client"; }
	virtual CKSTRING GetAuthor() override { return "Swung0x48"; }
	virtual CKSTRING GetDescription() override { return "A mod to upload records to Hall of Fame."; }
	DECLARE_BML_VERSION;

	virtual void OnPreStartMenu() override;
	virtual void OnLoadObject(CKSTRING filename, BOOL isMap, CKSTRING masterName,
		CK_CLASSID filterClass, BOOL addtoscene, BOOL reuseMeshes, BOOL reuseMaterials,
		BOOL dynamic, XObjectArray* objArray, CKObject* masterObj) override;
	virtual void OnStartLevel() override;
	virtual void OnProcess() override;
	virtual void OnPreEndLevel() override;
	virtual void OnPostEndLevel() override;
	virtual void OnLoad() override;
	virtual void OnCounterActive() override;
	virtual void OnCounterInactive() override;
	virtual void OnPauseLevel() override;
	virtual void OnUnpauseLevel() override;
	virtual void OnUnload() override;
};

