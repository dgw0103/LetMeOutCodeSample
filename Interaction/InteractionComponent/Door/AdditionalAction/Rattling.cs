using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoJin;

[Serializable]
public class Rattling
{
    [SerializeField] private AudioSource rattlingSound;
    [SerializeField] private bool isIgnorePreviousSound = true;
    [SerializeField] public float rattlingTime = 0.01f;
    public static readonly int rattleTriggerHash = Animator.StringToHash("Rattle");
    public const string doorKeyName = "Door";



    public void RattleByInteraction(Door door)
    {
        StageManager.Instance.UIs.SystemMessagePlayer.Stop();
        StageManager.Instance.UIs.SystemMessagePlayer.PlayOneByOne(
            new SystemMessagePlaying(new TranslatableSystemMessageData(TranslatorKeywordType.Interaction, doorKeyName)));
        Rattle(door);
    }
    public void Rattle(Door door)
    {
        if (isIgnorePreviousSound)
        {
            rattlingSound.PlayOneShotAudioClipOfAudioSource();
        }
        else
        {
            rattlingSound.Play();
        }
        door.Rattle(rattlingTime);
    }
}
