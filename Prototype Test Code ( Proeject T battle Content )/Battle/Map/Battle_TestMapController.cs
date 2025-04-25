using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��Ÿũ����Ʈ 1 ���� ũ�Ⱑ ��� 128 * 128 ���� �ִ� 256 * 256 �̴�
/// <summary>
/// 1 ������ Pos�� 16 �ʺ� �� ���� Cell = (16os)
/// 4 ������ Cell�� 8�� ���� Tile = (8Cell) = (16Pos)
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
