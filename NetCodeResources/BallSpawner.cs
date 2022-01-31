using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BallSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject ballPrefab;

    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }

        if (Input.GetMouseButtonDown(0)) {
            spawnBallServerRpc(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    [ServerRpc]
    private void spawnBallServerRpc(Vector2 spawnPosition) {
        NetworkObject ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
        ball.SpawnWithOwnership(OwnerClientId);
    }
}
