using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;
using System.Text;

public class ClientManager : Singleton<ClientManager>
{
    public DisconnectReason disconnectReason { get; private set; }
    public event Action<ConnectStatus> onConnectionFinished;
    public event Action onNetworkTimedOut;
    private HostManager hostLoginManager;

    private void Awake()
    {
        disconnectReason = new DisconnectReason();
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        hostLoginManager = GetComponent<HostManager>();
        hostLoginManager.onNetworkReadied += handleOnNetworkReadied;
        hostLoginManager.onConnectionFinished += handleOnConnectionFinished;
        hostLoginManager.onDisconnectionReasonReceived += handleOnDisconnectionReasonReceived;
        NetworkManager.Singleton.OnClientDisconnectCallback += handleOnClientDisconnect;
    }

    private void OnDestroy()
    {
        if (hostLoginManager == null) { return; }
        hostLoginManager.onNetworkReadied -= handleOnNetworkReadied;
        hostLoginManager.onConnectionFinished -= handleOnConnectionFinished;
        hostLoginManager.onDisconnectionReasonReceived -= handleOnDisconnectionReasonReceived;

        if (NetworkManager.Singleton == null) { return; }
        NetworkManager.Singleton.OnClientDisconnectCallback -= handleOnClientDisconnect;
    }

    public void startClient() {
        string payload = JsonUtility.ToJson(new ConnectionPayload()
        {
            clientGUID = Guid.NewGuid().ToString(),
            clientScene = SceneManager.GetActiveScene().buildIndex,
            playerName = PlayerPrefs.GetString("PlayerName", "Missing Name")
        });

        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        NetworkManager.Singleton.StartClient();
    }

    private void handleOnNetworkReadied() {
        if (!NetworkManager.Singleton.IsClient) { return; }
        if (!NetworkManager.Singleton.IsHost) {
            hostLoginManager.onUserDisconnectRequested += handleOnUserDisconnectRequested;
        }
    }

    private void handleOnUserDisconnectRequested() {
        disconnectReason.setDisconnectionReason(ConnectStatus.UserRequestedDisconnect);
        handleOnClientDisconnect(NetworkManager.Singleton.LocalClientId);
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("SceneLogin");
        
    }

    private void handleOnConnectionFinished(ConnectStatus status) {
        if (status != ConnectStatus.Success) {
            disconnectReason.setDisconnectionReason(status);
        }
        onConnectionFinished?.Invoke(status);
    }

    private void handleOnDisconnectionReasonReceived(ConnectStatus status) {
        disconnectReason.setDisconnectionReason(status);
    }

    private void handleOnClientDisconnect(ulong clientId) {
        if (!NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsHost) {
            hostLoginManager.onUserDisconnectRequested -= handleOnUserDisconnectRequested;

            if (SceneManager.GetActiveScene().name != "SceneLogin")
            {
                if (!disconnectReason.hasTransitionReason)
                {
                    disconnectReason.setDisconnectionReason(ConnectStatus.GenericDisconnect);
                }
                SceneManager.LoadScene("SceneLogin");
            }
            else {
                onNetworkTimedOut?.Invoke();
            }
        }
    }
}
