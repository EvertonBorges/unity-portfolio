using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Manager_Dots : Interactable
{

    [SerializeField] private int _rows;
    [SerializeField] private int _columns;

    [Header("Dot Parameters")]
    [SerializeField] private GameObject _dotPrefab;
    [SerializeField] private Transform _dotParent;

    [Header("Line Parameters")]
    [SerializeField] private Dots_Line _linePrefab;
    [SerializeField] private Transform _lineParent;

    private readonly List<Dots_Line> _lines = new();

    [Header("Camera Setting")]
    [SerializeField] private LayerMask _maskInteract;
    [SerializeField] private CinemachineVirtualCamera _camera;
    [SerializeField] private AnimationCurve _curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private float _duration = 0.5f;

    private LayerMask m_maskDefault;

    void Awake()
    {
        CreateDots();
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
    }

    private void Setup()
    {
        _lines.ForEach(x => x.Setup());
    }

    public void CreateDots()
    {
        if (_rows <= 0 || _columns <= 0)
            return;

        ClearDots();

        var rowSpace = 1f / (_rows - 1f);
        var columnSpace = 1f / (_columns - 1f);

        Dictionary<Tuple<int, int>, GameObject> dots = new();

        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                var dot = Instantiate(_dotPrefab, _dotParent);

                dot.transform.localPosition = new(j * columnSpace - 0.5f, 0.5f - i * rowSpace, 0f);

                dots.Add(new(i, j), dot);
            }
        }

        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                var dot = dots[new(i, j)];

                if (dots.ContainsKey(new(i + 1, j)))
                {
                    var nextDot = dots[new(i + 1, j)];

                    var line = Instantiate(_linePrefab, _lineParent);

                    line.transform.localPosition = (dot.transform.localPosition + nextDot.transform.localPosition) / 2f;

                    var distance = (nextDot.transform.localPosition - dot.transform.localPosition).magnitude;

                    line.transform.localScale = new(distance, 0.0125f, 1f);

                    line.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

                    _lines.Add(line);
                }

                if (dots.ContainsKey(new(i, j + 1)))
                {
                    var nextDot = dots[new(i, j + 1)];

                    var line = Instantiate(_linePrefab, _lineParent);

                    line.transform.localPosition = (dot.transform.localPosition + nextDot.transform.localPosition) / 2f;

                    var distance = (nextDot.transform.localPosition - dot.transform.localPosition).magnitude;

                    line.transform.localScale = new(distance, 0.0125f, 1f);

                    _lines.Add(line);
                }
            }
        }
    }

    public void ClearDots()
    {
        var dotChildCount = _dotParent.childCount;

        for (int i = 0; i < dotChildCount; i++)
            DestroyImmediate(_dotParent.GetChild(0).gameObject);

        var lineChildCount = _lineParent.childCount;

        for (int i = 0; i < lineChildCount; i++)
            DestroyImmediate(_lineParent.GetChild(0).gameObject);
    }

}
