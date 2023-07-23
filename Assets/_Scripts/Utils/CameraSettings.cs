using System;
using UnityEngine;

[Serializable]
public class CameraSettings
{
    public InspectorEvent _event;
    public AnimationCurve _curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public float _duration = 0.1f;
}