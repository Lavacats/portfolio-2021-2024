using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class BattleEngine_Skill_Buff 
{
    /// <summary>
    /// Buff에 대해서
    /// Buff는 실무에서는 전용 테이블과 데이터구조를 할당해 관리했던 복잡한 데이터 구조입니다.
    /// - 버프 우선 순위 칼럼
    /// - 중접 버프를 관리하기 위한 그룹 칼럼
    /// - 해제 가능 버프와 영구 버프 구분 칼럼 등
    /// 다양한 버프 구조를 가지고있지만, 해당 내용을 다 표현하기에는 복잡하기 때문에
    /// 간단하게 공격력 상승 버프와, 그 버프가 유지되는 await을 설명하겠습니다.
    /// </summary>



    public void OnBuff_Skill(BattleData casterData,List<BattleData> TargetArmy)
    {
        // caster가 가진 버프 스킬 가져온다.
        
        foreach(var data in TargetArmy)
        {
            // 위에서 언급했듯이, 이번 테스트 코드에서는 단순히 공격력 상승 버프에 대한 처리만 진행하겠습니다.
            Cal_Buff(casterData, data);
        }

    }
    private async void Cal_Buff(BattleData caster, BattleData targetData)
    {
        targetData.AttackPoint += caster.BuffValue;
        BaseEventManager.Instance.OnEvent(BaseEventManager.EVENT_BASE.BUFF_START, targetData.ArmyIdx);

        await WaitForSecondsAsync(caster.BuffCoolTime);

        targetData.AttackPoint = targetData.AttackPoint_Original;
        BaseEventManager.Instance.OnEvent(BaseEventManager.EVENT_BASE.BUFF_END, targetData.ArmyIdx);

    }
    public async Task WaitForSecondsAsync(float seconds)
    {
        int milliseconds = Mathf.RoundToInt(seconds * 1000f);
        await Task.Delay(milliseconds);
    }
}
