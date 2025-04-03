using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatchParentSettingKeyLock : KeyLock
{
    [SerializeField] private Transform parent;
    [SerializeField] private Transform latch;



    public override void OnBeUsed()
    {
        base.OnBeUsed();
        latch.parent = parent;
    }
}