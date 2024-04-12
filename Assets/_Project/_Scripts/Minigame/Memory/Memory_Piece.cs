using UnityEngine;
using TMPro;

public class Memory_Piece : MonoBehaviour
{

    [SerializeField] private int _row;
    public int Row => _row;
    [SerializeField] private int _column;
    public int Column => _column;

    [SerializeField] private TextMeshProUGUI _text;

    private string m_text = "";
    public string Text => m_text;

    private bool m_canInteract = false;
    private bool m_finished = false;
    public bool Finished => m_finished;

    public void Setup(string text)
    {
        m_text = text;
        _text.SetText("");
        m_canInteract = true;
        m_finished = false;
    }

    public void Select()
    {
        if (!m_canInteract || m_finished)
            return;

        _text.SetText(m_text);

        Manager_Events.Minigames.Memory.OnSelect.Notify(this);
    }

    public void Reset()
    {
        _text.SetText("");
    }

    public void Get()
    {
        m_canInteract = true;
    }

    public void Release()
    {
        m_canInteract = false;
    }

    public void Finish()
    {
        _text.SetText(m_text);
        m_finished = true;
    }

}
