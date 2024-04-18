using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Memory : Minigame
{

    [Header("References")]
    [SerializeField] private List<Memory_Piece> _pieces;
    [SerializeField] private GameObject _ctnRestart;
    [SerializeField] private float _timerSelection;

    private Memory_Piece m_lastPiece = null;

    private bool m_checkPair = false;
    private Coroutine m_coroutine = null;

    protected override void Awake()
    {
        base.Awake();

        Release();
    }

    protected override void Setup()
    {
        m_checkPair = false;

        List<int> numbers = new() { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8 };
        numbers.Shuffle();

        foreach (var piece in _pieces)
        {
            var number = numbers[0];
            numbers.RemoveAt(0);
            piece.Setup(number.ToString());
        }

        _ctnRestart.SetActive(false);
    }

    protected override void Release()
    {
        if (m_coroutine != null)
        {
            MonoBehaviourHelper.StopCoroutine(m_coroutine);
            m_coroutine = null;
        }

        foreach (var piece in _pieces)
        {
            piece.Reset();
            piece.Release();
        }

        _ctnRestart.SetActive(false);
    }

    public void BTN_Restart()
    {
        Setup();
    }

    private void OnSelect(Memory_Piece piece)
    {
        m_coroutine = MonoBehaviourHelper.StartCoroutine(WaitTimerSelection(piece));
    }

    private IEnumerator WaitTimerSelection(Memory_Piece piece)
    {
        bool check = m_checkPair && m_lastPiece != null;

        if (!check)
        {
            m_checkPair = true;
            m_lastPiece = piece;

            yield break;
        }

        foreach (var p in _pieces)
        {
            if (!p.Finished)
                p.Release();
        }

        bool right = m_lastPiece.Text == piece.Text;

        yield return new WaitForSeconds(_timerSelection);

        if (right)
        {
            m_lastPiece.Finish();
            piece.Finish();

            CheckVictory();
        }
        else
        {
            m_lastPiece.Reset();
            piece.Reset();
        }

        foreach (var p in _pieces)
        {
            if (!p.Finished)
                p.Get();
        }

        m_checkPair = false;
        m_lastPiece = null;

        m_coroutine = null;
    }

    private void CheckVictory()
    {
        bool victory = true;

        foreach (var piece in _pieces)
        {
            if (!piece.Finished)
            {
                victory = false;
                break;
            }
        }

        if (victory)
        {
            Manager_Events.GameManager.AddCoin.Notify();

            _ctnRestart.SetActive(true);
        }
    }

    void OnEnable()
    {
        Manager_Events.Minigames.Memory.OnSelect += OnSelect;
    }

    void OnDisable()
    {
        Manager_Events.Minigames.Memory.OnSelect -= OnSelect;
    }


}
