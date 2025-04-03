using HoJin;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class UsedObject : InteractionObject, IAfterEnemyAttack
{
    [SerializeField] private bool isUsingItemOneOff = true;
    [SerializeField] private bool isDisappearImmediatelyAfterUse = true;
    [SerializeField] private bool isStoppedUsingAnimationWhenAttacked = true;
    [SerializeField] private List<Item> usingItems = new List<Item>(3);
    private IUsed used;
    private IUsingAnimationPlayer usingAnimationPlayer;
    public event Action OnUsed;



    protected new void Awake()
    {
        base.Awake();
        TryGetComponent(out used);
        if (TryGetComponent(out usingAnimationPlayer) == false)
        {
            usingAnimationPlayer = gameObject.AddComponent<SelfPlayer>();
        }
    }



    private bool HasItem { get => StageManager.Instance.Player.Inventory.HasItem(usingItems); }
    public override void OnInteractionDown()
    {
        if (HasItem)
        {
            usingAnimationPlayer.PlayAnimation();
            StartCoroutine(nameof(ProceedOnAfterUsing));
            gameObject.SetLayerAs(Keyword.defaultText);
            used.OnBeUsed();
            if (isDisappearImmediatelyAfterUse)
            {
                OnItemDestroying();
            }
            OnUsed?.Invoke();
        }
        else
        {
            base.OnInteractionDown();
        }
    }
    private IEnumerator ProceedOnAfterUsing()
    {
        yield return usingAnimationPlayer.DuringAnimationPlaying;
        OnAfterUsing();
    }
    private void OnAfterUsing()
    {
        if (isDisappearImmediatelyAfterUse == false)
        {
            OnItemDestroying();
        }
        gameObject.SetLayerAs(Keyword.interaction);
        used.OnAfterUsing();
    }
    protected virtual void OnItemDestroying()
    {
        if (isUsingItemOneOff)
        {
            DestroyForefirstItem();
        }
    }
    public override void OnLookAt()
    {
        base.OnLookAt();
        if (HasItem)
        {
            //StageManager.Instance.UIs.InteractionDownHelpingUIs.SetActive(false);
            //StageManager.Instance.UIs.ItemUsingHelpingUIs.SetActive(true);
        }
    }
    public override void OnNoLookAt()
    {
        base.OnNoLookAt();
        //StageManager.Instance.UIs.ItemUsingHelpingUIs.SetActive(false);
    }
    private void DestroyForefirstItem()
    {
        Selection.UnselectAll();
        Item usingItem = StageManager.Instance.Player.Inventory.Use(usingItems);
        usingItems.Remove(usingItem);
        Destroy(usingItem.gameObject);
    }
    public void OnEnemyAttack()
    {
        if (isStoppedUsingAnimationWhenAttacked)
        {
            StopAllCoroutines();
            gameObject.SetLayerAs(Keyword.interaction);
        }
    }
    public List<Item> UsingItems { get => usingItems; }
}