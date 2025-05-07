using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DamageText : MonoBehaviour
{
    [SerializeField] Text m_DamageText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Text GetText() { return m_DamageText; }
}
