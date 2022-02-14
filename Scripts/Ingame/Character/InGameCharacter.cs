using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class InGameCharacter : NetworkBehaviour
{
    [SerializeField] Canvas attachedContainer;
    [SerializeField] ParticleSystem smokeParticle;
    public NetworkVariable<InGamePlayerInteractionData.ReceivedState> receivedState =
        new NetworkVariable<InGamePlayerInteractionData.ReceivedState>();

    public NetworkVariable<InGamePlayerInteractionData.ReceivedHitBy> receivedHitBy =
        new NetworkVariable<InGamePlayerInteractionData.ReceivedHitBy>();

    public NetworkVariable<float> damageTaken = new NetworkVariable<float>();

    public NetworkVariable<Point> point = new NetworkVariable<Point>();
    public NetworkVariable<int> teamIndex = new NetworkVariable<int>();
    public NetworkVariable<bool> isInteractable = new NetworkVariable<bool>();

    public InGameCharacterData characterData;
    public InGameCharacterAnimationManager animationManager;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        playerInitServerRpc();
        characterData = GetComponent<InGameCharacterData>();
        animationManager = GetComponent<InGameCharacterAnimationManager>();
        updatePositionServerRpc(transform.position);
        receivedState.OnValueChanged += handleOnReceivedDataFromServer;
        point.OnValueChanged += handleOnPointChanged;
    }

    private void Start()
    {
    }

    private void OnDestroy()
    {
        receivedState.OnValueChanged -= handleOnReceivedDataFromServer;
        point.OnValueChanged -= handleOnPointChanged;
    }

    private void Update()
    {
        if (!IsOwner) { return; }
    }

    private void handleOnReceivedDataFromServer(InGamePlayerInteractionData.ReceivedState previousValue, InGamePlayerInteractionData.ReceivedState newValue)
    {
        switch ((int)newValue) {
            case 0:
                break;
            case 1:
                onHit();
                break;
            default:
                break;
        }
    }

    private void handleOnPointChanged(Point oldValue, Point newValue) {
        setIsInteractableServerRpc(true);
        unhidePlayerServerRpc();
    }

    // called by server
    public void onHit() {
        switch (receivedHitBy.Value)
        {
            case InGamePlayerInteractionData.ReceivedHitBy.dash:
                spawnSmokeParticleServerRpc();
                characterData.deductHealth(damageTaken.Value);
                StartCoroutine(respawn());
                animationManager.setIdleAnimation(2);
                break;
            case InGamePlayerInteractionData.ReceivedHitBy.weapon:
                break;
            case InGamePlayerInteractionData.ReceivedHitBy.skill:
                break;
            default:
                break;
        }
    }


    private IEnumerator respawn()
    {
        hidePlayerServerRpc();
        setIsInteractableServerRpc(false);
        yield return new WaitForSecondsRealtime(1f);
        onHitByDashServerRpc();
    }

    public void updatePointAndPosition(Point newPoint) {
        updatePointServerRpc(newPoint);
        updatePositionServerRpc(newPoint.toWorldPosition());
    }

    [ServerRpc]
    private void playerInitServerRpc()
    {
        foreach (InGamePlayerState state in InGameGlobalDataManager.Instance.inGamePlayers)
        {
            if (state.clientId == OwnerClientId)
            {
                point.Value = state.initialPoint;
                teamIndex.Value = state.teamIndex;
                isInteractable.Value = true;
                receivedState.Value = 0;
                receivedHitBy.Value = 0;
            }
        }
    }

    [ServerRpc]
    private void spawnSmokeParticleServerRpc() {
        spawnSmokeParticleClientRpc();
    }

    [ServerRpc]
    private void updatePointServerRpc(Point newPoint) {
        point.Value = newPoint;
    }

    [ServerRpc]
    private void updatePositionServerRpc(Vector3 newPosition) {
        transform.position = newPosition;
    }

    [ServerRpc]
    private void setIsInteractableServerRpc(bool value) {
        isInteractable.Value = value;
    }

    [ServerRpc]
    private void hidePlayerServerRpc() {
        hidePlayerClientRpc();
        /*if (InGameGlobalDataManager.Instance.playerObjectDict.TryGetValue(clientId, out NetworkObject networkObject))
        {
            networkObject.GetComponent<InGameCharacter>().attachedContainer.enabled = false;
            networkObject.GetComponent<Renderer>().enabled = false;
        }*/
    }

    [ServerRpc]
    private void unhidePlayerServerRpc() {
        unhidePlayerClientRpc();
    }

    [ServerRpc]
    public void onHitByDashServerRpc() {
        Tiles respawnTile = MapBuilder.Instance.getRandomWalkableTile(); 
        transform.position = respawnTile.point.toWorldPosition();
        point.Value = respawnTile.point;
    }

    [ClientRpc]
    private void hidePlayerClientRpc()
    {
        attachedContainer.enabled = false;
        GetComponent<Renderer>().enabled = false;
    }

    [ClientRpc]
    private void unhidePlayerClientRpc()
    {
        attachedContainer.enabled = true;
        GetComponent<Renderer>().enabled = true;
    }

    [ClientRpc]
    private void spawnSmokeParticleClientRpc() {
        Instantiate(smokeParticle, transform.position, Quaternion.identity);
    }
}
