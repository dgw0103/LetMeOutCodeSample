using HoJin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Follower))]
public class PickingUp : MonoBehaviour, IInteractionDown, IInteractionUp
{
    private static PickingUp currentPickingUp;



    public virtual void OnInteractionDown()
    {
        KeyInput.PushAsHolding();
        //pointerFollower.StartFollowing();
        currentPickingUp = this;
    }
    public void OnInteractionUp()
    {
        //if (pointerFollower.IsFollowing)
        //{
        //    OnStopFollowing();
        //}
    }
    protected virtual void OnStopFollowing()
    {
        KeyInput.PopKeyLockStack();
        //pointerFollower.StopFollowing();
        currentPickingUp = null;
    }
    public static void PutDownCurrentPickingUp()
    {
        currentPickingUp?.OnStopFollowing();
    }
}