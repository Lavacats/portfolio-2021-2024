using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 배틀 맵을 관리하는 스크립트 입니다.
/// 맵, 길찾기 스크립트 등 다양한 정보를 가지고 있습니다.
/// </summary>

public class Battle_TestMapController : MonoBehaviour
{
    [SerializeField] private Battle_MapDirector MapManager;                 // 맵을 관리하는 스크립트
    [SerializeField] private Battle_Pathfinder_Controller Pathfinder;       // 길찾기 처리 스크립트

    private void OnEnable()
    {
        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.MAP_LOAD, (object value) => { 
            
            // 맵 정보를 AssetScript 로 관리하기 떄문에 해당 정보가 Load된 다음 Init할 수 있도록 순서보장을 해줘야 한다.

            MapManager.Init();
            Pathfinder.Init(MapManager);
        });
    }
    void Start()
    {
        // Engine_Manger에 각 스크립트 등록 ( Manager 통해 접근이 필요한 경우 접근할 수 있도록 )
        BattleEngine_Manager.Instance.MapDirector = MapManager;
        BattleEngine_Manager.Instance.Pathfinder = Pathfinder;
    }
    private void OnDestroy()
    {
        if(BaseEventManager.Instance)BaseEventManager.Instance.RemoveEvent(BaseEventManager.EVENT_BASE.MAP_LOAD, (object value) => { MapManager.Init(); });
    }

    void Update()
    {
    }
}
