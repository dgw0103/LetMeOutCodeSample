using HoJin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragging : MonoBehaviour, IInteractionDown, IInteractionUp
{
    private Vector2 delta;
    private bool isDragging;
    private static Dragging currentDragging;
    public const string pickingUpLayerName = "PickingUp";



    public void OnInteractionDown()
    {
        KeyInput.PushAsHolding();
        gameObject.SetLayerIncludingChildren(pickingUpLayerName);
        StageManager.Instance.Player.Camera.SetLayerMaskAsHolding();
        StartDragging();
    }
    private void StartDragging()
    {
        StartCoroutine(nameof(Drag));
        isDragging = true;
        currentDragging = this;
    }
    private IEnumerator Drag()
    {
        while (this)
        {
            yield return null;
        }
    }
    public void OnInteractionUp()
    {
        if (isDragging)
        {
            KeyInput.PopKeyLockStack();
            gameObject.SetLayerIncludingChildren(InteractionObject.interactionName);
            StageManager.Instance.Player.Camera.RestoreLayerMask();
            StopDragging();
        }
    }
    private void StopDragging()
    {
        StopAllCoroutines();
        currentDragging = null;
        isDragging = false;
    }
    public static void StopDraggingCurrentObject()
    {
        currentDragging?.StopDragging();
    }
}
