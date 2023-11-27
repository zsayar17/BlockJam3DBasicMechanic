using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class LevelManager
{
    static MyGrid[] grids;
    static int level = 1;
    static int playingLevel = 1;

    static public int Level {
        get { return level; }
    }

    public static void NextLevel() {
        level++;
        if (level <= 3) {
            playingLevel = level;
        }
        else playingLevel = Random.Range(1, 3);
    }

    public static MyGrid DesignLevel() {
        if (grids == null) InitGrid();

        ObjectManager.Instance.DeallocateObjects<Character>();
        ObjectManager.Instance.DeallocateObjects<CharacterSpawner>();
        ObjectManager.Instance.DeallocateObjects<Component>();

        ResetGrid();
        switch (playingLevel) {
            case 1:
                Level1(grids[playingLevel - 1]);
                break;
            case 2:
                Level2(grids[playingLevel - 1]);
                break;
            case 3:
                Level3(grids[playingLevel - 1]);
                break;
            default:
                break;
        }

        StackManager.Instance.CreateStack();
        ObjectManager.Instance.CheckObjects(grids[playingLevel - 1]);
        return grids[playingLevel - 1];
    }

    static void Level1(MyGrid grid) {
        int             x, y;
        List<Color>     colors;

        colors = new List<Color>();
        FillColorList(colors, Color.red, Color.yellow, Color.green, Color.blue);

        PutWallsToAround(grid);
        y = (int)grid.Size.y - 2;

        //first row
        x = 0;
        ObjectManager.Instance.AllocateObject<Component>(grid.GetNode(++x, y), Color.white);
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors));
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors));
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors));
        ObjectManager.Instance.AllocateObject<Component>(grid.GetNode(++x, y), Color.white);

        //second row
        x = 1;
        y--;
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors));
        ObjectManager.Instance.AllocateObject<CharacterSpawner>(grid.GetNode(++x, y), Color.black).Allocate(grid.GetNode(x, y - 1),
            GetColor(colors), GetColor(colors), GetColor(colors), GetColor(colors));
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors));

        //third row
        x = 1;
        y--;
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors));
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors));
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors));
    }

    static void Level2(MyGrid grid) {
        int         x, y;
        List<Color> colors;

        colors = new List<Color>();
        FillColorList(colors, Color.red, Color.yellow, Color.blue, Color.green);

        PutWallsToAround(grid);
        y = (int)grid.Size.y - 2;

        //first row
        x = 0;
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors)).InBarrier = true;
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors)).InBarrier = true;
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors)).InBarrier = true;
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors)).InBarrier = true;

        //second row
        x = 0;
        y--;
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors)).InBarrier = true;
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors)).InBarrier = true;
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors)).InBarrier = true;
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors)).InBarrier = true;

        //third row
        x = 0;
        y--;
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors));
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors));
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors));
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors));
    }

    static void Level3(MyGrid grid) {
        int         x, y;
        List<Color> colors;

        colors = new List<Color>();
        FillColorList(colors, Color.red, Color.red, Color.yellow, Color.yellow, Color.yellow, Color.blue, Color.blue, Color.blue);

        PutWallsToAround(grid);
        y = (int)grid.Size.y - 2;

        //first row
        x = 0;
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors));
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors));
        ObjectManager.Instance.AllocateObject<Component>(grid.GetNode(++x, y), Color.white);
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors));
        ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(++x, y), GetColor(colors));

        //other rows
        for (int i = 0; i < 4; i++) {
            y--;
            for (x = 1; x <= 5; x++)
                ObjectManager.Instance.AllocateObject<Character>(grid.GetNode(x, y), GetColor(colors));
        }
    }

    static void PutWallsToAround(MyGrid grid) {
        int y;

        for (int i = 0; i < grid.Size.x; i++) {
            y = (int)grid.Size.y - 1;
            for (int j = y; j >= 0; j--) {
                if (i == 0 || i == grid.Size.x - 1 || j == grid.Size.y - 1)
                    ObjectManager.Instance.AllocateObject<Component>(grid.GetNode(i, j), Color.white);
            }
        }
    }

    static void FillColorList(List<Color> colorList, params Color[] colors) {
        foreach (var color in colors) {
            for (int i = 0; i < 3; i++) colorList.Add(color);
        }
    }

    static Color GetColor(List<Color> colors) {
        Color color;

        color = colors[Random.Range(0, colors.Count)];
        colors.Remove(color);
        return color;
    }

    static void InitGrid() {
        Transform transform;

        transform = GameObject.Find("GameManager").transform;
        grids = new MyGrid[3];

        grids[0] = MyGrid.CreateNewGrid(new Vector2(7, 5), transform.position);
        grids[1] = MyGrid.CreateNewGrid(new Vector2(6, 5), transform.position);
        grids[2] = MyGrid.CreateNewGrid(new Vector2(7, 7), transform.position);
    }

    static void ResetGrid() {
        foreach (MyGrid grid in grids) {
            for (int x = 0; x < grid.Size.x; x++) {
                for (int y = 0; y < grid.Size.y; y++) grid.GetNode(x, y).walkable = true;
            }
        }
    }

    public static MyGrid GetCurrentGrid() {
        return grids[playingLevel - 1];
    }
}
