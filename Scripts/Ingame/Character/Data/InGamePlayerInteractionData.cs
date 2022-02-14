using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InGamePlayerInteractionData
{
    public const float DashDamage = 10f;

    public const float EnergyRechargeRate = 1.5f;

    public static readonly string[] facingDirections = { "IdleUp", "IdleRight", "IdleDown", "IdleLeft" };

    public static readonly string[] facingDirectionsWhenCharging = { "ChargingUp", "ChargingRight", "ChargingDown", "ChargingLeft" };

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

    public enum FacingDirection { 
        up, 
        right, 
        down, 
        left
    };
}
