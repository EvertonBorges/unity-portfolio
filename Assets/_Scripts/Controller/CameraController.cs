using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    
    private Camera m_mainCamera = null;
    public Camera MainCamera
    {
        get
        {
            m_mainCamera ??= Camera.main;

            return m_mainCamera;
        }
    }

}
