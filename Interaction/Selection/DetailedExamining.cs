using HoJin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DetailedExamining : Examining
{
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    private float scrollPosition;
    


    private float RotationSpeed { get => 10f; }
    private Transform InteractionExaminingObjectsParent { get => transform.GetChild(0); }



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
    protected new void Awake()
    {
        base.Awake();
        scrollPosition = ExaminingDistance;
    }



    public override void Select()
    {
        base.Select();
        InteractionExaminingObjectsParent.gameObject.SetLayerIncludingChildren(Keyword.interactionExamining);
    }
    public override void Unselect()
    {
        base.Unselect();
        InteractionExaminingObjectsParent.gameObject.SetLayerIncludingChildren(Keyword.defaultText);
    }
    protected override void KeepSelection()
    {
        base.KeepSelection();
        scrollPosition = ExaminingDistance;
        //KeyInput.DetailedExaminingActions.Enable();
        StageManager.Instance.UIs.SetActiveExaminingUI = true;
    }
    protected override void KeepUnselection()
    {
        base.KeepUnselection();
        //KeyInput.DetailedExaminingActions.Disable();
        StageManager.Instance.UIs.SetActiveExaminingUI = false;
    }
    public static void ZoomInOut(float value)
    {
        if (Selections.TryPeek(out Selection selection) && selection is DetailedExamining)
        {
            DetailedExamining detailedExamining = selection as DetailedExamining;
            float scrollDirection = GetScrollDirection();



            if (/*detailedExamining.IsComing == false &&*/
                (MathUtility.RoundAt(detailedExamining.scrollPosition + scrollDirection, 2) <= detailedExamining.maxDistance &&
                MathUtility.RoundAt(detailedExamining.scrollPosition + scrollDirection, 2) >= detailedExamining.minDistance))
            {
                detailedExamining.scrollPosition += scrollDirection;
                detailedExamining.transform.position += scrollDirection * StageManager.Instance.Player.Camera.Camera.transform.forward;
            }





            float GetScrollDirection()
            {
                return -value;
            }
        }
    }
    public static void Rotate(Vector2 value)
    {
        if (Selections.TryPeek(out Selection selection) && selection is DetailedExamining)
        {
            DetailedExamining detailedExamining = selection as DetailedExamining;



            Vector2 mouseDirection = value;
            detailedExamining.transform.RotateAround(detailedExamining.transform.position,
                StageManager.Instance.Player.Camera.Camera.transform.up, -mouseDirection.x * detailedExamining.RotationSpeed * Time.smoothDeltaTime);
            detailedExamining.transform.RotateAround(detailedExamining.transform.position,
                StageManager.Instance.Player.Camera.Camera.transform.right, mouseDirection.y * detailedExamining.RotationSpeed * Time.smoothDeltaTime);
        }
    }
}