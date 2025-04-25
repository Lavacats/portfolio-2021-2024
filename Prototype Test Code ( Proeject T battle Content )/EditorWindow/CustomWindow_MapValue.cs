using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CustomWindow_MapValue 
{
    private SerializedObject serializedObject;  

    private bool IsShowSettings = false;        // 세팅 메뉴 오픈 
    private bool IsShow_SetShow = false;        // 타일 SHOW 
    private bool IsShow_SetMapSize = false;     // 맵사이즈 입력 메뉴
    private bool IsShow_MapValue = false;       // 맵 VALUE 확인용
    private bool IsShow_MapTileValue = false;   // Tile 정보
    private bool IsShow_MapCellValue = false;   // Cell 정보
    private bool IsShow_MapPixelValue = false;  // PIxel 정보

    private MapDataScriptable mapData;
    public void OnEnable(UnityEngine.Object obj)
    {
        serializedObject = new SerializedObject(obj);
        mapData = AssetDatabase.LoadAssetAtPath<MapDataScriptable>("Assets/ScriptableObject/Map_Data.asset");
    }
    public void OnGui_MapValue()
    {
        serializedObject.Update();

        IsShowSettings = EditorGUILayout.Foldout(IsShowSettings, "Map_Settings");


        if (IsShowSettings)
        {
            EditorGUILayout.LabelField("  Cell : " + mapData.PixelCountX + " x " + mapData.PixelCountY + " Pixel");
            EditorGUILayout.LabelField("  Tile : " + mapData.CellCount_X + " x " + mapData.CellCount_Y + " Cell");
            EditorGUILayout.LabelField("  Tile : " + mapData.CellCount_X * mapData.PixelCountX + " x " + mapData.CellCount_Y * mapData.PixelCountY + " Pixel");
            EditorGUILayout.LabelField("  TileConut : " + mapData.TileCount_X + " x " + mapData.TileCount_Y);
            EditorGUILayout.LabelField("   ㄴCellConut : " + mapData.TileCount_X * mapData.CellCount_X + " x " + mapData.TileCount_Y * mapData.CellCount_Y);
            EditorGUILayout.LabelField("   ㄴPixelCount : " + mapData.TileCount_X * mapData.CellCount_X * mapData.PixelCountX + " x " + mapData.TileCount_Y * mapData.CellCount_Y * mapData.PixelCountY);


            #region Show Setting
            EditorGUI.indentLevel++;
            IsShow_SetShow = EditorGUILayout.Foldout(IsShow_SetShow, "    Set_ShowOption");
            if (IsShow_SetShow)
            { 
                mapData.isShowTile = EditorGUILayout.Toggle(" isShowTile", mapData.isShowTile);
                mapData.isShowCell = EditorGUILayout.Toggle(" isShowCell", mapData.isShowCell);
                mapData.isShowPixel = EditorGUILayout.Toggle(" isShowPixel", mapData.isShowPixel);
            }
            EditorGUI.indentLevel--;
            #endregion

            #region Map Setting
            EditorGUI.indentLevel++;
            IsShow_SetMapSize = EditorGUILayout.Foldout(IsShow_SetMapSize, "    Set_MapSize");
            if (IsShow_SetMapSize)
            {
                mapData.TileCount_X = EditorGUILayout.IntField(" TileCount_X Value", mapData.TileCount_X);
                mapData.TileCount_Y = EditorGUILayout.IntField(" TileCount_Y Value", mapData.TileCount_Y);
            }
            EditorGUI.indentLevel--;
            #endregion

            #region Show Map Value
            EditorGUI.indentLevel++;
            IsShow_MapValue = EditorGUILayout.Foldout(IsShow_MapValue, "    Show_MapValue");
            if (IsShow_MapValue)
            {
                EditorGUI.indentLevel++;
                IsShow_MapTileValue = EditorGUILayout.Foldout(IsShow_MapTileValue, "    Show_MapTileValue");
                if(IsShow_MapTileValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("TileSize : " + mapData.TileSize.ToString(), EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("TIleInterval : " + mapData.TIleInterval.ToString(), EditorStyles.boldLabel);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;

                EditorGUI.indentLevel++;
                IsShow_MapCellValue = EditorGUILayout.Foldout(IsShow_MapCellValue, "    Show_MapCellValue");
                if(IsShow_MapCellValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("CellCount_X : " + mapData.CellCount_X.ToString(), EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("CellCount_Y : " + mapData.CellCount_Y.ToString(), EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("CellSize : " + mapData.CellSize.ToString(), EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("CellInterval : " + mapData.CellInterval.ToString(), EditorStyles.boldLabel);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;

                EditorGUI.indentLevel++;
                IsShow_MapPixelValue = EditorGUILayout.Foldout(IsShow_MapPixelValue, "    Show_MapPixelValue");
                if(IsShow_MapPixelValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("PixelCountX : " + mapData.PixelCountX.ToString(), EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("PixelCountY : " + mapData.PixelCountY.ToString(), EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("PixelSize : " + mapData.PixelSize.ToString(), EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("PixelInterval : " + mapData.PixelInterval.ToString(), EditorStyles.boldLabel);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;

            }
            EditorGUI.indentLevel--;
            #endregion
        }
    }
}
