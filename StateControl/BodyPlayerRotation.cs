using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPlayerRotation : MonoBehaviour
{
    [SerializeField] private Transform body;
    [SerializeField] private Transform head;
    [SerializeField] private Transform pointToLookAt;
    [SerializeField] private float distanceOfPointToLookAt = 1f;
    [SerializeField] private float targetRotationSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float minAngle = 20f;
    [SerializeField] private float maxAngle = 170f;
    private Quaternion bodyTargetRotation;
    private float headTargetAngle = 0;
    private float headAngle = 0;
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
            headTargetAngle += rotationInputDirection.y * speed;

            headTargetAngle = Mathf.Clamp(headTargetAngle, minAngle, maxAngle);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, bodyTargetRotation, Time.smoothDeltaTime * rotationSpeed);
            headAngle = Mathf.SmoothStep(headAngle, headTargetAngle, Time.smoothDeltaTime * rotationSpeed) * Mathf.Deg2Rad;
            pointToLookAt.position = head.position +
                (distanceOfPointToLookAt * transform.TransformDirection(new Vector3(0, -Mathf.Cos(headAngle), Mathf.Sin(headAngle))));
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
        headTargetAngle = Vector3.Angle(transform.forward, pointToLookAt.position - head.position);
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
