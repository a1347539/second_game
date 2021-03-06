using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class InGameCharacterMovement : NetworkBehaviour
{
    public event Action<Point, Point> onPlayerMovedEvent;

    public Vector3 onMouseDownPosition { get { return controllerManager.onMouseDownPosition; } }
    public Vector3 mousePosition { get { return controllerManager.mousePosition; } }
    public float angleFromTouchToCurrent { get { return controllerManager.angleFromTouchToCurrent; } }
    public int vDistance { get; private set; } = 0;
    public int hDistance { get; private set; } = 0;
    InGameCharacterData characterData;
    InGameCharacterControl characterControl;
    InGameCharacter character;
    InGamePlayerControllerManager controllerManager;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        characterData = GetComponent<InGameCharacterData>();
        characterControl = GetComponent<InGameCharacterControl>();
        character = GetComponent<InGameCharacter>();
        controllerManager = GetComponent<InGamePlayerControllerManager>();
    }

    private void Start()
    {
        if (!IsOwner) { return; }
        controllerManager.mousePressedDownEvent += handleMousePressedDown;
    }

    private void OnDestroy()
    {
        controllerManager.mousePressedDownEvent -= handleMousePressedDown;
    }

    private void handleMousePressedDown() {
        if (Mathf.Abs(angleFromTouchToCurrent) == 1)
        {
            hDistance = 0;
            vDistance = Mathf.RoundToInt(mousePosition.y - onMouseDownPosition.y);
            vDistance = Mathf.Clamp(
                vDistance, -characterControl.maxDistanceFromSelf.Value.down, characterControl.maxDistanceFromSelf.Value.top);
            vDistance = Mathf.Clamp(vDistance, (int)characterData.energy.Value / 10 * -1, (int)(characterData.energy.Value / 10));

        }
        else
        {
            vDistance = 0;
            hDistance = Mathf.RoundToInt(mousePosition.x - onMouseDownPosition.x);
            hDistance = Mathf.Clamp(
                hDistance, -characterControl.maxDistanceFromSelf.Value.left, characterControl.maxDistanceFromSelf.Value.right);

            hDistance = Mathf.Clamp(hDistance, (int)characterData.energy.Value / 10 * -1, (int)(characterData.energy.Value / 10));
        }
    }

    public void move() {
        Point previousPoint = character.point.Value;
        Point destination = new Point(character.point.Value.x + hDistance, character.point.Value.y - vDistance);
        if (previousPoint == destination) { return; }

        character.updatePointAndPosition(destination);

        characterData.deductEnergy(hDistance == 0 ? Mathf.Abs(vDistance) * 10 : Mathf.Abs(hDistance) * 10);
        vDistance = 0; hDistance = 0;
        onPlayerMovedEvent?.Invoke(previousPoint, destination);
    }
}
