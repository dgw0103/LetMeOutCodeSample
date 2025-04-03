using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin;
using System;

public class KeyLock : BaseLock, IUsed
{
    public virtual void OnBeUsed()
    {
        CanDoorInteract = false;
    }
    public void OnAfterUsing()
    {
        CanDoorInteract = true;
        OpenDoor();
    }
}