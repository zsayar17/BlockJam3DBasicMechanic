using System;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

[Serializable]
public struct GridVars {
    public Vector2 gridWorldSize; // Size of the grid
    public LayerMask unwalkableMask; // LayerMask to determine what is unwalkable
    public float nodeRadius; // Radius of the node

    [HideInInspector] public Vector3 position; // position of the grid
}

public class MyGrid
{
    Vector2 gridWorldSize; // Size of the grid
    LayerMask unwalkableMask; // LayerMask to determine what is unwalkable
    float nodeRadius; // Radius of the node
    Vector3 position; // Position of the grid

    float nodeDiameter; // Diameter of the node
    int gridSizeX, gridSizeY; // Size of the grid in nodes
    Node[,] grid; // 2D array of nodes
    PathFinding pathFinding; // PathFinding object

    Node bottomNode;
    public Node BottomNode {
        get {
            if (bottomNode == null) bottomNode = GetBottomNode();
            return bottomNode;
        }
    }

    public Vector2 Size {
        get {
            return new Vector2(gridSizeX, gridSizeY);
        }
    }

    public MyGrid(GridVars gridVars) {
        gridWorldSize = gridVars.gridWorldSize;
        unwalkableMask = gridVars.unwalkableMask;
        nodeRadius = gridVars.nodeRadius;
        position = gridVars.position;
        CreateGrid();

        pathFinding = new PathFinding(this);
    }

    public static MyGrid CreateNewGrid(Vector2 gridCount, Vector3 position) {
        GridVars gridVars;

        gridVars = new GridVars();
        gridVars.gridWorldSize = gridCount;
        gridVars.nodeRadius = 0.5f;
        gridVars.unwalkableMask = LayerMask.GetMask("Unwalkable");
        gridVars.position = position;
        return new MyGrid(gridVars);
    }

    void CreateGrid() {
        Vector3 worldBottomLeft;
        Vector3 worldPoint;
        bool    walkable;

        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        // Create 2D array of nodes
        grid = new Node[gridSizeX, gridSizeY];

        worldBottomLeft = position - (Vector3.right * gridWorldSize.x / 2) - (Vector3.forward * gridWorldSize.y / 2);
        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                worldPoint = worldBottomLeft + (Vector3.right * ((x * nodeDiameter) + nodeRadius)) + (Vector3.forward * ((y * nodeDiameter) + nodeRadius));
                walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node GetNode(int x, int y) {
        return grid[x, y];
    }

    Node GetBottomNode() {
        int x, y;

        y = 0;
        x = Mathf.RoundToInt(gridSizeX / 2);

        for (int i = 0; i < gridSizeX / 2; i++) {
            if (grid[x - i, y].walkable) return grid[x - i, y];
            if (grid[x + i, y].walkable) return grid[x + i, y];
        }

        return grid[0, 0];
    }

    public List<Node> GetNeighbours(Node node) {
        List<Node> neigbours = new List<Node>();
        int checkX, checkY;

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) continue;
                if (x != 0 && y != 0) continue; // If the node is diagonal to the current node, skip it (we don't want diagonal movement)
                checkX = x + node.gridX;
                checkY = y + node.gridY;

                if (checkX < 0 || checkX >= gridSizeX || checkY < 0 || checkY >= gridSizeY) continue;
                neigbours.Add(grid[checkX, checkY]);
            }
        }

        return neigbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition) {
        float x, y;

        x = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        y = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        x = Mathf.Clamp01(x);
        y = Mathf.Clamp01(y);

        x = (gridSizeX - 1) * x;
        y = (gridSizeY - 1) * y;

        return grid[Mathf.RoundToInt(x), Mathf.RoundToInt(y)];
    }
    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos) {
        return pathFinding.FindPath(startPos, targetPos);
    }

    public void DrawGizmos() {
        Gizmos.DrawWireCube(position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        for (int x = 0; x < grid.GetLength(0); x++) {
            for (int y = 0; y < grid.GetLength(1); y++) {
                Gizmos.color = grid[x, y].walkable ? Color.white : Color.red;
                Gizmos.DrawCube(grid[x, y].worldPosition, Vector3.one * nodeDiameter);
            }
        }
    }
}
