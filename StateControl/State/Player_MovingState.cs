using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin;

public class Player_MovingState : StateMachineBehaviour<Player>
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Component.OnMovingStateUpdate();
    }
}