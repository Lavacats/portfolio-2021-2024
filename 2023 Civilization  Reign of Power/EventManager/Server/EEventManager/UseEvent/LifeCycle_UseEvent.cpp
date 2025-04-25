#include "GameAfx.h"
#include "LifeCycle_UseEvent.h"
#include "UseEvent/UserUseEventManager.h"
#include "UseEvent/UseEventManager.h"


void CLifeCycle_UseEvent_End::OnCompleted(bool isUpdate)
{
	CLifeCycle::OnCompleted(isUpdate);

	CUseEventManager::Instance()->SendServerUserUseEventReward(_contentskind);
}
