using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;
using System;

public struct LobbyPlayerState : INetworkSerializable, IEquatable<LobbyPlayerState>
{
    public ulong clientId;
    public FixedString32Bytes playerName;
    public bool isReady;

    public LobbyPlayerState(ulong clientId, FixedString32Bytes playerName, bool isReady) {
        this.clientId = clientId;
        this.playerName = playerName;
        this.isReady = isReady;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref isReady);
    }

    public bool Equals(LobbyPlayerState other)
    {
        return (clientId == other.clientId && playerName.Equals(other.playerName) && isReady == other.isReady);
    }


}
