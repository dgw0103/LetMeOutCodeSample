public class TriedUsingObject : UsedObject
{
    private ITriedUsing triedUsing;



    protected new void Awake()
    {
        base.Awake();
        TryGetComponent(out triedUsing);
    }



    protected override void OnItemDestroying()
    {
        if (triedUsing.CanBeUsed)
        {
            base.OnItemDestroying();
            triedUsing.OnSucceedToUsing();
        }
        else
        {
            triedUsing.OnFailToUsing();
        }
    }
}