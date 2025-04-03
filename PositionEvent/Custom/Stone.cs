using System.Collections.Generic;
using UnityEngine;

public class Stone : Area
{
    [SerializeField] private StepSound stoneSound;



    public override IEnumerable<IEnvironmentVisitor> OnEnter(Character character)
    {
        yield return new StepSoundCreator(character, stoneSound);
    }
}
