using UnityEngine;
using HoJin;
using System.Collections.Generic;
using System.Collections;
using System;

public class Player_IdleState : StateMachineBehaviour<Player>
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Component.OnIdleStateEnter();
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Component.OnIdleStateExit();
    }
}