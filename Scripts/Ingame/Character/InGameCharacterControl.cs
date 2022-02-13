using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public struct MaxDistanceStruct {
    public int top { get; private set; }
    public int right { get; private set; }
    public int down { get; private set; }
    public int left { get; private set; }
    public MaxDistanceStruct(int t, int r, int d, int l)
    {
        top = t; right = r; down = d; left = l;
    }
}

public class InGameCharacterControl : NetworkBehaviour
{
    [SerializeField] private ArrowsMovement arrowsMovement;
    public Vector3 onMouseDownPosition { get { 
        return controllerManager.onMouseDownPosition; 
        } }
    public Vector3 mousePosition { get {
        return controllerManager.mousePosition;
    } }
    public bool mouseHeldDown { get {
        return controllerManager.mouseHeldDown;
    } }
    private int regenPerTick = 1;
    private int previousTickValue = 0;

    public NetworkVariable<MaxDistanceStruct> maxDistanceFromSelf = new NetworkVariable<MaxDistanceStruct>();

    InGameCharacter character;
    InGameCharacterData characterData;
    InGameCharacterMovement characterMovement;
    InGamePlayerControllerManager controllerManager;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        character = GetComponent<InGameCharacter>();
        controllerManager = GetComponent<InGamePlayerControllerManager>();
        characterMovement = GetComponent<InGameCharacterMovement>();
        characterData = GetComponent<InGameCharacterData>();
        updateMaxDistanceFromSelfServerRpc(new MaxDistanceStruct(0, 0, 0, 0));
    }

    private void Start()
    {
        if (!IsOwner) { return; }
        controllerManager.onMouseDownEvent += handleOnMouseDown;
        controllerManager.onMouseReleasedEvent += handleOnMouseReleased;
        characterMovement.onPlayerMovedEvent += handleOnMoved;
    }

    // Update is called once per frame
    void Update() {
        if (!IsOwner) { return; }

        if (mouseHeldDown) {
            // check if is just pressing
            if (characterMovement.vDistance == 0 & characterMovement.hDistance == 0) {
                rechargeEnergy();
            }
        }
    }

    private void OnDestroy()
    {
        controllerManager.onMouseDownEvent -= handleOnMouseDown;
        controllerManager.onMouseReleasedEvent -= handleOnMouseReleased;
        characterMovement.onPlayerMovedEvent -= handleOnMoved;
    }

    private void handleOnMouseDown() {
        checkWalkableDistance();
    }

    private void handleOnMouseReleased() {
        arrowsMovement.arrowOnButtonUp();
        characterMovement.move();
    }

    private void handleOnMoved(Point previousValue, Point newValue)
    {
        if (!IsOwner) { return; }
        int pathInAxis; // 0 = horizontal, 1 = vertical
        int pathBeginAt; int pathEndAt;
        int lineAt;
        if (previousValue.x == newValue.x) {
            pathInAxis = 1;
            pathBeginAt = previousValue.y;
            pathEndAt = newValue.y;
            lineAt = newValue.x;
        }
        else {
            pathInAxis = 0;
            pathBeginAt = previousValue.x;
            pathEndAt = newValue.x;
            lineAt = newValue.y;
        }
        getOpponentInPathServerRpc(pathInAxis, pathBeginAt, pathEndAt, lineAt);
    }

    private void snapToGrid() {
        Vector3 position = new Vector3(
            Mathf.Floor(transform.position.x)+0.5f,
            Mathf.Floor(transform.position.y)+0.5f,
            transform.position.z
        );
        transform.position = position;
    }

    private void rechargeEnergy() {
        if (!characterData.isMaxEnergy())
        {
            if (NetworkManager.Singleton.ServerTime.Tick - previousTickValue >= regenPerTick)
            {
                previousTickValue = NetworkManager.Singleton.ServerTime.Tick;
                characterData.rechargeEnergy(InGamePlayerInteractionData.EnergyRechargeRate);
            }
        }
    }

    private void checkWalkableDistance()
    {
        int[] tempMaxDistance = new int[4];

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up);
        while (hit.collider.CompareTag("Player") && hit.collider.GetComponent<InGameCharacter>().teamIndex.Value != character.teamIndex.Value &&
            hit.collider.GetComponent<InGameCharacter>().isInteractable.Value)
        {

            hit = Physics2D.Raycast(hit.collider.transform.position, Vector2.up);

        }
        tempMaxDistance[0] = (int)(hit.transform.position.y - transform.position.y - 1);

        hit = Physics2D.Raycast(transform.position, Vector2.right);
        while (hit.collider.CompareTag("Player") && hit.collider.GetComponent<InGameCharacter>().teamIndex.Value != character.teamIndex.Value &&
            hit.collider.GetComponent<InGameCharacter>().isInteractable.Value)
        {
            hit = Physics2D.Raycast(hit.collider.transform.position, Vector2.right);
        }
        tempMaxDistance[1] = (int)(hit.transform.position.x - transform.position.x - 1);


        hit = Physics2D.Raycast(transform.position, Vector2.down);
        while (hit.collider.CompareTag("Player") && hit.collider.GetComponent<InGameCharacter>().teamIndex.Value != character.teamIndex.Value &&
            hit.collider.GetComponent<InGameCharacter>().isInteractable.Value)
        {

            hit = Physics2D.Raycast(hit.collider.transform.position, Vector2.down);

        }
        tempMaxDistance[2] = (int)(transform.position.y - hit.transform.position.y - 1);

        hit = Physics2D.Raycast(transform.position, Vector2.left);
        while (hit.collider.CompareTag("Player") && hit.collider.GetComponent<InGameCharacter>().teamIndex.Value != character.teamIndex.Value &&
            hit.collider.GetComponent<InGameCharacter>().isInteractable.Value)
        {

            hit = Physics2D.Raycast(hit.collider.transform.position, Vector2.left);
        }

        tempMaxDistance[3] = (int)(transform.position.x - hit.transform.position.x - 1);

        updateMaxDistanceFromSelfServerRpc(new MaxDistanceStruct(
            tempMaxDistance[0],
            tempMaxDistance[1],
            tempMaxDistance[2],
            tempMaxDistance[3]
        ));
    }

    [ServerRpc]
    private void updateMaxDistanceFromSelfServerRpc(MaxDistanceStruct maxDistanceStruct) {
        maxDistanceFromSelf.Value = maxDistanceStruct;
    }

    [ServerRpc]
    private void getOpponentInPathServerRpc(int pathInAxis, int pathBeginAt, int pathEndAt, int lineAt) {
        // 0 = horizontal, 1 = vertical
        InGameCharacter otherCharacter;

        List<ulong> targetClientId = new List<ulong>();

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
            }
        };


        foreach (KeyValuePair<ulong, NetworkObject> pair in InGameGlobalDataManager.Instance.playerObjectDict) {
            otherCharacter = pair.Value.GetComponent<InGameCharacter>();
            if (otherCharacter.OwnerClientId == OwnerClientId) { continue; }
            if (pathInAxis == 0)
            {
                if (otherCharacter.point.Value.y == lineAt)
                {
                    if (pathBeginAt <= otherCharacter.point.Value.x && otherCharacter.point.Value.x <= pathEndAt ||
                        pathEndAt <= otherCharacter.point.Value.x && otherCharacter.point.Value.x <= pathBeginAt)
                    {
                        otherCharacter.damageTaken.Value = InGamePlayerInteractionData.DashDamage;
                        otherCharacter.receivedHitBy.Value = (InGamePlayerInteractionData.ReceivedHitBy)1;
                        otherCharacter.receivedState.Value = (InGamePlayerInteractionData.ReceivedState)1;
                        // targetClientId.Add(otherCharacter.OwnerClientId);
                    }
                }
            }
            else {
                if (otherCharacter.point.Value.x == lineAt)
                {
                    if (pathBeginAt <= otherCharacter.point.Value.y && otherCharacter.point.Value.y <= pathEndAt ||
                        pathEndAt <= otherCharacter.point.Value.y && otherCharacter.point.Value.y <= pathBeginAt)
                    {
                        otherCharacter.damageTaken.Value = InGamePlayerInteractionData.DashDamage;
                        otherCharacter.receivedHitBy.Value = (InGamePlayerInteractionData.ReceivedHitBy)1;
                        otherCharacter.receivedState.Value = (InGamePlayerInteractionData.ReceivedState)1;
                        // targetClientId.Add(otherCharacter.OwnerClientId);
                    }
                }
            }
        }
        // clientRpcParams.Send.TargetClientIds = targetClientId.ToArray();
        // updatePlayerGotHitClientRpc(clientRpcParams);
    }

/*
    [ClientRpc]
    private void updatePlayerGotHitClientRpc(ClientRpcParams clientRpcParams = default) {
        GetComponent<InGameCharacter>().updateReceivedDataFromServerServerRpc(0, 0);
        // GetComponent<InGameCharacter>().onHit(0);
        // GetComponent<InGameCharacterData>().deductHealth(InGamePlayerInteractionData.DashDamage);
    }*/
}
