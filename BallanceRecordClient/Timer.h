#pragma once
#include <BML/BMLAll.h>
class Timer
{
private:
	double time_ = 0.0;
	bool isRunning_ = false;
	CKTimeManager* timeManager_ = nullptr;
public:
	Timer(CKTimeManager* timeManager, double initTime = 0.0) : timeManager_(timeManager), time_(initTime) {};
	inline void Process() { // To be called in OnProcess()
		if (isRunning_)
			this->time_ += this->timeManager_->GetLastDeltaTime();
	}
	inline void Start() { this->isRunning_ = true; }
	inline void Stop() { this->isRunning_ = false; }
	inline void Reset() { this->time_ = 0.0; }
	inline double GetTime() { return this->time_; }
};

