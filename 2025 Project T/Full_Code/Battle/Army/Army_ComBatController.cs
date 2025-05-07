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
        ///���� ��Ʋ�� ���� ����� �ֽ��ϴ� 
        /// - 1. ���ܸ� (�ε�) ���븦 �ٰŸ� ���밡 �����ϴ� ��� ( A-1 -> B-2 )
        /// - 2. �ٰŸ� �δ�� �ٰŸ� �δ븦 �����ϴ� ���        ( A-1 <> B-1 )
        /// - 3. ���Ÿ� �δ븦 ���� ���� �ٰŸ� �δ븦 �ٰŸ� �δ�� �����ϴ� ��� ( A-1 -> B-1 -> A-2 )

        /// �پ��� ���ð� ������ �̹����� ���� ����� ������ �ٰŸ� vs �ٰŸ� ������ �����غ��ڽ��ϴ�.
        /// attacker �� defender �� ��� ���� �����̶� �����մϴ�.

        // 1. ��ġ�� �������� (����: PixelPosition �Ǵ� CellPosition)
        Vector3 attackerPos = attacker.GetBattleArmyCell().transform.position; // �Ǵ� attacker.Cell.Position ��
        Vector3 defenderPos = defender.GetBattleArmyCell().transform.position;

        // 2. ��ġ�� �ش��ϴ� Tile ����Ʈ ȹ�� 
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

        // 3. ���� ��� ��ü ���� ���� ȹ��
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
