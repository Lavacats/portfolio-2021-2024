using System.Collections.Generic;
using UnityEngine;

public class Test_BattleMap_ShowCell 
{
    public Dictionary<Vector2, T_ShowCell> DIc_ShowCell = new Dictionary<Vector2, T_ShowCell>();
    public List<Battle_MapCell> List_Cur_ShowUnitCell = new List<Battle_MapCell>();
    public Transform ADD_Pixel(Vector3 cellPos,Vector2Int index, Transform parent)
    {
#if UNITY_EDITOR
        Transform reusultTransform = null;
        Battle_MapDataManager mpaData = Battle_MapDataManager.Instance;
        if (Battle_MapDataManager.Instance.isShowCell)
        {
            GameObject floorTile = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Renderer renderer = floorTile.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.material.color = Color.blue;
                renderer.enabled = Battle_MapDataManager.Instance.isShowCell;
            }
            floorTile.transform.name = "Cell:" + index.x + "/" + index.y;
  
            floorTile.transform.localScale = new Vector3(mpaData.CellSize * mpaData.CellInterval, mpaData.CellSize * mpaData.CellInterval, mpaData.CellSize * mpaData.CellInterval);
            floorTile.transform.localPosition = new Vector3(cellPos.x, 0.02f, cellPos.z);
            T_ShowCell showCell = floorTile.AddComponent<T_ShowCell>();
        
            reusultTransform = floorTile.transform;
            DIc_ShowCell[new Vector2(cellPos.x, cellPos.z)] = showCell;
            floorTile.transform.SetParent(parent);
        }
        return reusultTransform;
#endif
    }
    public void Set_ShowMapTileParent(Vector3 cellPos, Battle_MapCell mapCell)
    {
        DIc_ShowCell[new Vector2(cellPos.x, cellPos.z)].MapCell = mapCell;
    }
}
