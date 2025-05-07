using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class BattleUnitController : MonoBehaviour
{
    [SerializeField] BattleBaseUnit Prefab_HeroUnit;
    [SerializeField] BattleBaseUnit Prefab_NormalUnit;

    private BattleArmyCell ArmyCell;
    private BattleBaseUnit HeroUnit;
    private List<BattleBaseUnit> BattleUnitList = new List<BattleBaseUnit>();
    private string ArmyIdx = string.Empty;
    void Start()
    {
        
    }
    void Update()
    {

    }
    public void Update_UnitState(E_ARMY_STATE state,E_OrderType order)
    {
        switch (state)
        {
            case E_ARMY_STATE.Move:
                {
                    HeroUnit.Set_State(E_UNIT_STATE.Move, order);
                    foreach (var unit in BattleUnitList) { unit.Set_State(E_UNIT_STATE.Move, order); }
                }
                break;
            case E_ARMY_STATE.Attack:
                {
                    HeroUnit.UnitState = E_UNIT_STATE.Attack;
                    foreach (var unit in BattleUnitList) { unit.UnitState = E_UNIT_STATE.Attack; }
                }
                break;
            case E_ARMY_STATE.SKILL:
                {
                    HeroUnit.UnitState = E_UNIT_STATE.SKILL;
                    HeroUnit.Set_State(E_UNIT_STATE.SKILL, E_OrderType.User);
                    foreach (var unit in BattleUnitList) { unit.Set_State(E_UNIT_STATE.Idle, E_OrderType.User); }
                }
                break;

        }

    }
    public void Update_TargetUnitData(BattleUnitController TargetUnitController)
    {
        HeroUnit.GetUnit_Status().TargetUnit = TargetUnitController.GetHeroUnit();
        for(int i=0;i<BattleUnitList.Count; i++)
        {
            BattleUnitList[i].GetUnit_Status().TargetUnit = TargetUnitController.GetNormalUnit()[i];
            
        }
    }
    public void Init(BattleArmyCell armyCell, BattleData stat)
    {
        ArmyIdx = stat.ArmyIdx;
        if (stat.IsPlayer)
        {
            Quaternion spawnRotation = Quaternion.Euler(0, 90, 0);
            HeroUnit = Init_UnitData(Prefab_HeroUnit, armyCell, stat, new Vector2Int(5, 3), spawnRotation);

            BattleUnitList.Add(Init_UnitData(Prefab_NormalUnit, armyCell,stat,new Vector2Int(4, 3),spawnRotation));
            BattleUnitList.Add(Init_UnitData(Prefab_NormalUnit, armyCell,stat,new Vector2Int(3, 4),spawnRotation));
            BattleUnitList.Add(Init_UnitData(Prefab_NormalUnit, armyCell,stat,new Vector2Int(3, 2),spawnRotation));
            BattleUnitList.Add(Init_UnitData(Prefab_NormalUnit, armyCell,stat,new Vector2Int(2, 1),spawnRotation));
            BattleUnitList.Add(Init_UnitData(Prefab_NormalUnit, armyCell, stat, new Vector2Int(2, 5), spawnRotation));
        }
        else
        {
            Quaternion spawnRotation = Quaternion.Euler(0, -90, 0);
            HeroUnit = Init_UnitData(Prefab_HeroUnit, armyCell, stat, new Vector2Int(1, 3), spawnRotation);

            BattleUnitList.Add(Init_UnitData(Prefab_NormalUnit, armyCell,stat,new Vector2Int(2, 3),spawnRotation));
            BattleUnitList.Add(Init_UnitData(Prefab_NormalUnit, armyCell,stat,new Vector2Int(3, 4),spawnRotation));
            BattleUnitList.Add(Init_UnitData(Prefab_NormalUnit, armyCell,stat,new Vector2Int(3, 2),spawnRotation));
            BattleUnitList.Add(Init_UnitData(Prefab_NormalUnit, armyCell,stat,new Vector2Int(4, 1),spawnRotation));
            BattleUnitList.Add(Init_UnitData(Prefab_NormalUnit, armyCell, stat, new Vector2Int(4, 5), spawnRotation));
        }

    }

    public BattleBaseUnit Init_UnitData(BattleBaseUnit baseModel,BattleArmyCell armyCell,BattleData data, Vector2Int index, Quaternion spawnRotation )
    {
        Object normalUnit= Instantiate(baseModel, armyCell.Dic_IndexPixel[index].GetPixelPos(), spawnRotation);
        normalUnit.GetComponent<Transform>().SetParent(this.transform);
        BattleBaseUnit battleNormalUnit = normalUnit.GetComponent<BattleBaseUnit>();
        battleNormalUnit.Init(armyCell.Dic_IndexPixel[index], ArmyIdx, data.ArmyAttack);


        return battleNormalUnit;
    }


    public BattleBaseUnit GetHeroUnit() { return HeroUnit; }
    public List<BattleBaseUnit> GetNormalUnit() { return BattleUnitList; }
    public List<BattleBaseUnit> GetAllUnits()
    { 
        List<BattleBaseUnit> allUnit=new List<BattleBaseUnit>();
        foreach(var unit in BattleUnitList) {  allUnit.Add(unit); }
        allUnit.Add(HeroUnit);
        return allUnit;
    }
}
