using UnityEngine;
using Cinemachine;

public class CameraController : Singleton<CameraController>
{

    private class CameraConfig
    {
        public CinemachineVirtualCamera camera = null;
        public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public float duration = 0.5f;
    }

    private Camera m_mainCamera = null;
    public Camera MainCamera
    {
        get
        {
            m_mainCamera ??= Camera.main;

            return m_mainCamera;
        }
    }

    private CinemachineBrain m_brain;

    private CameraConfig m_previousCamera;
    private CameraConfig m_currentCamera = new();

    protected override void Init()
    {
        base.Init();

        m_brain = MainCamera.GetComponent<CinemachineBrain>();
    }

    private void OnTransitionCamera(CinemachineVirtualCamera camera, AnimationCurve curve, float duration)
    {
        if (m_currentCamera != null && camera == m_currentCamera.camera)
            return;

        if (camera != null)
        {
            if (m_currentCamera.camera != null)
                m_currentCamera.camera.Priority = 10;

            m_previousCamera = m_currentCamera;

            if (curve != null)
            {
                m_brain.m_DefaultBlend.m_CustomCurve = curve;
                m_brain.m_DefaultBlend.m_Time = duration;
            }

            m_currentCamera = new()
            {
                camera = camera,
                curve = curve,
                duration = duration
            };

            m_currentCamera.camera.Priority = 11;
        }
    }

    private void OnTransiteToPreviousCamera()
    {
        if (m_previousCamera != null)
            OnTransitionCamera(m_previousCamera.camera, m_previousCamera.curve, m_previousCamera.duration);
    }

    void OnEnable()
    {
        Manager_Events.Camera.OnTransitionCamera += OnTransitionCamera;

        Manager_Events.Camera.OnTransiteToPreviousCamera += OnTransiteToPreviousCamera;
    }

    void OnDisable()
    {
        Manager_Events.Camera.OnTransitionCamera -= OnTransitionCamera;

        Manager_Events.Camera.OnTransiteToPreviousCamera -= OnTransiteToPreviousCamera;
    }

}
