#include "GameAfx.h"
#include "LifeCycle_SeasonEvent.h"
#include "Contents/SeasonEvent/SeasonEventManager.h"
#include "Season/SeasonManager.h"

void CLifeCycle_SeasonEventPeriod_First::OnCompleted(bool isUpdate)
{
	CLifeCycle::OnCompleted(isUpdate);

	CSeasonEventManager::Instance()->SendServerUserSeasonEventReward(CSeasonManager::Instance()->GetSeasonNo(),CSeasonEventManager::SeasonEventPeriodState::Period_First);


}

void CLifeCycle_SeasonEventPeriod_Second::OnCompleted(bool isUpdate)
{
	CLifeCycle::OnCompleted(isUpdate);

	CSeasonEventManager::Instance()->SendServerUserSeasonEventReward(CSeasonManager::Instance()->GetSeasonNo(), CSeasonEventManager::SeasonEventPeriodState::Period_second);

}
