using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = System.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class RoomManager : MonoBehaviour
{
    public enum RoomType { FLOOR1, FLOOR2, PANTRY, LIVING, KITCHEN, TOILET, BED };

    public Character _character;
    public Room[] _roomPrefabs;
    public List<Room> ActiveRooms { get; set; } = new();

    public void Start()
    {
        InitializeNewRoom(Vector2.zero, null, RoomType.FLOOR1);
    }

    public void Update()
    {
        if (!Input.GetKeyDown(KeyCode.W)) return;

        // hänge an den bestehenden Raum nen neuen Raum an
        RoomConnection usedConnection = FindConnection();

        InitializeNewRoom(GetRoomPlacementFromConnection(usedConnection), usedConnection, RoomType.FLOOR1);

        _character.RecalibratePath();
    }

    public RoomConnection FindConnection()
    {
        RoomConnection foundConnection = null;
        int breakOut = 0;

        while (foundConnection == null && breakOut < 10000)
        {
            // find random room
            Room roomToAddTo = FindRoomToAddTo();

            // find random connection
            foundConnection = FindConnectionForRoom(roomToAddTo);

            breakOut++;
        }

        if (breakOut >= 10000)
        {
            throw new Exception("Broke out of endless loop");
        }

        return foundConnection;
    }

    public Room FindRoomToAddTo()
    {
        return ActiveRooms[UnityEngine.Random.Range(0, ActiveRooms.Count)];
    }

    public RoomConnection FindConnectionForRoom(Room baseRoom)
    {
        // wir nehmen von BaseRoom eine Connection die noch nicht besetzt ist

        // test ob dieser Raum noch was frei hat.
        bool isAllFull = true;

        foreach (RoomConnection room in baseRoom.RoomConnections)
        {
            if (room.ConnectingRoom == null)
            {
                isAllFull = false;
                break;
            }
        }

        if (isAllFull)
        {
            return null;
        }

        RoomConnection usedConnection = null;
        int breakOut = 0;
        while (usedConnection == null && breakOut < 10000)
        {
            RoomConnection testedConnection = baseRoom.RoomConnections[UnityEngine.Random.Range(0, baseRoom.RoomConnections.Count)];

            // einen Raum gefunden, der noch nicht gefüllt ist (hier muss noch validiert werden ob der zu platzierende Raum überhaupt platziert werden kann.
            if (testedConnection.ConnectingRoom == null)
            {
                usedConnection = testedConnection;
            }

            breakOut++;
        }

        if (breakOut >= 10000)
        {
            throw new Exception("Broken out of endless loop");
        }

        // kann besetzt werden
        return usedConnection;
    }

    public Vector2 GetRoomPlacementFromConnection(RoomConnection existingConnection)
    {
        // wir nutzen bisher nur 1x1 Räume
        return existingConnection.Room.WalkPoint + (existingConnection.GetConnectionPosition() - existingConnection.Room.WalkPoint) * 2;
    }

    public Room GetRoomPrefabFromType(RoomType type)
    {
        return _roomPrefabs[(int)type];
    }

    public void InitializeNewRoom(Vector2 position, RoomConnection existingConnection, RoomType type)
    {
        Room newRoom = Instantiate(GetRoomPrefabFromType(type), new Vector3(position.x, 0, position.y), Quaternion.identity);
        newRoom.InstantiateConnections();
        // room drehen, sodass eine der Connections in die richtige Richtung zeigt.
        // 1. Connection auswählen

        if (existingConnection != null)
        {
            RoomConnection newRoomConnection = newRoom.RoomConnections[UnityEngine.Random.Range(0, newRoom.RoomConnections.Count)];

            RotateNewRoom(newRoomConnection, existingConnection.Room);

            if (existingConnection.ConnectingRoom != null)
            {
                throw new Exception("There's already a room here");
            }

            // in beide Richtungen verbinden! -> vorher auch feststellen mit welcher existingConnection ich den raum verbinden will.
            newRoomConnection.ConnectingRoom = existingConnection.Room;
            existingConnection.ConnectingRoom = newRoom;
        }

        AddRoom(newRoom);
    }

    void RotateNewRoom(RoomConnection newRoomConnection, Room existingRoom)
    {
        // unrotated
        Vector2 toConnection = newRoomConnection.GetConnectionPosition() - newRoomConnection.Room.WalkPoint;

        // In Richtung drehen damit's eine passende Connection wird!
        Vector2 forward2D = existingRoom.WalkPoint - newRoomConnection.Room.WalkPoint;
        Vector3 forward3D = new Vector3(forward2D.x, 0, forward2D.y);
 
        Quaternion lookTowardsRotation = Quaternion.LookRotation(forward3D, Vector3.up);
        float angle = Vector2.SignedAngle(Vector2.up, toConnection);
        Debug.Log(angle);

        Quaternion rotateForRoomRotation = Quaternion.Euler(0, angle, 0);

        newRoomConnection.Room.transform.rotation = lookTowardsRotation * rotateForRoomRotation;
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
