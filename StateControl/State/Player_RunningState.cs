using UnityEngine;
using HoJin;

public class Player_RunningState : StateMachineBehaviour<Player>
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Component.OnRunningStateEnter();
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Component.OnRunningStateUpdate();
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Component.OnRunningStateExit();
    }
}