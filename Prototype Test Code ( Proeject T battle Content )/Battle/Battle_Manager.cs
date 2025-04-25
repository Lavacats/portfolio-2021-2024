using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// ���� ������ ��Ʈ��ũ �Ŵ������� �� �÷��̾�� player prefab�� ��ϵ� �������� ���������ְ� ����ȴ�.
// �� �Ѱ��� �����ؾ� �ϴ� BattleManager�� �̸� ����� �ۼ��ؾ��Ѵ�.
// BattlePlayerData


public class Battle_Manager : MonoBehaviour
{
    //[SerializeField] private NetworkManager Battle_NetWorkManager;
    [SerializeField] private BattleCameraController cameraController;

    // ��Ʋ ����
    private BattleEngine Engine = new BattleEngine();
    // ��Ʋ UI Manager

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
