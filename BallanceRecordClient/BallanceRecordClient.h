#pragma once
#define _SILENCE_CXX17_C_HEADER_DEPRECATION_WARNING
#include <BML/BMLAll.h>
#include <BML/Gui.h>
#include <unordered_map>
#include <thread>
#include "Services.h"

constexpr int BRC_MAJOR_VER = 0;
constexpr int BRC_MINOR_VER = 1;
constexpr int BRC_PATCH_VER = 0;
constexpr char BRC_VERSION[] = { BRC_MAJOR_VER + '0', '.', BRC_MINOR_VER + '0', '.', BRC_PATCH_VER + '0' };

extern "C" {
	__declspec(dllexport) IMod* BMLEntry(IBML* bml);
}

class BallanceRecordClient: public IMod
{
private:
	bool _isOffline = true;
	bool _isFirstDisplay = true;
	std::string _mapHash;
	IProperty* _props[2];
	std::unordered_map<std::string, std::thread> _threads;
	Services* _services = nullptr;
	//BGui::Gui* _gui = nullptr;
public:
	BallanceRecordClient(IBML* bml): IMod(bml) {}
	virtual CKSTRING GetID() override { return "RecordClient"; }
	virtual CKSTRING GetVersion() override { return BRC_VERSION; }
	virtual CKSTRING GetName() override { return "Ballance Record Client"; }
	virtual CKSTRING GetAuthor() override { return "Swung0x48"; }
	virtual CKSTRING GetDescription() override { return "A mod to upload records to Hall of Fame."; }
	DECLARE_BML_VERSION;

	virtual void OnPreStartMenu() override;
	virtual void OnStartLevel() override;
	virtual void OnProcess() override;
	virtual void OnPreEndLevel() override;
	virtual void OnLoad() override;
};

