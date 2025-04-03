public class StepSoundVolumeMultiplier : IEnvironmentVisitor
{
    private float value;



    public StepSoundVolumeMultiplier(Character character, float value)
    {
        this.value = value;
        for (int i = 0; i < character.StepSoundPlayer.StepSounds.Count; i++)
        {
            character.StepSoundPlayer.StepSounds[i].MultiplyVolume(value);
        }
    }



    public void OnExit(Character character)
    {
        for (int i = 0; i < character.StepSoundPlayer.StepSounds.Count; i++)
        {
            character.StepSoundPlayer.StepSounds[i].MultiplyVolume(1f / value);
        }
    }
}
