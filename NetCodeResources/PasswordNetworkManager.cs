using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Text;
using UnityEngine.UI;
//using HelloWorld;

namespace NetCodeTest
{
    public class PasswordNetworkManager : MonoBehaviour
    {
        [SerializeField] private InputField passwordInputField;
        [SerializeField] private InputField playerNameInputField;
        [SerializeField] private GameObject passwordEntryUI;
        [SerializeField] private GameObject leaveButton;

        private static Dictionary<ulong, PlayerData> clientData;

        public static PlayerData GetPlayerData(ulong clientId)
        {
            if (clientData.TryGetValue(clientId, out PlayerData playerdata))
            {
                return playerdata;
            }
            return null;
        }

        private void Start()
        {
            NetworkManager.Singleton.OnServerStarted += handleServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback += handleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += handleClientDisconnect;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton == null) { return; }

            NetworkManager.Singleton.OnServerStarted -= handleServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback -= handleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= handleClientDisconnect;
        }

        private byte[] getPayloadBytes(bool isClient)
        {
            return Encoding.ASCII.GetBytes(JsonUtility.ToJson(new ConnectionPayload(
                    isClient ? PlayerRole.Client : PlayerRole.Host,
                    passwordInputField.text,
                    playerNameInputField.text
                )));

        }

        public void Host()
        {
            clientData = new Dictionary<ulong, PlayerData>();
            clientData[NetworkManager.Singleton.LocalClientId] = new PlayerData(playerNameInputField.text);

            NetworkManager.Singleton.NetworkConfig.ConnectionData = getPayloadBytes(false);
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.StartHost();
        }

        public void Client()
        {
            NetworkManager.Singleton.NetworkConfig.ConnectionData = getPayloadBytes(true);
            NetworkManager.Singleton.StartClient();
        }

        public void Leave()
        {
            NetworkManager.Singleton.Shutdown();

            if (NetworkManager.Singleton.IsServer)
            {
                NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
            }
            passwordEntryUI.SetActive(true);
            leaveButton.SetActive(false);
        }

        private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
        {

            string payload = Encoding.ASCII.GetString(connectionData);
            ConnectionPayload connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);

            if ((int)connectionPayload.playerRole == 1)
            {
                callback(true, null, true, null, null);
                return;
            }

            string password = connectionPayload.password;
            bool approveConnection = (password == passwordInputField.text);

            Vector3 spawnPos = Vector3.zero;
            Quaternion spawnRotation = Quaternion.identity;

            if (approveConnection)
            {
                clientData[clientId] = new PlayerData(connectionPayload.playerName);
            }

            switch (NetworkManager.Singleton.ConnectedClients.Count)
            {
                case 1:
                    spawnPos = new Vector3(0f, 0f, 0f);
                    spawnRotation = Quaternion.Euler(0f, 180f, 0f);
                    break;
                case 2:
                    spawnPos = new Vector3(0f, 0f, 0f);
                    spawnRotation = Quaternion.Euler(0f, 255f, 0f);
                    break;


            }
            callback(true, null, approveConnection, new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f)), Quaternion.identity);
        }

        private void handleServerStarted()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                handleClientConnected(NetworkManager.Singleton.ServerClientId);
            }
        }

        private void handleClientConnected(ulong clientId)
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                passwordEntryUI.SetActive(false);
                leaveButton.SetActive(true);
            }
        }

        private void handleClientDisconnect(ulong clientId)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                clientData.Remove(clientId);
            }

            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                passwordEntryUI.SetActive(true);
                leaveButton.SetActive(false);
            }
        }
    }
}