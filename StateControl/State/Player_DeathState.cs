using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin;

public class Player_DeathState : StateMachineBehaviour<Player>
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Component.OnDeathStateEnter(animator);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Component.OnDeathStateExit(animator);
    }
}
