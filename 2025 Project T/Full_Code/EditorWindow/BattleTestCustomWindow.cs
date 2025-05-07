using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// 배틀에서 사용할 맵 데이터를 관리할 수 있도록 제작한 툴 입니다.
/// 툴에 추가할 기능들을 스크립트로 분류해 다양한 용도로 확장될 수 있게 작성했습니다.
/// </summary>

public class BattleTestCustomWindow : EditorWindow
{
    private CustomWindow_MapValue CustormMap = new CustomWindow_MapValue();         // 맵 관련 툴 데이터



    [MenuItem("12Lab/BattleTestWindow")]
    public static void ShowTool_Data()
    {
        BattleTestCustomWindow window = GetWindow<BattleTestCustomWindow>("Battle Setting");
        window.minSize = new Vector2(300, 200);
    }

    private void OnEnable()
    {
        CustormMap.OnEnable(this);
    }
    private void OnDisable()
    {
    }

    private void OnGUI()
    {
        CustormMap.OnGui_MapValue();        
    }

}
