using System.Collections.Generic;
using UnityEngine;

public class Carpet : Area
{
    [SerializeField] private StepSound carpetSound;



    public override IEnumerable<IEnvironmentVisitor> OnEnter(Character character)
    {
        yield return new DefaultStepSoundVolumeMultiplier(character, 0.01f);
        yield return new StepSoundCreator(character, carpetSound);
    }
}
