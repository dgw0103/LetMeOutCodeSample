using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin;
using System.Reflection;
using System.Linq;
using System;

public class CryingSoundManager : MonoBehaviour
{
    [SerializeField] private float from;
    [SerializeField] private float to;
    [SerializeField] private AudioLowPassFilter audioLowPassFilter;
    [SerializeField] private AxisType gradationDirection;
    [SerializeField] private Door door;
    private new Collider collider;
    private float playerPositionWeight;



    private void Awake()
    {
        TryGetComponent(out collider);
        door.OnInteraction += SetLowPassWhenInteractionDoor;
    }
    private void Update()
    {
        if (door.DoorState.Equals(DoorState.Closed))
        {
            Transform player = StageManager.Instance.Player.transform;



            playerPositionWeight = Mathf.Clamp01((DistanceFromCenter(player) / Length) + 0.5f);
            audioLowPassFilter.cutoffFrequency = Mathf.Lerp(from, to, playerPositionWeight);
        }
    }



    private void SetLowPassWhenInteractionDoor()
    {
        StopAllCoroutines();
        
        if (door.DoorState.Equals(DoorState.Opening))
        {
            StartCoroutine(nameof(IncreaseLowPassFrequency));
        }
        else if (door.DoorState.Equals(DoorState.Closing))
        {
            StartCoroutine(nameof(ReduceLowPassFrequency));
        }
    }
    private IEnumerator IncreaseLowPassFrequency()
    {
        float weight = 0;
        float originalAudioLowPassFilterCutoffFrequency = audioLowPassFilter.cutoffFrequency;



        while (weight <= 1f)
        {
            audioLowPassFilter.cutoffFrequency = Mathf.Lerp(originalAudioLowPassFilterCutoffFrequency, 20000f, weight);
            weight += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator ReduceLowPassFrequency()
    {
        float weight = 0;
        float targetAudioLowPassFilterCutoffFrequency =
            Mathf.Lerp(from, to, Mathf.Clamp01((DistanceFromCenter(StageManager.Instance.Player.transform) / Length) + 0.5f));



        while (weight <= 1f)
        {
            audioLowPassFilter.cutoffFrequency = Mathf.Lerp(20000f, targetAudioLowPassFilterCutoffFrequency, weight);
            weight += Time.deltaTime;
            yield return null;
        }
    }
    private float Length
    {
        get => Mathf.Abs(Vector3.Dot(gradationDirection.ToUnitVector(), Vector3.Scale(collider.bounds.size, collider.transform.WorldScale())));
    }
    private float DistanceFromCenter(Transform player)
    {
        return Vector3.Dot(gradationDirection.ToUnitVector(), player.position - collider.bounds.center);
    }
}
