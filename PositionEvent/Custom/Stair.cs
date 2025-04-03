using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Stair : Area
{
    [SerializeField] private StepSound woodCreakSound;



    public override IEnumerable<IEnvironmentVisitor> OnEnter(Character character)
    {
        yield return new StepSoundCreator(character, woodCreakSound);
    }
}