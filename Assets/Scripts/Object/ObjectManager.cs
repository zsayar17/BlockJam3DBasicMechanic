using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public struct ObjectVars {
    public GameObject parentObject;
    public List<GameObject> allObjects;
    public HashSet<GameObject> allocatedObjects;
    public int allocatedObjectsCount;
}

public class ObjectManager
{
    public int maxObjectSize;
    ObjectVars characterVars, spawnerVars, wallVars;

    static ObjectManager instance;
    public static ObjectManager Instance {
        get {
            if (instance == null) instance = new ObjectManager();
            return instance;
        }
    }

    public ObjectManager() {
        maxObjectSize = 25;

        CreateCharacters();
        CreateSpawners();
        CreateWalls();
    }

    void CreateCharacters() {
        InitVars(ref characterVars, GameManager.CharacterObjectName);
        CreateObjects<Character>(characterVars.allObjects, characterVars.parentObject, PrimitiveType.Sphere);
    }

    void CreateSpawners() {
        InitVars(ref spawnerVars, GameManager.SpawnerObjectName);
        CreateObjects<CharacterSpawner>(spawnerVars.allObjects, spawnerVars.parentObject, PrimitiveType.Cylinder);
    }

    void CreateWalls() {
        InitVars(ref wallVars, GameManager.WallObjectName);
        CreateObjects<Component>(wallVars.allObjects, wallVars.parentObject, PrimitiveType.Cube);
    }

    void InitVars(ref ObjectVars objectVars, string name) {
        objectVars.parentObject = new GameObject(name);
        objectVars.allObjects = new List<GameObject>();
        objectVars.allocatedObjects = new HashSet<GameObject>();
        objectVars.allocatedObjectsCount = 0;
    }

    public void CreateObjects<CostumComponent>(List<GameObject> list, GameObject parentObject, PrimitiveType primitiveType) where CostumComponent : Component {
        GameObject newObject;

        for (int i = 0; i < maxObjectSize; i++) {
            newObject = GameObject.CreatePrimitive(primitiveType);
            newObject.transform.parent = parentObject.transform;

            if (typeof(CostumComponent) != typeof(Character))newObject.transform.localScale = new Vector3(1, 0.5f, 1);
            else newObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            if (typeof(CostumComponent) != typeof(Component))newObject.AddComponent<CostumComponent>();
            newObject.SetActive(false);

            list.Add(newObject);
        }
    }

    public CostumType AllocateObject<CostumType>(Node node, Color color, bool active = true) {
        GameObject  _gObject;
        int         allocatedCount;

        if (typeof(CostumType) == typeof(Character)) {
            allocatedCount = characterVars.allocatedObjectsCount++;
            _gObject = characterVars.allObjects[allocatedCount];
            _gObject.GetComponent<Character>().node = node;
            _gObject.GetComponent<Character>().Color = color;
            _gObject.GetComponent<Character>().Speed = GameManager.characterSpeed;
            characterVars.allocatedObjects.Add(_gObject);
        } else if (typeof(CostumType) == typeof(CharacterSpawner)) {
            allocatedCount = spawnerVars.allocatedObjectsCount++;
            _gObject = spawnerVars.allObjects[allocatedCount];
            _gObject.GetComponent<CharacterSpawner>().node = node;
            spawnerVars.allocatedObjects.Add(_gObject);
        } else {
            allocatedCount = wallVars.allocatedObjectsCount++;
            _gObject = wallVars.allObjects[allocatedCount];
            wallVars.allocatedObjects.Add(_gObject);
        }

        node.walkable = false;
        _gObject.transform.position = node.worldPosition + Vector3.up * _gObject.transform.localScale.y;;
        _gObject.GetComponent<Renderer>().material.color = color;
        _gObject.SetActive(active);

        return _gObject.GetComponent<CostumType>();
    }

    public void DeallocateObjects<CostumType>() {
        HashSet<GameObject> allocatedCharacters;

        if (typeof(CostumType) == typeof(Character))
            allocatedCharacters = characterVars.allocatedObjects;
        else if (typeof(CostumType) == typeof(CharacterSpawner))
            allocatedCharacters = spawnerVars.allocatedObjects;
        else
            allocatedCharacters = wallVars.allocatedObjects;

        foreach (GameObject currentObject in allocatedCharacters.ToList())
            DeallocateObject<CostumType>(currentObject);
    }

    public void DeallocateObject<CostumType>(GameObject _object) {
        HashSet<GameObject> allocatedCharacters;
        Character           character;

        if (typeof(CostumType) == typeof(Character)) {
            allocatedCharacters = characterVars.allocatedObjects;
            characterVars.allocatedObjectsCount--;

            character = _object.GetComponent<Character>();
            character.Moveable = false;
            character.InBarrier = false;
            character.path = null;
            character.IsStacking = false;

            _object.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        } else if (typeof(CostumType) == typeof(CharacterSpawner)) {
            allocatedCharacters = spawnerVars.allocatedObjects;
            spawnerVars.allocatedObjectsCount = 0;

            _object.transform.localScale = new Vector3(1, 0.5f, 1);
        } else  {
            allocatedCharacters = wallVars.allocatedObjects;
            wallVars.allocatedObjectsCount = 0;

            _object.transform.localScale = new Vector3(1, 0.5f, 1);
        }

        _object.SetActive(false);
        allocatedCharacters.Remove(_object);
    }

    public void CheckObjects(MyGrid grid) {
        Character character;

        foreach (GameObject currentObject in characterVars.allocatedObjects) {
            character = currentObject.GetComponent<Character>();
            if (!character.Moveable) {
                if (grid.FindPath(character.node.worldPosition, grid.BottomNode.worldPosition) != null) {
                    if (character.InBarrier) character.InBarrier = false;
                    character.Moveable = true;
                }
            }
        }

        foreach (GameObject currentObject in spawnerVars.allocatedObjects) {
            currentObject.GetComponent<CharacterSpawner>().Spawn();
        }
    }

    public int GetAllocatedObjectCount<CostumType>() {
        if (typeof(CostumType) == typeof(Character)) return characterVars.allocatedObjectsCount;
        else if (typeof(CostumType) == typeof(CharacterSpawner)) return spawnerVars.allocatedObjectsCount;
        else return wallVars.allocatedObjectsCount;
    }
}
