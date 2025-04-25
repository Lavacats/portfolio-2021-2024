using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Test_BattleMap_ShowTile 
{
    public Dictionary<Vector2, T_ShowTile> DIc_ShoWTile = new Dictionary<Vector2, T_ShowTile>();
    public List<Battle_MapTile> List_Cur_ShowUnitTile = new List<Battle_MapTile>();
    public Transform ADD_AND_Return_Tile(Vector3 tilePos,Vector2Int index, Transform parent, float tileSize,float tileInterval)
    {
        Transform reusultTransform = null;
        if (Battle_MapDataManager.Instance.isShowTile)
        {
            GameObject floorTile = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Renderer renderer = floorTile.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.material.color = Color.white;
                renderer.enabled = Battle_MapDataManager.Instance.isShowTile;
            }
            floorTile.transform.name = "tile" + index.x + "/" + index.y; 
            floorTile.transform.localScale = new Vector3(tileSize * tileInterval, tileSize * tileInterval, tileSize * tileInterval);
            floorTile.transform.localPosition = new Vector3(tilePos.x, 0.01f, tilePos.z);
            floorTile.transform.SetParent(parent);
            T_ShowTile showTile = floorTile.AddComponent<T_ShowTile>();
        
            reusultTransform = floorTile.transform;
            DIc_ShoWTile[new Vector2(tilePos.x, tilePos.z)] = showTile;
        }
        return reusultTransform;
    }
    public void Set_ShowMapTileParent(Vector3 tilePos, Battle_MapTile mapTile)
    {
        DIc_ShoWTile[new Vector2(tilePos.x, tilePos.z)].MapTile = mapTile;
    }
}
