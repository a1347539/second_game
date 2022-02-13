using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisconnectReason : MonoBehaviour
{
    public ConnectStatus reason { get; private set; } = ConnectStatus.Undefined;

    public void setDisconnectionReason(ConnectStatus reason) {
        this.reason = reason;
    }

    public void clear() {
        reason = ConnectStatus.Undefined;
    }

    public bool hasTransitionReason => reason != ConnectStatus.Undefined;
}
