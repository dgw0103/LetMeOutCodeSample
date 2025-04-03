using System.Collections;
using UnityEngine;
using HoJin;

public class TiredWalkingState : StateMachineBehaviour<Player>
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Component.OnTiredWalkingState();
    }
}