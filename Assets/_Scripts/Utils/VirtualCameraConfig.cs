using System;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class VirtualCameraConfig : MonoBehaviour
{

    [Serializable]
    public class CameraSettings
    {
        public InspectorEvent _event;
        public AnimationCurve _curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public float _duration = 0.1f;
    }


    [SerializeField] private CameraSettings[] _settings;
    private CinemachineVirtualCamera _camera;

    void Awake()
    {
        _camera = GetComponent<CinemachineVirtualCamera>();

        foreach (var setting in _settings)
        {
            Manager_Events.Add(
                setting._event, 
                () => Manager_Events.Camera.OnTransitionCamera.Notify(_camera, setting._curve, setting._duration)
            );
        }
    }

}
