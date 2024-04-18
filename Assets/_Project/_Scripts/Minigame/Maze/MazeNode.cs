using System;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class MazeNode : MonoBehaviour
{

    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private MeshRenderer _renderer;
    public SerializedDictionary<MazeNodeDirection, GameObject> _walls;

    private Vector2Int m_index;
    public Vector2Int Index => m_index;

    private MazeNodeState m_state;
    private int m_weight;
    public int Weight => m_weight;

    private bool m_canInteract = false;
    private Action m_interaction = null;

    public void Setup(Vector2Int index, MazeNodeState state)
    {
        m_canInteract = true;
        m_index = index;
        m_interaction = null;
        SetState(state);

        _boxCollider.enabled = false;

        foreach (var wall in _walls)
            wall.Value.SetActive(true);
    }

    public void SetColor(Color color)
    {
        _renderer.enabled = true;
        _renderer.material.color = color;
    }

    public void SetState(MazeNodeState state)
    {
        m_state = state;

        _renderer.enabled = false;

        _renderer.material.color = m_state switch
        {
            MazeNodeState.Current => Color.yellow,
            MazeNodeState.Completed => Color.blue,
            _ => Color.white,
        };
    }

    public void SetWeight(int weight)
    {
        m_weight = weight;
    }

    public void SetInteraction(Action interaction)
    {
        m_interaction = interaction;
        _boxCollider.enabled = interaction != null;
    }

    public void DisableWallByDirection(MazeNodeDirection direction)
    {
        _walls[direction].SetActive(false);
    }

    public bool OneWallRemoved()
    {
        int count = 0;

        foreach (var wall in _walls)
        {
            if (!wall.Value.activeSelf)
                count++;

            if (count > 1)
                break;
        }

        return count <= 1;
    }

    public void EnableInteraction(bool enabled)
    {
        m_canInteract = enabled;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!m_canInteract)
            return;

        if (!other.TryGetComponent(out PlayerController _))
            return;

        m_interaction?.Invoke();
    }

}
