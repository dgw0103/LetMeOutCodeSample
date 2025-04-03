using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfPlayer : MonoBehaviour, IUsingAnimationPlayer
{
    private new Animation animation;
    private WaitForSeconds animationWaitingTime;



    private void Awake()
    {
        TryGetComponent(out animation);
        animationWaitingTime = new WaitForSeconds(animation.clip.length);
    }



    public IEnumerator DuringAnimationPlaying
    {
        get
        {
            yield return animationWaitingTime;
        }
    }
    public void PlayAnimation()
    {
        animation?.Play();
    }
}