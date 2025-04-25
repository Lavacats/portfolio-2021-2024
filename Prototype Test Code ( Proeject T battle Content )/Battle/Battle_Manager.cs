using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// 현재 구조상 네트워크 매니저에서 각 플레이어마다 player prefab에 등록된 프리팹을 생성시켜주고 연결된다.
// 즉 한개만 존재해야 하는 BattleManager는 이를 고려해 작성해야한다.
// BattlePlayerData


public class Battle_Manager : MonoBehaviour
{
    //[SerializeField] private NetworkManager Battle_NetWorkManager;
    [SerializeField] private BattleCameraController cameraController;

    // 배틀 엔진
    private BattleEngine Engine = new BattleEngine();
    // 배틀 UI Manager

    void Start()
    {
        Engine.Init();
    }
    public void OnDestroy()
    {
        Engine.OnDestroy();
    }
    public void Update()
    {
        Engine.Update();
    }
}
