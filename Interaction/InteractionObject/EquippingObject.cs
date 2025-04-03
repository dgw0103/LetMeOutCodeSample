using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class EquippingObject : InteractionObject
{
    [SerializeField] private EquippableItem equippingItem;
    private Selection selection;



    protected new void Awake()
    {
        base.Awake();
        TryGetComponent(out selection);
    }
    private void Start()
    {
        //selection.UnselectionAction.performed += OnUnselection;
    }



    protected virtual void OnUnselection(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.action.IsPressed() == true && KeyInput.IsLocked(InputType.DeselectionOrItemGetting) == false)
        {
            selection.OnUnselection();
            Destroy(gameObject);
            GetItem();
        }
    }
    [ContextMenu(nameof(GetItem))]
    private void GetItem()
    {
        LetMeOut.Equipment.GasMask gasMask = Instantiate(equippingItem.gameObject,
                StageManager.Instance.Player.Camera.transform).GetComponent<LetMeOut.Equipment.GasMask>();
        //LetMeOutSystem.Instance.Player.Equipments.Add(gasMask.GetType(), gasMask);
    }
}
