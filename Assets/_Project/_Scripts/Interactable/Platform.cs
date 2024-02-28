using UnityEngine;

public class Platform : MonoBehaviour, IInteractable
{

    [SerializeField] private Interactable _interactable;

    public void Interact()
    {
        _interactable.Interact();
    }

    public void InverseInteract()
    {
        _interactable.InverseInteract();
    }

}
