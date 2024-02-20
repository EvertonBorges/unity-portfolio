using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{

    public virtual void Interact() {}
    public virtual void InverseInteract() {}
    
}
