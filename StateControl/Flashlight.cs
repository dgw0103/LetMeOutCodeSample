using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.HighDefinition;
using HoJin;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private LightState[] modes;
    [SerializeField] private AudioSource switchingSound;
    [SerializeField] private bool hasThis;
    private MeshRenderer meshRenderer;
    private new Light light;
    private HDAdditionalLightData hDAdditionalLightData;
    private int currentLightState;
    public event Action<int> OnSwitch;
    private readonly int lightSiblingIndex = 0;
    public const int turnedOffLightStateIndex = -1;



    private void Awake()
    {
        TryGetComponent(out meshRenderer);
        transform.GetChild(lightSiblingIndex).TryGetComponent(out light);
        light.TryGetComponent(out hDAdditionalLightData);
        SetCurrentLightStateAsTurnedOff();
    }
    private void Start()
    {
        if (hasThis == true)
        {
            FlashlightObjectSetActive = true;
        }
        else
        {
            FlashlightObjectSetActive = false;
        }
    }



    public bool HasThis { get => hasThis; set => hasThis = value; }
    public bool FlashlightObjectSetActive { set => meshRenderer.enabled = value; }
    public bool IsTurnedOn { get => currentLightState > turnedOffLightStateIndex; }
    public HDAdditionalLightData HDAdditionalLightData { get => hDAdditionalLightData; }
    public void ChangeLightState()
    {
        if (hasThis == true)
        {
            switchingSound.Play();
            currentLightState++;
            SetLightStateAsCurrentState();
            OnSwitch?.Invoke(currentLightState);
        }
    }
    public IEnumerator BlinkCoroutine()
    {
        int blinkCount = 10;
        int count = 0;
        float reducedIntensity;

        hDAdditionalLightData.intensity *= 0.1f;
        reducedIntensity = hDAdditionalLightData.intensity;
        do
        {
            hDAdditionalLightData.intensity = 0;
            yield return new WaitForSeconds(0.05f);
            hDAdditionalLightData.intensity = reducedIntensity;
            yield return new WaitForSeconds(0.05f);
            count++;
        } while (count <= blinkCount);

        SetLightStateAsCurrentState();
    }
    public IEnumerator BlinkRandomlyCoroutine()
    {
        int blinkCount = 10;
        int count = 0;
        float reducedIntensity;



        hDAdditionalLightData.intensity *= 0.1f;
        reducedIntensity = hDAdditionalLightData.intensity;
        do
        {
            hDAdditionalLightData.intensity = 0;
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.5f));
            hDAdditionalLightData.intensity = reducedIntensity;
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.5f));
            count++;
        } while (count <= blinkCount);

        SetLightStateAsCurrentState();
    }
    public void SetLightStateAsCurrentState()
    {
        try
        {
            modes[currentLightState].SetLightSetting(hDAdditionalLightData);
        }
        catch (IndexOutOfRangeException)
        {
            SetCurrentLightStateAsTurnedOff();
        }

        TurnByLightState();



        void TurnByLightState()
        {
            if (currentLightState > -1)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }
    }
    public void TurnOff()
    {
        light.enabled = false;
    }
    public void TurnOn()
    {
        if (hasThis == true)
        {
            light.enabled = true;
        }
    }
    public void HandsIn()
    {
        hasThis = true;
        FlashlightObjectSetActive = true;
    }
    public void TurnOnIfThisHaveTurnedOff()
    {
        if (IsTurnedOn == false)
        {
            TurnOn();
            currentLightState = 0;
            SetLightStateAsCurrentState();
        }
    }
    private void SetCurrentLightStateAsTurnedOff()
    {
        currentLightState = turnedOffLightStateIndex;
    }
    public void SetFlashlightResolutionByPreference()
    {
        hDAdditionalLightData.shadowResolution.level = GameManager.Instance.PreferencesData.FlashlightShadowResolution;
    }
    public float LightRadiusInScreenByMode
    {
        get
        {
            switch (currentLightState)
            {
                case turnedOffLightStateIndex:
                    return 0;
                case 0:
                    return Screen.currentResolution.height * 0.5f;
                case 1:
                    return Screen.currentResolution.height * 0.5f * 0.8f;
                default:
                    return 0;
            }
        }
    }
    public void Disappear()
    {
        meshRenderer.enabled = false;
        TurnOff();
    }
    public void Appear()
    {
        hasThis = true;
        meshRenderer.enabled = true;
        if (currentLightState.Equals(turnedOffLightStateIndex) == false)
        {
            TurnOn();
        }
    }
}