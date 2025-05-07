using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// ��Ʋ���� ����� �� �����͸� ������ �� �ֵ��� ������ �� �Դϴ�.
/// ���� �߰��� ��ɵ��� ��ũ��Ʈ�� �з��� �پ��� �뵵�� Ȯ��� �� �ְ� �ۼ��߽��ϴ�.
/// </summary>

public class BattleTestCustomWindow : EditorWindow
{
    private CustomWindow_MapValue CustormMap = new CustomWindow_MapValue();         // �� ���� �� ������



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
