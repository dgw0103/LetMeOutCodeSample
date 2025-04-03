using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin;
using System;
using UnityEngine.Animations.Rigging;
using LetMeOut;
using HoaxGames;
using LetMeOut.Equipment;
using System.Linq;
using UnityEngine.Video;
using StarterAssets;
using UnityEngine.UI;

public partial class Player : Character, IInitializer
{
    [SerializeField] private PlayerHeadCamera head;
    [SerializeField] private Flashlight flashlight;
    [SerializeField] private AudioSource deepBreathSound;
    private Quaternion bodyTargetRotation;
    private Vector2 rotationInputDirection;
    private IState<PlayerStaminaState> playerStaminaState;
    private Inventory inventory;
    private RigBuilder rigBuilder;
    private HashSet<Equipment> equipments = new HashSet<Equipment>(2);
    private AnimatorParameterSaver animatorParameterSaver;
    private Vector2 movingDirection = Vector2.zero;
    private HeadRotation headRotation;
    private FirstPersonController firstPersonController;
    private float originalWalkingSpeed;
    private float originalSprintingSpeed;
    private float speed = 1f;
    private AudioListener audioListener;
    public event Action<bool> OnActivate;
    public event Action OnResurrect;
    public const string playerName = "Player";
    public const string deathTypeName = "DeathType";
    public const string playerBoneName = "PlayerBone";
    private readonly string dieName = "Die";
    public static readonly int isWalkingHash = Animator.StringToHash("IsWalking");
    public static readonly int isRunningHash = Animator.StringToHash("IsRunning");
    public static readonly int resurrectTriggerHash = Animator.StringToHash("Resurrect");
    public static readonly int isShakingHash = Animator.StringToHash("IsShaking");
    public static readonly int deathWaitingStateHash = Animator.StringToHash("DeathWaiting");
    public static readonly int skipDeathTriggerHash = Animator.StringToHash("SkipDeath");
    private readonly int inventorySiblingIndex = 0;
    public readonly static Vector3 spawnAdditionalPosition = new Vector3(0.498f, 0, -0.072f);



    protected new void Reset()
    {
        base.Reset();
        name = playerName;
    }
    public void Init()
    {
        Awake();
        TryGetComponent(out headRotation);
        Initializable.Init(head, headRotation);
        playerStaminaState = new FieldState<PlayerStaminaState>(PlayerStaminaState.Default);
        rotationInputDirection = Vector2.zero;
        bodyTargetRotation = transform.localRotation;
        head.transform.GetChild(inventorySiblingIndex).TryGetComponent(out inventory);
        StateMachineBehaviour<Player>.DefineComponentInStateMachineBehaviour(Animator, this);
        TryGetComponent(out rigBuilder);
        TryGetComponent(out animatorParameterSaver);
        AwakeStamina();
        TryGetComponent(out firstPersonController);
        Camera.TryGetComponent(out audioListener);

        head.FirstPersonTargeting = new FirstPersonTargeting<InteractionObject>(head.Camera, head.InteractableRange,
            LayerMask.GetMask(InteractionObject.interactionName, Keyword.interactionExamining), LayerMask.GetMask(Keyword.examining, UnityUtility.defaultName),
            () => StageManager.Instance.Player.HeadRotation.HeadForward);
        originalWalkingSpeed = firstPersonController.MoveSpeed;
        originalSprintingSpeed = firstPersonController.SprintSpeed;
    }
    private void Start()
    {
        SetFlashlightShadowQuality();
        head.SetLightVisibilityByPreference();
        Animator.keepAnimatorControllerStateOnDisable = true;
        FixRotation();
    }
    private void Update()
    {  
        if (KeyInput.IsLocked(InputType.InteractionOrItemUsing) == false)
        {
            head.UpdateInteractionObjectBeingLookingAt();
        }
        if (KeyInput.IsLocked(InputType.Camera) == false)
        {
            float deltaTime = Time.deltaTime > Time.fixedDeltaTime ? Time.fixedDeltaTime : Time.deltaTime;
            Vector2 rotationInputDirection = KeyInput.CameraInput * deltaTime * 6f;



            RotateBodyBy(rotationInputDirection.x * GameManager.Instance.PreferencesData.Sensitivity);
            headRotation.RotateByAngleAroundHeadRightAxis(rotationInputDirection.y * GameManager.Instance.PreferencesData.Sensitivity);
        }
        if (IsWalkable)
        {
            firstPersonController.UpdateMovementByInput(KeyInput.WalkingInput, KeyInput.RunningInput && IsRunnable);
            Walk(KeyInput.WalkingInput.Equals(Vector2.zero) == false);
            Run(KeyInput.RunningInput && IsRunnable);
        }
        UpdateStamina();
    }
    private void OnDestroy()
    {
        Selection.ClearSelections();
        HoldableObject.SetCurrentHoldingObjectNull();
    }
    


