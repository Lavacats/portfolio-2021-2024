using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 알고리즘 도전기
// 필요한건
// 1. 움직일 유닛들 
// 2. 맵전체 정보
// 3. 장애물 정보

// 1. 유닛 혹은 유닛들 선택
// 2. 이동 장소 선택
// 3. 이동 알고리즘 개시
// 4. 이동개시


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
        // 1. 클리어
        TestShowFinder.TestShowUnit_Pixel();


        // 2. 클리어가된 다음 유닛 픽셀 표시
        foreach (var unitInfo in UnitDataManager.Instance.GetDicUnit())
        {
            BattleBaseUnit Unit = unitInfo.Value;
            Vector3 unitPos = Unit.transform.position;

            // 1.유닛 소속 타일 검색
            Battle_MapTile unitTile = MapDirector.GetTile(unitPos);

            if (unitTile != null)
            {
                // Show용으로 저장
                MapDirector.ShowTIleController.List_Cur_ShowUnitTile.Add(unitTile);

                // 2.유닛 소속 CELL 검색
                Battle_MapCell unitCell = unitTile.GetCell(unitPos);
                if (unitCell != null)
                {
                    // Show용으로 저장
                    unitTile.ShowCellController.List_Cur_ShowUnitCell.Add(unitCell);


                    // 3. 유닛 소속 Pixel 검색
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
