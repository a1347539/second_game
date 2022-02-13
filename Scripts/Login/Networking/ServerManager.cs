using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;
using System.Text;

public class ServerManager : Singleton<ServerManager>
{
    private const int MaxConnectionPayload = 1024;

    [SerializeField] private int maxPlayerCount = 4;
    private Dictionary<string, PlayerData> clientData;
    private Dictionary<ulong, string> clientIdToGuid;
    private Dictionary<ulong, int> clientSceneMap;
    public PlayerData? getPlayerData(ulong clientId)
    {
        if (clientIdToGuid.TryGetValue(clientId, out string clientGuid))
        {
            if (clientData.TryGetValue(clientGuid, out PlayerData playerData))
            {
                return playerData;
            }
        }
        return null;
    }

    private bool gameInProgress;

    private HostManager hostLoginManager;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        hostLoginManager = GetComponent<HostManager>();
        hostLoginManager.onNetworkReadied += handleOnNetworkReadied;

        NetworkManager.Singleton.ConnectionApprovalCallback += approvalCheck;
        NetworkManager.Singleton.OnServerStarted += handleOnServerStarted;

        clientData = new Dictionary<string, PlayerData>();
        clientIdToGuid = new Dictionary<ulong, string>();
        clientSceneMap = new Dictionary<ulong, int>();
    }

    private void OnDestroy()
    {
        if (hostLoginManager == null) { return; }
        hostLoginManager.onNetworkReadied -= handleOnNetworkReadied;

        if (NetworkManager.Singleton == null) { return; }
        NetworkManager.Singleton.ConnectionApprovalCallback -= approvalCheck;
        NetworkManager.Singleton.OnServerStarted -= handleOnServerStarted;
    }

    private void handleOnNetworkReadied() {
        if (!NetworkManager.Singleton.IsServer) { return; }
        hostLoginManager.onUserDisconnectRequested += handleOnUserDisconnectRequested;
        NetworkManager.Singleton.OnClientDisconnectCallback += handleOnClientDisconnect;
        hostLoginManager.onClientSceneChanged += handleOnClientSceneChanged;

        NetworkManager.Singleton.SceneManager.LoadScene("SceneLobby", LoadSceneMode.Single);
        if (NetworkManager.Singleton.IsHost) {
            clientSceneMap[NetworkManager.Singleton.LocalClientId] = SceneManager.GetActiveScene().buildIndex;
        }
    }

    private void handleOnClientDisconnect(ulong clientId) {
        clientSceneMap.Remove(clientId);
        if (clientIdToGuid.TryGetValue(clientId, out string guid)) {
            clientIdToGuid.Remove(clientId);
            if (clientData[guid].clientId == clientId) {
                clientData.Remove(guid);
            }
        }
        if (clientId == NetworkManager.Singleton.LocalClientId) {
            hostLoginManager.onUserDisconnectRequested -= handleOnUserDisconnectRequested;
            NetworkManager.Singleton.OnClientDisconnectCallback -= handleOnClientDisconnect;
            hostLoginManager.onClientSceneChanged -= handleOnClientSceneChanged;
        }
    }

    private void handleOnClientSceneChanged(ulong clientId, int sceneIndex) {
        clientSceneMap[clientId] = sceneIndex;
    }

    private void handleOnUserDisconnectRequested() {
        handleOnClientDisconnect(NetworkManager.Singleton.LocalClientId);
        NetworkManager.Singleton.Shutdown();
        clearData();
        SceneManager.LoadScene("SceneLogin");
    }

    private void handleOnServerStarted() {
        if (!NetworkManager.Singleton.IsHost) { return; }
        string clientGuid = Guid.NewGuid().ToString();
        string playerName = PlayerPrefs.GetString("PlayerName", "Missing Name");
        clientData.Add(clientGuid, new PlayerData(playerName, NetworkManager.Singleton.LocalClientId));
        clientIdToGuid.Add(NetworkManager.Singleton.LocalClientId, clientGuid);
    }

    private void clearData()
    {
        clientData.Clear();
        clientIdToGuid.Clear();
        clientSceneMap.Clear();
        gameInProgress = false;
    }

    public void startGame() {
        gameInProgress = true;
        NetworkManager.Singleton.SceneManager.LoadScene("SceneInGame", LoadSceneMode.Single);

    }

    public void endGame() {
        gameInProgress = false;
        NetworkManager.Singleton.SceneManager.LoadScene("SceneLobby", LoadSceneMode.Single);
    }

    private void approvalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        if (connectionData.Length > MaxConnectionPayload) {
            callback(false, 0, false, null, null);
            return;
        }
        if (clientId == NetworkManager.Singleton.LocalClientId) {
            callback(false, null, true, null, null);
            return;
        }

        string payload = Encoding.UTF8.GetString(connectionData);
        ConnectionPayload connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);

        ConnectStatus gameReturnStatus = ConnectStatus.Success;

        if (gameInProgress)
        {
            gameReturnStatus = ConnectStatus.GameInProgress;
        }
        else if (clientData.Count >= maxPlayerCount)
        {
            gameReturnStatus = ConnectStatus.ServerFull;
        }

        if (gameReturnStatus == ConnectStatus.Success) {
            clientSceneMap[clientId] = connectionPayload.clientScene;
            clientIdToGuid[clientId] = connectionPayload.clientGUID;
            clientData[connectionPayload.clientGUID] = new PlayerData(connectionPayload.playerName, clientId);
        }

        callback(false, 0, true, null, null);
        hostLoginManager.serverToClientConnectResult(clientId, gameReturnStatus);

        if (gameReturnStatus != ConnectStatus.Success) {
            StartCoroutine(WaitToDisconnectClien(clientId, gameReturnStatus));
        }
    }

    private IEnumerator WaitToDisconnectClien(ulong clientId, ConnectStatus status) {
        hostLoginManager.serverToClientSetDisconnectReason(clientId, status);
        yield return new WaitForSeconds(0);
        kickClient(clientId);
    }

    private void kickClient(ulong clientId) {
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        if (networkObject != null) {
            networkObject.Despawn(true);
        }
        NetworkManager.Singleton.DisconnectClient(clientId);
    }
}
