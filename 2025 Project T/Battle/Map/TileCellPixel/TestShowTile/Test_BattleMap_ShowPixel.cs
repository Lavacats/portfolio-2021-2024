using System.Collections.Generic;
using UnityEngine;

public class Test_BattleMap_ShowPixel
{
    public Dictionary<Vector2, T_ShowPixel> DIc_ShowPixel = new Dictionary<Vector2, T_ShowPixel>();
    public void ADD_Pixel(Vector3 pixelPos,Vector2Int index, Vector2Int cellindex, Transform cellTransform, Battle_MapPixel mapPixel)
    {
#if UNITY_EDITOR
        if (Battle_MapDataManager.Instance.IsShowSet())
        {
            GameObject floorTile = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Renderer renderer = floorTile.GetComponent<Renderer>();
            Battle_MapDataManager mpaData = Battle_MapDataManager.Instance;
 
            if (renderer != null)
            {
                renderer.material.color = Color.green;
                renderer.enabled = mpaData.isShowPixel;
            }
            floorTile.transform.name = "Pixel:" + index.x + "/" + index.y;

            floorTile.transform.localScale = new Vector3(mpaData.PixelSIze * mpaData.PixelInterval, mpaData.PixelSIze * mpaData.PixelInterval, mpaData.PixelSIze * mpaData.PixelInterval);
            floorTile.transform.localPosition = new Vector3(pixelPos.x, 0.03f, pixelPos.z);
            T_ShowPixel pixel = floorTile.AddComponent<T_ShowPixel>();
            pixel.MpaPixel = mapPixel;
            DIc_ShowPixel[new Vector2(pixelPos.x, pixelPos.z)] = pixel;
            floorTile.transform.SetParent(cellTransform);
        }
#endif
    }
    public void ShowPixelBlock(Battle_MapPixel pixel,bool isShow,Color color)
    {
        if(DIc_ShowPixel.ContainsKey(pixel.PixelPos))
        {
            Renderer renderer = DIc_ShowPixel[pixel.PixelPos].GetComponent<Renderer>();
            if(renderer!=null)
            {
                if (isShow)
                {
                    renderer.material.color = color;
                }
                else
                {
                    if (renderer.material.color == Color.red)
                    {
                        renderer.material.color = Color.green;
                    }
                }
            }
        }
    }
    public void ShowPixel(Battle_MapPixel pixel, bool isShow, Color color)
    {
        if (pixel.ShowPixel == null) return;
        Renderer renderer = pixel.ShowPixel.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (isShow)
            {
                renderer.material.color = color;
            }
            else
            {
                if (renderer.material.color == Color.red)
                {
                    renderer.material.color = Color.green;
                }
            }
        }
    }
}
