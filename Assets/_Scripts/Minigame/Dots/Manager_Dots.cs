using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Manager_Dots : Minigame
{

    [Header("UI")]
    [SerializeField] private GameObject _ctnGameover;
    [SerializeField] private TextMeshProUGUI _txtWinner;
    [SerializeField] private GameObject _ctnTurn;
    [SerializeField] private TextMeshProUGUI _txtTurn;
    [SerializeField] private GameObject _ctnScore;
    [SerializeField] private TextMeshProUGUI _txtPlayerScore;
    [SerializeField] private TextMeshProUGUI _txtNpcScore;

    [Header("MiniGame Parameters")]
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;

    [SerializeField] private Color _playerColor = Color.white;
    [SerializeField] private Color _npcColor = Color.black;

    [Space(10f)]
    [SerializeField] private GameObject _dotPrefab;
    [SerializeField] private Transform _dotParent;

    [Space(10f)]
    [SerializeField] private Dots_Line _linePrefab;
    [SerializeField] private Transform _lineParent;

    [Space(10f)]
    [SerializeField] private Dots_Square _squarePrefab;
    [SerializeField] private Transform _squareParent;

    private readonly List<Dots_Line> _lines = new();
    private readonly List<Dots_Square> _squares = new();
    
    private int m_playerOrder = 0;

    void Awake()
    {
        CreateDots();

        Release();
    }

    protected override void Setup()
    {
        m_playerOrder = 0;

        _ctnGameover.SetActive(false);

        _ctnTurn.SetActive(true);

        _ctnScore.SetActive(true);

        _lines.ForEach(x => x.Restart());

        _squares.ForEach(x => x.Restart());

        UpdateTurnUI();

        UpdateScoreUI();
    }

    protected override void Release()
    {
        _ctnGameover.SetActive(false);

        _ctnTurn.SetActive(false);

        _ctnScore.SetActive(false);

        UpdateTurnUI();

        _lines.ForEach(x => x.Release());
    }

    private void OnCheckSquare(Dots_Line line)
    {
        var playerSelection = m_playerOrder % 2 == 0;
        var marked = false;

        line.SetColor(playerSelection ? _playerColor : _npcColor);

        foreach (var square in line.SquaresLinked)
        {
            if (square.NorthLine.Selected && square.EastLine.Selected && square.SouthLine.Selected && square.WestLine.Selected)
            {
                square.Mark(playerSelection);
                square.SetColor(playerSelection ? _playerColor : _npcColor);
                marked = true;
            }
        }

        NextTurn(marked);

        CheckVictory();
    }

    private void NextTurn(bool marked)
    {
        if (!marked)
            m_playerOrder++;

        if (m_playerOrder % 2 != 0)
            NpcTurn();

        UpdateTurnUI();

        UpdateScoreUI();
    }

    private void NpcTurn()
    {
        MonoBehaviourHelper.StartCoroutine(WaitNpcTurn());
    }

    private IEnumerator WaitNpcTurn()
    {
        _lines.ForEach(x => x.Release());

        Dots_Line line = null;

        var square = _squares.Find(x => x.OneLeft(out line));

        if (!square)
        {
            var lines = _lines.FindAll(x => !x.Selected);
            line = lines[UnityEngine.Random.Range(0, lines.Count)];
        }

        yield return new WaitForSeconds(1f);
        
        line.Select(false);

        _lines.ForEach(x => x.Get());
    }

    private void CheckVictory()
    {
        var allPoints = _squares.Select(x => x.PlayerPoint).ToList();
        var allMarked = allPoints.Count(x => x.HasValue);

        var totalSquares = (_rows - 1) * (_columns - 1);

        if (allMarked < totalSquares)
            return;

        var playerMarked = allPoints.Count(x => x.HasValue && x.Value);
        var npcMarked = allPoints.Count(x => x.HasValue && !x.Value);

        _ctnGameover.SetActive(true);

        _txtTurn.gameObject.SetActive(false);

        if (playerMarked > npcMarked)
            _txtWinner.SetText($"Player WIN");
        else if (playerMarked < npcMarked)
            _txtWinner.SetText($"CPU WIN");
        else
            _txtWinner.SetText("DRAW");
    }

    private void UpdateTurnUI()
    {
        var player = m_playerOrder % 2 == 0;

        _txtTurn.SetText($"{(player ? "Player" : "CPU")}");

        _txtTurn.color = player ? _playerColor : _npcColor;
    }

    private void UpdateScoreUI()
    {
        var allPoints = _squares.Select(x => x.PlayerPoint).ToList();

        var playerScore = allPoints.Count(x => x.HasValue && x.Value);
        var npcScore = allPoints.Count(x => x.HasValue && !x.Value);

        _txtPlayerScore.text = $"{playerScore}";
        _txtNpcScore.text = $"{npcScore}";
    }

    public void BTN_Restart()
    {
        Setup();
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

                dot.name = $"Dot_{i}x{j}";

                dots.Add(new(i, j), dot);
            }
        }

        _lines.Clear();

        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                var dot = dots[new(i, j)];

                if (dots.ContainsKey(new(i + 1, j)))
                {
                    var nextDot = dots[new(i + 1, j)];

                    var line = Instantiate(_linePrefab, _lineParent);

                    line.Setup(dot, nextDot);

                    line.transform.localPosition = (dot.transform.localPosition + nextDot.transform.localPosition) / 2f;

                    line.name = $"Line_{dot.name}_{nextDot.name}";

                    var distance = (nextDot.transform.localPosition - dot.transform.localPosition).magnitude;

                    line.transform.localScale = new(distance, 0.0125f, 1f);

                    line.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

                    _lines.Add(line);
                }

                if (dots.ContainsKey(new(i, j + 1)))
                {
                    var nextDot = dots[new(i, j + 1)];

                    var line = Instantiate(_linePrefab, _lineParent);

                    line.Setup(dot, nextDot);

                    line.transform.localPosition = (dot.transform.localPosition + nextDot.transform.localPosition) / 2f;

                    line.name = $"Line_{dot.name}_{nextDot.name}";

                    var distance = (nextDot.transform.localPosition - dot.transform.localPosition).magnitude;

                    line.transform.localScale = new(distance, 0.0125f, 1f);

                    _lines.Add(line);
                }
            }
        }

        _squares.Clear();

        for (int i = 0; i < _rows - 1; i++)
        {
            for (int j = 0; j < _columns - 1; j++)
            {
                var dot1 = dots[new(i, j)];
                var dot2 = dots[new(i + 1, j)];
                var dot3 = dots[new(i, j + 1)];
                var dot4 = dots[new(i + 1, j + 1)];

                var northLine = _lines.Find(x => (x.PointA == dot1 || x.PointA == dot3) && (x.PointB == dot1 || x.PointB == dot3));
                var eastLine = _lines.Find(x => (x.PointA == dot3 || x.PointA == dot4) && (x.PointB == dot3 || x.PointB == dot4));
                var southLine = _lines.Find(x => (x.PointA == dot2 || x.PointA == dot4) && (x.PointB == dot2 || x.PointB == dot4));
                var westLine = _lines.Find(x => (x.PointA == dot1 || x.PointA == dot2) && (x.PointB == dot1 || x.PointB == dot2));

                var square = Instantiate(_squarePrefab, _squareParent);
                square.Setup(northLine, eastLine, southLine, westLine);
                square.transform.localPosition = (dot1.transform.localPosition + dot4.transform.localPosition) / 2f;
                square.transform.localScale = Vector3.one * northLine.transform.localScale.x;
                square.name = $"Square_{i}x{j}";

                northLine.LinkSquare(square);
                eastLine.LinkSquare(square);
                southLine.LinkSquare(square);
                westLine.LinkSquare(square);

                _squares.Add(square);
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

        var squareChildCount = _squareParent.childCount;

        for (int i = 0; i < squareChildCount; i++)
            DestroyImmediate(_squareParent.GetChild(0).gameObject);
    }

    void OnEnable()
    {
        Manager_Events.Minigames.Dots.OnCheckSquare += OnCheckSquare;
    }

    void OnDisable()
    {
        Manager_Events.Minigames.Dots.OnCheckSquare -= OnCheckSquare;
    }

}
