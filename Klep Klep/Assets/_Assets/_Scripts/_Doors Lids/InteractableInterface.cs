using UnityEngine;

public abstract class InteractableInterface : MonoBehaviour
{
    public void BaseInteract()
    {
        Interact();
    }

    protected virtual void Interact() { }
}
