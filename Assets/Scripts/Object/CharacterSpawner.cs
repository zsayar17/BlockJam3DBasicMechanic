using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSpawner : MonoBehaviour
{
    public Node node;
    Node spawnNode;
    List<Character> characters;
    TextMeshPro textMeshPro;

    void Awake() {
        //textMeshPro = gameObject.AddComponent<TextMeshPro>();
        //textMeshPro.fontSize = 24;
        //textMeshPro.alignment = TextAlignmentOptions.Top;
        //textMeshPro.renderMode = TextRenderFlags.Render;
    }

    public void Allocate(Node allocatedNode, params Color[] colors) {
        Character character;

        characters = new List<Character>();
        spawnNode = allocatedNode;

        foreach (Color color in colors) {
            character = ObjectManager.Instance.AllocateObject<Character>(spawnNode, color, false);
            characters.Add(character);
        }
        //textMeshPro.text = characters.Count.ToString();
    }

    public void Spawn() {
        Character character;

        if (!spawnNode.walkable || characters.Count == 0) return;


        character = characters[Random.Range(0, characters.Count)];
        characters.Remove(character);

        //textMeshPro.text = characters.Count.ToString();
        character.gameObject.SetActive(true);
        character.node.walkable = false;
    }
}
