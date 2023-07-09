using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Room : MonoBehaviour
{
    public Transform _walkPointTransform;

    public List<Transform> _connectionTransforms;

    public List<RoomConnection> RoomConnections { get; set; } = new();

    public Vector2 WalkPoint => new(_walkPointTransform.position.x, _walkPointTransform.position.z);

    public void SetConnectionsFromTransforms()
    {
        // Turn connectionTransforms into RoomConnections
        foreach (Transform connectionTransform in _connectionTransforms)
        {
            RoomConnections.Add(new RoomConnection(new Vector2(connectionTransform.position.x, connectionTransform.position.z), this));
        }
    }

    public List<Room> NeighbourRooms { get; } = new();

}
