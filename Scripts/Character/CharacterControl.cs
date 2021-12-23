using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    bool isFirstTouchRecorded = false;
    public Vector3 onMouseDownPosition{ get; private set; }
    public Vector3 position{ get; private set; }
    public Vector3 mousePosition{ get; private set; }
    ArrowsMovement arrowsMovement;
    private Camera cam;
    private float energyGenerationSpeed = 0.02f;
    private float energyGenerationInitalTime = 0f;


    public bool mouseHeldDown{ get; private set; }

    // Start is called before the first frame update
    void Start() {
        arrowsMovement = transform.GetChild(0).GetComponent<ArrowsMovement>();
        cam = Camera.main;
        mouseHeldDown = false;
        snapToGrid();
        
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
            if (Mathf.Abs((mousePosition-onMouseDownPosition).x) < 0.5 &&
                Mathf.Abs((mousePosition-onMouseDownPosition).y) < 0.5) {
                rechargeEnergy();
            }
        }
    }

    private void snapToGrid() {

        position = new Vector3(
            Mathf.Floor(transform.position.x)+0.5f,
            Mathf.Floor(transform.position.y)+0.5f,
            transform.position.z
        );
        transform.position = position;
    }

    void mouseClick() {
        if (Input.GetMouseButtonDown(0)) {
            mouseHeldDown = true;
            isFirstTouchRecorded = false;
            arrowsMovement.arrowOnButtonDown();
        } else if (Input.GetMouseButtonUp(0)) {
            mouseHeldDown = false;
            arrowsMovement.arrowOnButtonUp();
        }
    }

    public void rechargeEnergy() {
        if (!GetComponent<CharacterData>().isMaxEnergy()) {
            energyGenerationInitalTime += Time.deltaTime;
            if (energyGenerationInitalTime >= energyGenerationSpeed) {
                energyGenerationInitalTime = 0;
                GetComponent<CharacterData>().rechargeEnergy(0.5f);
            }
        }
    }
}
