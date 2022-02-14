using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class InGameCharacterAnimationManager : NetworkBehaviour
{
    private Animator animator;
    private InGamePlayerInteractionData.FacingDirection currentlyFacing;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        currentlyFacing = InGamePlayerInteractionData.FacingDirection.up;
    }

    public void changeFacingDirectionWhenCharging(int newState) {
        if ((int)currentlyFacing == newState) return;
        currentlyFacing = (InGamePlayerInteractionData.FacingDirection)newState;
        setAnimationServerRpc(InGamePlayerInteractionData.facingDirectionsWhenCharging[newState]);
    }

    public void changeOnMouseDownAnimation()
    {
        setAnimationServerRpc(InGamePlayerInteractionData.facingDirectionsWhenCharging[(int)currentlyFacing]);
    }

    public void setIdleAnimation(int newState = 0, bool provided = false) {
        if (!provided)
        {
            setAnimationServerRpc(InGamePlayerInteractionData.facingDirections[(int)currentlyFacing]);
        }
        else {
            currentlyFacing = (InGamePlayerInteractionData.FacingDirection)newState;
            setAnimationServerRpc(InGamePlayerInteractionData.facingDirections[newState]);
        }
    }

    [ServerRpc]
    private void setAnimationServerRpc(string newState) {
        setAnimationClientRpc(newState);
    }

    [ClientRpc]
    private void setAnimationClientRpc(string newState) {
        animator.Play(newState);
    }
}
