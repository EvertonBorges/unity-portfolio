using UnityEngine;

public class Platform : MonoBehaviour
{

    [SerializeField] private Interactable _interactable;
    [SerializeField] private GameObject _hint;

    void Awake()
    {
        HideHint();
    }

    public void Interact()
    {
        _interactable.Interact();
    }

    public void InverseInteract()
    {
        _interactable.InverseInteract();
    }

    protected virtual void ShowHint()
    {
        _hint.SetActive(true);
    }

    protected virtual void HideHint()
    {
        _hint.SetActive(false);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player))
            ShowHint();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Player))
            HideHint();
    }

}
