using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSoundCreator : IEnvironmentVisitor
{
    private StepSound[] stepSounds;



    public StepSound[] CreatedStepSounds { get => stepSounds; }



    public StepSoundCreator(Character character, params StepSound[] stepSounds)
    {
        List<StepSound> instantiateds = new List<StepSound>();

        for (int i = 0; i < stepSounds.Length; i++)
        {
            instantiateds.Add(character.StepSoundPlayer.CreateStepSound(stepSounds[i]));
        }
        this.stepSounds = new StepSound[instantiateds.Count];
        instantiateds.CopyTo(this.stepSounds);
        SetVolumeByStateOfCharacter();





        void SetVolumeByStateOfCharacter()
        {
            if (character.IsRunningState)
            {
                for (int i = 0; i < instantiateds.Count; i++)
                {
                    Character.AddStepSoundsVolumeAsRunning(instantiateds[i]);
                }
            }
        }
    }



    public void OnExit(Character character)
    {
        character.StepSoundPlayer.RemoveStepSounds(stepSounds);
    }
}
