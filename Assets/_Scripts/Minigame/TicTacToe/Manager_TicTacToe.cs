using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using TMPro;

public class Manager_TicTacToe : Interactable
{

    [SerializeField] private LayerMask _maskInteract;
    private LayerMask m_maskDefault;

    [Header("UI")]
    [SerializeField] private GameObject _ctnGameover;
    [SerializeField] private TextMeshProUGUI _txtWinner;
    [SerializeField] private GameObject _ctnTurn;
    [SerializeField] private TextMeshProUGUI _txtTurn;

    [Header("Camera Setting")]
    [SerializeField] private CinemachineVirtualCamera _camera;
    [SerializeField] private AnimationCurve _curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private float _duration = 0.5f;

    [Header("Minigame Parameters")]
    [SerializeField] private Sprite _pieceEmpty;
    [SerializeField] private Sprite _piecePlayer;
    [SerializeField] private Sprite _pieceNpc;

    [SerializeField] private List<TicTacToe_Piece> _pieces;
    [SerializeField] private List<TicTacToe_WinnerLine> _winnerLines;

    private int m_playerOrder = 0;

    private readonly Dictionary<Tuple<int, int>, TicTacToe_Piece> m_piecesDictionary = new();

    void Awake()
    {
        foreach (var piece in _pieces)
            m_piecesDictionary.Add(new(piece.Row, piece.Column), piece);

        _ctnGameover.SetActive(false);

        _ctnTurn.SetActive(false);
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

        ReleasePieces();
    }

    private void Setup()
    {
        m_playerOrder = 0;

        ClearBoard();
    }

    private void ClearBoard()
    {
        var pieceValue = new TicTacToe_PieceValue() { playerSelection = null, sprite = _pieceEmpty };

        foreach (var piece in _pieces)
            piece.Setup(pieceValue);

        foreach (var winnerLine in _winnerLines)
            winnerLine.gameObject.SetActive(false);

        UpdateTurnUI();

        _ctnGameover.SetActive(false);

        _ctnTurn.SetActive(true);

        _txtTurn.gameObject.SetActive(true);
    }

    private void ReleasePieces()
    {
        foreach (var piece in _pieces)
            piece.Release();

        _ctnGameover.SetActive(false);

        _ctnTurn.SetActive(false);
    }

