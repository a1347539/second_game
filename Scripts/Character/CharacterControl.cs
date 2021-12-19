using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{

    Vector3 position;

    bool mouseHeldDown = false;

    // Start is called before the first frame update
    void Start() {
        snapToGrid();
    }

    // Update is called once per frame
    void Update() {
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
        } else if (Input.GetMouseButtonUp(0)) {
            mouseHeldDown = false;
        }
    }
}
