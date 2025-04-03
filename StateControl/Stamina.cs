using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using HoJin;

public partial class Player
{
    [SerializeField] private Scrollbar staminaBar;
    [SerializeField] private AudioSource tiredSound;
    [SerializeField] private AudioSource recoveredSound;
    [SerializeField] private StaminaData staminaData;
    [SerializeField] private AudioSource breathSound;
    [SerializeField] private float staminaDelta;
    [SerializeField] private float stamina = 1f;
    private WaitForSeconds tiredTime = new WaitForSeconds(2f);
    private CanvasRenderer staminaBarCanvasRenderer;
    private float previousTime = 0f;
    public const float originalMovingSpeed = 1f;
    public const float movingSpeedInTiredState = 0.3f;
    public static int staminaHash = Animator.StringToHash("Stamina");
    public static int beTiredTriggerHash = Animator.StringToHash("BeTired");



    public void AwakeStamina()
    {
        staminaDelta = staminaData.originalStaminaDelta;
        staminaBarCanvasRenderer = staminaBar.targetGraphic.GetComponent<CanvasRenderer>();
    }
    public void UpdateStamina()
    {
        if (Time.timeScale > 0)
        {
            stamina += staminaDelta * Time.deltaTime * 100f;
            stamina = Mathf.Clamp01(stamina);
            staminaBar.size = stamina;
            staminaBar.targetGraphic.color = new Color(1f, stamina, stamina);



            if (stamina <= 0 && playerStaminaState.State.Equals(PlayerStaminaState.Default))
            {
                StartBeingTiredState();
            }
            else if (stamina > 0.5f && playerStaminaState.State.Equals(PlayerStaminaState.Recovering))
            {
                BeDefaultState();
            }



            #region stamina bar blink and breathing
            float currentTime = Time.time;



            if (stamina < 0.5f || (stamina >= 0.5f && currentTime.Equals(0f) == false && staminaBarCanvasRenderer.GetAlpha().RoundAt(2) < 1f))
            {
                staminaBar.targetGraphic.CrossFadeAlpha((Mathf.Cos(10f * currentTime) + 2f) / 3f, 0f, false);
            }
            if (stamina < 0.5f)
            {
                
                if (breathSound.isPlaying == false)
                {
                    breathSound.volume = 1f;
                    breathSound.Play();
                }
            }
            else
            {
                if (breathSound.isPlaying)
                {
                    breathSound.volume -= Time.deltaTime;
                    if (breathSound.volume.Equals(0))
                    {
                        breathSound.Stop();
                    }
                }
            }
            previousTime = currentTime;
            #endregion
        }
    }
    public void StopCheckingAllStaminaConsuming()
    {
        StopCoroutine(nameof(CheckAllStaminaConsuming));
    }
    private IEnumerator CheckAllStaminaConsuming()
    {
        while (this)
        {
            if (stamina <= 0)
            {
                StartBeingTiredState();
                StopCheckingAllStaminaConsuming();
            }
            yield return null;
        }
    }
    private void StartBeingTiredState()
    {
        StartCoroutine(nameof(BeTiredState));
    }
    private IEnumerator BeTiredState()
    {
        Run(false);
        PlayerStaminaState = PlayerStaminaState.Tired;
        tiredSound.Play();
        firstPersonController.MoveSpeed *= 0.1f;
        staminaDelta = staminaData.staminaDeltaInTiredState;
        yield return tiredTime;

        playerStaminaState.State = PlayerStaminaState.Recovering;
        firstPersonController.MoveSpeed *= 10f;
    }
    private void BeDefaultState()
    {
        recoveredSound.Play();
        staminaDelta = staminaData.originalStaminaDelta;
        playerStaminaState.State = PlayerStaminaState.Default;
    }
    [ContextMenu(nameof(LogStaminaState))]
    private void LogStaminaState()
    {
        Debug.Log($"stamina : {stamina}{Environment.NewLine}stamina delta : {staminaDelta}");
    }
}