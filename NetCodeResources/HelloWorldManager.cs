using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

enum Mode{
    Host, Server, Client
}

namespace HelloWorld {
    public class HelloWorldManager : MonoBehaviour
    {
        static Mode mode;

        static void StartButtons() {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        static void StatusLabels() {
            mode = NetworkManager.Singleton.IsHost ? Mode.Host : 
                NetworkManager.Singleton.IsServer ? Mode.Server : Mode.Client;

            GUILayout.Label("Transport: " + 
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + ((Mode)mode).ToString());
        }

        public static void SubmitNewPosition() {
            if(GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change")) {
                if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient) {
                    foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds) {
                        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<HelloWorldPlayer>().Move();
                    }
                } else {
                    NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                    HelloWorldPlayer player = playerObject.GetComponent<HelloWorldPlayer>();
                    player.Move();
                }
            }
        }

        // private void OnGUI() {
        //     GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        //     if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) {
        //         StartButtons();
        //     }
        //     else {
        //         StatusLabels();
        //         SubmitNewPosition();
        //     }
        //     GUILayout.EndArea();
        // }
    }
}