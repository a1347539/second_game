using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InGamePlayerInteractionData
{
    public const float DashDamage = 10f;

    public const float EnergyRechargeRate = 1.5f;

    public enum ReceivedState { 
        initialization,
        onHit,
    };

    public enum ReceivedHitBy {
        initialization,
        dash,
        weapon,
        skill
    };
}
