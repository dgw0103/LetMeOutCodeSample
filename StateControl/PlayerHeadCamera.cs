using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;
using System;
using UnityEngine.Audio;

[RequireComponent(typeof(Camera), typeof(AudioListener))]
public class PlayerHeadCamera : MonoBehaviour, IInitializer
{
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private Transform headBone;
    private FirstPersonTargeting<InteractionObject> firstPersonTargeting;
    private new Camera camera;
    private new Light light;
    private HDAdditionalLightData hDAdditionalLightData;
    private HDAdditionalCameraData hDAdditionalCameraData;
    private Vector3 originalPosition;
    private Quaternion originalRotation;



    public void Init()
    {
        TryGetComponent(out camera);
        TryGetComponent(out light);
        TryGetComponent(out hDAdditionalLightData);
        TryGetComponent(out hDAdditionalCameraData);
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }



    public Camera Camera { get => camera; }
    public void InitTargetingDirection()
    {
        firstPersonTargeting.SetRayDirection(() => StageManager.Instance.Player.HeadRotation.HeadForward);
    }
    public FirstPersonTargeting<InteractionObject> FirstPersonTargeting { get => firstPersonTargeting; set => firstPersonTargeting = value; }
    public void UpdateInteractionObjectBeingLookingAt()
    {
        if (KeyInput.IsLocked(InputType.InteractionOrItemUsing) == false)
        {
            firstPersonTargeting.UpdateTarget();
        }
    }
    public void SetLayerMaskWhenExamine()
    {
        SetInteractionLayer(Keyword.interactionExamining);
    }
    public void SetLayerMaskAsHolding()
    {
        SetInteractionLayer(Keyword.holding);
    }
    public void RestoreLayerMask()
    {
        SetInteractionLayer(Keyword.interaction, Keyword.examining);
    }
    private void SetInteractionLayer(params string[] layerNames)
    {
        firstPersonTargeting.InteractionLayerMask = 0;
        for (int i = 0; i < layerNames.Length; i++)
        {
            firstPersonTargeting.InteractionLayerMask |= (1 << LayerMask.NameToLayer(layerNames[i]));
        }
    }
    public void TurnOn(LightState lightState)
    {
        light.enabled = true;
        lightState.SetLightSetting(hDAdditionalLightData);
    }
    public void TurnOff()
    {
        light.enabled = false;
    }
    public void TurnOnAsDefaultLayer(LightState lightState)
    {
        TurnOn(lightState);
        hDAdditionalLightData.lightlayersMask = LightLayerEnum.LightLayerDefault;
    }
    public void SetLightLayer(int layerMask)
    {
        hDAdditionalLightData.lightlayersMask = (LightLayerEnum)layerMask;
    }
    public void InitLightLayer()
    {
        hDAdditionalLightData.lightlayersMask = LightLayerEnum.LightLayerDefault | LightLayerEnum.LightLayer1;
    }
    public void RestoreCameraParent()
    {
        camera.transform.SetParent(headBone);
        camera.transform.localPosition = originalPosition;
        camera.transform.localRotation = originalRotation;
        camera.transform.localScale = Vector3.one;
    }
    public void SetParentTo(Transform parent)
    {
        camera.transform.SetParent(parent);
    }
    public void SetParentTo(Transform parent, Vector3 localPosition, Quaternion localRotation)
    {
        SetParentTo(parent);
        camera.transform.ResetLocalSpace();
        camera.transform.localPosition = localPosition;
        camera.transform.localRotation = localRotation;
    }
    public void InitCameraFOV()
    {
        camera.fieldOfView = 60f;
    }
    public IEnumerator IncreaseFOV(float speed, float until)
    {
        while (camera.fieldOfView < until)
        {
            camera.fieldOfView += speed;
            yield return null;
        }
        camera.fieldOfView = until;
    }
    public IEnumerator DecreaseFOV(float speed, float until)
    {
        while (camera.fieldOfView > until)
        {
            camera.fieldOfView -= speed;
            yield return null;
        }
        camera.fieldOfView = until;
    }
    public void SetLightVisibilityByPreference()
    {
        //hDAdditionalCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.AtmosphericScattering,
        //    GameManager.Instance.PreferencesData.IsLightVisiablity);
    }
    public void TurnOffCamera()
    {
        StageManager.Instance.PostEffect.TurnOffCamera();
    }
    public void StartSettingRayDirectionAsPointer()
    {
        StartCoroutine(nameof(SetRaydirectionAsPointer));
    }
    public void StopSettingRayDirectionAsPointer()
    {
        StopCoroutine(nameof(SetRaydirectionAsPointer));
        InitTargetingDirection();
    }
    private IEnumerator SetRaydirectionAsPointer()
    {
        while (this)
        {
            firstPersonTargeting.SetRayDirection(GetCameraToPointerDirection);
            yield return null;
        }





        Vector3 GetCameraToPointerDirection()
        {
            return camera.ScreenPointToRay(Pointer.current.position.ReadValue()).direction;
        }
    }
    public void SetPositionToOriginal()
    {
        transform.localPosition = originalPosition;
    }
    [ContextMenu(nameof(LogInteractionLayerMask))]
    public void LogInteractionLayerMask()
    {
        Debug.Log(firstPersonTargeting.InteractionLayerMask.value);
    }
    [ContextMenu(nameof(LogBlockingLayerMask))]
    public void LogBlockingLayerMask()
    {
        Debug.Log(firstPersonTargeting.BlockingLayerMask.value);
    }
    public float InteractableRange { get => interactionRange; }
    [ContextMenu(nameof(LogTarget))]
    private void LogTarget()
    {
        Debug.Log(firstPersonTargeting.Target.gameObject.name);
    }
}