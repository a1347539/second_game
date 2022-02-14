using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class InGamePlayerControllerManager : NetworkBehaviour
{
    public event Action onMouseDownEvent;
    public event Action onMouseReleasedEvent;
    public event Action mousePressedDownEvent;

    public Vector3 onMouseDownPosition { get; private set; }
    public Vector3 mousePosition { get; private set; }
    public bool mouseHeldDown { get; private set; }
    public bool onMouseDown = false;
    public bool onMouseRelease = false;

    public float angleFromTouchToCurrent { get; private set; }

    // Update is called once per frame
    void Update()
    {

        if (!IsOwner) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            mouseHeldDown = true;
            onMouseDownPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            onMouseDownEvent?.Invoke();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mouseHeldDown = false;
            onMouseReleasedEvent?.Invoke();
        }

        if (mouseHeldDown) {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            angleFromTouchToCurrent =
                Mathf.Atan2(mousePosition.y - onMouseDownPosition.y,
                mousePosition.x - onMouseDownPosition.x) * 180 / Mathf.PI;
            angleFromTouchToCurrent = Mathf.RoundToInt(angleFromTouchToCurrent * 2f / 180f);
            mousePressedDownEvent?.Invoke();
        }


    }

    void mouseClick() {
        if (Input.GetMouseButtonDown(0)) {
            mouseHeldDown = true;
            onMouseDown = true;
            onMouseDownPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        } else if (Input.GetMouseButtonUp(0)) {
            onMouseRelease = true;
            mouseHeldDown = false;
        }
    }
}
