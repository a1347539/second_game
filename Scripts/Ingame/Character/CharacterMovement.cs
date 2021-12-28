using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
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
    public float angleFromTouchToCurrent { get; private set; }
    public int vDistance { get; private set; }
    public int hDistance { get; private set; }
    Vector3 distanceToTravel;
    CharacterData characterData;
    CharacterControl characterControl;

    // Start is called before the first frame update
    void Start() {
        vDistance = 0; hDistance = 0;
        characterData = GetComponent<CharacterData>();
        characterControl = GetComponent<CharacterControl>();
    }

    // Update is called once per frame
    void Update() {
        if (mouseHeldDown) {
            angleFromTouchToCurrent = 
                Mathf.Atan2(mousePosition.y-onMouseDownPosition.y, 
                mousePosition.x-onMouseDownPosition.x)*180 / Mathf.PI;
            angleFromTouchToCurrent = Mathf.RoundToInt(angleFromTouchToCurrent*2f/180f);

            if (Mathf.Abs(angleFromTouchToCurrent) == 1) {
                hDistance = 0;
                vDistance = Mathf.RoundToInt(mousePosition.y - onMouseDownPosition.y);
                vDistance = Mathf.Clamp(
                    vDistance, -characterControl.maxDistanceFromSelf[2], characterControl.maxDistanceFromSelf[0]);
                vDistance = Mathf.Clamp(vDistance, (int)characterData.energy/10*-1, (int)(characterData.energy/10));

            } else {
                vDistance = 0;
                hDistance = Mathf.RoundToInt(mousePosition.x - onMouseDownPosition.x);
                hDistance = Mathf.Clamp(
                    hDistance, -characterControl.maxDistanceFromSelf[3], characterControl.maxDistanceFromSelf[1]);
                
                hDistance = Mathf.Clamp(hDistance, (int)characterData.energy/10*-1, (int)(characterData.energy/10));
            }
        }
    }

    public void move() {
        distanceToTravel = new Vector3(transform.position.x + hDistance, 
            transform.position.y + vDistance, 
            transform.position.z);
        GetComponent<CharacterData>().deductEnergy(hDistance==0?Mathf.Abs(vDistance)*10:Mathf.Abs(hDistance)*10);

        transform.position = distanceToTravel;
    }
}
