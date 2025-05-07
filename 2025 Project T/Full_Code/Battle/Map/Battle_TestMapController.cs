using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ʋ ���� �����ϴ� ��ũ��Ʈ �Դϴ�.
/// ��, ��ã�� ��ũ��Ʈ �� �پ��� ������ ������ �ֽ��ϴ�.
/// </summary>

public class Battle_TestMapController : MonoBehaviour
{
    [SerializeField] private Battle_MapDirector MapManager;                 // ���� �����ϴ� ��ũ��Ʈ
    [SerializeField] private Battle_Pathfinder_Controller Pathfinder;       // ��ã�� ó�� ��ũ��Ʈ

    private void OnEnable()
    {
        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.MAP_LOAD, (object value) => { 
            
            // �� ������ AssetScript �� �����ϱ� ������ �ش� ������ Load�� ���� Init�� �� �ֵ��� ���������� ����� �Ѵ�.

            MapManager.Init();
            Pathfinder.Init(MapManager);
        });
    }
    void Start()
    {
        // Engine_Manger�� �� ��ũ��Ʈ ��� ( Manager ���� ������ �ʿ��� ��� ������ �� �ֵ��� )
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
