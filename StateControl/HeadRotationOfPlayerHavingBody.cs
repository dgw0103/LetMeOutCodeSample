using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin;

public class HeadRotationOfPlayerHavingBody : HeadRotation
{
    private float targetAngle;
    private float angle;
    private Animator animator;



    public override void Init()
    {
        TryGetComponent(out animator);
    }
    private void Update()
    {
        Debug.DrawRay(Head.position, Body.forward);
        Debug.DrawRay(Head.position, HeadForward);
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (layerIndex.Equals(0))
        {
            Vector3 targetDirection = (Mathf.Cos(angle * Mathf.Deg2Rad) * HeadForward) + (Mathf.Sin(angle * Mathf.Deg2Rad) * HeadUp);
            Debug.DrawRay(Head.position, targetDirection, Color.red);



            animator.SetLookAtWeight(1f);
            animator.SetLookAtPosition(Head.position + targetDirection);
        }
    }



    public override void RotateByAngleAroundHeadRightAxis(float angle, float minLimit, float maxLimit)
    {
        targetAngle += angle;
        targetAngle = Mathf.Clamp(targetAngle, minLimit, maxLimit);
        this.angle = Mathf.SmoothStep(this.angle, targetAngle, 0.3f);
    }
    public override void ClampHeadRotation(float min, float max)
    {
        targetAngle = Mathf.Clamp(targetAngle, min, max);
    }

    public override void LookAt(Vector3 target)
    {

        RotateByAngleAroundHeadRightAxis(Vector3.SignedAngle(Vector3.ProjectOnPlane(target - Head.position, HeadRight), HeadForward,HeadRight));
        
    }

    [ContextMenu(nameof(FixRotation))]
    public override void FixRotation()
    {
        
    }
    protected override void SetHeadRotationBy(Quaternion value, float minLimit, float maxLimit)
    {
        
    }
    [ContextMenu(nameof(LogBodyForwardToHeadForwardAngle))]
    private void LogBodyForwardToHeadForwardAngle()
    {
        Debug.Log(Vector3.SignedAngle(Body.forward, HeadForward, -HeadRight));
    }
}