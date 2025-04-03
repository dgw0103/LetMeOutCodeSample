using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HoJin;

public class DialLock : CombinationLock
{
    [SerializeField] private RotationDial rotationDial;



    protected new void Awake()
    {
        base.Awake();
        rotationDial.OnCorrect += () => Unlock();
    }



    protected override void KeepSelection()
    {
        base.KeepSelection();
        rotationDial.enabled = true;
        rotationDial.gameObject.SetLayerAs(Keyword.interactionExamining);
    }
    protected override void KeepUnselection()
    {
        base.KeepUnselection();
        rotationDial.enabled = false;
    }
    public RotationDial RotationDial { get => rotationDial; }
}