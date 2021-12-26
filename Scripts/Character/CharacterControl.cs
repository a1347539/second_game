using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    [SerializeField] private ArrowsMovement arrowsMovement;

    public Vector3 onMouseDownPosition { get { 
        return MouseManager.Instance.onMouseDownPosition; 
        } }
    public Vector3 mousePosition { get {
        return MouseManager.Instance.mousePosition;
    } }
    public bool mouseHeldDown { get {
        return MouseManager.Instance.mouseHeldDown;
    } }
    private Camera cam;
    private float energyGenerationSpeed = 0.02f;
    private float energyGenerationInitalTime = 0f;
    

    // Start is called before the first frame update
    void Start() {
        cam = Camera.main;
        snapToGrid();
        
    }

    // Update is called once per frame
    void Update() {
        if (mouseHeldDown) {
            if (Mathf.Abs((mousePosition-onMouseDownPosition).x) < 0.5 &&
                Mathf.Abs((mousePosition-onMouseDownPosition).y) < 0.5) {
                rechargeEnergy();
            }
        }

        if (MouseManager.Instance.onMouseDown) {
            arrowsMovement.arrowOnButtonDown();
            MouseManager.Instance.onMouseDown = false;
        }

        if (MouseManager.Instance.onMouseRelease) {
            arrowsMovement.arrowOnButtonUp();
            GetComponent<CharacterMovement>().move(
            transform.position, 
            arrowsMovement.vDistance*-1, 
            arrowsMovement.hDistance*-1);
            MouseManager.Instance.onMouseRelease = false;
        }
    
    }

    private void snapToGrid() {
        Vector3 position = new Vector3(
            Mathf.Floor(transform.position.x)+0.5f,
            Mathf.Floor(transform.position.y)+0.5f,
            transform.position.z
        );
        transform.position = position;
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
