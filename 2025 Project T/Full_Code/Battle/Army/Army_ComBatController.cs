using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Army_ComBatController
{

    private Battle_MapDirector MapDirector;
    private PathFinder_Astar_Region Path_Finder = new PathFinder_Astar_Region();

    private List<Tuple<BattleArmy, BattleArmy>> combatFinght=new List<Tuple<BattleArmy, BattleArmy>>();
    private float timer = 0f;
    public float interval = 0.5f;
    public void Init()
    {
        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.COMBAT_START, SetArmyData);
        MapDirector = ArmyDataManager.Instance.MapDirector;
    }
    public void OnDestroy()
    {
        if (BaseEventManager.Instance != null) BaseEventManager.Instance.RemoveEvent(BaseEventManager.EVENT_BASE.COMBAT_START, SetArmyData);


    }
    public void SetArmyData(object value)
    {
        Tuple<BattleArmy, BattleArmy> data = (Tuple<BattleArmy, BattleArmy>)value;
        if (combatFinght.Contains(data) ==false)
        {
            combatFinght.Add(data);
        }
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            foreach (var data in combatFinght)
            {
                SetCombatBattle(data.Item1, data.Item2);
            }
        }

  
    }


    public void SetCombatBattle(BattleArmy attacker, BattleArmy defender)
    {
        ///근접 배틀의 경우는 몇가지가 있습니다 
        /// - 1. 원겨리 (부동) 군대를 근거리 군대가 습격하는 경우 ( A-1 -> B-2 )
        /// - 2. 근거리 부대와 근거리 부대를 습격하는 경우        ( A-1 <> B-1 )
        /// - 3. 원거릴 부대를 습격 중인 근거리 부대를 근거리 부대로 습격하는 경우 ( A-1 -> B-1 -> A-2 )

        /// 다양한 예시가 있지만 이번에는 가장 계산이 복잡한 근거리 vs 근거리 난전만 구현해보겠습니다.
        /// attacker 와 defender 는 모두 근접 유닛이라 가정합니다.

        // 1. 위치값 가져오기 (예시: PixelPosition 또는 CellPosition)
        Vector3 attackerPos = attacker.GetBattleArmyCell().transform.position; // 또는 attacker.Cell.Position 등
        Vector3 defenderPos = defender.GetBattleArmyCell().transform.position;

        // 2. 위치에 해당하는 Tile 리스트 획득 
        Battle_MapTile  attackerTiles = MapDirector.GetTile(attackerPos);
        Battle_MapTile  defenderTiles = MapDirector.GetTile(defenderPos);
        List<Battle_MapPixel> battle_MapPixels = new List<Battle_MapPixel>();

        if(attackerTiles==defenderTiles)
        {
            foreach (var pixel in attackerTiles.Get_ALL_BattleMapPixel()) battle_MapPixels.Add(pixel.Value);
        }
        else
        {
            foreach (var pixel in attackerTiles.Get_ALL_BattleMapPixel()) battle_MapPixels.Add(pixel.Value);
            foreach (var pixel in defenderTiles.Get_ALL_BattleMapPixel()) battle_MapPixels.Add(pixel.Value);
        }

        // 3. 공격 방어 전체 유닛 정보 획득
        List<BattleBaseUnit> attackerUnits = attacker.GetBattleUnitController().GetAllUnits();
        List<BattleBaseUnit> defenderUnits = defender.GetBattleUnitController().GetAllUnits();

        foreach(var unit in attackerUnits)
        {
            List<Vector2Int> pathLine = Path_Finder.FindPathForUnit(unit, attackerUnits, defenderUnits, battle_MapPixels);
            if(pathLine.Count>1)
            {
                unit.NextPixel = MapDirector.Get_ALL_BattleMapPixel_Index()[pathLine[1]];
            }
        }
        foreach (var unit in defenderUnits)
        {
            List<Vector2Int> pathLine = Path_Finder.FindPathForUnit(unit, defenderUnits, attackerUnits, battle_MapPixels);
            if (pathLine.Count > 1)
            {
                unit.NextPixel = MapDirector.Get_ALL_BattleMapPixel_Index()[pathLine[1]];
            }
        }
    }

}