    private void OnCheckVictory()
    {
        var pieces = m_piecesDictionary;

        bool? victory = null;

        // Check Rows
        if (pieces[new(0, 0)].PlayerSelection.HasValue && (pieces[new(0, 0)].PlayerSelection == pieces[new(0, 1)].PlayerSelection) && (pieces[new(0, 1)].PlayerSelection == pieces[new(0, 2)].PlayerSelection))
        {
            ShowWinnerLine(x => x._winnerLineRow0);
            victory = pieces[new(0, 0)].PlayerSelection.Value;
        }
        else if (pieces[new(1, 0)].PlayerSelection.HasValue && (pieces[new(1, 0)].PlayerSelection == pieces[new(1, 1)].PlayerSelection) && (pieces[new(1, 1)].PlayerSelection == pieces[new(1, 2)].PlayerSelection))
        {
            ShowWinnerLine(x => x._winnerLineRow1);
            victory = pieces[new(1, 0)].PlayerSelection.Value;
        }
        else if (pieces[new(2, 0)].PlayerSelection.HasValue && (pieces[new(2, 0)].PlayerSelection == pieces[new(2, 1)].PlayerSelection) && (pieces[new(2, 1)].PlayerSelection == pieces[new(2, 2)].PlayerSelection))
        {
            ShowWinnerLine(x => x._winnerLineRow2);
            victory = pieces[new(2, 0)].PlayerSelection.Value;
        }

        // Check Columns
        else if (pieces[new(0, 0)].PlayerSelection.HasValue && (pieces[new(0, 0)].PlayerSelection == pieces[new(1, 0)].PlayerSelection) && (pieces[new(1, 0)].PlayerSelection == pieces[new(2, 0)].PlayerSelection))
        {
            ShowWinnerLine(x => x._winnerLineColumn0);
            victory = pieces[new(0, 0)].PlayerSelection.Value;
        }
        else if (pieces[new(0, 1)].PlayerSelection.HasValue && (pieces[new(0, 1)].PlayerSelection == pieces[new(1, 1)].PlayerSelection) && (pieces[new(1, 1)].PlayerSelection == pieces[new(2, 1)].PlayerSelection))
        {
            ShowWinnerLine(x => x._winnerLineColumn1);
            victory = pieces[new(0, 1)].PlayerSelection.Value;
        }
        else if (pieces[new(0, 2)].PlayerSelection.HasValue && (pieces[new(0, 2)].PlayerSelection == pieces[new(1, 2)].PlayerSelection) && (pieces[new(1, 2)].PlayerSelection == pieces[new(2, 2)].PlayerSelection))
        {
            ShowWinnerLine(x => x._winnerLineColumn2);
            victory = pieces[new(0, 2)].PlayerSelection.Value;
        }

        // Check diagonals
        else if (pieces[new(0, 0)].PlayerSelection.HasValue && (pieces[new(0, 0)].PlayerSelection == pieces[new(1, 1)].PlayerSelection) && (pieces[new(1, 1)].PlayerSelection == pieces[new(2, 2)].PlayerSelection))
        {
            ShowWinnerLine(x => x._winnerLineDiagonal0);
            victory = pieces[new(0, 0)].PlayerSelection.Value;
        }
        else if (pieces[new(0, 2)].PlayerSelection.HasValue && (pieces[new(0, 2)].PlayerSelection == pieces[new(1, 1)].PlayerSelection) && (pieces[new(1, 1)].PlayerSelection == pieces[new(2, 0)].PlayerSelection))
        {
            ShowWinnerLine(x => x._winnerLineDiagonal1);
            victory = pieces[new(0, 2)].PlayerSelection.Value;
        }

        if (victory.HasValue)
            ShowVictory(victory.Value);
        else
        {
            if (pieces[new(0, 0)].PlayerSelection.HasValue && pieces[new(0, 1)].PlayerSelection.HasValue && pieces[new(0, 2)].PlayerSelection.HasValue &&
                pieces[new(1, 0)].PlayerSelection.HasValue && pieces[new(1, 1)].PlayerSelection.HasValue && pieces[new(1, 2)].PlayerSelection.HasValue &&
                pieces[new(2, 0)].PlayerSelection.HasValue && pieces[new(2, 1)].PlayerSelection.HasValue && pieces[new(2, 2)].PlayerSelection.HasValue)
            {
                ShowDraw();
            }
        }
    }

    private void ShowWinnerLine(Predicate<TicTacToe_WinnerLine> predicate)
    {
        var winnerLine = _winnerLines.Find(predicate);

        winnerLine.gameObject.SetActive(true);
    }

    private void ShowVictory(bool playerVictory)
    {
        _ctnGameover.SetActive(true);

        _txtWinner.SetText($"P{(playerVictory ? "1" : "2")} WIN");

        _txtTurn.gameObject.SetActive(false);

        foreach (var piece in _pieces)
            if (!piece.PlayerSelection.HasValue)
                piece.Release();
    }

    private void ShowDraw()
    {
        _ctnGameover.SetActive(true);

        _txtWinner.SetText("DRAW");

        _txtTurn.gameObject.SetActive(false);
    }

    private void OnSelect(TicTacToe_Piece piece)
    {
        if (piece.PlayerSelection.HasValue)
            return;

        var playerSelection = m_playerOrder % 2 == 0;

        var pieceValue = new TicTacToe_PieceValue() { playerSelection = playerSelection, sprite = playerSelection ? _piecePlayer : _pieceNpc };

        piece.Select(pieceValue);

        m_playerOrder++;

        UpdateTurnUI();
    }

    private void UpdateTurnUI()
    {
        _txtTurn.SetText($"{(m_playerOrder % 2 == 0 ? "X" : "O")}");
    }

    public void BTN_Restart()
    {
        Setup();
    }

    void OnEnable()
    {
        Manager_Events.Minigames.TicTacToe.OnCheckVictory += OnCheckVictory;
        Manager_Events.Minigames.TicTacToe.OnSelect += OnSelect;
    }

    void OnDisable()
    {
        Manager_Events.Minigames.TicTacToe.OnCheckVictory -= OnCheckVictory;
    }


}
