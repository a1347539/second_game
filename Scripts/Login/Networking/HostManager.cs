using System;
using System.Collections;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;


public class HostManager : Singleton<HostManager>
{
    public event Action onNetworkReadied;
    public event Action<ConnectStatus> onConnectionFinished;
    public event Action<ConnectStatus> onDisconnectionReasonReceived;
    public event Action<ulong, int> onClientSceneChanged;
    public event Action onUserDisconnectRequested;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += handleOnNetworkReadied;
        NetworkManager.Singleton.OnClientConnectedCallback += handleOnClientConnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= handleOnNetworkReadied;
            NetworkManager.Singleton.OnClientConnectedCallback -= handleOnClientConnected;

            if (NetworkManager.Singleton.SceneManager != null)
            {
                NetworkManager.Singleton.SceneManager.OnSceneEvent -= handleOnSceneChanged;
            }

            if (NetworkManager.Singleton.CustomMessagingManager != null)
            {
                UnregisterClientMessageHandlers();
            }
        }
    }

    public void startHost() {
        NetworkManager.Singleton.StartHost();
        RegisterClientMessageHandlers();
    }

    public void RequestDisconnect() {
        onUserDisconnectRequested?.Invoke();
    }

    private void handleOnClientConnected(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) { return; }
        handleOnNetworkReadied();
        NetworkManager.Singleton.SceneManager.OnSceneEvent += handleOnSceneChanged;
    }

    private void handleOnNetworkReadied()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            onConnectionFinished?.Invoke(ConnectStatus.Success);
        }
        onNetworkReadied?.Invoke();
    }

    private void RegisterClientMessageHandlers()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("ServerToClientConnectResult", (senderClientId, reader) =>
        {
            reader.ReadValueSafe(out ConnectStatus status);
            onConnectionFinished?.Invoke(status);
        });

        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("ServerToClientSetDisconnectReason", (senderClientId, reader) =>
        {
            reader.ReadValueSafe(out ConnectStatus status);
            onDisconnectionReasonReceived?.Invoke(status);
        });
    }

    private void UnregisterClientMessageHandlers()
    {
        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("ServerToClientConnectResult");
        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("ServerToClientSetDisconnectReason");
    }

    private void handleOnSceneChanged(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType != SceneEventType.LoadComplete) { return; }
        onClientSceneChanged?.Invoke(sceneEvent.ClientId, SceneManager.GetSceneByName(sceneEvent.SceneName).buildIndex);
    }

    public void serverToClientConnectResult(ulong clientId, ConnectStatus status) {
        FastBufferWriter writer = new FastBufferWriter(sizeof(ConnectStatus), Allocator.Temp);
        writer.WriteValueSafe(status);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("ServerToClientConnectResult", clientId, writer);
    }

    public void serverToClientSetDisconnectReason(ulong clientId, ConnectStatus status) {
        var writer = new FastBufferWriter(sizeof(ConnectStatus), Allocator.Temp);
        writer.WriteValueSafe(status);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("ServerToClientSetDisconnectReason", clientId, writer);
    }
}
