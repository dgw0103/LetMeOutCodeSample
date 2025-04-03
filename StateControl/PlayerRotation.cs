using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] private Transform body;
    [SerializeField] private Transform head;
    [SerializeField] private float targetRotationSpeed;
    [SerializeField] private float rotationSpeed;
    private Quaternion bodyTargetRotation;
    private Quaternion headTargetRotation;
    private Vector2 rotationInputDirection;



    private void Reset()
    {
        TryGetComponent(out body);
    }
    public void Update()
    {
        if (KeyInput.IsLocked(InputType.Camera) == false)
        {
            float speed = GameManager.Instance.PreferencesData.Sensitivity * targetRotationSpeed;

            bodyTargetRotation *= Quaternion.Euler(0f, rotationInputDirection.x * speed, 0);
            headTargetRotation *= Quaternion.Euler(-rotationInputDirection.y * speed, 0, 0);

            ClampHeadTargetRotationXAxis();

            transform.localRotation = Quaternion.Slerp(transform.localRotation, bodyTargetRotation, Time.smoothDeltaTime * rotationSpeed);
            head.transform.localRotation = Quaternion.Slerp(head.transform.localRotation, headTargetRotation, Time.smoothDeltaTime * rotationSpeed);
        }
    }



    public void InitRotationInput()
    {
        rotationInputDirection = Vector2.zero;
    }
    public void FixRotation()
    {
        InitRotationInput();
        bodyTargetRotation = body.localRotation;
        headTargetRotation = head.localRotation;
    }
    public void ClampHeadTargetRotationXAxis()
    {
        Quaternion q = headTargetRotation;

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;
        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        //angleX = Mathf.Clamp(angleX, -upsideMaxAngle, downsideMaxAngle);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        headTargetRotation = q;
    }
    public void ClampHeadRotationXAxis(float upsideMaxAngle, float downsideMaxAngle)
    {
        Quaternion q = head.transform.localRotation;

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;
        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, -upsideMaxAngle, downsideMaxAngle);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        head.transform.localRotation = q;
    }
}
