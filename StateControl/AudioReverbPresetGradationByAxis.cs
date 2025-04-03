using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin;
using System.Reflection;
using System.Linq;
using System;

public class AudioReverbPresetGradationByAxis : MonoBehaviour
{
    [SerializeField] [Range(-10000f, 2000f)] private float from;
    [SerializeField] [Range(-10000f, 2000f)] private float to;
    [SerializeField] private AxisType gradationDirection;
    private new Collider collider;
    private Coroutine propertySettingCoroutine;
    private Vector3 lastPlayerPosition;
    private float playerPositionWeight;



    private void Awake()
    {
        TryGetComponent(out collider);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            propertySettingCoroutine = StartCoroutine(SetProperties(player.transform));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            StopCoroutine(propertySettingCoroutine);
            if (Vector3.Distance(player.transform.position, lastPlayerPosition).Equals(0) == false)
            {
                GameManager.Instance.AudioMixerReverbProperties = new AudioMixerReverbProperties(GameManager.Instance.AudioMixerReverbProperties)
                {
                    reverb = from
                };
            }
            else
            {
                if (Mathf.Abs(1f - playerPositionWeight) < playerPositionWeight)
                {
                    GameManager.Instance.AudioMixerReverbProperties = new AudioMixerReverbProperties(GameManager.Instance.AudioMixerReverbProperties)
                    {
                        reverb = to
                    };
                }
                else if (Mathf.Abs(1f - playerPositionWeight) >= playerPositionWeight)
                {
                    GameManager.Instance.AudioMixerReverbProperties = new AudioMixerReverbProperties(GameManager.Instance.AudioMixerReverbProperties)
                    {
                        reverb = from
                    };
                }
            }
        }
    }



    private IEnumerator SetProperties(Transform player)
    {
        while (this)
        {
            playerPositionWeight = Mathf.Clamp01((DistanceFromCenter(player) / Length) + 0.5f);
            GameManager.Instance.ReverbLevel = Mathf.Lerp(from, to, playerPositionWeight);
            lastPlayerPosition = player.position;
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