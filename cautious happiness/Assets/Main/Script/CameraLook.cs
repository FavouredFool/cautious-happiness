using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public RoomManager _roomManager;

    public float _speed;

    Vector3 _offset;
    void Start()
    {
        _offset = transform.position;
    }

    void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, Center() + _offset, _speed);
    }

    public Vector3 Center()
    {
        Vector3 addUp = Vector3.zero;

        foreach (Room room in _roomManager.ActiveRooms)
        {
            addUp += new Vector3(room.WalkPoint.x, 0, room.WalkPoint.y);
        }

        return addUp / _roomManager.ActiveRooms.Count;
    }

    
}
