using UnityEngine;

public class Museum_Trigger : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform _switch;
    [SerializeField] private TotemController _totem;

    [Header("Switch Infos")]
    [SerializeField] private float _switchOffsetActivation;
    [SerializeField] private SO_Sound _switchActivationSound;
    [SerializeField] private SO_Sound _switchDesactivationSound;

    private Vector3 m_switchStartPosition = default;

    private void Awake()
    {
        m_switchStartPosition = _switch.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController _))
            return;

        _totem.PlayVideo();
        _switch.position = m_switchStartPosition + Vector3.down * _switchOffsetActivation;
        if (_switchActivationSound != null) _switchActivationSound.Play();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController _))
            return;

        _totem.StopVideo();
        _switch.position = m_switchStartPosition;
        if (_switchDesactivationSound != null) _switchDesactivationSound.Play();
    }

}
