using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUsingAnimationPlayer
{
    public void PlayAnimation();
    public IEnumerator DuringAnimationPlaying { get; }
}