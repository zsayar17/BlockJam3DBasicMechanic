using System.Collections.Generic;
using UnityEngine;

public class StackManager
{
    static StackManager instance;

    MyGrid grid;
    List<Character> characters;

    static public StackManager Instance {
        get {
            if (instance == null) instance = new StackManager();
            return instance;
        }
    }

    public void CreateStack() {
        MyGrid currentGrid;
        Vector3 position;

        currentGrid = LevelManager.GetCurrentGrid();
        position = currentGrid.BottomNode.worldPosition - Vector3.forward * GameManager.stackDistanceByGrid;

        grid = MyGrid.CreateNewGrid(new Vector2(7, 1), position);

        characters = new List<Character>();
    }

    public void OrderStackNode(Character character) {
        int   x;

        x = 0;
        if (characters.Count == grid.Size.x)  return;

        while (x < characters.Count) {
            if (characters[x].Color == character.Color) {
                while (x < characters.Count && characters[x].Color == character.Color) x++;
                SlideStack(x);
                if (x >= characters.Count) characters.Add(character);
                else characters[x] = character;
                character.path = new List<Node> { grid.GetNode(x, 0) };

                return;
            }
            x++;
        }
        characters.Add(character);
        character.path = new List<Node> { grid.GetNode(x, 0) };
    }

    void SlideStack(int index) {
        Character character;

        for (int i = 6; i >= index; i--) {
            if (i >= characters.Count) continue;
            character = characters[i];
            if (i + 1 >= characters.Count) characters.Add(character);
            else characters[i + 1] = character;
            character.path = new List<Node> { grid.GetNode(i + 1, 0) };
        }
    }

    public void ControlStack() {
        int count;

        for (int i = 0; i < characters.Count; i++) {
            count = 0;
            while (i + count < characters.Count && characters[i].Color == characters[i + count].Color)
                count++;
            if (count == 3) {
                for (int j = 0; j < 3; j++) {
                    ObjectManager.Instance.DeallocateObject<Character>(characters[i].gameObject);
                    characters.RemoveAt(i);
                }
                for (int j = i; j < characters.Count; j++)
                    characters[j].path = new List<Node> { grid.GetNode(j, 0) };
            }
        }
        if (characters.Count == 7) GameManager.gameState = GameState.GameOver;
        else if (ObjectManager.Instance.GetAllocatedObjectCount<Character>() == 0)
            GameManager.gameState = GameState.LevelCompleted;
    }
}
