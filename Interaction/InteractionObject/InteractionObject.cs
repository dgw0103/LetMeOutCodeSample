using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using HoJin;
using UnityEngine.InputSystem;

public class InteractionObject : MonoBehaviour, ITargetable
{
    [HideInInspector] public int interactionObjectTargetingIndex = 0;
    private IInteractionDown interactionDown;
    private IInteractionObjectTargeting interactionObjectTargeting;
    public const string interactionName = "Interaction";



    protected void Reset()
    {
        gameObject.layer = LayerMask.NameToLayer(Keyword.interaction);
    }
    protected void Awake()
    {
        interactionObjectTargeting = GetComponents<IInteractionObjectTargeting>()[interactionObjectTargetingIndex];
        TryGetComponent(out interactionDown);
    }



    public virtual void OnInteractionDown()
    {
        interactionDown?.OnInteractionDown();
    }
    public virtual void OnLookAt()
    {
        interactionObjectTargeting.OnLookAt();
    }
    public virtual void OnNoLookAt()
    {
        interactionObjectTargeting.OnNoLookAt();
    }
}