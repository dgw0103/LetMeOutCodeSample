using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin;

public class CameraHeadRotation : HeadRotation
{
    private Quaternion headTargetRotation;



    public override void RotateByAngleAroundHeadRightAxis(float angle, float minLimit, float maxLimit)
    {
        headTargetRotation *= Quaternion.Euler(-angle * HeadRightAxis);
        headTargetRotation = GetClampedRotationHeadRightAxis(headTargetRotation);

        Head.transform.localRotation = Quaternion.Slerp(Head.transform.localRotation, headTargetRotation, 0.3f);
    }
    public override void ClampHeadRotation(float min, float max)
    {
        Head.localRotation = headTargetRotation.ClampRotation(min * HeadRightAxis, max * HeadRightAxis);
    }
    public override void LookAt(Vector3 target)
    {
        Head.transform.LookAt(Head.position + Vector3.ProjectOnPlane(target - Head.position, HeadRight));
        FixRotation();
    }
    public override void FixRotation()
    {
        headTargetRotation = Head.localRotation;
    }
    protected override void SetHeadRotationBy(Quaternion value, float minLimit, float maxLimit)
    {
        headTargetRotation = value;
        headTargetRotation = GetClampedRotationHeadRightAxis(headTargetRotation);

        Head.transform.localRotation = headTargetRotation;
    }
}