using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BattleValue : MonoBehaviour
{
    [SerializeField] List<Text> textList = new List<Text>();

    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        var unitList = UnitDataManager.Instance.GetCameraSelectUnit();
        foreach (var item in textList)
        {
            item.text = string.Empty;
        }
        for (int i=0;i<unitList.Count;i++)
        {
            textList[i].text = unitList[i].name;
        }
    }
}
