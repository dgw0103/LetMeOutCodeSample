using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExaminingObjectLock : MonoBehaviour, ILock
{
    [SerializeField] private Examining @lock;



    public bool IsLocked
    {
        get
        {
            try
            {
                return @lock.gameObject.activeSelf;
            }
            catch (System.Exception)
            {
                return @lock.gameObject;
            }
        }
        set => @lock.gameObject.SetActive(value);
    }
    public void RattleByInteraction()
    {
        @lock.OnInteractionDown();
    }
}