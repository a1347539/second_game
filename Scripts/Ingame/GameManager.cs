using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform playerContainer;
    [SerializeField] Transform spawnerContainer;
    [SerializeField] Character player_1;
    [SerializeField] GameObject spawner;
    public List<GameObject> spawnersA;
    public List<GameObject> spawnersB;

    public List<Character> character;
    private int[][] spawnA;
    private int[][] spawnB;
    private Vector3 origin;
    
    private void Awake() {
        character = new List<Character>();
        spawnersA = new List<GameObject>();
        spawnersB = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnA = MapBuilder.Instance.mapData.spawnA;
        spawnB = MapBuilder.Instance.mapData.spawnB;
        origin = MapBuilder.Instance.origin;
        initSpawnPoint();
        spawnPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void initSpawnPoint() {
        for (int i = 0; i < spawnA.Length; i++) {
            GameObject s = Instantiate(spawner);
            spawnerSetup(s, "SpawnerA_"+i.ToString(), spawnA[i]);
            spawnersA.Add(s);
        }
        for (int i = 0; i < spawnB.Length; i++) {
            GameObject s = Instantiate(spawner);
            spawnerSetup(s, "SpawnerB_"+i.ToString(), spawnB[i]);
            spawnersB.Add(s);
        }
    }

    private void spawnerSetup(GameObject spawner, string name, int[] point) {
        spawner.transform.position = new Vector3(
            MapBuilder.Instance.origin.x + MapBuilder.Instance.tileWidth * (point[0] - 0.5f),
            MapBuilder.Instance.origin.y - MapBuilder.Instance.tileHeight * (point[1] - 0.5f),
            0
        );
        spawner.name = name;
        spawner.transform.parent = spawnerContainer;
    }

    private void spawnPlayers() {
        Character newCharacter = Instantiate(player_1).GetComponent<Character>();
        newCharacter.setup(spawnersA[0].transform.position, spawnA[0], playerContainer);
        character.Add(newCharacter);
    }
}
