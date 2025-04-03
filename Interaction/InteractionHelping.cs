using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin;
using System;

public class InteractionHelping : MonoBehaviour, IInteractionObjectTargeting
{
    [SerializeField] private bool isAppearInteractionHelpingUI = true;



    public virtual void OnLookAt()
    {
        if (isAppearInteractionHelpingUI)
        {
            StageManager.Instance.UIs.AppearInteractionDownHelping();
        }
    }
    public virtual void OnNoLookAt()
    {
        if (isAppearInteractionHelpingUI)
        {
            StageManager.Instance.UIs.DisappearInteractionDownHelping();
        }
    }
}