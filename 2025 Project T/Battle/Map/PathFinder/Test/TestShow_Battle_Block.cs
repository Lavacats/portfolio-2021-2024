using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShow_Battle_Block : MonoBehaviour
{
    public List<T_ShowPixel> BlockCell = new List<T_ShowPixel>();

    public void Add_BlockTile(T_ShowPixel block)
    {
        BlockCell.Add(block);
        Renderer renderer = block.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.color = Color.black;
        }
    }
    public void Clear_BlockTIle()
    {
        foreach(var pixel in BlockCell)
        {
            Renderer renderer = pixel.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.material.color = Color.white;
            }
        }
        BlockCell.Clear();
    }
}
