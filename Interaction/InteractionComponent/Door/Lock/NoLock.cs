using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoLock : MonoBehaviour, ILock
{
    public bool IsLocked { get => false; set { } }
    public void RattleByInteraction()
    {
    }
}