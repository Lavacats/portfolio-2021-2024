using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseModel : MonoBehaviour
{
    [SerializeField] private Animator ModelAnimation;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayAnimation(string animationName)
    {
        ModelAnimation.Play(animationName);
    }
}
