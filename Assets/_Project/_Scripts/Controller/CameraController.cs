using UnityEngine;
using Unity.Cinemachine;

public class CameraController : Singleton<CameraController>
{

    private class CameraConfig
    {
        public CinemachineCamera camera = null;
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

    private bool m_isFpsCam = false;
    public bool IsFpsCam => m_isFpsCam;

    private CinemachineBrain m_brain;

    private CameraConfig m_previousCamera;
    private CameraConfig m_currentCamera = new();

    protected override void Init()
    {
        base.Init();

        m_brain = MainCamera.GetComponent<CinemachineBrain>();
    }

    private void OnTransitionCamera(CinemachineCamera camera, AnimationCurve curve, float duration)
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
                m_brain.DefaultBlend.CustomCurve = curve;
                m_brain.DefaultBlend.Time = duration;
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

    private void OnFpsCam()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_isFpsCam = true;
    }

    private void OnTpsCam()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        m_isFpsCam = false;
    }

    private void OnPause()
    {
        if (!m_isFpsCam)
            return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnUnpause()
    {
        if (!m_isFpsCam)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        Manager_Events.Camera.OnTransitionCamera += OnTransitionCamera;
        Manager_Events.Camera.OnTransiteToPreviousCamera += OnTransiteToPreviousCamera;

        Manager_Events.Camera.Events.OnFpsCam += OnFpsCam;
        Manager_Events.Camera.Events.OnTpsCam += OnTpsCam;

        Manager_Events.GameManager.Pause += OnPause;
        Manager_Events.GameManager.Unpause += OnUnpause;
    }

    void OnDisable()
    {
        Manager_Events.Camera.OnTransitionCamera -= OnTransitionCamera;
        Manager_Events.Camera.OnTransiteToPreviousCamera -= OnTransiteToPreviousCamera;

        Manager_Events.Camera.Events.OnFpsCam -= OnFpsCam;
        Manager_Events.Camera.Events.OnTpsCam -= OnTpsCam;

        Manager_Events.GameManager.Pause -= OnPause;
        Manager_Events.GameManager.Unpause -= OnUnpause;
    }

}
