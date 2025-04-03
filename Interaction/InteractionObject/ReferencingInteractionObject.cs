using UnityEngine;

public class ReferencingInteractionObject : InteractionObject, IInteractionDown, IInteractionObjectTargeting
{
    [SerializeField] private InteractionObject interactionObject;



    public override void OnInteractionDown()
    {
        interactionObject.OnInteractionDown();
    }
    public override void OnLookAt()
    {
        interactionObject.OnLookAt();
    }
    public override void OnNoLookAt()
    {
        interactionObject.OnNoLookAt();
    }
}
