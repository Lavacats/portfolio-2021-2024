using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Map ������ ScriptableObject�� ����ϴ� ����
/// * ������Ʈ �ܺο��� �и��� ���� ���� ���� = ScriptableObject�� �밳 ���� ����
/// * Map�� ���� ���� ������ �Ǵ� ��� , �� ���� ScriptableObject �и��� ������ �޸� ȿ���� ����
/// * ������ ������ ��Ȯ�� ���� = ������ / ó���� �и�
/// </summary>

[CreateAssetMenu(fileName = "Map_Data", menuName = "ScriptableObject/Map_Data", order = int.MaxValue)]
public class MapDataScriptable : ScriptableObject
{
    [Header(" TEST Ÿ��/��/�ȼ� Show Check")]
    public bool isShowTile = false;
    public bool isShowCell = false;
    public bool isShowPixel = false;

    [Header(" MAP SIZE")]
    public int TileCount_X = 4;
    public int TileCount_Y = 4;

    [Header(" Tile Value")]

    public  float TileSize = 8;
    public  float TIleInterval = 0.1f;

    [Header(" Cell Value")]
    public  int CellCount_X = 4;
    public  int CellCount_Y = 4;
    public  float CellSize = 2f;
    public  float CellInterval = 0.1f;

    [Header(" Pixel Value")]
    public  int PixelCountX = 4;
    public  int PixelCountY = 4;
    public  float PixelSize = 0.5f;
    public  float PixelInterval = 0.1f;



    


}
