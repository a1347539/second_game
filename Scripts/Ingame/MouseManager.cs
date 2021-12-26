using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : Singleton<MouseManager>
{
    public Vector3 onMouseDownPosition { get; private set; }
    public Vector3 mousePosition { get; private set; }
    public bool mouseHeldDown { get; private set; }
    public bool onMouseDown = false;
    public bool onMouseRelease = false;
    private Camera cam;
    private GridLayout gridLayout;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    private void FixedUpdate() {
    }

    // Update is called once per frame
    void Update()
    {
        mouseClick();
        if (mouseHeldDown) {
            mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void mouseClick() {
        if (Input.GetMouseButtonDown(0)) {
            mouseHeldDown = true;
            onMouseDown = true;
            onMouseDownPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        } else if (Input.GetMouseButtonUp(0)) {
            mouseHeldDown = false;
            onMouseRelease = true;
        }
    }
}
