using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

[RequireComponent(typeof(CameraFixing))]
[RequireComponent(typeof(PuzzleDeviceConverter))]
public class CameraFixingComponent : Selection
{
    private CameraFixing cameraFixing;
    private new Collider collider;
    private PuzzleDeviceConverter deviceConverter;



    protected void Awake()
    {
        TryGetComponent(out cameraFixing);
        TryGetComponent(out collider);
        TryGetComponent(out deviceConverter);
    }



    [ContextMenu(nameof(Select))]
    public override void Select()
    {
        base.Select();
        cameraFixing.StartFixingCamera();
        KeyInput.PushAsInteraction();
        collider.enabled = false;
        deviceConverter.OnSelect();
    }
    [ContextMenu(nameof(Unselect))]
    public override void Unselect()
    {
        base.Unselect();
        cameraFixing.StopFixingCamera();
        KeyInput.PopKeyLockStack();
        collider.enabled = true;
        deviceConverter.OnUnselect();
    }
    protected bool EnableCollider { set => collider.enabled = value; }
}