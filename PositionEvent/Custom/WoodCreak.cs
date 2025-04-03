using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodCreak : Area
{
    [SerializeField] private StepSound woodCreakSound;



    public override IEnumerable<IEnvironmentVisitor> OnEnter(Character character)
    {
        yield return new StepSoundCreator(character, woodCreakSound);
    }
}