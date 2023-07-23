using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class VirtualCameraConfig : MonoBehaviour
{

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
