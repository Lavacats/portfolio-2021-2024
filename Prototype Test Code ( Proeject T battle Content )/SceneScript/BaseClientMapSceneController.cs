using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameSceneParam : SceneBaseControllerParam
{
    public string gameServerIP;
    public ushort gameServerPort;
    public int mapID;
    //public GamePlayerInfo playerInfo;
    //public ePlayType playType;
    public bool isReJoinGame = false;
    public ulong userId;
}

public class BaseClientMapSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
