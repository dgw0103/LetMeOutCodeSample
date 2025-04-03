using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DefaultStepSoundReplacer : IEnvironmentVisitor
{
    private StepSound originalDefaultStepSound;
    private StepSoundCreator stepSoundCreator;



    public DefaultStepSoundReplacer(Character character, StepSound newDefaultStepSound)
    {
        originalDefaultStepSound = character.StepSoundPlayer.DefaultStepSound;
        stepSoundCreator = new StepSoundCreator(character, newDefaultStepSound);
        character.StepSoundPlayer.DefaultStepSound = stepSoundCreator.CreatedStepSounds[0];
        character.StepSoundPlayer.StepSounds.Remove(character.StepSoundPlayer.StepSounds.Last());
    }



    public void OnExit(Character character)
    {
        stepSoundCreator.OnExit(character);
        if (originalDefaultStepSound)
        {
            character.StepSoundPlayer.StepSounds.Insert(0, originalDefaultStepSound);
        }
    }
}
