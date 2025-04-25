using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class BattleTestCustomWindow : EditorWindow
{
    private CustomWindow_MapValue CustormMap = new CustomWindow_MapValue();



    [MenuItem("12Lab/BattleTestWindow")]
    public static void ShowDockableWindow()
    {
        BattleTestCustomWindow window = GetWindow<BattleTestCustomWindow>("Battle Setting");
        window.minSize = new Vector2(300, 200);
    }

    private void OnEnable()
    {
        CustormMap.OnEnable(this);
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    private void OnDisable()
    {
        // 이벤트 구독 해제
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnGUI()
    {
        //if (state == PlayModeStateChange.ExitingEditMode)
        {

            CustormMap.OnGui_MapValue();
        }
    }
    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
        }
    }
}
