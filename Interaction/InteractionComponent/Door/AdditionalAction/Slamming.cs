using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Slamming
{
    [SerializeField] private AudioSource slammingSound;
    [SerializeField] private float closingSpeed = 10f;



    public void Slam(Door door)
    {
        door.Close(closingSpeed, false);
        slammingSound.Play();
    }
}
