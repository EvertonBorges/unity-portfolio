using UnityEngine;

public class Dots_Square : MonoBehaviour
{

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _spritePlayer;
    [SerializeField] private Sprite _spriteNpc;

    private Dots_Line m_northLine;
    public Dots_Line NorthLine => m_northLine;

    private Dots_Line m_eastLine;
    public Dots_Line EastLine => m_eastLine;

    private Dots_Line m_southLine;
    public Dots_Line SouthLine => m_southLine;

    private Dots_Line m_westLine;
    public Dots_Line WestLine => m_westLine;

    private bool? m_playerPoint = null;
    public bool? PlayerPoint => m_playerPoint;

    void Awake()
    {
        _spriteRenderer ??= GetComponent<SpriteRenderer>();

        _spriteRenderer.enabled = false;

        UpdateSprite();
    }

    public void Setup(Dots_Line northLine, Dots_Line eastLine, Dots_Line southLine, Dots_Line westLine)
    {
        m_northLine = northLine;
        m_eastLine = eastLine;
        m_southLine = southLine;
        m_westLine = westLine;

        m_playerPoint = null;

        UpdateSprite();
    }

    public void Restart()
    {
        m_playerPoint = null;

        _spriteRenderer.color = Color.white;

        UpdateSprite();
    }

    public void SetColor(Color value)
    {
        _spriteRenderer.color = value;
    }

    public void Mark(bool playerPoint)
    {
        m_playerPoint = playerPoint;

        _spriteRenderer.enabled = true;

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        _spriteRenderer.sprite = !m_playerPoint.HasValue ? null : (m_playerPoint.Value ? _spritePlayer : _spriteNpc);
    }

    public bool OneLeft(out Dots_Line line)
    {
        line = null;

        if (m_playerPoint.HasValue)
            return false;

        var count = m_northLine.Selected ? 1 : 0;

        count += m_eastLine.Selected ? 1 : 0;
        count += m_southLine.Selected ? 1 : 0;
        count += m_westLine.Selected ? 1 : 0;

        var result = count == 3;

        if (result)
        {
            line = 
                !m_northLine.Selected ? 
                    m_northLine : 
                    (!m_eastLine.Selected ? 
                        m_eastLine : 
                        (!m_southLine.Selected ? 
                            m_southLine : 
                            m_westLine
                        )
                    );
        }

        return result;
    }

}
