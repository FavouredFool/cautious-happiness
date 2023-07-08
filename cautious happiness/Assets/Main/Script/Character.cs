using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Character : MonoBehaviour
{
    

    public RoomManager _roomManager;

    public ScheduleManager _scheduleManager;

    public float _speed = 1f;


    public List<Vector2> WalkLinePath { get; set; } = new() {Vector2.zero};

    float _t = 0;



    public void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            _t -= _speed * Time.deltaTime;
        }

        else if (Input.GetKey(KeyCode.D))
        {
            _t += _speed * Time.deltaTime;
        }

        if (_t >= 1)
        {
            float excess = _t - 1;
            _t = excess;
        }

        if (_t < 0)
        {
            _t = 1 - _t;
        }

        transform.position = MoveCharacter(_t);
    }

    public Vector3 MoveCharacter(float t)
    {
        Vector2 interpolated = Interpolate(WalkLinePath, t);

        return new Vector3(interpolated.x, 0, interpolated.y);
    }

    public static Vector2 Interpolate(List<Vector2> path, float t)
    {
        if (path.Count < 4)
        {
            return path[0];
        }

        if (Math.Abs(t - 1) < 0.01f)
        {
            t = 0;
        }

        float totalDistance = GetTotalDistance(path);
        float targetDistance = totalDistance * Mathf.Clamp01(t);

        float currentDistance = 0f;
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector2 currentPoint = path[i];
            Vector2 nextPoint = path[i + 1];
            float segmentDistance = Vector2.Distance(currentPoint, nextPoint);

            if (currentPoint == nextPoint)
            {
                continue;
            }

            if (currentDistance + segmentDistance >= targetDistance)
            {
                float remainingDistance = targetDistance - currentDistance;
                float segmentT = remainingDistance / segmentDistance;
                return Vector2.Lerp(currentPoint, nextPoint, segmentT);
            }

            currentDistance += segmentDistance;
        }

        throw new Exception("Failed to interpolate path. Invalid t value.");
    }

    static float GetTotalDistance(List<Vector2> path)
    {
        float totalDistance = 0f;
        for (int i = 0; i < path.Count - 1; i++)
        {
            totalDistance += Vector2.Distance(path[i], path[i + 1]);
        }
        return totalDistance;
    }

    public void RecalibratePath()
    {
        _t = 0;
        List <Vector2> newWalkLinePath = new();

        // Create new Walkline at my current position
        Vector2 currentPositionWalkLine = new Vector2(transform.position.x, transform.position.z);
        //newWalkLinePath.Add(currentPositionWalkLine);

        Room startRoom = CalculateStartRoom();

        Room activeRoom = startRoom;

        List<Room> allRooms = new();
        allRooms.Add(startRoom);
        allRooms.AddRange(_scheduleManager.GetRoomOrderList());
        allRooms.Add(startRoom);

        foreach (Room room in allRooms)
        {
            List<Vector2> waypointsWalkTowardsRoom = _roomManager.WalkTowardsRoom(room, activeRoom, null);
            newWalkLinePath.AddRange(waypointsWalkTowardsRoom);

            activeRoom = room;
        }

        //newWalkLinePath.Add(currentPositionWalkLine);

        
        WalkLinePath = newWalkLinePath;
    }

    public Room CalculateStartRoom()
    {
        // zum der in WalkLinePath und der entsprechenden t-Value am nächsten ist.

        Room bestRoom = null;
        float minDistance = float.PositiveInfinity;

        foreach (Room room in _roomManager.ActiveRooms)
        {

            float distanceToPlayer = Vector2.Distance(new Vector2(transform.position.x,transform.position.z), room.WalkPoint);

            if (minDistance > distanceToPlayer)
            {
                minDistance = distanceToPlayer;
                bestRoom = room;
            }

        }

        return bestRoom;
    }
}
