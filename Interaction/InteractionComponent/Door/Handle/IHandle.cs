using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin;

public interface IHandle
{
    public IEnumerator Open_Coroutine();
    public HandleType HandleType { get; }
}