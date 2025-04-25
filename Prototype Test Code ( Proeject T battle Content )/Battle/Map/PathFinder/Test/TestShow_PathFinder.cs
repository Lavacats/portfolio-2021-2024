using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShow_PathFinder 
{
    private Battle_MapDirector MapDirector;
    private TestShow_Battle_Block MapBlock;
    private Battle_Pathfinder_Block Pathfinder_Block;
    public void Init(Battle_MapDirector mapDirector, TestShow_Battle_Block mapBlock, Battle_Pathfinder_Block pathfinder_Block)
    {
        MapDirector = mapDirector;
        MapBlock = mapBlock;
        Pathfinder_Block = pathfinder_Block;


    }
    public void Update()
    {
        SetBlock_Pixel();
    }
    private void SetBlock_Pixel()
    {
        if (Battle_MapDataManager.Instance.isShowPixel == false) return;

        if (Input.GetKey(KeyCode.X))
        {
            if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Down
            {
                if (Battle_MapDataManager.Instance.MapCamaera == null) return;

                
                RaycastHit hit;
                Ray ray = Battle_MapDataManager.Instance.MapCamaera.ScreenPointToRay(Input.mousePosition); // ���콺 ��ġ���� ���� ����

                if (Physics.Raycast(ray, out hit, Mathf.Infinity)) // ����ĳ��Ʈ�� ���� �浹 üũ
                {
                    GameObject hitObject = hit.collider.gameObject;
                    T_ShowPixel pixel = hitObject.GetComponent<T_ShowPixel>(); // Ư�� ��ũ��Ʈ�� �˻� (��: MyScript)
                    MapBlock.Add_BlockTile(pixel);
                    Pathfinder_Block.BlockPixel.Add(pixel.MpaPixel.PixelPos);
                }
            }
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKey(KeyCode.Z))
            {
                MapBlock.BlockCell.Clear();
            }
        }
    }

    public void TestShowUnit_Pixel()
    {
        if (Battle_MapDataManager.Instance.isShowPixel == false) return;
        if (MapDirector == null) return;

        // 1. �ȼ� ����Ʈ Ŭ����
        foreach (var tileInfo in MapDirector.ShowTIleController.List_Cur_ShowUnitTile)
        {
            foreach (var cellInfo in tileInfo.ShowCellController.List_Cur_ShowUnitCell)
            {
                foreach (var pixelInfo in cellInfo.Dic_MapPixel)
                {
                    cellInfo.ShowPixelController.ShowPixelBlock(pixelInfo.Value, false,Color.green);
                }
            }
            tileInfo.ShowCellController.List_Cur_ShowUnitCell.Clear();
        }
        MapDirector.ShowTIleController.List_Cur_ShowUnitTile.Clear();
    }
}
