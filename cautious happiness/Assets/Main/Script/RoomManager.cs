using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public Room _roomPrefab;
    public List<Room> ActiveRooms { get; set; } = new();

    public void Start()
    {
        InitializeNewRoom(Vector2.zero, null);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // hänge an den bestehenden Raum nen neuen Raum an
            FindAndAddNewRoom(ActiveRooms[0]);
        }
    }

    public void FindAndAddNewRoom(Room baseRoom)
    {
        // wir nehmen von BaseRoom eine Connection die noch nicht besetzt ist

        foreach (RoomConnection existingConnection in baseRoom.RoomConnections)
        {
            if (existingConnection.ConnectingRoom != null) continue;

            // kann besetzt werden
            InitializeNewRoom(GetRoomPlacementFromConnection(existingConnection), existingConnection);
            return;
        }

        Debug.Log("es wurde keine nicht besetzte Connection gefunden :c");
    }

    public Vector2 GetRoomPlacementFromConnection(RoomConnection existingConnection)
    {
        // wir nutzen bisher nur 1x1 Räume
        return existingConnection.Room.WalkPoint + (existingConnection.ConnectionPosition - existingConnection.Room.WalkPoint) * 2;
    }

    public void InitializeNewRoom(Vector2 position, RoomConnection existingConnection)
    {
        Room newRoom = Instantiate(_roomPrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
        newRoom.SetConnectionsFromTransforms();
        // room drehen, sodass eine der Connections in die richtige Richtung zeigt.
        // 1. Connection auswählen



        if (existingConnection != null)
        {
            RotateNewRoom(newRoom, existingConnection.Room);

            if (existingConnection.ConnectingRoom != null)
            {
                throw new Exception("There's already a room here");
            }

            // in beide Richtungen verbinden! -> vorher auch feststellen mit welcher existingConnection ich den raum verbinden will.
            existingConnection.ConnectingRoom = newRoom;
        }

        AddRoom(newRoom);
    }

    void RotateNewRoom(Room newRoom, Room existingRoom)
    {
        foreach (RoomConnection newConnection in newRoom.RoomConnections)
        {
            if (newConnection.ConnectingRoom != null) continue;

            //Vector2 toConnection = newConnection.ConnectionPosition - newRoom.WalkPoint;

            // In Richtung drehen damit's eine passende Connection wird!
            //newRoom.transform.right = (existingRoom.WalkPoint - newRoom.WalkPoint) ;
            // + Rotation je nachdem welche Connection es ist oben drauf?
            

            return;
        }

        Debug.Log("Didnt find a good rotation");
    }

    public void AddRoom(Room room)
    {
        ActiveRooms.Add(room);
    }

    public void RemoveRoom(Room room)
    {
        ActiveRooms.Remove(room);
    }
}
