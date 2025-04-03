using System.Collections.Generic;
using UnityEngine;

public class Bone : Area
{
    [SerializeField] private StepSound boneSound;



    public override IEnumerable<IEnvironmentVisitor> OnEnter(Character character)
    {
        yield return new StepSoundCreator(character, boneSound);
    }
}