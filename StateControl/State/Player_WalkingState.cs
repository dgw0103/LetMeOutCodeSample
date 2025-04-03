using UnityEngine;
using HoJin;

public class Player_WalkingState : StateMachineBehaviour<Player>
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Component.OnWalkingStateEnter();
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Component.OnWalkingStateUpdate(stateInfo);
    }
}