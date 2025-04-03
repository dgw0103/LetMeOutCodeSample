using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Door))]
public class BaseLock : MonoBehaviour, ILock
{
    [SerializeField] private bool isLocked = false;
    [SerializeField] private Rattling rattling;
    private Door door;



    protected void Awake()
    {
        TryGetComponent(out door);
    }



    public bool IsLocked { get => isLocked; set => isLocked = value; }
    public void RattleByInteraction()
    {
        rattling.RattleByInteraction(door);
    }
    public void Rattle()
    {
        rattling.Rattle(door);
    }
    protected void OpenDoor()
    {
        isLocked = false;
        door.Open();
    }
    protected bool CanDoorInteract { set => door.CanInteract = value; }
}
