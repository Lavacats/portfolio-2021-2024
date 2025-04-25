using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 맵에 배치할 Tile,Cell,Pixel정보를 생성하는 스크립트
/// EditorWindow를 통해 확장한 툴을 통해 Show 옵션을 선택한경우
/// 실제로 게임에서 Tile, Cell, Pixel을 확인할 수 있는 Test_BattleMap_Show 를 통해 Plane을 배치해 확인할 수 있습니다.
/// </summary>

public class Battle_MapDirector : MonoBehaviour
{
    [SerializeField] MeshCollider MeshCollider_Obj;         // Map의 바닥 MeshCollider
    [SerializeField] GameObject TileList;                   // Test용 보여주기 Tile,Cell,Pixel을 생성할 때 저장할 GameObject

    // Test용 Show Object를 관리하는 스크립트
    public Test_BattleMap_ShowTile ShowTIleController = new Test_BattleMap_ShowTile();

    // 실제 계산에서 사용되는 클래스를 저장하는 스크립트
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

    #region Get Map Data
    // 포지션에 따른 Tile 정보
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

    // 포지션에 따른 Cell 정보
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

    // 포지션에 따른 Pixel정보
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

    // 맵 모든 픽셀 정보
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
    #endregion
}
