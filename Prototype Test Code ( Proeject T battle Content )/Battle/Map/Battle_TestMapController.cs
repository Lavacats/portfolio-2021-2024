using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스타크래프트 1 에서 크기가 평균 128 * 128 에서 최대 256 * 256 이다
/// <summary>
/// 1 사이즈 Pos를 16 너비 개 가진 Cell = (16os)
/// 4 사이즈 Cell을 8개 가진 Tile = (8Cell) = (16Pos)
/// </summary>

public class Battle_TestMapController : MonoBehaviour
{
  
    [SerializeField] private Battle_MapDirector MapManager;
    [SerializeField] private Battle_Pathfinder_Controller Pathfinder;
    private void OnEnable()
    {
        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.MAP_LOAD, (object value) => { 
            MapManager.Init();
            Pathfinder.Init(MapManager);
        });


    }
    void Start()
    {
        var singdle = Battle_MapDataManager.Instance;
        BattleEngine_Manager.Instance.MapDirector = MapManager;
        BattleEngine_Manager.Instance.Pathfinder = Pathfinder;
    }
    private void OnDestroy()
    {
        if(BaseEventManager.Instance)BaseEventManager.Instance.RemoveEvent(BaseEventManager.EVENT_BASE.MAP_LOAD, (object value) => { MapManager.Init(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
