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
    public int[] maxDistanceFromSelf { get; private set; } // top right down left
    

    // Start is called before the first frame update
    void Start() {
        maxDistanceFromSelf = new int[4];
        cam = Camera.main;
        // snapToGrid();
        
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
            checkWalkableDistance();
            arrowsMovement.arrowOnButtonDown();
            
            MouseManager.Instance.onMouseDown = false;
        }

        if (MouseManager.Instance.onMouseRelease) {
            arrowsMovement.arrowOnButtonUp();
            GetComponent<CharacterMovement>().move();
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

    private void rechargeEnergy() {
        if (!GetComponent<CharacterData>().isMaxEnergy()) {
            energyGenerationInitalTime += Time.deltaTime;
            if (energyGenerationInitalTime >= energyGenerationSpeed) {
                energyGenerationInitalTime = 0;
                GetComponent<CharacterData>().rechargeEnergy(0.5f);
            }
        }
    }

    private void checkWalkableDistance() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up);
        if (hit.collider != null) {
            maxDistanceFromSelf[0] = (int)(hit.transform.position.y - transform.position.y - 1);
        }
        hit = Physics2D.Raycast(transform.position, Vector2.right);
        if (hit.collider != null) {
            maxDistanceFromSelf[1] = (int)(hit.transform.position.x - transform.position.x - 1);
        }
        hit = Physics2D.Raycast(transform.position, Vector2.down);
        
        if (hit.collider != null) {
            maxDistanceFromSelf[2] = (int)(transform.position.y - hit.transform.position.y - 1);

        }
        hit = Physics2D.Raycast(transform.position, Vector2.left);
        if (hit.collider != null) {
            maxDistanceFromSelf[3] = (int)(transform.position.x - hit.transform.position.x - 1);
        }
    }
}
