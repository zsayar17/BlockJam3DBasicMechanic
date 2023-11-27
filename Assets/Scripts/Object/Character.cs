using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [HideInInspector] public Node node;

    bool isMoveable;
    bool inBarrier;
    bool isStacking;
    public List<Node> path;

    Color color;
    float speed;
    public bool Moveable {
        get {
            return isMoveable;
        }
        set {
            isMoveable = value;
            if (isMoveable) transform.localScale += transform.localScale / 2;
        }
    }

    public bool InBarrier {
        get {
            return inBarrier;
        }
        set {
            inBarrier = value;
            if (inBarrier) gameObject.GetComponent<Renderer>().material.color = Color.cyan;
            else gameObject.GetComponent<Renderer>().material.color = color;
        }
    }

    public Color Color {
        set {
            color = value;
        }
        get {
            return color;
        }
    }

    public bool IsStacking {
        get {
            return isStacking;
        }
        set {
            isStacking = value;
        }
    }

    public float Speed {
        get {
            return speed;
        }
        set {
            speed = value;
        }
    }

    void Update() {
        Move();
    }

    void Move() {
        if (path == null) return;

        transform.position = Vector3.MoveTowards(transform.position, path[0].worldPosition, speed * Time.deltaTime);
        if (transform.position == path[0].worldPosition && !isStacking) {
            if (path[0].parent == node) {
                node.walkable = true;
                ObjectManager.Instance.CheckObjects(LevelManager.GetCurrentGrid());
            }
            if (path[0].gridY == 0) {
                path.Clear();
                StackManager.Instance.OrderStackNode(this);
                isStacking = true;
                speed *= 2;
            }
            else path.RemoveAt(0);
        }
        else if (transform.position == path[0].worldPosition) {
            path.Clear();
            StackManager.Instance.ControlStack();
        }

        if (path != null && path.Count == 0) path = null;
    }

}
