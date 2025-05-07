using UnityEngine;
using System;

public class BaseModel : MonoBehaviour
{
    [SerializeField] private Animator ModelAnimation;

    private Action Hit_Event = null;

    private Action AnimationEndEvent = null;
    private bool IsCallEndEvent = false;
    private string Name_EndEventAnimation = string.Empty;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = ModelAnimation.GetCurrentAnimatorStateInfo(0);

 
        if (stateInfo.IsName(Name_EndEventAnimation) && stateInfo.normalizedTime >= 1.0f)
        {
            if (IsCallEndEvent)
            {
                AnimationEndEvent?.Invoke();
                IsCallEndEvent = false;
                Name_EndEventAnimation = string.Empty;
                AnimationEndEvent = null;
            }
        }
     
    }
    public void Init(Action hitEvent)
    {
        Hit_Event = hitEvent;
    }
    public void Set_AnimationEnd_Event(string animName, Action endEvent)
    {
        IsCallEndEvent = true;
        Name_EndEventAnimation = animName;
        AnimationEndEvent = endEvent;
    }

    public void PlayAnimation(string animationName)
    {
        if (ModelAnimation != null)
        {
            AnimatorStateInfo stateInfo = ModelAnimation.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName(animationName) == false)
            {
                ModelAnimation.Play(animationName);
            }
        }
    }
    public bool Is_PlayAnimation(string animationName)
    {
        AnimatorStateInfo stateInfo = ModelAnimation.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(animationName) == false)
        {
            return false;
        }
        return true;
    }
    public void HitEvent()
    {
        Hit_Event?.Invoke();
    }
    public Animator GetAnimator() { return ModelAnimation; }
}
