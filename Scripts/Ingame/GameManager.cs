using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance => instance;
    private static GameManager instance;

    public event Action onSpawnerReadied;

    [SerializeField] Transform playerContainer;
    [SerializeField] Transform spawnerContainer;
    public Transform getPlayerContainer() { return playerContainer; }
    public Transform getSpawnerContainer() { return spawnerContainer; }
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject spawner;

    private List<List<GameObject>> spawnerObjects;

    public int[][][] spawnPoints { get; private set; }
    
    private void Awake() {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        spawnerObjects = new List<List<GameObject>>();
        spawnPoints = new int[2][][];
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = new int[2][][];
        spawnPoints[0] = MapBuilder.Instance.mapData.spawnA;
        spawnPoints[1] = MapBuilder.Instance.mapData.spawnB;
        initSpawnPoint();
        onSpawnerReadied?.Invoke();
    }

    private void initSpawnPoint() {

        for (int i = 0; i < spawnPoints.Length; i++) {
            spawnerObjects.Add(new List<GameObject>());
            for (int j = 0; j < spawnPoints[i].Length; j++) {
                GameObject s = Instantiate(spawner);
                spawnerSetup(s, $"Spawner{(char)(i+65)}_{j}", spawnPoints[i][j]);
                spawnerObjects[i].Add(s);
            }
        }
    }

    private void spawnerSetup(GameObject spawner, string name, int[] point) {
        spawner.transform.position = (new Point(point[0], point[1])).toWorldPosition();
        spawner.name = name;
        spawner.transform.parent = spawnerContainer;
    }

    public void spawnPlayer(ulong clientId, string playerName, int playerIndex) {
        if (!IsOwner) { return; }
        spawnPlayerServerRpc(clientId, playerName, playerIndex);
    }

    [ServerRpc]
    private void spawnPlayerServerRpc(ulong clientId, string playerName, int playerIndex)
    {
        int teamIndex = 0;
        if (playerIndex < 2)
        { // A team
            teamIndex = 0;
        }
        else if (playerIndex < 4)
        { // B team
            teamIndex = 1;
        }

        NetworkObject playerInstance = Instantiate(
                playerPrefab,
                spawnerObjects[teamIndex][playerIndex].transform.position,
                Quaternion.identity
            ).GetComponent<NetworkObject>();

        InGamePlayerState newInGamePlayer = new InGamePlayerState(
            clientId,
            playerName,
            playerInstance.NetworkObjectId,
            teamIndex,
            playerIndex,
            new Point(spawnPoints[teamIndex][playerIndex][0], spawnPoints[teamIndex][playerIndex][1])
        );
        InGameGlobalDataManager.Instance.inGamePlayers.Add(newInGamePlayer);
        InGameGlobalDataManager.Instance.playerObjectDict[clientId] = playerInstance;
        playerInstance.SpawnAsPlayerObject(clientId);
        playerInstance.transform.SetParent(playerContainer);
    }
}
