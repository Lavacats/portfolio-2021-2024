using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyDataManager : SingleTon<ArmyDataManager>
{
    private Dictionary<string, BattleArmy> Dic_Army = new Dictionary<string, BattleArmy>();
    public BattleEngine Engine = null;
    public Battle_MapDirector MapDirector = null;




    public List<BattleArmy> GetArmyList(BattleArmy curArmy, float range)
    {
        List<BattleArmy> result = new List<BattleArmy>();

        foreach(var army in Dic_Army)
        {
            if(army.Value.GetBattleArmyBattleData().IsPlayer!= curArmy.GetBattleArmyBattleData().IsPlayer)
            {
                if (Vector3.Distance(army.Value.transform.position, curArmy.transform.position)< range)
                {
                    result.Add(army.Value);
                }
            }
        }
        return result;
    }

    public Dictionary<string, BattleArmy> Get_DicArmy() { return Dic_Army; }
    public void ALL_DeselectArmy()
    {
        foreach(var army in Dic_Army)
        {
            army.Value.OnSelectArmy(false);
            //army.Value.GetBattleUnitController().GetHeroUnit().OnSelect(false);
        }
    }
    public void AddBattleArmy(string idx,BattleArmy battleArmy)
    {
        if(battleArmy!=null && idx!=string.Empty)
        {
            Dic_Army[idx] = battleArmy;
        }
    }
    public BattleArmy GetBattleArmy(string idx)
    {
        if(Dic_Army.ContainsKey(idx))
        {
            return Dic_Army[idx];
        }
        else
        {
            return null;
        }
    }
}
