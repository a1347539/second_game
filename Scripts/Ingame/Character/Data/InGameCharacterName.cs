using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;
using System;

public class InGameCharacterName : NetworkBehaviour
{
    [SerializeField] private TMP_Text nameText;

    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        setName($"Player {OwnerClientId}");
    }

    public void setName(string name) {
        if (!IsOwner) { return; }
        setNameServerRpc(name);
    }

    private void Update()
    {

    }

    private void OnEnable()
    {
        playerName.OnValueChanged += handleOnNameChanged;
    }

    private void OnDisable()
    {
        playerName.OnValueChanged -= handleOnNameChanged;
    }

    private void handleOnNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        nameText.text = newValue.ToString();
    }

    [ServerRpc]
    private void setNameServerRpc(string name) {
        this.playerName.Value = name;
    }
    
}
