using Cinemachine;
using UnityEngine;

public abstract class Minigame : Interactable
{
    
    [Header("Camera Setting")]
    [SerializeField] private LayerMask _maskInteract;
    [SerializeField] private CinemachineVirtualCamera _camera;
    [SerializeField] private AnimationCurve _curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private float _duration = 0.5f;

    private LayerMask m_maskDefault;

    protected abstract void Setup();
    protected abstract void Release();

    protected virtual void Awake()
    {
        _camera.Priority = 10;
    }

    public override void Interact()
    {
        Setup();

        m_maskDefault = CameraController.Instance.MainCamera.cullingMask;

        CameraController.Instance.MainCamera.cullingMask = _maskInteract;

        Manager_Events.Camera.OnTransitionCamera.Notify(_camera, _curve, _duration);
    }

    public override void InverseInteract()
    {
        Manager_Events.Camera.Events.OnTpsCam.Notify();

        CameraController.Instance.MainCamera.cullingMask = m_maskDefault;

        Release();
    }

}
