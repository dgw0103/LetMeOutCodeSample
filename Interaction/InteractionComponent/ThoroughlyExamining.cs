using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using HoJin;
using System;

[RequireComponent(typeof(Examining))]
public class ThoroughlyExamining : MonoBehaviour
{
    [SerializeField] private InputActionReference zoomInOutActionReference;
    [SerializeField] private InputActionReference pressingCheckingActionReference;
    [SerializeField] private InputActionReference rotationActionReference;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    private Examining examining;
    private float scrollPosition;
    private bool isStartingRotation = false;
    private InputAction zoomInOutAction;
    private InputAction pressingCheckingAction;
    private InputAction rotationAction;
    


    protected void Reset()
    {
#if UNITY_EDITOR
        Transform interactionExaminingObjectsParent;
        if (transform.childCount == 0 || transform.GetChild(0).name != "InteractionExaminingObjects")
        {
            interactionExaminingObjectsParent = new GameObject("InteractionExaminingObjects").transform;
            interactionExaminingObjectsParent.transform.SetParent(transform);
            interactionExaminingObjectsParent.ResetLocalSpace();
            interactionExaminingObjectsParent.SetAsFirstSibling();
        }
        UnityEditor.EditorUtility.SetDirty(transform);
#endif
    }
    protected void Awake()
    {
        TryGetComponent(out examining);
        scrollPosition = examining.ExaminingDistance;

        zoomInOutAction = zoomInOutActionReference.action.Clone();
        pressingCheckingAction = pressingCheckingActionReference.action.Clone();
        rotationAction = rotationActionReference.action.Clone();

        zoomInOutAction.performed += ZoomInOut;
        pressingCheckingAction.performed += StartRotation;
        rotationAction.performed += Rotate;
    }
    private void OnDisable()
    {
        zoomInOutAction.Disable();
        pressingCheckingAction.Disable();
        rotationAction.Disable();
    }



    private static float RotationSpeed { get => 10f; }
    private Transform InteractionExaminingObjectsParent { get => transform.GetChild(0); }
    public void SetSelectionState()
    {
        SetInteractionExaminingObjectsLayer(Keyword.interactionExamining);
        StageManager.Instance.UIs.SetActiveExaminingUI = true;
        scrollPosition = examining.ExaminingDistance;

        zoomInOutAction.Enable();
        pressingCheckingAction.Enable();
        rotationAction.Enable();
    }
    public void SetUnselectionState()
    {
        SetInteractionExaminingObjectsLayer(Keyword.defaultText);
        StageManager.Instance.UIs.SetActiveExaminingUI = false;
        OnDisable();
    }
    public void ZoomInOut(InputAction.CallbackContext callbackContext)
    {
        float scrollDirection = GetScrollDirection();

        if ((MathUtility.RoundAt(scrollPosition + scrollDirection, 2) <= maxDistance &&
            MathUtility.RoundAt(scrollPosition + scrollDirection, 2) >= minDistance))
        {
            scrollPosition += scrollDirection;

            examining.ExaminingDistance += scrollDirection;
        }





        float GetScrollDirection()
        {
            return callbackContext.ReadValue<float>();
        }
    }
    public void StartRotation(InputAction.CallbackContext callbackContext)
    {
        isStartingRotation = callbackContext.action.IsPressed();
    }
    public void Rotate(InputAction.CallbackContext callbackContext)
    {
        if (isStartingRotation && KeyInput.IsLocked(InputType.InteractionOrItemUsing) == false)
        {
            Vector2 mouseDirection = callbackContext.ReadValue<Vector2>();

            transform.RotateAround(transform.position, StageManager.Instance.Player.Camera.Camera.transform.up, -mouseDirection.x * RotationSpeed * Time.smoothDeltaTime);
            transform.RotateAround(transform.position, StageManager.Instance.Player.Camera.Camera.transform.right, mouseDirection.y * RotationSpeed * Time.smoothDeltaTime);
        }
    }
    private void SetInteractionExaminingObjectsLayer(string keyword)
    {
        for (int i = 0; i < InteractionExaminingObjectsParent.childCount; i++)
        {
            InteractionExaminingObjectsParent.GetChild(i).gameObject.layer = LayerMask.NameToLayer(keyword);
        }
    }
}