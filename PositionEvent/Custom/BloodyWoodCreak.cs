using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BloodyWoodCreak : Area
{
    [SerializeField] private StepSound woodCreakSound;
    [SerializeField] private StepSound bloodySound;



    public override IEnumerable<IEnvironmentVisitor> OnEnter(Character character)
    {
        yield return new StepSoundCreator(character, woodCreakSound);
        yield return new StepSoundCreator(character, bloodySound);
    }
}