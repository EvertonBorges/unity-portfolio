using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class VirtualCameraConfig : MonoBehaviour
{

    [SerializeField] private CameraSettings[] _settings;
    private CinemachineCamera _camera;

    void Awake()
    {
        _camera = GetComponent<CinemachineCamera>();

        foreach (var setting in _settings)
        {
            Manager_Events.Add(
                setting._event, 
                () => Manager_Events.Camera.OnTransitionCamera.Notify(_camera, setting._curve, setting._duration)
            );
        }
    }

}
