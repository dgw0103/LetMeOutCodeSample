using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Area : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Character character))
        {
            character.EnterToArea(this);
        }
    }
    protected void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Character character))
        {
            character.ExitFromArea(this);
        }
    }



    public abstract IEnumerable<IEnvironmentVisitor> OnEnter(Character character);
}