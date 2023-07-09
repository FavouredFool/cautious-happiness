using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using static RoomManager;
using Quaternion = UnityEngine.Quaternion;

public class Character : MonoBehaviour
{
    

    public RoomManager _roomManager;

    public ScheduleManager _scheduleManager;

    public float _speed = 1f;


    public List<Vector2> WalkLinePath { get; set; } = new() {Vector2.zero};

    public Room GoalRoom { get; set; }

    public Room LatestRoom { get; set; }

    public Vector2 ActiveWaypoint { get; set; }

    public Animator animator;

    public void InitializeCharacter()
    {
        LatestRoom = CalculateCurrentRoom();
        animator.GetComponent<Animator>();
    }

    public void Update()
    {
        GoalRoom = _scheduleManager.GetGoalRoomFromTValue();
        Debug.Log("goal: " + GoalRoom);
        Debug.Log("active: " + LatestRoom);
        MoveCharacter();
    }

    public void AnimateCharacter(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.5f);


        // why does it not work?
        //animator.SetTrigger("Bob_Walk");
    }

    public void MoveCharacter()
    {
        if (Vector3.Distance(transform.position, new Vector3(GoalRoom.WalkPoint.x, 0, GoalRoom.WalkPoint.y)) < 0.01f)
        {
            return;
        }


        CalculateActiveWaypoint(LatestRoom, GoalRoom);

        Vector3 direction = (new Vector3(ActiveWaypoint.x, 0, ActiveWaypoint.y) - transform.position).normalized;

        AnimateCharacter(direction);

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
            foreach (BoxCollider roomcollider in room._colliders)
            {
                if (roomcollider.bounds.Contains(transform.position))
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
                // angle test
                Vector2 directFromPlayer = (neighbourRoom.WalkPoint - new Vector2(transform.position.x, transform.position.z)).normalized;

                Vector2 directFromWaypoint = (neighbourRoom.WalkPoint - latestRoom.WalkPoint).normalized;

                if (Math.Abs(Vector2.Dot(directFromPlayer, directFromWaypoint) - 1) < 0.05f)
                {
                    ActiveWaypoint = neighbourRoom.WalkPoint;
                }
                else
                {
                    ActiveWaypoint = latestRoom.WalkPoint;
                }

                
                break;
            }
        }
    }
}
