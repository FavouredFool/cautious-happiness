using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using static RoomManager;

public class Character : MonoBehaviour
{
    

    public RoomManager _roomManager;

    public ScheduleManager _scheduleManager;

    public float _speed = 1f;


    public List<Vector2> WalkLinePath { get; set; } = new() {Vector2.zero};

    public Room GoalRoom { get; set; }

    public Room LatestRoom { get; set; }

    public Vector2 ActiveWaypoint { get; set; }

    public void InitializeCharacter()
    {
        GoalRoom = _roomManager.GetRoomFromRoomType(RoomType.BED);
        LatestRoom = CalculateCurrentRoom();
    }

    public void Update()
    {
        MoveCharacter();

        if (Input.GetKeyDown(KeyCode.W))
        {
            GoalRoom = _roomManager.GetRoomFromRoomType(_roomManager.GetRandomType(EnumToList<RoomType>()));
        }
        
        
    }

    public void MoveCharacter()
    {
        if (Vector3.Distance(transform.position, new Vector3(GoalRoom.WalkPoint.x, 0, GoalRoom.WalkPoint.y)) < 0.01f)
        {
            return;
        }

        CalculateActiveWaypoint(LatestRoom, GoalRoom);

        Vector3 direction = (new Vector3(ActiveWaypoint.x, 0, ActiveWaypoint.y) - transform.position).normalized;

        transform.position += direction * _speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, new Vector3(ActiveWaypoint.x, 0, ActiveWaypoint.y)) < 0.01f)
        {
            LatestRoom = CalculateCurrentRoom();
        }
    }

    public Room CalculateCurrentRoom()
    {
        foreach (Room room in _roomManager.ActiveRooms)
        {
            foreach (BoxCollider collider in room._colliders)
            {
                if (collider.bounds.Contains(transform.position))
                {
                    return room;
                }
            }
        }

        Debug.LogWarning("Found no rooms");
        return _roomManager.ActiveRooms[0];
    }

    public void CalculateActiveWaypoint(Room latestRoom, Room goalRoom)
    {
        foreach (RoomConnection neighbourRoomConnection in latestRoom.RoomConnections)
        {
            Room neighbourRoom = neighbourRoomConnection.ConnectingRoom;

            if (neighbourRoom == null) continue;

            if (_roomManager.WalksTowardsRoom(goalRoom, neighbourRoom, latestRoom))
            {
                ActiveWaypoint = neighbourRoom.WalkPoint;
                break;
            }
        }
    }
}
