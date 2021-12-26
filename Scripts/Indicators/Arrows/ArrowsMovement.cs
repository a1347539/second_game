using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowsMovement : MonoBehaviour
{
    public Vector3 onMouseDownPosition { get { 
        return MouseManager.Instance.onMouseDownPosition; 
        } }
    public Vector3 mousePosition { get {
        return MouseManager.Instance.mousePosition;
    } }
    public bool mouseHeldDown { get {
        return MouseManager.Instance.mouseHeldDown;
    } }
    private float prevAngle;
    float angleFromTouchToCurrent;
    public int vDistance { get; private set; }
    public int validVDistance { get; private set; }
    public int hDistance { get; private set; }
    public int validHDistance { get; private set; }
    CharacterControl CharacterControl;
    CharacterData characterData;
    private RaycastHit2D hit;
    Camera cam;


    // Start is called before the first frame update
    void Start() {
        prevAngle = transform.eulerAngles.z;
        vDistance = 0; hDistance = 0;
        transform.localScale = new Vector3(0, 1, 1);
        CharacterControl = GetComponentInParent<CharacterControl>();
        characterData = GetComponentInParent<CharacterData>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        if (mouseHeldDown) {
            angleFromTouchToCurrent = 
                Mathf.Atan2(mousePosition.y-onMouseDownPosition.y, 
                mousePosition.x-onMouseDownPosition.x)*180 / Mathf.PI;
            angleFromTouchToCurrent = Mathf.RoundToInt(angleFromTouchToCurrent*2f/180f);
            
            transform.eulerAngles = Vector3.forward * angleFromTouchToCurrent*90f;
            // if angle changed
            if (prevAngle != transform.eulerAngles.z) {

                prevAngle = transform.eulerAngles.z;
            }

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
        
        hit = Physics2D.Raycast(transform.position, -Vector2.up);

        if (hit.collider.GetComponent<CollidableTiles>() != null) {
            Debug.Log(123);
            Debug.Log(hit.collider.transform.position);
        }
    }

    public void arrowOnButtonUp() {
        transform.localScale = new Vector3(0, 1, 1);
    }

    private bool isInCollidable() {
        if (isPointOutOfBound()) { return true; }

        hit = Physics2D.Raycast(new Vector3(transform.position.x - hDistance, 
                                            transform.position.y - vDistance, 
                                            transform.position.z), 
                                Vector2.zero);
        if (hit.collider != null) {
            if (hit.collider.gameObject.GetComponent<CollidableTiles>() != null) {
                return true;
            }
        }
        return false;
    }

    public bool isPointOutOfBound() { 
        Vector3 screenPoint = cam.WorldToViewportPoint(
            new Vector3(transform.position.x - hDistance, transform.position.y - vDistance, transform.position.z));
        if (screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1) {
            return false;
        }
        return true;
    }

}
