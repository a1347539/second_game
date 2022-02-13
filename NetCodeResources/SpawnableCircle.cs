using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

namespace NetCodeTest
{
    public class SpawnableCircle : NetworkBehaviour
    {
        private NetworkVariable<Color> ballColor = new NetworkVariable<Color>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!IsOwner) { return; }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                destroyBallServerRpc();
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                changeBallColorServerRpc();
            }
        }

        private void OnEnable()
        {
            ballColor.OnValueChanged += onBallValueChanged;
        }

        private void OnDisable()
        {
            ballColor.OnValueChanged -= onBallValueChanged;
        }

        private void onBallValueChanged(Color previousValue, Color newValue)
        {
            GetComponent<SpriteRenderer>().color = newValue;
        }

        [ServerRpc]
        private void changeBallColorServerRpc()
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(OwnerClientId, out NetworkClient networkClient))
            {
                ballColor.Value = networkClient.PlayerObject.GetComponent<Renderer>().material.GetColor("_Color");
            }
        }

        [ServerRpc]
        private void destroyBallServerRpc()
        {
            Destroy(gameObject);
        }
    }
}