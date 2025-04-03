using UnityEngine;
using HoJin;

public class Player_ResurrectionState : StateMachineBehaviour<Player>
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Component.OnResurrectionStateEnter(animator);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Component.OnResurrectionStateUpdate(stateInfo);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Component.OnResurrectionStateExit(animator);
    }
}