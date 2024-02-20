using UnityEngine;

public class DialogTrigger : MonoBehaviour
{

    [SerializeField] private string[] _dialogs;

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController _))
            return;

        Manager_Events.Dialog.ShowDialog.Notify(_dialogs);
    }

}
