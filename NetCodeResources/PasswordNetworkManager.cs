using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Text;
using UnityEngine.UI;
//using HelloWorld;

public class PasswordNetworkManager : MonoBehaviour
{
    [SerializeField] private InputField passwordInputField;
    [SerializeField] private GameObject passwordEntryUI;
    [SerializeField] private GameObject leaveButton;

    private void Start() {
        NetworkManager.Singleton.OnServerStarted += handleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += handleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += handleClientDisconnect;
    }

    private void OnDestroy() {
        if (NetworkManager.Singleton == null) { return; }

        NetworkManager.Singleton.OnServerStarted -= handleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= handleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= handleClientDisconnect;
    }

    public void Host() {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();
        //HelloWorldManager.SubmitNewPosition();
    }

    public void Client() {
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(passwordInputField.text);
        NetworkManager.Singleton.StartClient();
        //HelloWorldManager.SubmitNewPosition();
    }

    public void Leave() {
        if (NetworkManager.Singleton.IsHost) {
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        } else if (NetworkManager.Singleton.IsClient) {
            NetworkManager.Singleton.Shutdown();
        }
        passwordEntryUI.SetActive(true);
        leaveButton.SetActive(false);
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback) {

        string password = Encoding.ASCII.GetString(connectionData);

        bool approveConnection = (password == passwordInputField.text);

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;

        switch(NetworkManager.Singleton.ConnectedClients.Count) {
            case 1:
                spawnPos = new Vector3(0f, 0f, 0f);
                spawnRotation = Quaternion.Euler(0f, 180f, 0f);
                break;
            case 2:
                spawnPos = new Vector3(0f, 0f, 0f);
                spawnRotation = Quaternion.Euler(0f, 255f, 0f);
                break;

        }

        callback(true, null, approveConnection, null, null);
    }

    private void handleServerStarted() {
        if (NetworkManager.Singleton.IsHost) {
            handleClientConnected(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void handleClientConnected(ulong clientId) {
        if (clientId == NetworkManager.Singleton.LocalClientId) {
            passwordEntryUI.SetActive(false);
            leaveButton.SetActive(true);
        }
    }

    private void handleClientDisconnect(ulong clientId) {
        if (clientId == NetworkManager.Singleton.LocalClientId) {
            passwordEntryUI.SetActive(true);
            leaveButton.SetActive(false);
        }
    }
}
