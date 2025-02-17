using UnityEngine;

public class DialogTrigger : MonoBehaviour
{

    [SerializeField] private bool _onlyOneTime = false;

    [SerializeField] private TriggerType _triggerType = TriggerType.TRIGGER_ENTER;
    [SerializeField] private SO_Dialogs _soDialog;

    [SerializeField] private GameObject _objectToActive;

    void Awake()
    {
#if !UNITY_EDITOR
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());
#endif

        if (_objectToActive != null)
            _objectToActive.SetActive(false);

        if (_onlyOneTime && PlayerPrefsUtils.GetBool(_soDialog.title))
        {
            PostCallback();
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (_triggerType != TriggerType.TRIGGER_ENTER)
            return;

        Trigger(other);
    }

    void OnTriggerStay(Collider other)
    {
        if (_triggerType != TriggerType.TRIGGER_STAY)
            return;

        Trigger(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (_triggerType != TriggerType.TRIGGER_EXIT)
            return;

        Trigger(other);
    }

    private void Trigger(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController _))
            return;

        if (_onlyOneTime)
            PlayerPrefsUtils.SetBool(_soDialog.title, true);

        Manager_Events.Dialog.ShowDialog.Notify(_soDialog, null, PostCallback);

        gameObject.SetActive(false);
    }

    private void PostCallback()
    {
        if (_objectToActive != null)
            _objectToActive.SetActive(true);
    }

}
