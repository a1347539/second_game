using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace NetCodeTest
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
        public NetworkVariable<Vector2> Position = new NetworkVariable<Vector2>();
        public NetworkVariable<Color> playerColor = new NetworkVariable<Color>();
        public Rigidbody2D rb;

        public override void OnNetworkSpawn()
        {
            rb = GetComponent<Rigidbody2D>();
            if (IsLocalPlayer)
            {
                print(NetworkManager.Singleton.LocalClientId);
            }
        }

        void Update()
        {
            if (!IsLocalPlayer) { return; }
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                changePositionServerRpc(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                rb.MovePosition(rb.position + Position.Value * 5f * Time.fixedDeltaTime);
            }
        }

        [ServerRpc]
        public void setColorServerRpc(Color color)
        {
            playerColor.Value = color;
        }

        [ServerRpc]
        private void changePositionServerRpc(float x, float y)
        {
            Vector2 newPos = new Vector2(x, y);
            Position.Value = newPos;
        }

        private void OnEnable()
        {
            playerColor.OnValueChanged += onColorChanged;
            Position.OnValueChanged += onPositionChanged;
        }

        private void OnDisable()
        {
            playerColor.OnValueChanged -= onColorChanged;
            Position.OnValueChanged -= onPositionChanged;
        }

        private void onPositionChanged(Vector2 previousValue, Vector2 newValue)
        {
            rb.MovePosition(rb.position + Position.Value * 5f * Time.fixedDeltaTime);
        }

        private void onColorChanged(Color previousValue, Color newValue)
        {
            GetComponent<Renderer>().material.SetColor("_Color", newValue);
        }
    }
}