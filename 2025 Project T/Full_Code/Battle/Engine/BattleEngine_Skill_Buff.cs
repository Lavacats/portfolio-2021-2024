using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class BattleEngine_Skill_Buff 
{
    /// <summary>
    /// Buff�� ���ؼ�
    /// Buff�� �ǹ������� ���� ���̺�� �����ͱ����� �Ҵ��� �����ߴ� ������ ������ �����Դϴ�.
    /// - ���� �켱 ���� Į��
    /// - ���� ������ �����ϱ� ���� �׷� Į��
    /// - ���� ���� ������ ���� ���� ���� Į�� ��
    /// �پ��� ���� ������ ������������, �ش� ������ �� ǥ���ϱ⿡�� �����ϱ� ������
    /// �����ϰ� ���ݷ� ��� ������, �� ������ �����Ǵ� await�� �����ϰڽ��ϴ�.
    /// </summary>



    public void OnBuff_Skill(BattleData casterData,List<BattleData> TargetArmy)
    {
        // caster�� ���� ���� ��ų �����´�.
        
        foreach(var data in TargetArmy)
        {
            // ������ ����ߵ���, �̹� �׽�Ʈ �ڵ忡���� �ܼ��� ���ݷ� ��� ������ ���� ó���� �����ϰڽ��ϴ�.
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
