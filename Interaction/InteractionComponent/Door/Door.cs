using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoJin;
using UnityEngine.AI;

public abstract class Door : MonoBehaviour, IInteractionDown
{
    [SerializeField] public AudioSource openingSound;
    [SerializeField] public AudioSource closingSound;
    [SerializeField] private Collider[] doors;
    private List<NavMeshObstacle> navMeshObstacles = new List<NavMeshObstacle>();
    private ILock @lock;
    private IHandle handle;
    public event Action OnInteraction;
    public event Action OnOpened;
    public event Action OnClosed;
    public const float defaultSpeed = 0.01f;



    protected void Awake()
    {
        InitLockInstance();
        InitHandleInstance();
        InitNavMeshObstacles();





        void InitLockInstance()
        {
            if (TryGetComponent(out @lock) == false)
            {
                @lock = gameObject.AddComponent<NoLock>();
            }
        }
        void InitHandleInstance()
        {
            if (TryGetComponent(out handle) == false)
            {
                handle = new NoHandle();
            }
        }
        void InitNavMeshObstacles()
        {
            foreach (var item in doors)
            {
                if (item.TryGetComponent(out NavMeshObstacle navMeshObstacle))
                {
                    navMeshObstacles.Add(navMeshObstacle);
                }
            }
        }
    }
    protected void Start()
    {
        SetNavMeshObstaclesEnalbedByIsLocked();
    }



    public abstract DoorState DoorState { get; set; }
    protected Collider[] Doors { get => doors; }
    public bool CanInteract
    {
        set
        {
            string layerName = value ? InteractionObject.interactionName : Keyword.defaultText;

            foreach (var item in doors)
            {
                item.gameObject.SetLayerAs(layerName);
            }
        }
    }
    public bool CanCollide
    {
        set
        {
            foreach (var item in doors)
            {
                item.isTrigger = !value;
            }
        }
    }
    public bool IsLocked
    {
        get => @lock.IsLocked;
        set
        {
            @lock.IsLocked = value;
            SetNavMeshObstaclesEnalbedByIsLocked();
        }
    }
    private void SetNavMeshObstaclesEnalbedByIsLocked()
    {
        //NavMeshObstaclesEnalbed = @lock.IsLocked;
    }
    public void OnInteractionDown()
    {
        MoveByLockState();
        OnInteraction?.Invoke();
    }
    public void MoveByLockState()
    {
        if (@lock.IsLocked)
        {
            @lock.RattleByInteraction();
        }
        else
        {
            MoveByState();
        }
    }
    public void MoveByState()
    {
        StopAllCoroutines();
        CanInteract = false;
        switch (DoorState)
        {
            case DoorState.Closed:
                Open();
                break;
            case DoorState.Closing:
                OpenDuringClosing();
                break;
            case DoorState.Opened:
            case DoorState.Opening:
                Close();
                break;
        }
    }
    public void Open(float speed = defaultSpeed, bool isPlaySound = true)
    {
        DoorState = DoorState.Opening;
        StartCoroutine(Open_Coroutine(speed, isPlaySound));
    }
    public void Close(float speed = defaultSpeed, bool isPlaySound = true)
    {
        DoorState = DoorState.Closing;
        StartCoroutine(Close_Coroutine(speed, isPlaySound));
    }
    public void OpenDuringClosing(float speed = defaultSpeed, bool isPlaySound = true)
    {
        DoorState = DoorState.Opening;
        StartCoroutine(OpenDuringClosing_Coroutine(speed, isPlaySound));
    }
    private IEnumerator Open_Coroutine(float speed, bool isPlaySound = true)
    {
        if (handle != null)
        {
            yield return handle.Open_Coroutine();
        }

        yield return OpenDuringClosing_Coroutine(speed, isPlaySound);
    }
    private IEnumerator OpenDuringClosing_Coroutine(float speed, bool isPlaySound = true)
    {
        if (isPlaySound)
        {
            openingSound.Play();
        }
        yield return Move_Coroutine(MoveAsOpening, OpeningTime, speed);
        NavMeshObstaclesEnalbed = true;
        DoorState = DoorState.Opened;
        OnOpened?.Invoke();
    }
    public abstract void MoveAsOpening(float speed = defaultSpeed);
    protected abstract float OpeningTime { get; }
    private IEnumerator Close_Coroutine(float speed, bool isPlaySound = true)
    {
        if (isPlaySound)
        {
            closingSound.Play();
        }

        yield return Move_Coroutine(MoveAsClosing, ClosingTime, speed);
        NavMeshObstaclesEnalbed = false;
        DoorState = DoorState.Closed;
        OnClosed?.Invoke();
    }
    public abstract void MoveAsClosing(float speed = 1f);
    protected abstract float ClosingTime { get; }
    private IEnumerator Move_Coroutine(Action<float> movingAction, float movingTime, float speed)
    {
        CanCollide = false;
        NavMeshObstaclesEnalbed = false;
        movingAction.Invoke(speed);
        yield return new WaitForSeconds(movingTime * 0.5f);

        CanInteract = true;
        yield return new WaitForSeconds(movingTime * 0.5f);

        CanCollide = true;
    }
    public abstract void Rattle(float rattlingTime);
    private bool NavMeshObstaclesEnalbed
    {
        set
        {
            foreach (var item in navMeshObstacles)
            {
                item.enabled = value;
            }
        }
    }
}