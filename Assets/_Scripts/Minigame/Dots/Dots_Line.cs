using UnityEngine;

public class Dots_Line : MonoBehaviour
{

    [SerializeField] private SpriteRenderer _spriteRenderer;

    private bool m_selected = false;
    public bool Selected => m_selected;

    private bool m_canSelect = false;

    void Awake()
    {
        _spriteRenderer ??= GetComponent<SpriteRenderer>();

        UpdateSpriteRenderer();
    }

    public void Setup()
    {
        m_canSelect = true;

        m_selected = false;

        UpdateSpriteRenderer();
    }

    public void Select()
    {
        if (!m_canSelect || m_selected)
            return;

        m_selected = true;

        UpdateSpriteRenderer();
    }

    private void UpdateSpriteRenderer()
    {
        _spriteRenderer.enabled = m_selected;
    }

}
