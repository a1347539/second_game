using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class VisualEffectSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject myParticle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }

        if (Input.GetKeyDown(KeyCode.Space)) { 
            SpawnParticleServerRpc();
            Instantiate(myParticle, transform.position, transform.rotation);
        }
    }

    [ServerRpc(Delivery = RpcDelivery.Unreliable)]
    private void SpawnParticleServerRpc() {
        SpawnParticleClientRpc();
    }

    [ClientRpc(Delivery = RpcDelivery.Unreliable)]
    private void SpawnParticleClientRpc() {
        if (IsOwner) { return; }
        Instantiate(myParticle, transform.position, transform.rotation);
    }
}
