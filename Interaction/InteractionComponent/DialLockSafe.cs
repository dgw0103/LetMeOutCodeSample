using LetMeOut;
using System.Collections;
using UnityEngine;

public class DialLockSafe : CameraFixingComponent
{
    [SerializeField] private RotationDial rotationDial;
    [SerializeField] private AudioSource handleRotationSound;
    [SerializeField] private Transform handle;
    [SerializeField] private Transform door;
    private readonly float handleRotationTime = 1f;
    private readonly float handleRotationSpeed = 1f;
    private readonly float doorOpeningSpeed = 300f;



    protected new void Awake()
    {
        base.Awake();
        rotationDial.OnCorrect += () => StartCoroutine(nameof(UnlockCoroutine));
    }
    protected override void KeepSelection()
    {
        base.KeepSelection();
        rotationDial.enabled = true;
    }
    protected override void KeepUnselection()
    {
        base.KeepUnselection();
        rotationDial.enabled = false;
    }


    private IEnumerator UnlockCoroutine()
    {
        float timer = 0;
        Quaternion openRotation = door.localRotation * Quaternion.Euler(Vector3.back * 90f);



        KeyInput.PushAsAll();
        handleRotationSound.Play();
        while (timer < handleRotationTime)
        {
            timer += Time.smoothDeltaTime;
            handle.localRotation *= Quaternion.Euler(handleRotationSpeed * Vector3.forward);
            yield return null;
        }

        yield return door.RotateLerpingTo(TransformType.Local, openRotation, Time.smoothDeltaTime * doorOpeningSpeed, 1f);

        KeyInput.PopKeyLockStack();
        OnUnselection();
        EnableCollider = false;
        gameObject.layer = LayerMask.NameToLayer(Keyword.defaultText);
    }
}