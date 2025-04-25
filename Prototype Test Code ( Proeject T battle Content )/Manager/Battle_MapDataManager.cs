using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Battle_MapDataManager : SingleTon<Battle_MapDataManager>
{
    public Camera MapCamaera = null;
    public bool isShowTile = false;
    public bool isShowCell = false;
    public bool isShowPixel = false;

    public int PixelCountX ;
    public int PixelCountY ;
    public float PixelSIze ;
    public float PixelInterval;

    public int CellCount_X;
    public int CellCount_Y;
    public float CellSize;
    public float CellInterval;

    public int TileCount_X;
    public int TileCount_Y;
    public float TileSize;
    public float TIleInterval;

    private MapDataScriptable mapData;


    protected override void OnAwakeSingleton()
    {
        base.OnAwakeSingleton();

        mapData = AssetDatabase.LoadAssetAtPath<MapDataScriptable>("Assets/ScriptableObject/Map_Data.asset");

        if (mapData != null)
        {
            isShowTile = mapData.isShowTile;
            isShowCell = mapData.isShowCell;
            isShowPixel = mapData.isShowPixel;

            PixelCountX = mapData.PixelCountX;
            PixelCountY = mapData.PixelCountY;
            PixelSIze = mapData.PixelSize;
            PixelInterval = mapData.PixelInterval;

            CellCount_X = mapData.CellCount_X;
            CellCount_Y = mapData.CellCount_Y;
            CellSize = mapData.CellSize;
            CellInterval = mapData.CellInterval;

            TileCount_X = mapData.TileCount_X;
            TileCount_Y = mapData.TileCount_Y;
            TileSize = mapData.TileSize;
            TIleInterval = mapData.TIleInterval;

            if (BaseEventManager.Instance.OnEvent(BaseEventManager.EVENT_BASE.MAP_LOAD,null)==false)
            {
                Debug.Log("서순 실패.");
            }
        }
        else
        {
            Debug.LogError("EditorGameData를 로드할 수 없습니다.");
        }
    }
    public bool IsShowSet()
    {
        if (isShowTile || isShowCell || isShowPixel)
            return true;
        return false;
    }
}
