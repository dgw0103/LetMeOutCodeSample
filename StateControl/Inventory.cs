using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LetMeOut.GameScene;
using System.Linq;
using HoJin;

public class Inventory : MonoBehaviour
{
    private List<Item> items = new List<Item>();
    private UISelector uISelector;
    public static readonly int inventoryCapacity = 8;



    private void Start()
    {
        StageManager.Instance.UIs.InventoryAnimator.TryGetComponent(out uISelector);
        StateMachineBehaviour<Inventory>.DefineComponentInStateMachineBehaviour(StageManager.Instance.UIs.InventoryAnimator, this);
        UpdateInventoryUIs();
    }



    public List<Item> Items { get => items; }
    public void OpenInventory()
    {
        StageManager.Instance.Player.Stop();
        StageManager.Instance.Player.FixRotation();
        StageManager.Instance.UIs.PlayInventoryAnimation("On");
        KeyInput.PushAsInventory();
    }
    public void CloseInventory()
    {
        StageManager.Instance.Player.Resume();
        StageManager.Instance.UIs.PlayInventoryAnimation("Off");
        KeyInput.PopKeyLockStack();
    }
    public void PutIn(Item item)
    {
        item.transform.SetParent(transform);
        items.Add(item);
        UpdateInventoryUIs();
    }
    public void PutOut(Item item)
    {
        items.Remove(item);
        UpdateInventoryUIs();
    }
    public void UpdateInventoryUIs()
    {
        if (StageManager.Instance.Player.Flashlight.HasThis)
        {
            StageManager.Instance.UIs.FlashlightImage.SetActive(true);
        }
        for (int i = 0; i < inventoryCapacity; i++)
        {
            if (items.Count > i)
            {
                StageManager.Instance.UIs.SetInventoryImage(items[i].Icon, i);
            }
            else
            {
                StageManager.Instance.UIs.SetActiveFalseInventoryImage(i);
            }
        }
    }
    public bool HasItem(Type type)
    {
        return items.Any((x) => (x.GetType() == type));
    }
    public bool HasItem(IEnumerable<Item> items)
    {
        bool has = false;



        try
        {
            foreach (var item in items)
            {
                has = has || this.items.Contains(item);
            }

            return has;
        }
        catch (Exception)
        {
            return true;
        }
    }
    public Item Use(IEnumerable<Item> usingItems)
    {
        Item usingItem = items.First((x) => usingItems.Contains(x));



        PutOut(usingItem);

        return usingItem;
    }
    public void Clear()
    {
        List<Item> itemsToDestroy = new List<Item>(inventoryCapacity);
        foreach (var item in items)
        {
            if (item.Disappear())
            {
                itemsToDestroy.Add(item);
            }
        }
        foreach (var item in itemsToDestroy)
        {
            items.Remove(item);
        }

        UpdateInventoryUIs();
    }
    public void OnInventoryOnStateEnter()
    {
        DeviceManager.OnChangedToGamepad += uISelector.StartSelectingFirstSelectedObject;
        DeviceManager.OnChangedToKeyboardAndMouse += uISelector.UnselectSelectedObject;
        if (GameManager.Instance.DeviceManager.CurrentControlScheme.Equals(DeviceManager.gamepadSchemeName))
        {
            uISelector.StartSelectingFirstSelectedObject();
        }
    }
    public void OnInventoryOffStateEnter()
    {
        DeviceManager.OnChangedToGamepad -= uISelector.StartSelectingFirstSelectedObject;
        DeviceManager.OnChangedToKeyboardAndMouse -= uISelector.UnselectSelectedObject;
    }
}