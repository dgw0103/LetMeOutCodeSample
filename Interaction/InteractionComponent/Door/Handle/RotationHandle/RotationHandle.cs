using HoJin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[RequireComponent(typeof(Door))]
public class RotationHandle : MonoBehaviour, IHandle
{
    [SerializeField] private SnapAxis axis;
    [SerializeField] private RotationHandleIndividualData[] rotationHandleIndividualDatas;
    [SerializeField] private AudioSource handleOpeningSound;
    private Vector3[] originalEulerAngles;
    private float time = 0;



    private void Awake()
    {
        originalEulerAngles = new Vector3[rotationHandleIndividualDatas.Length];
        for (int i = 0; i < rotationHandleIndividualDatas.Length; i++)
        {
            originalEulerAngles[i] = rotationHandleIndividualDatas[i].handle.transform.localEulerAngles;
        }
    }



    public HandleType HandleType { get => HandleType.RotationHandle; }
    public void AddClip(List<KeyValuePair<AnimationClip, AnimationClip>> doorMovingClips, AnimatorOverrideController animatorOverrideController,
        Transform root)
    {
        AnimationClip animationClip = new AnimationClip();
        animationClip.wrapMode = WrapMode.Once;
        foreach (var item in rotationHandleIndividualDatas)
        {
            foreach (var propertyName in AnimationClipSetCurveUtility.GetLocalEulerAnglesAsSnapAxis(axis))
            {
                animationClip.SetCurve(root.GetRelativePath(item.handle, false), typeof(Transform), propertyName, item.movingDelta);
            }
        }
    }
    public AnimationClip AddHandleDefaultCurve(AnimationClip openingClip, Transform root)
    {
        foreach (var item in rotationHandleIndividualDatas)
        {
            foreach (var propertyName in AnimationClipSetCurveUtility.GetLocalEulerAnglesAsSnapAxis(axis))
            {
                openingClip.SetCurve(root.GetRelativePath(item.handle, false), typeof(Transform), propertyName,
                    AnimationCurve.Constant(0, 0, item.handle.localEulerAngles.GetValueOnAxis(axis)));
            }
        }

        return openingClip;
    }
    public IEnumerator Open_Coroutine()
    {
        float openingHalfTime = rotationHandleIndividualDatas.Max((x) => x.movingDelta.keys.Last().time);



        handleOpeningSound.Play();
        yield return Move_Coroutine(openingHalfTime, 1f);
        StartCoroutine(Move_Coroutine(0, -1f));
    }
    private IEnumerator Move_Coroutine(float destinationTime, float speed)
    {
        float previousTime = time;



        while (time * (speed / Mathf.Abs(speed)) < destinationTime)
        {
            time += Time.deltaTime * speed;
            for (int i = 0; i < rotationHandleIndividualDatas.Length; i++)
            {
                rotationHandleIndividualDatas[i].handle.transform.localEulerAngles +=
                    (rotationHandleIndividualDatas[i].movingDelta.Evaluate(time) * axis.GetUnitVectorBySnapAxis()) -
                    rotationHandleIndividualDatas[i].movingDelta.Evaluate(previousTime) * axis.GetUnitVectorBySnapAxis();
            }
            previousTime = time;
            yield return null;
        }

        for (int i = 0; i < rotationHandleIndividualDatas.Length; i++)
        {
            rotationHandleIndividualDatas[i].handle.transform.localEulerAngles =
                originalEulerAngles[i] + (rotationHandleIndividualDatas[i].movingDelta.Evaluate(destinationTime) * axis.GetUnitVectorBySnapAxis());
        }
    }
}