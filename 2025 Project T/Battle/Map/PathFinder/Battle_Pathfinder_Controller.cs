using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// �˰��� ������
// �ʿ��Ѱ�
// 1. ������ ���ֵ� 
// 2. ����ü ����
// 3. ��ֹ� ����

// 1. ���� Ȥ�� ���ֵ� ����
// 2. �̵� ��� ����
// 3. �̵� �˰��� ����
// 4. �̵�����


public class Battle_Pathfinder_Controller : MonoBehaviour
{
    [SerializeField] private Battle_Pathfinder_Block Pathfinder_Block = new Battle_Pathfinder_Block();
    [SerializeField] private TestShow_Battle_Block MapBlock;

    [SerializeField] SerializableDictionary<Vector2, Battle_MapPixel> Dic_Map = new SerializableDictionary<Vector2, Battle_MapPixel>();

    private TestShow_PathFinder TestShowFinder = new TestShow_PathFinder();
    private Battle_MapDirector MapDirector;

    void Start()
    {
     
    }
    public void Init(Battle_MapDirector mapDirector)
    {
        MapDirector = mapDirector;
        Dic_Map = mapDirector.Get_ALL_BattleMapPixel();

        TestShowFinder.Init(mapDirector, MapBlock, Pathfinder_Block);

    }
    private void OnDestroy()
    {
    }


    void Update()
    {
        TestShowFinder.Update();
        Update_UnitsPos();

    }
    public void GetUnitPathFinder(Battle_MapPixel curPixel,Battle_MapPixel arrivePixel)
    {

    }






    private void Update_UnitsPos()
    {
        if (Battle_MapDataManager.Instance.isShowPixel == false) return;
        if (MapDirector == null) return;
        // 1. Ŭ����
        TestShowFinder.TestShowUnit_Pixel();


        // 2. Ŭ����� ���� ���� �ȼ� ǥ��
        foreach (var unitInfo in UnitDataManager.Instance.GetDicUnit())
        {
            BattleBaseUnit Unit = unitInfo.Value;
            Vector3 unitPos = Unit.transform.position;

            // 1.���� �Ҽ� Ÿ�� �˻�
            Battle_MapTile unitTile = MapDirector.GetTile(unitPos);

            if (unitTile != null)
            {
                // Show������ ����
                MapDirector.ShowTIleController.List_Cur_ShowUnitTile.Add(unitTile);

                // 2.���� �Ҽ� CELL �˻�
                Battle_MapCell unitCell = unitTile.GetCell(unitPos);
                if (unitCell != null)
                {
                    // Show������ ����
                    unitTile.ShowCellController.List_Cur_ShowUnitCell.Add(unitCell);


                    // 3. ���� �Ҽ� Pixel �˻�
                    Battle_MapPixel curUnitPixel = unitCell.GetPixel(unitPos);

                    Unit.PrePixel = Unit.CurPixel;
                    Unit.CurPixel = curUnitPixel;

                    // TEST SHOW
                    if (Battle_MapDataManager.Instance.isShowPixel)
                    {
                        unitCell.ShowPixelController.ShowPixelBlock(Unit.PrePixel, true, Color.red);
                    }
                }
            }
        }
    }
}
