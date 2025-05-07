using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Map 관리에 ScriptableObject를 사용하는 이유
/// * 프로젝트 외부에서 분리된 구조 관리 가능 = ScriptableObject를 통개 구조 관리
/// * Map이 만약 수십 종류가 되는 경우 , 각 맵은 ScriptableObject 분리해 관리시 메모리 효율성 증가
/// * 데이터 구조의 명확성 증가 = 데이터 / 처리의 분리
/// </summary>

[CreateAssetMenu(fileName = "Map_Data", menuName = "ScriptableObject/Map_Data", order = int.MaxValue)]
public class MapDataScriptable : ScriptableObject
{
    [Header(" TEST 타일/쎌/픽셀 Show Check")]
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
