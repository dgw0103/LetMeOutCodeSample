using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightHolding : MonoBehaviour
{
    [SerializeField] private Transform flashlight;
    [SerializeField] private Transform head;
    [SerializeField] private Vector3 flashlightPositionFromHead;
    [SerializeField] private Vector3 flashlightRotationFromHead;



    private void LateUpdate()
    {
        flashlight.rotation = head.rotation * Quaternion.Euler(flashlightRotationFromHead);
        flashlight.position = head.position + (head.rotation * flashlightPositionFromHead);
    }
}
