using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using System;
using LetMeOut;
using HoJin;
using System.Linq;

public class Examining : Selection
{
    [SerializeField] private AudioSource pickUpSound;
    [SerializeField] private float examiningDistance;
    [SerializeField] private Vector3 examiningAngle;
    [SerializeField] private LightState lightState = new LightState(5f, 0, 80f, 1.5f);
    [SerializeField] private bool isMoveImmediately = false;
    [SerializeField] private bool isAppearExaminingPanel = true;
    [SerializeField] private GameObject[] notChangeLayers;
    [SerializeField] private bool isLookAtCameraContinuously;
    private float originalExaminingDistance;
    private Collider[] colliders;
    private IRendererLayerSetter rendererLayerSetter;
    private Coroutine comingCoroutine;
    private ThoroughlyExamining examining;
    private KeyObject keyObject;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    public event Action OnPickUp;
    public event Action OnPutDown;
    private readonly float comingSpeed = 5f;



    protected void Awake()
    {
        TryGetComponent(out keyObject);
        TryGetComponent(out examining);
        colliders = GetComponents<Collider>();
        if (TryGetComponent(out rendererLayerSetter) == false)
        {
            rendererLayerSetter = new RendererSetter(this);
        }
        originalExaminingDistance = examiningDistance;
        UpdateOriginalTransform();
    }



    public float ExaminingDistance { get => examiningDistance; set => examiningDistance = value; }
    public KeyObject KeyObject { get => keyObject; }
    public override void Select()
    {
        base.Select();
        pickUpSound.Play();
        MoveToExminingTransform(StageManager.Instance.Player.Camera.Camera.transform, StageManager.Instance.Player.Camera.Camera.transform, examiningAngle);





        void MoveToExminingTransform(Transform head, Transform camera, Vector3 angle)
        {
            MoveInFrontOfCamera();
            LookAtCamera();
            OnPickUp?.Invoke();





            void MoveInFrontOfCamera()
            {
                if (isMoveImmediately == false)
                {
                    StartMovingSmoothly();
                }
                else
                {
                    StartMovingImmediately();
                }





                void StartMovingSmoothly()
                {
                    comingCoroutine = StartCoroutine(MoveSmoothly());





                    IEnumerator MoveSmoothly()
                    {
                        while (this)
                        {
                            transform.position = Vector3.Lerp(transform.position,
                                camera.position + (StageManager.Instance.Player.HeadRotation.HeadForward * examiningDistance), comingSpeed * Time.deltaTime);
                            if (isLookAtCameraContinuously)
                            {
                                LookAtCamera();
                            }
                            yield return null;
                        }
                    }
                }
                void StartMovingImmediately()
                {
                    comingCoroutine = StartCoroutine(MoveImmediatelyToPlayer());





                    IEnumerator MoveImmediatelyToPlayer()
                    {
                        while (this)
                        {
                            transform.position = camera.position + (StageManager.Instance.Player.HeadRotation.HeadForward * examiningDistance);
                            if (isLookAtCameraContinuously)
                            {
                                LookAtCamera();
                            }
                            yield return null;
                        }
                    }
                }
                
            }
            void LookAtCamera()
            {
                transform.LookAt((transform.position) + StageManager.Instance.Player.Camera.Camera.transform.forward,
                            StageManager.Instance.Player.Camera.Camera.transform.up);
                Quaternion localRotation = transform.localRotation;
                transform.localRotation = localRotation * Quaternion.Euler(angle);
            }
        }
    }
    protected override void KeepSelection()
    {
        base.KeepSelection();
        examiningDistance = originalExaminingDistance;
        foreach (var item in colliders)
        {
            if (item)
            {
                item.isTrigger = true;
            }
        }
        StageManager.Instance.Player.TurnOnCameraLight(lightState, 2);
        StageManager.Instance.Player.Camera.SetLayerMaskWhenExamine();
        gameObject.SetLayerIncludingChildren(Keyword.examining, notChangeLayers.Cast<Transform>().ToArray());
        rendererLayerSetter.SetRendererLayerMask((uint)LightLayerEnum.LightLayer1);
        CheckIsAppearPickingUpPanelAndAct(StageManager.Instance.UIs.AppearPickingUpPanel);
        if (isAppearExaminingPanel)
        {
            UpdateExplanationPanel();
        }
        StageManager.Instance.UIs.AimPoint.SetActive(false);
        examining?.SetSelectionState();
    }
    public override void Unselect()
    {
        base.Unselect();
        foreach (var item in colliders)
        {
            if (item)
            {
                item.isTrigger = false;
            }
        }
        StageManager.Instance.Player.TurnOffCameraLight();
        StageManager.Instance.Player.Camera.RestoreLayerMask();
        StageManager.Instance.Player.Camera.InitLightLayer();
        gameObject.SetLayerIncludingChildren(InteractionObject.interactionName);
        rendererLayerSetter.SetRendererLayerMask((uint)LightLayerEnum.LightLayerDefault);
        CheckIsAppearPickingUpPanelAndAct(StageManager.Instance.UIs.DisappearPickingUpPanel);
        transform.localPosition = originalLocalPosition;
        transform.localRotation = originalLocalRotation;
        if (ReferenceEquals(comingCoroutine, null) == false)
        {
            StopCoroutine(comingCoroutine);
        }
        OnPutDown?.Invoke();
    }
    protected override void KeepUnselection()
    {
        base.KeepUnselection();
        StageManager.Instance.UIs.AimPoint.SetActive(true);
        examining?.SetUnselectionState();
    }
    protected virtual void UpdateExplanationPanel()
    {
        StageManager.Instance.UIs.UpdateExaminingPanel(keyObject.Key);
    }
    private void CheckIsAppearPickingUpPanelAndAct(Action action)
    {
        if (isAppearExaminingPanel)
        {
            action.Invoke();
        }
    }
    public void UpdateOriginalTransform()
    {
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
    }
}