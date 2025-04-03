using UnityEngine;
using System;
using HoJin;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public abstract class Character : MonoBehaviour
{
    private StepSoundPlayer stepSoundPlayer;
    private Animator animator;
    private CharacterController characterController;
    private Transform soundParent;
    private List<Area> currentAreas = new List<Area>();
    private Dictionary<Type, IEnumerable<IEnvironmentVisitor>> enteredCharacters = new Dictionary<Type, IEnumerable<IEnvironmentVisitor>>(3);
    public static readonly int stateHash = Animator.StringToHash("State");
    public static readonly int runningStateHash = Animator.StringToHash("Running");
    public const string movingStateName = "MovingState";
    public const string soundParentName = "SoundParent";



    protected void Reset()
    {
        gameObject.SetLayerAs("Character");
#if UNITY_EDITOR
        transform.CheckAndCreateInChild(soundParentName).SetSiblingIndex(0);
#endif
    }
    protected void Awake()
    {
        TryGetComponent(out stepSoundPlayer);
        TryGetComponent(out animator);
        TryGetComponent(out characterController);
        soundParent = transform.Find(soundParentName);
    }



    public void AddStepSoundsVolumeAsRunning()
    {
        for (int i = 0; i < stepSoundPlayer.StepSounds.Count; i++)
        {
            AddStepSoundsVolumeAsRunning(stepSoundPlayer.StepSounds[i]);
        }
    }
    public static void AddStepSoundsVolumeAsRunning(StepSound stepSound)
    {
        stepSound.AddVolumeAsRunning();
    }
    public void RestoreStepSoundsVolumeAsRunning()
    {
        for (int i = 0; i < stepSoundPlayer.StepSounds.Count; i++)
        {
            RestoreStepSoundsVolumeAsRunning(stepSoundPlayer.StepSounds[i]);
        }
    }
    public static void RestoreStepSoundsVolumeAsRunning(StepSound stepSound)
    {
        stepSound.RestoreVolumeAsRunning();
    }
    private void PlayStepSound()
    {
        stepSoundPlayer.PlayStepSound();
    }
    public abstract float Speed { get; set; }
    public virtual bool IsRunningState { get => Animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(runningStateHash); }
    public Animator Animator { get { return animator; } set => animator = value; }
    public CharacterController CharacterController { get { return characterController; } }
    public bool SetDetectCollisions { set => characterController.detectCollisions = value; }
    public StepSoundPlayer StepSoundPlayer { get => stepSoundPlayer; }
    public Transform SoundParent
    {
        get
        {
            if (soundParent == null)
            {
                soundParent = transform.Find(soundParentName);
            }
            return soundParent;
        }
    }
    public void EnterToArea(Area area)
    {
        if (currentAreas.Any((x) => x.GetType().Equals(area.GetType())) == false)
        {
            if (enteredCharacters.ContainsKey(area.GetType()) == false)
            {
                enteredCharacters.Add(area.GetType(), OnEnter(area, this, Vector3.zero));





                IEnumerable<IEnvironmentVisitor> OnEnter(Area area, Character character, Vector3 deltaPosition)
                {
                    List<IEnvironmentVisitor> visitors = new List<IEnvironmentVisitor>();



                    foreach (var item in area.OnEnter(character))
                    {
                        visitors.Add(item);
                    }

                    return visitors;
                }
            }
        }
        if (currentAreas.Contains(area) == false)
        {
            currentAreas.Add(area);
        }
    }
    public void ExitFromArea(Area area)
    {
        if (currentAreas.Contains(area))
        {
            currentAreas.Remove(area);
        }
        if (currentAreas.Any((x) => x.GetType().Equals(area.GetType())) == false)
        {
            if (enteredCharacters.TryGetValue(area.GetType(), out IEnumerable<IEnvironmentVisitor> values))
            {
                foreach (var item in values)
                {
                    item.OnExit(this);
                }
                enteredCharacters.Remove(area.GetType());
            }
        }
    }
    protected List<Area> CurrentAreas { get => currentAreas; }
    [ContextMenu(nameof(LogCurrentAreas))]
    private void LogCurrentAreas()
    {
        foreach (var item in currentAreas)
        {
            Debug.Log(item.GetType());
        }
    }
}