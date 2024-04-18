using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Manager_Maze : Minigame
{

    [Header("Maze References")]
    [SerializeField] private MazeNode _nodePrefab;
    [SerializeField] private Transform _mazeParent;
    [SerializeField] private Vector2Int _mazeSize;
    [SerializeField] private float _offset = 1.05f;

    private bool m_mazeSolved = false;
    private bool m_mazeStarted = false;

    private readonly Dictionary<Vector2Int, MazeNode> m_nodes = new();

    void Start()
    {
        m_mazeStarted = false;
        m_mazeSolved = false;

        StartGame();
    }

    protected override void Setup()
    {

    }

    protected override void Release()
    {
        if (!m_mazeSolved)
            return;

        MonoBehaviourHelper.StartCoroutine(ReleaseCoroutine());
    }

    private void StartGame()
    {
        CreateNodes();

        var startIndex = StartIndex();
        CalculatePaths(startIndex);
    }

    private Vector2Int StartIndex()
    {
        var list = GetEdges();
        list.Shuffle();
        return list[0];
    }

    private Vector2Int EndIndex()
    {
        var list = GetCenter();
        list.Shuffle();
        return list[0];
    }

    private void CreateNodes()
    {
        Vector3 parentPosition = transform.position;
        Vector3 nodeScale = _nodePrefab.transform.localScale;

        for (int i = 0; i < _mazeSize.x; i++)
        {
            for (int j = 0; j < _mazeSize.y; j++)
            {
                var x = parentPosition.x + i * nodeScale.x * _offset + nodeScale.x / 2f - nodeScale.x * _mazeSize.x / 2f * _offset;
                var y = parentPosition.y;
                var z = parentPosition.z - j * nodeScale.z * _offset - nodeScale.z / 2f + nodeScale.z * _mazeSize.y / 2f * _offset;
                var position = new Vector3(x, y, z);

                var index = new Vector2Int(i, j);

                MazeNode node;
                if (m_nodes.ContainsKey(index))
                    node = m_nodes[index];
                else
                    node = Instantiate(_nodePrefab, position, Quaternion.identity, _mazeParent);

                node.name = $"Node {i}x{j}";
                node.Setup(index, MazeNodeState.Available);

                if (!m_nodes.ContainsKey(index))
                    m_nodes.Add(index, node);
            }
        }
    }

    private List<Vector2Int> GetEdges()
    {
        var list = new List<Vector2Int>();

        for (int i = 0; i < _mazeSize.x; i++)
        {
            for (int j = 0; j < _mazeSize.y; j++)
            {
                if (i == 0 || i == _mazeSize.x - 1)
                    list.Add(new Vector2Int(i, j));

                if (j == 0 || j == _mazeSize.y - 1)
                    list.Add(new Vector2Int(i, j));
            }
        }

        return list.Distinct().ToList();
    }

    private List<Vector2Int> GetCenter()
    {
        var list = new List<Vector2Int>();

        var oddWidth = _mazeSize.x % 2 == 1;
        var oddHeight = _mazeSize.y % 2 == 1;

        var listWidth = new List<int>();
        if (oddWidth)
            listWidth.Add((_mazeSize.x - 1) / 2);
        else
        {
            listWidth.Add(_mazeSize.x / 2);
            listWidth.Add(_mazeSize.x / 2 - 1);
        }
        var listHeight = new List<int>();
        if (oddHeight)
            listHeight.Add((_mazeSize.y - 1) / 2);
        else
        {
            listHeight.Add(_mazeSize.y / 2);
            listHeight.Add(_mazeSize.x / 2 - 1);
        }

        for (int i = 0; i < _mazeSize.x; i++)
        {
            for (int j = 0; j < _mazeSize.y; j++)
            {
                if (listWidth.Contains(i) && listHeight.Contains(j))
                    list.Add(new Vector2Int(i, j));
            }
        }

        return list.Distinct().ToList();
    }

    private List<Vector2Int> GetDiagonals()
    {
        var list = new List<Vector2Int>();

        for (int i = 0; i < _mazeSize.x; i++)
        {
            for (int j = 0; j < _mazeSize.y; j++)
            {
                if (i == j)
                    list.Add(new Vector2Int(i, j));

                if (i == (_mazeSize.y - 1) - j)
                    list.Add(new Vector2Int(i, j));
            }
        }

        return list.Distinct().ToList();
    }

    private List<Vector2Int> GetRow(int row)
    {
        var list = new List<Vector2Int>();

        for (int i = 0; i < _mazeSize.x; i++)
        {
            for (int j = 0; j < _mazeSize.y; j++)
            {
                if (j == row)
                    list.Add(new Vector2Int(i, j));
            }
        }

        return list.Distinct().ToList();
    }

    private List<Vector2Int> GetColumn(int column)
    {
        var list = new List<Vector2Int>();

        for (int i = 0; i < _mazeSize.x; i++)
        {
            for (int j = 0; j < _mazeSize.y; j++)
            {
                if (i == column)
                    list.Add(new Vector2Int(i, j));
            }
        }

        return list.Distinct().ToList();
    }

    private void AdjustCubeHeight(float height, params Vector2Int[] cubesIndex)
    {
        Vector3 parentPosition = transform.position;
        Vector3 cubeScale = _nodePrefab.transform.localScale;

        foreach (var index in cubesIndex)
        {
            var node = m_nodes[new(index.x, index.y)];
            var position = node.transform.position;
            position.y = parentPosition.y - cubeScale.y / 2f + height;
            node.transform.position = position;
        }
    }

    private void CalculatePaths(Vector2Int startIndex)
    {
        List<MazeNode> currentPath = new();
        List<MazeNode> completedPath = new();

        var startNode = m_nodes[new(startIndex.x, startIndex.y)];

        startNode.SetState(MazeNodeState.Current);
        currentPath.Add(startNode);

        int weight = 0;
        startNode.SetWeight(weight);

        DisableStartNodeWall(startIndex);

        while (completedPath.Count < m_nodes.Count)
        {
            var currentNode = currentPath[^1];
            var index = currentNode.Index;

            Vector2Int upIndex = new(index.x, index.y - 1);
            Vector2Int rightIndex = new(index.x + 1, index.y);
            Vector2Int downIndex = new(index.x, index.y + 1);
            Vector2Int leftIndex = new(index.x - 1, index.y);

            var upNode = m_nodes.ContainsKey(upIndex) ? m_nodes[upIndex] : null;
            var rightNode = m_nodes.ContainsKey(rightIndex) ? m_nodes[rightIndex] : null;
            var downNode = m_nodes.ContainsKey(downIndex) ? m_nodes[downIndex] : null;
            var leftNode = m_nodes.ContainsKey(leftIndex) ? m_nodes[leftIndex] : null;

            Dictionary<MazeNodeDirection, MazeNode> possibleNodes = new();

            if (upNode != null)
            {
                if (!currentPath.Contains(upNode) &&
                    !completedPath.Contains(upNode))
                {
                    possibleNodes.Add(MazeNodeDirection.Up, upNode);
                }
            }
            if (rightNode != null)
            {
                if (!currentPath.Contains(rightNode) &&
                    !completedPath.Contains(rightNode))
                {
                    possibleNodes.Add(MazeNodeDirection.Right, rightNode);
                }
            }
            if (downNode != null)
            {
                if (!currentPath.Contains(downNode) &&
                    !completedPath.Contains(downNode))
                {
                    possibleNodes.Add(MazeNodeDirection.Down, downNode);
                }
            }
            if (leftNode != null)
            {
                if (!currentPath.Contains(leftNode) &&
                    !completedPath.Contains(leftNode))
                {
                    possibleNodes.Add(MazeNodeDirection.Left, leftNode);
                }
            }

            if (!possibleNodes.IsEmpty())
            {
                var directions = possibleNodes.Keys.ToArray();
                directions.Shuffle();
                var direction = directions[0];

                var nextNode = possibleNodes[direction];
                nextNode.SetState(MazeNodeState.Current);
                currentPath.Add(nextNode);

                weight++;
                nextNode.SetWeight(weight);

                currentNode.DisableWallByDirection(direction);
                nextNode.DisableWallByDirection(direction.InverseDirection());
            }
            else
            {
                var lastNode = currentPath[^1];

                lastNode.SetState(MazeNodeState.Completed);
                completedPath.Add(lastNode);

                weight--;

                currentPath.RemoveAt(currentPath.Count - 1);
            }
        }

        ShowStartEndNodes(completedPath);
    }

    private void ShowStartEndNodes(List<MazeNode> completedPath)
    {
        var startNode = completedPath[^1];
        var farNodes = completedPath.FindAll(x => x.OneWallRemoved()).OrderByDescending(x => x.Weight).ToList();
        var endNode = farNodes[Random.Range(0, 3)];

        endNode.SetColor(Color.green);

        startNode.SetInteraction(Interact);
        endNode.SetInteraction(InverseInteract);
    }

    public override void Interact()
    {
        m_mazeStarted = true;

        base.Interact();
    }

    public override void InverseInteract()
    {
        Manager_Events.GameManager.AddCoin.Notify();
        
        m_mazeSolved = true;

        foreach (var node in m_nodes)
            node.Value.EnableInteraction(false);

        base.InverseInteract();
    }

    private void DisableStartNodeWall(Vector2Int startIndex)
    {
        var direction = MazeNodeDirection.Up;

        if (startIndex.x == 0 && startIndex.y == 0)
        {
            direction = Random.Range(0, 2) == 0 ? MazeNodeDirection.Up : MazeNodeDirection.Left;
        }
        else if (startIndex.x == _mazeSize.x - 1 && startIndex.y == _mazeSize.y - 1)
        {
            direction = Random.Range(0, 2) == 0 ? MazeNodeDirection.Down : MazeNodeDirection.Right;
        }
        else if (startIndex.x == 0 && startIndex.y == _mazeSize.y - 1)
        {
            direction = Random.Range(0, 2) == 0 ? MazeNodeDirection.Down : MazeNodeDirection.Left;
        }
        else if (startIndex.x == _mazeSize.x - 1 && startIndex.y == 0)
        {
            direction = Random.Range(0, 2) == 0 ? MazeNodeDirection.Up : MazeNodeDirection.Right;
        }
        else if (startIndex.x == 0)
        {
            direction = MazeNodeDirection.Left;
        }
        else if (startIndex.x == _mazeSize.x - 1)
        {
            direction = MazeNodeDirection.Right;
        }
        else if (startIndex.y == 0)
        {
            direction = MazeNodeDirection.Up;
        }
        else if (startIndex.y == _mazeSize.y - 1)
        {
            direction = MazeNodeDirection.Down;
        }

        m_nodes[new(startIndex.x, startIndex.y)].DisableWallByDirection(direction);
    }

    private IEnumerator ReleaseCoroutine(float duration = 0.5f)
    {
        float time = 0f;
        var position = _mazeParent.position;

        while (time <= duration)
        {
            time += Time.deltaTime;
            var t = time / duration;
            var lerp = Mathf.Lerp(0f, -3f, t);

            position.y = lerp;
            _mazeParent.position = position;
            yield return null;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController _))
            return;

        if (m_mazeStarted)
            base.InverseInteract();

        if (!m_mazeSolved)
            return;

        StartGame();

        MonoBehaviourHelper.StartCoroutine(OnTriggerExitCoroutine());
    }

    private IEnumerator OnTriggerExitCoroutine(float duration = 0.5f)
    {
        float time = 0f;
        var position = _mazeParent.position;

        while (time <= duration)
        {
            time += Time.deltaTime;
            var t = time / duration;
            var lerp = Mathf.Lerp(-3f, 0f, t);

            position.y = lerp;
            _mazeParent.position = position;
            yield return null;
        }

        m_mazeSolved = false;
        m_mazeStarted = false;
    }

}
