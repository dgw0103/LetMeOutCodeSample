using UnityEngine;

public class DefaultStepSoundVolumeMultiplier : IEnvironmentVisitor
{
    private float value;



    public DefaultStepSoundVolumeMultiplier(Character character, float value)
    {
        this.value = value;
        character.StepSoundPlayer.DefaultStepSound.MultiplyVolume(value);
    }



    public void OnExit(Character character)
    {
        character.StepSoundPlayer.DefaultStepSound.MultiplyVolume(1f / value);
    }
}