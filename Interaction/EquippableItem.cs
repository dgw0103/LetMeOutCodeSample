using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EquippableItem : Item
{
    [SerializeField] private Equipment equippingItem;
    private Equipment instantiated;



    [ContextMenu((nameof(EquippableItem) + nameof(GetItem)))]
    public override void GetItem()
    {
        base.GetItem();
        instantiated = Instantiate(equippingItem.gameObject, StageManager.Instance.Player.Camera.transform).GetComponent<Equipment>();
        StageManager.Instance.Player.Equipments.Add(instantiated);
    }
    public override void DestroyFromInventory()
    {
        base.DestroyFromInventory();
        StageManager.Instance.Player.Equipments.Remove(instantiated);
        Destroy(instantiated.gameObject);
    }
    protected Equipment Equipment
    {
        get
        {
            Equipment equipment = instantiated;

            if (instantiated is null)
            {
                throw new NullReferenceException($"{equippingItem.name} is not instantiated yet.");
            }

            return equipment;
        }
    }
}