using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoJin;

public class FlashlightDetector : MonoBehaviour
{
    [SerializeField] private GameObject flashlightModeColliderParent;
    [SerializeField] private Player player;
    private Flashlight flashlight;
    private int currentFlashlightState;



    private void Awake()
    {
        TryGetComponent(out flashlight);
        flashlight.OnSwitch += (x) =>
        {
            currentFlashlightState = x;
            Activate(x);
        };
        player.OnResurrect += ActivateByCurrentFlashlightState;
    }



    public void ActivateByCurrentFlashlightState()
    {
        if (flashlightModeColliderParent.activeSelf == false)
        {
            SetActive = true;
        }
        Activate(currentFlashlightState);
    }
    private void Activate(int index)
    {
        for (int i = 0; i < flashlightModeColliderParent.transform.childCount; i++)
        {
            flashlightModeColliderParent.transform.GetChild(i).gameObject.SetActive(false);
        }

        if (currentFlashlightState > Flashlight.turnedOffLightStateIndex)
        {
            flashlightModeColliderParent.transform.GetChild(index).gameObject.SetActive(true);
        }
    }
    public void AddSightDetectorToEachMode(Action onLookAt)
    {
        for (int i = 0; i < flashlightModeColliderParent.transform.childCount; i++)
        {
            if (flashlightModeColliderParent.transform.GetChild(i).TryGetComponent(out EnemySight sightDetector))
            {
                Destroy(sightDetector);
            }
            sightDetector = flashlightModeColliderParent.transform.GetChild(i).gameObject.AddComponent<EnemySight>();
            sightDetector.Eye = transform;
            sightDetector.WhoamI = EnemySight.KindOfThis.Player;
            sightDetector.OnRayHitted += onLookAt;

            if (flashlightModeColliderParent.transform.GetChild(i).gameObject.activeSelf)
            {
                Collider collider = sightDetector.GetComponent<Collider>();
                collider.enabled = false;
                collider.enabled = true;
            }
        }
    }
    public EnemySight[] SightDetectorsEachMode { get => flashlightModeColliderParent.GetComponentsInChildren<EnemySight>(true); }
    public bool SetActive
    {
        set
        {
            flashlightModeColliderParent.SetActive(value);
        }
    }
}