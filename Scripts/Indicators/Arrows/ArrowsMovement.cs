using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowsMovement : MonoBehaviour
{
    Vector3 onMouseDownPosition;
    Vector3 mousePosition;
    float angleFromTouchToCurrent;
    private int vDistance = 0;
    private int hDistance = 0;
    CharacterControl CharacterControl;
    CharacterData characterData;


    // Start is called before the first frame update
    void Start() {
        transform.localScale = new Vector3(0, 1, 1);
        CharacterControl = GetComponentInParent<CharacterControl>();
        characterData = GetComponentInParent<CharacterData>();
        
    }

    // Update is called once per frame
    void Update() {
        if (CharacterControl.mouseHeldDown) {
            onMouseDownPosition = CharacterControl.onMouseDownPosition;
            mousePosition = CharacterControl.mousePosition;
            angleFromTouchToCurrent = 
                Mathf.Atan2(mousePosition.y-onMouseDownPosition.y, 
                mousePosition.x-onMouseDownPosition.x)*180 / Mathf.PI;
            angleFromTouchToCurrent = Mathf.RoundToInt(angleFromTouchToCurrent*2f/180f);
            transform.eulerAngles = Vector3.forward * angleFromTouchToCurrent*90f;
            if (Mathf.Abs(angleFromTouchToCurrent) == 1) {
                vDistance = Mathf.RoundToInt(onMouseDownPosition.y - mousePosition.y);
                vDistance = Mathf.Clamp(vDistance, (int)characterData.energy/10*-1, (int)(characterData.energy/10));
                transform.localScale = new Vector3(Mathf.Abs(vDistance), 1, 1);
            } else {
                hDistance = Mathf.RoundToInt(onMouseDownPosition.x - mousePosition.x);
                hDistance = Mathf.Clamp(hDistance, (int)characterData.energy/10*-1, (int)(characterData.energy/10));
                transform.localScale = new Vector3(Mathf.Abs(hDistance), 1, 1);
            }
        }
    }

    public void arrowOnButtonDown() {
        vDistance = 0;
        hDistance = 0;
    }

    public void arrowOnButtonUp() {
        transform.localScale = new Vector3(0, 1, 1);
        transform.parent.GetComponent<CharacterMovement>().move(
            transform.parent.transform.position, 
            vDistance*-1, 
            hDistance*-1);
    }
}
