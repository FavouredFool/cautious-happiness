using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static RoomManager;

public class Room : MonoBehaviour
{
    public RoomType _roomType;

    public Transform _walkPointTransform;

    public List<Transform> _connectionTransforms;

    public List<RoomConnection> RoomConnections { get; set; } = new();

    public Vector2 WalkPoint => new(_walkPointTransform.position.x, _walkPointTransform.position.z);

    public RoomType RoomType => _roomType;

    public void InstantiateConnections()
    {
        for (int i = 0; i < _connectionTransforms.Count; i++)
        {
            RoomConnections.Add(new RoomConnection(i, this));
        }
    }

    public Vector2 GetConnectionFromTransform(int index)
    {
        return new Vector2(_connectionTransforms[index].position.x , _connectionTransforms[index].position.z);
    }

    public List<Room> NeighbourRooms { get; } = new();

}
