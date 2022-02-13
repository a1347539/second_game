using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using System;

public struct InGamePlayerState : INetworkSerializable, IEquatable<InGamePlayerState>
{
    public ulong clientId;
    public FixedString32Bytes playerName;
    public ulong playerObjectId;
    public int teamIndex;
    public int playerIndex;
    public Point initialPoint;

    public InGamePlayerState(ulong clientId, FixedString32Bytes playerName, ulong playerObjectId, int teamIndex, int playerIndex, Point initialPoint)
    {
        this.clientId = clientId;
        this.playerName = playerName;
        this.playerObjectId = playerObjectId;
        this.teamIndex = teamIndex;
        this.playerIndex = playerIndex;
        this.initialPoint = initialPoint;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerObjectId);
        serializer.SerializeValue(ref teamIndex);
        serializer.SerializeValue(ref playerIndex);
        serializer.SerializeValue(ref initialPoint);
    }

    public bool Equals(InGamePlayerState other)
    {
        throw new NotImplementedException();
    }

}
