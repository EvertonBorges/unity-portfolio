using UnityEngine;

public class Memory_Piece : MonoBehaviour
{

    [SerializeField] private int _row;
    public int Row => _row;
    [SerializeField] private int _column;
    public int Column => _column;

    private bool m_canInteract = false;

    public void Setup()
    {
        m_canInteract = true;
    }

    public void Select()
    {
        if (!m_canInteract)
            return;

        Manager_Events.Minigames.Memory.OnSelect.Notify(this);
    }

    public void Get()
    {
        m_canInteract = true;
    }

    public void Release()
    {
        m_canInteract = false;
    }

}
