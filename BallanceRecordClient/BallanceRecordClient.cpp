#include "BallanceRecordClient.h"
#include <vector>
#include <sstream>

IMod* BMLEntry(IBML* bml) {
	return new BallanceRecordClient(bml);
}

void BallanceRecordClient::OnPreEndLevel()
{
	CKDataArray* array_Energy = m_bml->GetArrayByName("Energy");
	CKDataArray* array_AllLevel = m_bml->GetArrayByName("AllLevel");
	CKDataArray* array_CurrentLevel = m_bml->GetArrayByName("CurrentLevel");

	int points, lifes, lifebouns, currentLevelNumber, levelBouns;
	array_Energy->GetElementValue(0, 0, &points);
	array_Energy->GetElementValue(0, 1, &lifes);
	array_Energy->GetElementValue(0, 5, &lifebouns);
	array_CurrentLevel->GetElementValue(0, 0, &currentLevelNumber);
	array_AllLevel->GetElementValue(currentLevelNumber - 1, 6, &levelBouns);

	if (levelBouns != currentLevelNumber * 100)
	{
		m_bml->SendIngameMessage("The current Ballance instance may be modified.");
		m_bml->SendIngameMessage("This record will be considered invaild.");

		return;
	}

	std::ostringstream istr;
	istr << "Points: " << points;
	m_bml->SendIngameMessage(istr.str().c_str()); istr.str("");
	istr << "Lifes: " << lifes;
	m_bml->SendIngameMessage(istr.str().c_str()); istr.str("");
	istr << "Level bouns: " << levelBouns << std::endl;
	m_bml->SendIngameMessage(istr.str().c_str()); istr.str("");
	istr << "Score: " << points + lifes * lifebouns + levelBouns;
	m_bml->SendIngameMessage(istr.str().c_str());
}
