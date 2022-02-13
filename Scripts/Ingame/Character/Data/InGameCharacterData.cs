using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class InGameCharacterData : NetworkBehaviour
{
    public int maxHealth { get; private set; } = 100;
    public float maxEnergy { get; private set; } = 100;

    public NetworkVariable<float> health = new NetworkVariable<float>();

    public NetworkVariable<float> energy = new NetworkVariable<float>();

    private float localEnergy;

    private InGameCharacterCanvas characterCanvas;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        characterCanvas = GetComponent<InGameCharacterCanvas>();
        energy.OnValueChanged += handleOnEnergyChanged;
    }

    void Start() {
        if (!IsOwner) { return; }
        localEnergy = maxEnergy;
        updatePlayerHealthServerRpc(maxHealth);
        updatePlayerEnergyServerRpc(maxEnergy);

    }

    private void OnDestroy()
    {
        energy.OnValueChanged -= handleOnEnergyChanged;
    }

    private void handleOnEnergyChanged(float oldValue, float newValue) {
        if (oldValue > newValue) { characterCanvas.updateEnergy(newValue, false); }
        else { characterCanvas.updateEnergy(newValue, true); }
    }

    public void deductHealth(float healthDecrement) {
        updatePlayerHealthServerRpc(health.Value - healthDecrement);
    }

    public bool isMaxEnergy() {
        return (energy.Value >= maxEnergy);
    }

    public void rechargeEnergy(float energyIncrease, bool nonNatural=false) {
        localEnergy += energyIncrease;
        updatePlayerEnergyServerRpc(localEnergy);
        if (nonNatural)
        {
            // do something else
            return;
        }
    }

    public void deductEnergy(int usedEnergy) {
        localEnergy -= usedEnergy;
        updatePlayerEnergyServerRpc(localEnergy);
    }

    [ServerRpc]
    private void updatePlayerEnergyServerRpc(float newEnergy) {
        energy.Value = newEnergy;
    }

    [ServerRpc]
    private void updatePlayerHealthServerRpc(float newHealth)
    {
        health.Value = newHealth;
        updatePlayerHealthBarClientRpc(health.Value);
    }

    [ClientRpc]
    private void updatePlayerHealthBarClientRpc(float health) {
        GetComponent<InGameCharacterCanvas>().updateHealth(health);
    }
}
