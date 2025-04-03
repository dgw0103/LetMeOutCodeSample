using HoJin;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System;

public class NoHandle : IHandle
{
    public HandleType HandleType { get => HandleType.None; }
    public IEnumerator Open_Coroutine()
    {
        yield return null;
    }
}