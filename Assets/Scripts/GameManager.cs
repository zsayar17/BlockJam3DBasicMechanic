using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public enum GameState {
    Playing,
    LevelCompleted,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameState gameState;

    public static int characterSpeed;
    public int characterBeginSpeed;

    public static float stackDistanceByGrid;
    public float stackDistance;

    public static string CharacterObjectName = "Characters";
    public static string SpawnerObjectName = "Spawners";
    public static string WallObjectName = "Walls";
    MyGrid grid;
    TMPro.TextMeshProUGUI text;

    void Awake() {
        text = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        gameState = GameState.Playing;
        characterSpeed = characterBeginSpeed;
        stackDistanceByGrid = stackDistance;
        grid = LevelManager.DesignLevel();
        text.text = "Level: " + LevelManager.Level;

        DontDestroyOnLoad(GameObject.Find(CharacterObjectName));
        DontDestroyOnLoad(GameObject.Find(SpawnerObjectName));
        DontDestroyOnLoad(GameObject.Find(WallObjectName));
    }

    void Update() {
        OnClick();
        CheckLevelState();
    }

    /*void OnDrawGizmos() {
        if (grid != null) grid.DrawGizmos();
    }*/

    void CheckLevelState() {
        if (gameState == GameState.Playing) return;

        if (gameState == GameState.LevelCompleted) {
            LevelManager.NextLevel();
            grid = LevelManager.DesignLevel();
            text.text = "Level: " + LevelManager.Level;
            gameState = GameState.Playing;
            SceneManager.LoadSceneAsync(2);
        } else {
            grid = LevelManager.DesignLevel();
            gameState = GameState.Playing;
            SceneManager.LoadSceneAsync(3);
        }
    }

    public void OnClick() {
        Ray ray;
        RaycastHit hit;
        Character character;

        if (Input.GetMouseButtonDown(0) && gameState == GameState.Playing) {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                character = hit.collider.gameObject.GetComponent<Character>();
                if (character != null && character.Moveable && character.path == null && !character.IsStacking) {
                    character.path = grid.FindPath(character.node.worldPosition, grid.BottomNode.worldPosition);
                }
            }
        }
    }
}
