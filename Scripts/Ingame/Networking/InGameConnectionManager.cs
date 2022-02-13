using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class InGameConnectionManager : NetworkBehaviour
{
    public static InGameConnectionManager Instance => instance;
    private static InGameConnectionManager instance;

    private void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += handleOnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += handleOnClientDisconnected;
            GameManager.Instance.onSpawnerReadied += handleOnSpawnerReadied;
        }
    }


    private void OnDestroy()
    {
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= handleOnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= handleOnClientDisconnected;
        }
        if (GameManager.Instance) {
            GameManager.Instance.onSpawnerReadied -= handleOnSpawnerReadied;
        }
    }

    private void handleOnSpawnerReadied() {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            handleOnClientConnected(client.ClientId);
        }
    }

    private void handleOnClientConnected(ulong clientId)
    {
        PlayerData? playerData = ServerManager.Instance.getPlayerData(clientId);
        if (!playerData.HasValue) { return; }
        GameManager.Instance.spawnPlayer(clientId, playerData.Value.playerName, InGameGlobalDataManager.Instance.inGamePlayers.Count);
    }

    private void handleOnClientDisconnected(ulong clientId)
    {
        for (int i = 0; i < InGameGlobalDataManager.Instance.inGamePlayers.Count; i++)
        {
            if (InGameGlobalDataManager.Instance.inGamePlayers[i].clientId == clientId)
            {
                InGameGlobalDataManager.Instance.inGamePlayers.RemoveAt(i);
                break;
            }
        }
    }
}
