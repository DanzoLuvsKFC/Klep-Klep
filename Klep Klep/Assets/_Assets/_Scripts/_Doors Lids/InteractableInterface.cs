using UnityEngine;

public abstract class InteractableInterface : MonoBehaviour
{
    public GameObject _highlight;
    public GameObject _typeUI;
    public void BaseInteract()
    {
        Interact();
    }

    public void IHighlightClass()
    {
        IHighlight();
    }

    protected virtual void Interact() { }
    protected virtual void IHighlight() { }
}
