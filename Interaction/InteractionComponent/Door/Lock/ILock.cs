using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILock
{
    public bool IsLocked { get; set; }
    public void RattleByInteraction();
}