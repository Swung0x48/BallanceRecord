#pragma once
#define _SILENCE_CXX17_C_HEADER_DEPRECATION_WARNING
#include <BML/BMLAll.h>

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
	bool _isOffline = true;
	IProperty* _props[2];
	Services* _services = nullptr;
public:
	BallanceRecordClient(IBML* bml): IMod(bml) {}
	virtual CKSTRING GetID() override { return "RecordClient"; }
	virtual CKSTRING GetVersion() override { return BRC_VERSION; }
	virtual CKSTRING GetName() override { return "Ballance Record Client"; }
	virtual CKSTRING GetAuthor() override { return "Swung0x48"; }
	virtual CKSTRING GetDescription() override { return "A mod to upload records to Hall of Fame."; }
	DECLARE_BML_VERSION

	virtual void OnPreStartMenu() override;
	virtual void OnPreEndLevel() override;
	virtual void OnLoad() override;
};

