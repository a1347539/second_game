using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System;

public class LobbyUI : NetworkBehaviour
{
    [SerializeField] private PlayerContainer[] lobbyPlayerCards;
    [SerializeField] private Button startGameButton;
    private NetworkList<LobbyPlayerState> lobbyPlayers;

    private void Awake()
    {
        lobbyPlayers = new NetworkList<LobbyPlayerState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient) {
            lobbyPlayers.OnListChanged += handleOnLobbyPlayerStatesChanged;
        }
        if (IsServer) {
            startGameButton.gameObject.SetActive(true);
            NetworkManager.Singleton.OnClientConnectedCallback += handleOnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += handleOnClientDisconnected;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList) {
                handleOnClientConnected(client.ClientId);
            }
        }
    }

    private void Start() {
        // debug
        // onStartGameClicked();
    }

    private void OnDestroy()
    {
        base.OnDestroy();
        lobbyPlayers.OnListChanged -= handleOnLobbyPlayerStatesChanged;
        if (NetworkManager.Singleton) {
            NetworkManager.Singleton.OnClientConnectedCallback -= handleOnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= handleOnClientDisconnected;
        }
    }

    private void handleOnLobbyPlayerStatesChanged(NetworkListEvent<LobbyPlayerState> changeEvent)
    {
        for (int i = 0; i < lobbyPlayerCards.Length; i++) {
            if (lobbyPlayers.Count > i)
            {
                lobbyPlayerCards[i].updateDisplay(lobbyPlayers[i]);
            }
            else {
                lobbyPlayerCards[i].disableDisplay();
            }
        }
        if (IsHost) {
            startGameButton.interactable = isEveryoneReady();
        }
    }

    private void handleOnClientConnected(ulong clientId) {
        PlayerData? playerData = ServerManager.Instance.getPlayerData(clientId);

        if (!playerData.HasValue) { return; }
        lobbyPlayers.Add(new LobbyPlayerState(
            clientId,
            playerData.Value.playerName,
            false
        ));
    }

    private void handleOnClientDisconnected(ulong clientId) {
        for (int i = 0; i < lobbyPlayers.Count; i++) {
            if (lobbyPlayers[i].clientId == clientId) {
                lobbyPlayers.RemoveAt(i);
                break;
            }
        }
    }

    private bool isEveryoneReady()
    {
        return true; // debug
        if (lobbyPlayers.Count < 2) {
            return false;
        }
        foreach (LobbyPlayerState playerState in lobbyPlayers) {
            if (!playerState.isReady) { return false; }
        }
        return true;
    }

    public void onLeaveClicked() {
        HostManager.Instance.RequestDisconnect();
    }

    public void onReadyClicked() {
        updateIsPlayerReadyServerRpc();
    }

    public void onStartGameClicked() {
        startGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void updateIsPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        for (int i = 0; i < lobbyPlayers.Count; i++) {
            if (lobbyPlayers[i].clientId == serverRpcParams.Receive.SenderClientId) {
                lobbyPlayers[i] = new LobbyPlayerState(
                    lobbyPlayers[i].clientId,
                    lobbyPlayers[i].playerName,
                    !lobbyPlayers[i].isReady
                );
            }
        }
    }

    [ServerRpc]
    private void startGameServerRpc(ServerRpcParams serverRpcParams = default) 
    {
        if (serverRpcParams.Receive.SenderClientId != NetworkManager.Singleton.LocalClientId) { return; }
        if (!isEveryoneReady()) { return; }
        ServerManager.Instance.startGame();
    }
}
