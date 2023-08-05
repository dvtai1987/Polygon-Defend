using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField] Vector2Int startCoordinates;
    [SerializeField] Vector2Int destinationCoordinates;


    Node startNode;
    Node destNode;
    Node currentSearchNode;

    Queue<Node> frontier = new Queue<Node>();
    Dictionary<Vector2Int, Node> reached = new Dictionary<Vector2Int, Node>();

    Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
    GridManager gridManager;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();

        if (gridManager != null)
        {
            grid = gridManager.Grid;
        }

        GetNewPath();
    }

    void Start()
    {
        startNode = gridManager.Grid[startCoordinates];
        destNode = gridManager.Grid[destinationCoordinates];


    }

    public List<Node> GetNewPath()
    {
        gridManager.ResetNodes();
        BreadFirstSearch();
        return BuildPath();
    }

    void ExploreNeighbors()
    {
        List<Node> neighbors = new List<Node>();

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborCoordinates = currentSearchNode.coordinates + direction;

            if (grid.ContainsKey(neighborCoordinates))
            {
                neighbors.Add(grid[neighborCoordinates]);
            }
        }

        foreach (Node neighbor in neighbors)
        {
            if (!reached.ContainsKey(neighbor.coordinates) && neighbor.isWalkable)
            {
                neighbor.connectedTo = currentSearchNode;
                reached.Add(neighbor.coordinates, neighbor);
                frontier.Enqueue(neighbor);
            }
        }
    }

    void BreadFirstSearch()
    {
        frontier.Clear();
        reached.Clear();

        bool isRunning = true;

        frontier.Enqueue(startNode);
        reached.Add(startCoordinates, startNode);

        while (frontier.Count > 0 && isRunning)
        {
            currentSearchNode = frontier.Dequeue();
            currentSearchNode.isExplored = true;
            ExploreNeighbors();

            if (currentSearchNode.coordinates == destinationCoordinates)
            {
                isRunning = false;
            }
        }
    }

    List<Node> BuildPath()
    {
        List<Node> path = new List<Node>();
        Node currentnode = destNode;

        while (currentnode.connectedTo != null)
        {
            currentnode = currentnode.connectedTo;
            path.Add(currentnode);
            currentnode.isPath = true;
        }

        path.Reverse();

        return path;
    }

    public bool WillBlockPath(Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates))
        {
            bool previousState = grid[coordinates].isWalkable;

            grid[coordinates].isWalkable = false;
            List<Node> newPath = GetNewPath();
            grid[coordinates].isWalkable = previousState;

            if (newPath.Count <= 1)
            {
                GetNewPath();
                return true;
            }
        }
        return false;
    }
}
