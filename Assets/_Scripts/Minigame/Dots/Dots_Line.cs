using System.Collections.Generic;
using UnityEngine;

public class Dots_Line : MonoBehaviour
{

    [SerializeField] private SpriteRenderer _spriteRenderer;

    private bool m_selected = false;
    public bool Selected => m_selected;

    private bool m_canSelect = false;

    private GameObject m_pointA;
    public GameObject PointA => m_pointA;

    private GameObject m_pointB;
    public GameObject PointB => m_pointB;

    private readonly List<Dots_Square> m_squaresLinked = new();
    public IList<Dots_Square> SquaresLinked => m_squaresLinked.AsReadOnly();

    void Awake()
    {
        _spriteRenderer ??= GetComponent<SpriteRenderer>();

        UpdateSpriteRenderer();
    }

    public void Setup(GameObject pointA, GameObject pointB)
    {
        m_pointA = pointA;

        m_pointB = pointB;

        m_squaresLinked.Clear();
    }

    public void LinkSquare(Dots_Square square)
    {
        m_squaresLinked.Add(square);
    }

    public void Get()
    {
        m_canSelect = true;

        m_selected = false;

        SetColor(Color.white);

        UpdateSpriteRenderer();
    }

    public void Release()
    {
        m_canSelect = false;
    }

    public void Select()
    {
        if (!m_canSelect || m_selected)
            return;

        m_selected = true;

        UpdateSpriteRenderer();

        Manager_Events.Minigames.Dots.OnCheckSquare.Notify(this);
    }

    public void SetColor(Color value)
    {
        _spriteRenderer.color = value;
    }

    private void UpdateSpriteRenderer()
    {
        _spriteRenderer.enabled = m_selected;
    }

}
