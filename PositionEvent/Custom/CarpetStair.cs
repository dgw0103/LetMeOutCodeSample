using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarpetStair : Area
{
    [SerializeField] private StepSound carpetSound;
    [SerializeField] private StepSound woodCreakSound;



    public override IEnumerable<IEnvironmentVisitor> OnEnter(Character character)
    {
        yield return new DefaultStepSoundVolumeMultiplier(character, 0.2f);
        yield return new StepSoundCreator(character, carpetSound);
        yield return new StepSoundCreator(character, woodCreakSound);
    }
}
