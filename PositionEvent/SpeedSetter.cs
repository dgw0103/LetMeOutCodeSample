using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSetter : IEnvironmentVisitor
{
    private float originalSpeed;



    public SpeedSetter(Character character, float speed)
    {
        originalSpeed = character.Speed;
        character.Speed = speed;
    }



    public void OnExit(Character character)
    {
        character.Speed = originalSpeed;
    }
}