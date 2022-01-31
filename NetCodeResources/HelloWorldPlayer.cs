using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class HelloWorldPlayer : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    private NetworkVariable<Color> playerColor = new NetworkVariable<Color>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Move();
        }
    }

    public void Move()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Vector3 randomPosition = GetRandomPositionOnPlane();
            transform.position = randomPosition;
            Position.Value = randomPosition;
        }
        else
        {
            SubmitPositionRequestServerRpc();
        }
    }

    [ServerRpc]
    public void setColorServerRpc(Color color) {
        playerColor.Value = color;
    }

    private void OnEnable() {
        playerColor.OnValueChanged += onColorChanged;
    }

    private void OnDisable() {
        playerColor.OnValueChanged -= onColorChanged;
    }

    private void onColorChanged(Color previousValue, Color newValue)
    {
        if (!IsClient) { return; }
        GetComponent<Renderer>().material.SetColor("_Color", newValue);
    }

    [ServerRpc]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value = GetRandomPositionOnPlane();
    }

    static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 1f);
    }

    void Update()
    {
        transform.position = Position.Value;
    }
}
