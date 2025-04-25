using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 맵은 여러개의 타일로 이루어져있다.


public class Battle_MapDirector : MonoBehaviour
{
    [SerializeField] MeshCollider MeshCollider_Obj;
    [SerializeField] GameObject TileList;

    public Test_BattleMap_ShowTile ShowTIleController = new Test_BattleMap_ShowTile();
    public Dictionary<Vector2, Battle_MapTile> Dic_MapTile = new Dictionary<Vector2, Battle_MapTile>();

    private void Start()
    {

    }
    public void Init()
    {
        Battle_MapDataManager mpaData = Battle_MapDataManager.Instance;
        for (int i = 0; i < mpaData.TileCount_X; i++)
        {
            for (int j = 0; j < mpaData.TileCount_Y; j++)
            {
                float posX = (i - mpaData.TileCount_X / 2f + 0.5f) * mpaData.TileSize;
                float posZ = (j - mpaData.TileCount_Y / 2f + 0.5f) * mpaData.TileSize;

                Transform parentTransForm = TileList.transform;
                Vector3 tilePos = new Vector3(posX, 0, posZ);
                Vector2 tileKey = new Vector2(posX, posZ);
                Vector2Int tileIndex = new Vector2Int(i, j);

                if (Battle_MapDataManager.Instance.IsShowSet())
                {
                    Transform tileTransform = ShowTIleController.ADD_AND_Return_Tile(tilePos, tileIndex, TileList.transform, mpaData.TileSize, mpaData.TIleInterval);
                    if (tileTransform != null)
                    {
                        parentTransForm = tileTransform;
                    }
                }

                Dic_MapTile[tileKey] = new Battle_MapTile(tileIndex, tileKey, parentTransForm);

                if(parentTransForm!= TileList.transform) ShowTIleController.Set_ShowMapTileParent(tilePos, Dic_MapTile[tileKey]);
            }
        }
    }
    public Battle_MapTile GetTile(Vector3 pos)
    {
        Battle_MapTile result = null;
        foreach(var tile in Dic_MapTile)
        {
            float minTileX = tile.Key.x - Battle_MapDataManager.Instance.TileSize / 2.0f;
            float maxTileX = tile.Key.x + Battle_MapDataManager.Instance.TileSize / 2.0f;
            float minTileY = tile.Key.y - Battle_MapDataManager.Instance.TileSize / 2.0f;
            float maxTileY = tile.Key.y + Battle_MapDataManager.Instance.TileSize / 2.0f;

            if (pos.x >= minTileX && pos.x < maxTileX)
            {
                if (pos.z >= minTileY && pos.z< maxTileY)
                {
                    result = tile.Value;
                    break;
                }
            }
        }
        return result;
    }
    public Battle_MapCell GetCell(Vector3 pos)
    {
        Battle_MapCell result = null;

        Battle_MapTile tile = GetTile(pos);
        if (tile != null)
        {
            result = tile.GetCell(pos);
        }
        return result;
    }
    public Battle_MapPixel GetPixel(Vector3 pos)
    {
        Battle_MapPixel result = null;

        Battle_MapTile tile = GetTile(pos);
        if (tile != null)
        {
            Battle_MapCell mapCell = tile.GetCell(pos);
            if (mapCell != null) 
            {
                result = mapCell.GetPixel(pos);
            }
        }
        return result;
    }
    public SerializableDictionary<Vector2, Battle_MapPixel> Get_ALL_BattleMapPixel()
    {
        SerializableDictionary<Vector2, Battle_MapPixel> result = new SerializableDictionary<Vector2, Battle_MapPixel>();
        foreach (var tile in Dic_MapTile)
        {
            foreach (var pixel in tile.Value.Get_ALL_BattleMapPixel())
            {
                result[pixel.Key] = pixel.Value;
            }
        }
        return result;
    }

}
