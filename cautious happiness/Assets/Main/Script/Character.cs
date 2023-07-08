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
        // zum der in WalkLinePath und der entsprechenden t-Value am nächsten ist.

        Room bestRoom = null;
        float minDistance = float.PositiveInfinity;

        foreach (Room room in _roomManager.ActiveRooms)
        {
            float distanceToPlayer = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), room.WalkPoint);

            if (minDistance > distanceToPlayer)
            {
                minDistance = distanceToPlayer;
                bestRoom = room;
            }
        }

        return bestRoom;
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
