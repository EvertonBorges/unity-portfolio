using UnityEngine;

public class DialogTrigger : MonoBehaviour
{

    [SerializeField] private SO_Dialogs _dialog;

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController _))
            return;

        Manager_Events.Dialog.ShowDialog.Notify(_dialog);
    }

}
