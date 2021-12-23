using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    [SerializeField] private EnergyBar energyBar;
    public int maxHealth { get; private set; }
    public float maxEnergy { get; private set; }
    public float energy { get; private set; }
    public int health { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 10;
        maxEnergy = 100;
        energy = 10;
        health = 10;
        energyBar.updateEnergy(energy);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isMaxEnergy() {
        return (energy >= maxEnergy);
    }

    public void rechargeEnergy(float energyIncrease, bool nonNatural=false) {
        energy+=energyIncrease;
        if (nonNatural) {
            return;
        }
        energyBar.updateEnergy(energy);
    }

    public void deductEnergy(int usedEnergy) {
        energy-=usedEnergy;
        energyBar.updateEnergy(energy);
    }
}
