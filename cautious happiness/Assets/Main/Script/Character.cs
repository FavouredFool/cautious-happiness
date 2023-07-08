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

    public float _speed = 1f;


    public List<Vector2> WalkLinePath { get; set; }

    float _t = 0;

    public void Start()
    {
        WalkLinePath = RecalibratePath();
    }

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
        if (path.Count < 3)
        {
            return path[0];
        }

        float totalDistance = GetTotalDistance(path);
        float targetDistance = totalDistance * Mathf.Clamp01(t);

        float currentDistance = 0f;
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector2 currentPoint = path[i];
            Vector2 nextPoint = path[i + 1];
            float segmentDistance = Vector2.Distance(currentPoint, nextPoint);

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

    List<Vector2> RecalibratePath()
    {
        List<Vector2> newWalkLinePath = new();

        // Create new Walkline at my current position
        Vector2 currentPositionWalkLine = new Vector2(transform.position.x, transform.position.z);
        newWalkLinePath.Add(currentPositionWalkLine);

        // Calculate the order of walklines based on schedule and avaliable rooms
        // Currenttime is always t=0 ??? -> Even though its not in "scheduletime" t=0?
        // -> kinda shit. Maybe make a new path

        foreach (Room room in _roomManager.ActiveRooms)
        {
            if (Vector2.Distance(room.WalkPoint, currentPositionWalkLine) < 0.01f)
            {
                continue;
            }

            newWalkLinePath.Add(room.WalkPoint);
        }

        newWalkLinePath.Add(currentPositionWalkLine);

        return newWalkLinePath;
    }
}
