using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class Manager_TicTacToe : Minigame
{

    [Header("UI")]
    [SerializeField] private GameObject _ctnGameover;
    [SerializeField] private TextMeshProUGUI _txtWinner;
    [SerializeField] private GameObject _ctnTurn;
    [SerializeField] private TextMeshProUGUI _txtTurn;

    [Header("Minigame Parameters")]
    [SerializeField] private Sprite _pieceEmpty;
    [SerializeField] private Sprite _piecePlayer;
    [SerializeField] private Sprite _pieceNpc;

    [SerializeField] private List<TicTacToe_Piece> _pieces;
    [SerializeField] private List<TicTacToe_WinnerLine> _winnerLines;

    private int m_playerOrder = 0;

    private readonly Dictionary<Tuple<int, int>, TicTacToe_Piece> m_piecesDictionary = new();


    protected override void Awake()
    {
        base.Awake();

        foreach (var piece in _pieces)
            m_piecesDictionary.Add(new(piece.Row, piece.Column), piece);

        _ctnGameover.SetActive(false);

        _ctnTurn.SetActive(false);
    }

    protected override void Setup()
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

    protected override void Release()
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
            else
            {
                if (m_playerOrder % 2 == 0)
                    NpcTurn();
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
        var winText = Manager_Translation.Instance.FindTranslation("win");

        _ctnGameover.SetActive(true);

        _txtWinner.SetText($"P{(playerVictory ? "1" : "2")} {winText}");

        _txtTurn.gameObject.SetActive(false);

        foreach (var piece in _pieces)
            if (!piece.PlayerSelection.HasValue)
                piece.Release();
    }

    private void ShowDraw()
    {
        var drawText = Manager_Translation.Instance.FindTranslation("draw");

        _ctnGameover.SetActive(true);

        _txtWinner.SetText($"{drawText}");

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

    private void PlayerTurn()
    {
        foreach (var piece in _pieces)
            piece.Get();
    }

    private void NpcTurn()
    {
        foreach (var piece in _pieces)
            piece.Release();

        MonoBehaviourHelper.StartCoroutine(WaitToMark());
    }

    private IEnumerator WaitToMark()
    {
        var piece00 = m_piecesDictionary[new(0, 0)];
        var piece01 = m_piecesDictionary[new(0, 1)];
        var piece02 = m_piecesDictionary[new(0, 2)];
        var piece10 = m_piecesDictionary[new(1, 0)];
        var piece11 = m_piecesDictionary[new(1, 1)];
        var piece12 = m_piecesDictionary[new(1, 2)];
        var piece20 = m_piecesDictionary[new(2, 0)];
        var piece21 = m_piecesDictionary[new(2, 1)];
        var piece22 = m_piecesDictionary[new(2, 2)];

        var hasValue00 = piece00.PlayerSelection.HasValue;
        var hasValue01 = piece01.PlayerSelection.HasValue;
        var hasValue02 = piece02.PlayerSelection.HasValue;
        var hasValue10 = piece10.PlayerSelection.HasValue;
        var hasValue11 = piece11.PlayerSelection.HasValue;
        var hasValue12 = piece12.PlayerSelection.HasValue;
        var hasValue20 = piece20.PlayerSelection.HasValue;
        var hasValue21 = piece21.PlayerSelection.HasValue;
        var hasValue22 = piece22.PlayerSelection.HasValue;

        var value00 = hasValue00 && piece00.PlayerSelection.Value;
        var value01 = hasValue01 && piece01.PlayerSelection.Value;
        var value02 = hasValue02 && piece02.PlayerSelection.Value;
        var value10 = hasValue10 && piece10.PlayerSelection.Value;
        var value11 = hasValue11 && piece11.PlayerSelection.Value;
        var value12 = hasValue12 && piece12.PlayerSelection.Value;
        var value20 = hasValue20 && piece20.PlayerSelection.Value;
        var value21 = hasValue21 && piece21.PlayerSelection.Value;
        var value22 = hasValue22 && piece22.PlayerSelection.Value;

        yield return new WaitForSeconds(1f);

        var selected = false;

        // Rows
        if ((hasValue00 && !value00) && (hasValue01 && !value01) && !hasValue02)
            Select(piece02, ref selected);
        else if ((hasValue00 && !value00) && (hasValue02 && !value02) && !hasValue01)
            Select(piece01, ref selected);
        else if ((hasValue01 && !value01) && (hasValue02 && !value02) && !hasValue00)
            Select(piece00, ref selected);

        else if ((hasValue10 && !value10) && (hasValue11 && !value11) && !hasValue12)
            Select(piece12, ref selected);
        else if ((hasValue10 && !value10) && (hasValue12 && !value12) && !hasValue11)
            Select(piece11, ref selected);
        else if ((hasValue11 && !value11) && (hasValue12 && !value12) && !hasValue10)
            Select(piece10, ref selected);

        else if ((hasValue20 && !value20) && (hasValue21 && !value21) && !hasValue22)
            Select(piece22, ref selected);
        else if ((hasValue20 && !value20) && (hasValue22 && !value22) && !hasValue21)
            Select(piece21, ref selected);
        else if ((hasValue21 && !value21) && (hasValue22 && !value22) && !hasValue20)
            Select(piece20, ref selected);

        // Columns
        else if ((hasValue00 && !value00) && (hasValue10 && !value10) && !hasValue20)
            Select(piece20, ref selected);
        else if ((hasValue00 && !value00) && (hasValue20 && !value20) && !hasValue10)
            Select(piece10, ref selected);
        else if ((hasValue10 && !value10) && (hasValue20 && !value20) && !hasValue00)
            Select(piece00, ref selected);

        else if ((hasValue01 && !value01) && (hasValue11 && !value11) && !hasValue21)
            Select(piece21, ref selected);
        else if ((hasValue01 && !value01) && (hasValue21 && !value21) && !hasValue11)
            Select(piece11, ref selected);
        else if ((hasValue11 && !value11) && (hasValue21 && !value21) && !hasValue01)
            Select(piece01, ref selected);

        else if ((hasValue02 && !value02) && (hasValue12 && !value12) && !hasValue22)
            Select(piece22, ref selected);
        else if ((hasValue02 && !value02) && (hasValue22 && !value22) && !hasValue12)
            Select(piece12, ref selected);
        else if ((hasValue12 && !value12) && (hasValue22 && !value22) && !hasValue02)
            Select(piece02, ref selected);

        // Diagonals
        else if ((hasValue00 && !value00) && (hasValue11 && !value11) && !hasValue22)
            Select(piece22, ref selected);
        else if ((hasValue00 && !value00) && (hasValue22 && !value22) && !hasValue11)
            Select(piece11, ref selected);
        else if ((hasValue11 && !value11) && (hasValue22 && !value22) && !hasValue00)
            Select(piece00, ref selected);

        else if ((hasValue02 && !value02) && (hasValue11 && !value11) && !hasValue20)
            Select(piece20, ref selected);
        else if ((hasValue02 && !value02) && (hasValue20 && !value20) && !hasValue11)
            Select(piece11, ref selected);
        else if ((hasValue11 && !value11) && (hasValue20 && !value20) && !hasValue02)
            Select(piece02, ref selected);

        // Rows
        else if ((hasValue00 && value00) && (hasValue01 && value01) && !hasValue02)
            Select(piece02, ref selected);
        else if ((hasValue00 && value00) && (hasValue02 && value02) && !hasValue01)
            Select(piece01, ref selected);
        else if ((hasValue01 && value01) && (hasValue02 && value02) && !hasValue00)
            Select(piece00, ref selected);

        else if ((hasValue10 && value10) && (hasValue11 && value11) && !hasValue12)
            Select(piece12, ref selected);
        else if ((hasValue10 && value10) && (hasValue12 && value12) && !hasValue11)
            Select(piece11, ref selected);
        else if ((hasValue11 && value11) && (hasValue12 && value12) && !hasValue10)
            Select(piece10, ref selected);

        else if ((hasValue20 && value20) && (hasValue21 && value21) && !hasValue22)
            Select(piece22, ref selected);
        else if ((hasValue20 && value20) && (hasValue22 && value22) && !hasValue21)
            Select(piece21, ref selected);
        else if ((hasValue21 && value21) && (hasValue22 && value22) && !hasValue20)
            Select(piece20, ref selected);

        // Columns
        else if ((hasValue00 && value00) && (hasValue10 && value10) && !hasValue20)
            Select(piece20, ref selected);
        else if ((hasValue00 && value00) && (hasValue20 && value20) && !hasValue10)
            Select(piece10, ref selected);
        else if ((hasValue10 && value10) && (hasValue20 && value20) && !hasValue00)
            Select(piece00, ref selected);

        else if ((hasValue01 && value01) && (hasValue11 && value11) && !hasValue21)
            Select(piece21, ref selected);
        else if ((hasValue01 && value01) && (hasValue21 && value21) && !hasValue11)
            Select(piece11, ref selected);
        else if ((hasValue11 && value11) && (hasValue21 && value21) && !hasValue01)
            Select(piece01, ref selected);

        else if ((hasValue02 && value02) && (hasValue12 && value12) && !hasValue22)
            Select(piece22, ref selected);
        else if ((hasValue02 && value02) && (hasValue22 && value22) && !hasValue12)
            Select(piece12, ref selected);
        else if ((hasValue12 && value12) && (hasValue22 && value22) && !hasValue02)
            Select(piece02, ref selected);

        // Diagonals
        else if ((hasValue00 && value00) && (hasValue11 && value11) && !hasValue22)
            Select(piece22, ref selected);
        else if ((hasValue00 && value00) && (hasValue22 && value22) && !hasValue11)
            Select(piece11, ref selected);
        else if ((hasValue11 && value11) && (hasValue22 && value22) && !hasValue00)
            Select(piece00, ref selected);

        else if ((hasValue02 && value02) && (hasValue11 && value11) && !hasValue20)
            Select(piece20, ref selected);
        else if ((hasValue02 && value02) && (hasValue20 && value20) && !hasValue11)
            Select(piece11, ref selected);
        else if ((hasValue11 && value11) && (hasValue20 && value20) && !hasValue02)
            Select(piece02, ref selected);

        if (!selected)
        {
            var emptyPieces = _pieces.FindAll(x => !x.PlayerSelection.HasValue);
            var piece = emptyPieces[UnityEngine.Random.Range(0, emptyPieces.Count)];
            Select(piece, ref selected);
        }

        PlayerTurn();
    }

    private void Select(TicTacToe_Piece piece, ref bool selected)
    {
        piece.Select(false);
        selected = true;
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
