public interface ITriedUsing
{
    public bool CanBeUsed { get; }
    public void OnSucceedToUsing();
    public void OnFailToUsing();
}