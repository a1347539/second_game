using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowsMovement : MonoBehaviour
{
    public bool mouseHeldDown { get {
        return MouseManager.Instance.mouseHeldDown;
    } }
    CharacterMovement characterMovement;


    // Start is called before the first frame update
    void Start() {
        transform.localScale = new Vector3(0, 1, 1);

        characterMovement = GetComponentInParent<CharacterMovement>();
    }

    // Update is called once per frame
    void Update() {
        if (mouseHeldDown) {
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
