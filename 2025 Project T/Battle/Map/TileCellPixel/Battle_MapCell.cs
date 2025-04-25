using System.Collections.Generic;
using UnityEngine;

// Cell은 여러개의 pixel로 이루어져있다.
[System.Serializable]
public class Battle_MapCell 
{
    public Vector2Int CellIndex;
    public Vector2 CellPos;
    public Test_BattleMap_ShowPixel ShowPixelController = new Test_BattleMap_ShowPixel();
    public Dictionary<Vector2, Battle_MapPixel> Dic_MapPixel= new Dictionary<Vector2, Battle_MapPixel>();

    public Battle_MapCell(Vector2Int index,  Vector2 pos, Transform map)
    {
        CellIndex = index;
        CellPos = pos;
        Battle_MapDataManager mpaData = Battle_MapDataManager.Instance;
        for (int i = 0; i < mpaData.PixelCountX; i++)
        {
            for (int j = 0; j < mpaData.PixelCountY; j++)
            {
                float posX = (i - mpaData.PixelCountX / 2f + 0.5f) * mpaData.PixelSIze + pos.x;
                float posZ = (j - mpaData.PixelCountY / 2f + 0.5f) * mpaData.PixelSIze + pos.y;

                Vector2Int pixelIndex = new Vector2Int(i, j)+ CellIndex* Battle_MapDataManager.Instance.PixelCountX;
                Vector2 pixelPos = new Vector2(posX, posZ);
                Vector3 pixelPos3 = new Vector3(posX, 0, posZ);

                Battle_MapPixel mapPixel = new Battle_MapPixel(pixelIndex, pixelPos); 

                ShowPixelController.ADD_Pixel(pixelPos3, pixelIndex, CellIndex, map, mapPixel);
                if(ShowPixelController.DIc_ShowPixel.ContainsKey(new Vector2(pixelPos3.x, pixelPos3.z)))
                {
                    mapPixel.ShowPixel = ShowPixelController.DIc_ShowPixel[new Vector2(pixelPos3.x, pixelPos3.z)];
                }
         

                Dic_MapPixel[pixelPos] = mapPixel;
            }
        }
    }
    public Battle_MapPixel GetPixel(Vector3 pos)
    {
        Battle_MapPixel result = null;
        foreach (var pixel in Dic_MapPixel)
        {
            float minPixelX = pixel.Key.x - Battle_MapDataManager.Instance.PixelSIze / 2.0f;
            float maxPixelX = pixel.Key.x + Battle_MapDataManager.Instance.PixelSIze / 2.0f;
            float minPixelY = pixel.Key.y - Battle_MapDataManager.Instance.PixelSIze / 2.0f;
            float maxPixelY = pixel.Key.y + Battle_MapDataManager.Instance.PixelSIze / 2.0f;

            if (pos.x >= minPixelX && pos.x < maxPixelX)
            {
                if (pos.z >= minPixelY && pos.z< maxPixelY)
                {
                    result = pixel.Value;
                    break;
                }
            }
        }
        return result;
    }
}
