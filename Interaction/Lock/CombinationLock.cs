
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationLock : Examining
{
    [SerializeField] private Door door;
    [SerializeField] private GameObject latch;
    private new Rigidbody rigidbody;
    private new Animation animation;
    private WaitForSeconds unlockingTime;



    protected new void Awake()
    {
        base.Awake();
        TryGetComponent(out rigidbody);
        TryGetComponent(out animation);
        unlockingTime = new WaitForSeconds(animation.clip.length);
    }



    public override void Select()
    {
        base.Select();
        rigidbody.isKinematic = true;
    }
    public override void Unselect()
    {
        base.Unselect();
        rigidbody.isKinematic = false;
    }
    protected void Unlock()
    {
        animation.Play();
        StartCoroutine(nameof(Unlock_Coroutine));
    }
    private IEnumerator Unlock_Coroutine()
    {
        KeyInput.PushAsAll();
        yield return unlockingTime;
        KeyInput.PopKeyLockStack();
        OnUnselection();
        door.IsLocked = false;
        door.Open();
        if (latch)
        {
            Destroy(latch);
        }
    }
}
