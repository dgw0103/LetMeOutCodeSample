using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin;
using UnityEngine.Serialization;

public abstract class HeadRotation : MonoBehaviour, IInitializer
{
    [SerializeField] private Transform head;
    [SerializeField] private FlagsAxisType headForwardAxis;
    [SerializeField] private FlagsAxisType headUpAxis;
    [SerializeField] private Transform body;
    [SerializeField, Range(-180f, 180f)] private float minLimit;
    [SerializeField, Range(-180f, 180f)] private float maxLimit;



    protected void Reset()
    {
        TryGetComponent(out body);
    }
    public virtual void Init()
    {
        
    }



    public Transform Head { get => head; }
    public void RotateByAngleAroundHeadRightAxis(float angle)
    {
        RotateByAngleAroundHeadRightAxis(angle, minLimit, maxLimit);
    }
    public abstract void RotateByAngleAroundHeadRightAxis(float angle, float minLimit, float maxLimit);
    public Vector3 HeadForward { get { return head.rotation * headForwardAxis.ToUnitVector(); } }
    public Vector3 HeadForwardAxis { get => headForwardAxis.ToUnitVector(); }
    public Vector3 HeadUp { get => head.rotation * headUpAxis.ToUnitVector(); }
    public Vector3 HeadUpAxis { get => headUpAxis.ToUnitVector(); }
    public Vector3 HeadRight { get => (head.rotation * Vector3.Cross(headUpAxis.ToUnitVector(), headForwardAxis.ToUnitVector())).normalized; }
    public Vector3 HeadRightAxis { get => Vector3.Cross(headUpAxis.ToUnitVector(), headForwardAxis.ToUnitVector()); }
    public abstract void FixRotation();
    public abstract void ClampHeadRotation(float min, float max);
    public abstract void LookAt(Vector3 target);
    protected Transform Body { get => body; }
    public void SetHeadRotation(Quaternion value)
    {
        SetHeadRotationBy(value, minLimit, maxLimit);
    }
    protected abstract void SetHeadRotationBy(Quaternion value, float minLimit, float maxLimit);
    public Quaternion GetClampedRotationHeadRightAxis(Quaternion rotation)
    {
        return rotation.ClampRotation(minLimit * HeadRightAxis, maxLimit * HeadRightAxis);
    }
}