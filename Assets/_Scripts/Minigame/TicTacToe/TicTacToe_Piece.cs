using UnityEngine;

public class TicTacToe_Piece : MonoBehaviour
{

    [SerializeField] private int _row;
    public int Row => _row;
    [SerializeField] private int _column;
    public int Column => _column;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    private bool? m_playerSelection = null;
    public bool? PlayerSelection => m_playerSelection;

    private bool m_canInteract = false;

    void Awake()
    {
        _spriteRenderer ??= GetComponent<SpriteRenderer>();
    }

    public void Setup(TicTacToe_PieceValue pieceValue)
    {
        m_canInteract = true;
        _spriteRenderer.sprite = pieceValue.sprite;
        m_playerSelection = pieceValue.playerSelection;
    }

    public void Select(TicTacToe_PieceValue pieceValue)
    {
        Setup(pieceValue);

        Manager_Events.Minigames.TicTacToe.OnCheckVictory.Notify();
    }

    public void Select()
    {
        if (!m_canInteract)
            return;

        Manager_Events.Minigames.TicTacToe.OnSelect.Notify(this);
    }

    public void Release()
    {
        m_canInteract = false;
    }

}
