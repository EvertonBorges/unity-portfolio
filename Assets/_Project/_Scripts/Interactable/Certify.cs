using Unity.Cinemachine;
using UnityEngine;

public class Certify : Interactable
{

    [SerializeField] private LayerMask _maskInteract;
    private LayerMask m_maskDefault;

    [Header("Camera Setting")]
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private AnimationCurve _curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private float _duration = 0.5f;

    public override void Interact()
    {
        m_maskDefault = CameraController.Instance.MainCamera.cullingMask;

        CameraController.Instance.MainCamera.cullingMask = _maskInteract;

        Manager_Events.Camera.OnTransitionCamera.Notify(_camera, _curve, _duration);
    }

    public override void InverseInteract()
    {
        Manager_Events.Camera.Events.OnTpsCam.Notify();

        CameraController.Instance.MainCamera.cullingMask = m_maskDefault;
    }

}
