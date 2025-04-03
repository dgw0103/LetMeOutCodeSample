using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LetMeOut;
using HoJin;
using System;
using UnityEngine.Serialization;

public class Item : Examining
{
    [SerializeField] private Sprite icon;
    [SerializeField] private bool isDestroyedWhenReload;
    private bool isCollected;
    public event Action OnCollected;
    private readonly string getKey = "Get";



    public override void Unselect()
    {
        base.Unselect();
        if (isCollected)
        {
            SetGettingState();
        }
        else
        {
            GetItem();
            AppearGetLog();
            OnCollected?.Invoke();
        }
    }
    public Sprite Icon { get => icon; }
    [ContextMenu(nameof(GetItem))]
    public virtual void GetItem()
    {
        StageManager.Instance.Player.Inventory.PutIn(this);
        SetGettingState();
    }
    private void SetGettingState()
    {
        isCollected = true;
        transform.localPosition = Vector3.forward;
        transform.localRotation = Quaternion.identity;
        gameObject.SetActive(false);
    }
    public void AppearGetLog()
    {
        StageManager.Instance.UIs.SystemMessagePlayer.Stop();
        StageManager.Instance.UIs.SystemMessagePlayer.PlayMultiplesAtOnce(
            new SystemMessagePlaying(new TranslatableSystemMessageData(TranslatorKeywordType.Name, KeyObject.Key)),
            new SystemMessagePlaying(new TranslatableSystemMessageData(TranslatorKeywordType.Default, getKey)));
    }
    public bool Disappear()
    {
        if (isDestroyedWhenReload)
        {
            DestroyFromInventory();
        }

        return isDestroyedWhenReload;
    }
    public virtual void DestroyFromInventory()
    {
        Destroy(gameObject);
    }
    public bool IsCollected { get => isCollected; }
}