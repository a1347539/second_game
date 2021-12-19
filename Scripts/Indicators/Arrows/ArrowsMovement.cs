using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowsMovement : MonoBehaviour
{

    bool mouseHeldDown = false;
    bool isFirstTouchRecorded = false;
    Vector3 onMouseDownPosition;
    Vector3 mousePosition;
    float angleFromTouchToCurrent;
    private Camera cam;
    private int vDistance = 0;
    private int hDistance = 0;


    // Start is called before the first frame update
    void Start() {
        transform.localScale = new Vector3(0, 1, 1);
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        mouseClick();

        if (mouseHeldDown) {
            mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            if (!isFirstTouchRecorded) {
                onMouseDownPosition = mousePosition;
                isFirstTouchRecorded = true;
            }

            angleFromTouchToCurrent = 
                Mathf.Atan2(mousePosition.y-onMouseDownPosition.y, 
                mousePosition.x-onMouseDownPosition.x)*180 / Mathf.PI;
            angleFromTouchToCurrent = Mathf.RoundToInt(angleFromTouchToCurrent*2f/180f);
            transform.eulerAngles = Vector3.forward * angleFromTouchToCurrent*90f;
            if (Mathf.Abs(angleFromTouchToCurrent) == 1) {
                vDistance = Mathf.RoundToInt(onMouseDownPosition.y - mousePosition.y);
                transform.localScale = new Vector3(Mathf.Abs(vDistance), 1, 1);
            } else {
                hDistance = Mathf.RoundToInt(onMouseDownPosition.x - mousePosition.x);
                transform.localScale = new Vector3(Mathf.Abs(hDistance), 1, 1);
            }
        }
    }

    void mouseClick() {
        if (Input.GetMouseButtonDown(0)) {
            mouseHeldDown = true;
            vDistance = 0;
            hDistance = 0;
            isFirstTouchRecorded = false;
        } else if (Input.GetMouseButtonUp(0)) {
            mouseHeldDown = false;
            transform.localScale = new Vector3(0, 1, 1);
            transform.parent.GetComponent<CharacterMovement>().move(
                transform.parent.transform.position, 
                vDistance*-1, 
                hDistance*-1);
        }
    }
}
