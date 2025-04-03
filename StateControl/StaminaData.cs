using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(StaminaData), menuName = nameof(StaminaData))]
public class StaminaData : ScriptableObject
{
    public float maximumStamina = 1f;
    public float minimumStamina = 0f;
    public float originalStaminaDelta = 0.003f;
    public float staminaDeltaInRunningState = -0.001f;
    public float staminaDeltaInTiredState = 0.0005f;
    public float staminaDeltaInRecoveringState = 0.001f;
}
