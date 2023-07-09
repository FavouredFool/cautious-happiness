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

    public Room ActiveWaypoint { get; set; }

    public Animator animator;

    public void Awake()
    {
        animator.GetComponent<Animator>();
    }

    public void InitializeCharacter()
    {
        LatestRoom = CalculateCurrentRoom();
        ActiveWaypoint = LatestRoom;
    }

    public void Update()
    {
        GoalRoom = _scheduleManager.GetGoalRoomFromTValue();
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


        if (Vector3.Distance(transform.position, new Vector3(ActiveWaypoint.WalkPoint.x, 0, ActiveWaypoint.WalkPoint.y)) < 0.1f)
        {
            LatestRoom = ActiveWaypoint;
        }


        CalculateActiveWaypoint(LatestRoom, GoalRoom);

        Vector3 direction = (new Vector3(ActiveWaypoint.WalkPoint.x, 0, ActiveWaypoint.WalkPoint.y) - transform.position).normalized;

        AnimateCharacter(direction);

        transform.position += direction * _speed * Time.deltaTime;
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
        return null;
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

                if (Math.Abs(Vector2.Dot(directFromPlayer, directFromWaypoint) - 1) < 0.005f)
                {
                    ActiveWaypoint = neighbourRoom;
                }
                else
                {
                    ActiveWaypoint = latestRoom;
                }

                
                break;
            }
        }
    }
}
