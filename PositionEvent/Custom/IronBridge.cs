using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronBridge : Area
{
    [SerializeField] private StepSound ironSound;



    public override IEnumerable<IEnvironmentVisitor> OnEnter(Character character)
    {
        yield return new StepSoundCreator(character, ironSound);
    }
}