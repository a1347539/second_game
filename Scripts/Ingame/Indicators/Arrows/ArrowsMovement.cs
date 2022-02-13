using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ArrowsMovement : NetworkBehaviour
{
    InGameCharacterMovement characterMovement;
    InGamePlayerControllerManager controllerManager;


    // Start is called before the first frame update
    void Start() {
        if (!IsOwner) { return; }
        controllerManager = GetComponentInParent<InGamePlayerControllerManager>();
        characterMovement = GetComponentInParent<InGameCharacterMovement>();
        transform.localScale = new Vector3(0, 1, 1);
    }

    // Update is called once per frame
    void Update() {
        if (!IsOwner) { return; }
        if (controllerManager.mouseHeldDown) {
            transform.eulerAngles = Vector3.forward * characterMovement.angleFromTouchToCurrent*90f;
            if (characterMovement.hDistance != 0) {
                transform.localScale = new Vector3(
                    Mathf.Abs(characterMovement.hDistance), 1, 1);
            } else {
                transform.localScale = new Vector3(
                    Mathf.Abs(characterMovement.vDistance), 1, 1);
            }
        }
    }

    public void arrowOnButtonUp() {
        transform.localScale = new Vector3(0, 1, 1);
    }
}
