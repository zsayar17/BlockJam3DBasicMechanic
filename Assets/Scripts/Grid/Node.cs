using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Node {

    public bool walkable; // Is the node walkable?
    public Vector3 worldPosition; // Position in the world
    public int gridX, gridY; // Position in the grid
    public int gCost, hCost; // gCost = distance from starting node, hCost = distance from target node
    public Node parent; // Parent node

    public int fCost { // fCost = gCost + hCost
        get {
            return gCost + hCost;
        }
    }

    // Constructor
    public Node (bool _walkable, Vector3 _worldPosition, int _x, int _y) {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _x;
        gridY = _y;
    }
}
