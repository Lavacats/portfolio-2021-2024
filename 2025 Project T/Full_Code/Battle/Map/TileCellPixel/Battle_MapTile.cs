using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 타일은 여러개의 Cell로 이루어져있다.
[System.Serializable]
public class Battle_MapTile 
{
    public Vector2Int TileIndex;
    public Vector2 TilePos;
    public Dictionary<Vector2, Battle_MapCell> Dic_MapCell = new Dictionary<Vector2, Battle_MapCell>();
    public Test_BattleMap_ShowCell ShowCellController = new Test_BattleMap_ShowCell();

    public Battle_MapTile(Vector2Int index,Vector2 pos,Transform tileTransform)
    {
        TileIndex = index;
        Battle_MapDataManager mpaData = Battle_MapDataManager.Instance;
        for (int i = 0; i < mpaData.CellCount_X; i++)
        {
            for (int j = 0; j < mpaData.CellCount_Y; j++)
            {
                float posX = (i - mpaData.CellCount_X / 2f + 0.5f) * mpaData.CellSize + pos.x;
                float posZ = (j - mpaData.CellCount_X / 2f + 0.5f) * mpaData.CellSize + pos.y;

                Transform parentTransForm = tileTransform;
                Vector3 cellPos = new Vector3(posX, 0, posZ);
                Vector2 cellKey = new Vector2(posX, posZ);
                Vector2Int cellIndex = new Vector2Int(i, j) + Battle_MapDataManager.Instance.TileCount_X * TileIndex;

                if (Battle_MapDataManager.Instance.IsShowSet())
                {
                    Transform cellTransform = ShowCellController.ADD_Pixel(cellPos, cellIndex,tileTransform);
                    if (cellTransform != null)
                    {
                        parentTransForm = cellTransform;
                    }
                }
                Dic_MapCell[cellKey] = new Battle_MapCell(cellIndex, cellKey, parentTransForm);

                if (parentTransForm != tileTransform) ShowCellController.Set_ShowMapTileParent(cellPos, Dic_MapCell[cellKey]);
            }
        }
    }
    public Battle_MapCell GetCell(Vector3 pos)
    {
        Battle_MapCell result = null;
        foreach (var cell in Dic_MapCell)
        {
            float minCellX = cell.Key.x - Battle_MapDataManager.Instance.CellSize / 2.0f;
            float maxCellX = cell.Key.x + Battle_MapDataManager.Instance.CellSize / 2.0f;
            float minCellY = cell.Key.y - Battle_MapDataManager.Instance.CellSize / 2.0f;
            float maxCellY = cell.Key.y + Battle_MapDataManager.Instance.CellSize / 2.0f;

            if (pos.x >= minCellX && pos.x < maxCellX)
            {
                if (pos.z >= minCellY && pos.z < maxCellY)
                {
                    result = cell.Value;
                    break;
                }
            }
        }
        return result;
    }
    public Dictionary<Vector2, Battle_MapPixel> Get_ALL_BattleMapPixel()
    {
        Dictionary<Vector2, Battle_MapPixel> result = new Dictionary<Vector2, Battle_MapPixel>();
        foreach(var cell in Dic_MapCell)
        {
            foreach (var pixel in cell.Value.Dic_MapPixel) 
            {
                result[pixel.Value.PixelIndex] = pixel.Value; 
            }
        
        }
        return result; 
    }
    public Dictionary<Vector2Int, Battle_MapPixel> Get_ALL_BattleMapPixel_Index()
    {
        Dictionary<Vector2Int, Battle_MapPixel> result = new Dictionary<Vector2Int, Battle_MapPixel>();
        foreach (var cell in Dic_MapCell)
        {
            foreach (var pixel in cell.Value.Dic_MapPixel_Index)
            {
                result[pixel.Value.PixelIndex] = pixel.Value;
            }

        }
        return result;
    }
}