    public bool IsWalkable { get => KeyInput.IsLocked(InputType.Walking) == false; }
    private bool IsRunnable { get => KeyInput.IsLocked(InputType.Running) == false && playerStaminaState.State.Equals(PlayerStaminaState.Default); }
    private void RotateBodyBy(float angle)
    {
        bodyTargetRotation *= Quaternion.Euler(angle * BodyUp);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, bodyTargetRotation, 0.3f);
    }
    public void SetBodyRotation(Quaternion value)
    {
        bodyTargetRotation = value;
        transform.localRotation = value;
    }
    public PlayerHeadCamera Camera { get => head; }
    public Flashlight Flashlight { get => flashlight; }
    public Inventory Inventory { get => inventory; }
    public bool IsCollided
    {
        get => CharacterController.enabled;
        set
        {
            CharacterController.enabled = value;
        }
    }
    public PlayerStaminaState PlayerStaminaState
    {
        get
        {
            return playerStaminaState.State;
        }
        set
        {
            playerStaminaState.State = value;
        }
    }
    public bool RigBuilderEnabled { set => rigBuilder.enabled = value; }
    public HashSet<Equipment> Equipments { get => equipments; }
    public bool TryGetEquipment<T>(out T equipment) where T : Equipment
    {
        try
        {
            equipment = equipments.Single((x) => x is T) as T;
        }
        catch (Exception)
        {
            equipment = null;
        }

        return equipment;
    }
    public void InitRotationInput()
    {
        //rotationInputDirection = Vector2.zero;
    }
    public void FixRotation()
    {
        InitRotationInput();
        bodyTargetRotation = transform.localRotation;
        headRotation.FixRotation();
    }
    public void Walk(bool isWalking)
    {
        Animator.SetBool(isWalkingHash, isWalking);
    }
    public void Run(bool runningInput)
    {
        Animator.SetBool(isRunningHash, runningInput);
    }
    public void Stop()
    {
        if (gameObject.activeSelf)
        {
            Animator.SetFloat(AnimatorHash.speed, 0);
            Animator.CrossFade(AnimatorHash.idle, 0);
            Walk(false);
            Run(false);
        }
    }
    public void Resume()
    {
        Animator.SetFloat(AnimatorHash.speed, 1f);
    }
    public void TurnOnCameraLight(LightState lightState)
    {
        head.TurnOn(lightState);
        flashlight.TurnOff();
    }
    public void TurnOnCameraLight(LightState lightState, int layerMask)
    {
        TurnOnCameraLight(lightState);
        head.SetLightLayer(layerMask);
    }
    public void TurnOnCameraLightAsDefaultLayer(LightState lightState)
    {
        head.TurnOnAsDefaultLayer(lightState);
        flashlight.TurnOff();
    }
    public void TurnOffCameraLight()
    {
        head.TurnOff();
        head.InitLightLayer();
        flashlight.SetLightStateAsCurrentState();
    }
    public void LookAt(Vector3 target)
    {
        transform.RotateAround(transform.position, -transform.up, Vector3.SignedAngle(transform.forward,
                    Vector3.ProjectOnPlane((target - transform.position).normalized, transform.up), -transform.up));
        headRotation.LookAt(target);
        FixRotation();
    }
    public IEnumerator RotateUntilLookingAt(Vector3 target, float speed = 1f)
    {
        Quaternion bodyStartingRotation = transform.localRotation;
        Quaternion bodyRotationDelta = Quaternion.FromToRotation(BodyForward, Vector3.ProjectOnPlane(target - transform.position, BodyUp));
        Quaternion bodyTargetRotation = bodyRotationDelta * transform.localRotation;

        Quaternion headStartingRotation = bodyRotationDelta * headRotation.Head.localRotation;
        Quaternion headTargetRotation = Quaternion.Euler(Vector3.SignedAngle((bodyRotationDelta * headRotation.HeadForward).normalized, (target - headRotation.Head.position).normalized,
            bodyRotationDelta * headRotation.HeadRight) * headRotation.HeadRightAxis) * headRotation.Head.localRotation;
        float time = 0;



        do
        {
            SetBodyRotation(Quaternion.Lerp(bodyStartingRotation, bodyTargetRotation, time));
            headRotation.SetHeadRotation(Quaternion.Lerp(headStartingRotation, headTargetRotation, time));
            FixRotation();
            time += Time.deltaTime * speed;
            yield return null;
        } while (time < 1f);
        SetBodyRotation(Quaternion.Lerp(bodyStartingRotation, bodyTargetRotation, 1f));
        headRotation.SetHeadRotation(Quaternion.Lerp(headStartingRotation, headTargetRotation, 1f));
        FixRotation();
    }
    public void StartRotatingUntilLookingAt(Vector3 target, float speed = 1f)
    {
        StartCoroutine(RotateUntilLookingAt(target, speed));
    }
    private Vector3 BodyForward { get => transform.forward; }
    private Vector3 BodyUp { get => transform.up; }
    public void PlayDieAnimationAs(int type)
    {
        Animator.SetTrigger(dieName);
        Animator.SetInteger(deathTypeName, type);
    }
    private void OnEndAnimation()
    {
        KeyInput.PopKeyLockStack();
    }
    public void SetFlashlightShadowQuality()
    {
        if (GameManager.Instance.PreferencesData.FlashlightShadowResolution.Equals(0))
        {
            flashlight.HDAdditionalLightData.EnableShadows(false);
        }
        else
        {
            flashlight.HDAdditionalLightData.EnableShadows(true);
            flashlight.HDAdditionalLightData.shadowResolution.level = GameManager.Instance.PreferencesData.FlashlightShadowResolution - 1;
        }
    }
    public bool IsHeadShaking
    {
        set
        {
            if (value)
            {
                PlayerHeadShaking.AddComponent(head.gameObject);
            }
            else
            {
                if (head.TryGetComponent(out PlayerHeadShaking playerHeadShaking))
                {
                    Destroy(playerHeadShaking);
                }
            }
        } 
    }
    public void Resurrect()
    {
        Animator.SetTrigger(resurrectTriggerHash);
    }
    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
        OnActivate?.Invoke(value);
    }
    public void ClampHeadRotation(float min, float max)
    {
        headRotation.ClampHeadRotation(min, max);
    }
    public HeadRotation HeadRotation { get => headRotation; }
    public void UpdateTarget()
    {
        head.FirstPersonTargeting.UpdateTarget();
    }
    public override float Speed
    {
        get => speed;
        set
        {
            speed = value;
            Animator.SetFloat(AnimatorHash.speed, value);
            firstPersonController.MoveSpeed = originalWalkingSpeed * speed;
            firstPersonController.SprintSpeed = originalSprintingSpeed * speed;
        }
    }
    public void SkipDeathAnimation()
    {
        Animator.SetTrigger(skipDeathTriggerHash);
    }
    public RawImage CreatePlayerViewImage(GameObject playerViewImage, RenderTexture renderTexture)
    {
        RawImage image = Instantiate(playerViewImage, StageManager.Instance.UIs.transform).GetComponent<RawImage>();



        Camera.Camera.targetTexture = renderTexture;

        return image;
    }
    public AudioListener AudioListener { get => audioListener; }
    public AudioSource DeepBreathSound { get => deepBreathSound; }



    #region animator state
    public void OnIdleStateEnter()
    {
    }
    public void OnIdleStateExit()
    {
    }
    public void OnWalkingStateEnter()
    {
    }
    public void OnWalkingStateUpdate(AnimatorStateInfo stateInfo)
    {
        //Debug.Log(stateInfo.normalizedTime);
        //UnityEditor.EditorApplication.isPaused = true;
    }
    public void OnRunningStateEnter()
    {
        staminaDelta = staminaData.staminaDeltaInRunningState;
        AddStepSoundsVolumeAsRunning();
    }
    public void OnRunningStateUpdate()
    {
        
    }
    public void OnRunningStateExit()
    {
        if (PlayerStaminaState.Equals(PlayerStaminaState.Tired) == false)
        {
            staminaDelta = staminaData.originalStaminaDelta;
        }
        RestoreStepSoundsVolumeAsRunning();
    }
    public void OnMovingStateUpdate()
    {
        Animator.SetFloat(AnimatorHash.deltaX, KeyInput.WalkingInput.x);
        Animator.SetFloat(AnimatorHash.deltaZ, KeyInput.WalkingInput.y);
    }
    public void OnResurrectionStateEnter(Animator animator)
    {
        Camera.TurnOffCamera();
        Camera.RestoreCameraParent();
        Flashlight.TurnOff();
        TurnOffCameraLight();
        StageManager.Instance.Reload();
        StageManager.Instance.UIs.AppearUIs();
        inventory.Clear();
        animator.applyRootMotion = true;

        KeyInput.PushAsAll();
        StageManager.Instance.PostEffect.OnEnterReloadingProduction();
        stamina = staminaData.maximumStamina;
        RigBuilderEnabled = false;
        CharacterController.enabled = false;
        Area[] currentAreasClone = new Area[CurrentAreas.Count];
        CurrentAreas.CopyTo(currentAreasClone, 0);
        foreach (var item in currentAreasClone)
        {
            ExitFromArea(item);
        }
        OnResurrect?.Invoke();
    }
    public void OnResurrectionStateUpdate(AnimatorStateInfo animatorStateInfo)
    {
        StageManager.Instance.PostEffect.SetReloadingProductionValueByAnimatorStateNormalizedTime(animatorStateInfo.normalizedTime);
    }
    public void OnResurrectionStateExit(Animator animator)
    {
        animator.applyRootMotion = false;
        KeyInput.Init();
        Resume();
        RigBuilderEnabled = true;
        deepBreathSound.Play();
        CharacterController.enabled = true;
    }
    public void OnDeathStateEnter(Animator animator)
    {
        CharacterController.enabled = true;
        RigBuilderEnabled = false;
        animator.applyRootMotion = true;
    }
    public void OnDeathStateExit(Animator animator)
    {
        RigBuilderEnabled = true;
    }
    public void OnDeathWaitingStateEnter()
    {
        StartCoroutine(nameof(MovePlayerToEmptyPlace));
        Camera.TurnOffCamera();
        StageManager.Instance.ResetReverb();
    }
    private IEnumerator MovePlayerToEmptyPlace()
    {
        Animator.applyRootMotion = false;

        yield return null;
        transform.SetPositionAndRotation(StageManager.Instance.CurrentReloadingData.spawnPosition +
            (Quaternion.Euler(StageManager.Instance.CurrentReloadingData.spawnRotation) * spawnAdditionalPosition),
            Quaternion.Euler(StageManager.Instance.CurrentReloadingData.spawnRotation));
        FixRotation();
    }
    public void OnTiredWalkingState()
    {
        StartBeingTiredState();
    }
    #endregion



    #region animation event
    private void PlayStepSoundByAngleRange(DirectionAngleRange inputDirectionAngleRange)
    {
        if (inputDirectionAngleRange.IsBetweenThis(Mathf.Atan2(Animator.GetFloat(AnimatorHash.deltaZ), Animator.GetFloat(AnimatorHash.deltaX)) * Mathf.Rad2Deg))
        {
            StepSoundPlayer.PlayStepSound();
        }
    }
    #endregion



    [ContextMenu("Print inventory")]
    public void PrintInventory()
    {
        foreach (var item in inventory.Items)
        {
            Debug.Log(item.GetType());
        }
    }
    [ContextMenu("Subtract spawn additional position")]
    private void SubtractSpawnAdditionalPosition()
    {
        Debug.Log(transform.position - (transform.rotation * spawnAdditionalPosition));
    }
    [ContextMenu(nameof(LogEquipments))]
    private void LogEquipments()
    {
        foreach (var item in equipments)
        {
            Debug.Log(item.name);
        }
    }
}