using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{

    [SerializeField] private DoorAnimatorController _doorAnimatorController;
    [SerializeField] private GameObject[] _canvas;
    [SerializeField] private GameObject _collider;
    [SerializeField] private Transform _target;
    [SerializeField] private InspectorEvent _event;

    private bool m_opened = false;

    private void Awake()
    {
        EnableCanvas(false);
    }

    public void Interact()
    {
        if (m_opened)
            return;

        m_opened = true;

        _doorAnimatorController.Open(m_opened);

        if (_event == Manager_Events.Camera.Events.OnFpsCam)
            Manager_Events.GameManager.Area.EnableMuseum.Notify();
        else if (_event == Manager_Events.Camera.Events.OnTpsCam)
            Manager_Events.GameManager.Area.EnableExternalArea.Notify();

        _event.Notify();

        _collider.SetActive(false);

        EnableCanvas(false);

        Manager_Events.Player.OnCanRotateFpsCamera.Notify(false);

        Manager_Events.Player.OnLookAt.Notify(_target);

        MonoBehaviourHelper.StartCoroutine(WaitAnimation());
    }

    private IEnumerator WaitAnimation()
    {
        yield return new WaitForSeconds(1f); // TODO check if animation really finished

        Manager_Events.Player.OnStartWalk.Notify(gameObject, _target);

        Manager_Events.Player.OnCanRotateFpsCamera.Notify(true);
    }

    public void InverseInteract()
    {

    }

    private void OnFinishWalk(GameObject go)
    {
        if (gameObject != go)
            return;

        m_opened = false;

        _collider.SetActive(true);

        _doorAnimatorController.Open(m_opened);
    }

    private void EnableCanvas(bool enabled)
    {
        foreach (var canvas in _canvas)
            canvas.SetActive(enabled);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController _))
            return;

        if (!CameraController.Instance.IsFpsCam)
            return;

        EnableCanvas(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController _))
            return;

        EnableCanvas(false);
    }

    void OnEnable()
    {
        Manager_Events.Player.OnFinishWalk += OnFinishWalk;
    }

    void OnDisable()
    {
        Manager_Events.Player.OnFinishWalk -= OnFinishWalk;
    }

}