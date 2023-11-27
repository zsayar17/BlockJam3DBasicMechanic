using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    private MyGrid grid;

    public PathFinding(MyGrid _grid) {
        grid = _grid;
    }

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos) {
        Node            startNode, targetNode, currentNode;
        List<Node>      openSet;
        HashSet<Node>   closedSet;
        int             distance;

        openSet = new List<Node>();
        closedSet = new HashSet<Node>();

        startNode = grid.NodeFromWorldPoint(startPos);
        targetNode = grid.NodeFromWorldPoint(targetPos);
        openSet.Add(startNode);

        while (openSet.Count > 0) { // While there are nodes to check
            currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++) { // Find the node with the lowest fCost
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)) // If the fCost is lower or if the fCost is the same but the hCost is lower
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode) // If we reached the target node
                return RetracePath(startNode, targetNode);

            foreach (Node neighbour in grid.GetNeighbours(currentNode)) { // Check each neighbour of the current node
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue; // If the neighbour is not walkable or if it is in the closed set, skip it

                distance = GetDistance(neighbour, currentNode) + currentNode.gCost;
                if (distance < neighbour.gCost || !openSet.Contains(neighbour)) { // If the distance is lower than the neighbour's gCost or if the neighbour is not in the open set
                    neighbour.gCost = distance;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour); // If the neighbour is not in the open set, add it
                }
            }
        }

        return null;
    }

    List<Node> RetracePath(Node startNode, Node endNode) {
        List<Node> path;
        Node currentNode;

        path = new List<Node>();
        currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    int GetDistance(Node current, Node target) {
        int distx, disty;

        distx = Mathf.Abs(current.gridX - target.gridX);
        disty = Mathf.Abs(current.gridY - target.gridY);

        return distx > disty ? (14 * disty + 10 * (distx - disty)) : (14 * distx + 10 * (disty - distx));
    }
}
