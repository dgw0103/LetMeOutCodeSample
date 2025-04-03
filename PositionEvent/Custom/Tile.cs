using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile : Area
{
    [SerializeField] private StepSound tileSound;
    [SerializeField] private StepSound juliaTileSound;



    public override IEnumerable<IEnvironmentVisitor> OnEnter(Character character)
    {
        Type characterType = character.GetType();



        if (characterType.Equals(typeof(Julia)))
        {
            yield return new DefaultStepSoundReplacer(character, juliaTileSound);
        }
        else
        {
            yield return new StepSoundCreator(character, tileSound);
        }
    }
}
