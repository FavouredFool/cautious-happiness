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

    public BoxCollider Collider { get; set; }

    public void Awake()
    {
        Collider = GetComponent<BoxCollider>();
    }

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

    public static List<Vector2> GetSpotChecksPerType(RoomType type)
    {
        List<Vector2> spotChecks = new() { new Vector2(0, 0) };

        switch (type)
        {
            case RoomType.FLOOR1:
                spotChecks.Add(new Vector2(0, -1));
                spotChecks.Add(new Vector2(0, 1));
                break;
            case RoomType.FLOOR2:
                spotChecks.Add(new Vector2(0, 1));
                spotChecks.Add(new Vector2(1, 0));
                spotChecks.Add(new Vector2(2, 0));
                break;
            case RoomType.PANTRY:
                break;
            case RoomType.LIVING:
                spotChecks.Add(new Vector2(0, -1));
                spotChecks.Add(new Vector2(0, 1));
                spotChecks.Add(new Vector2(1, -1));
                spotChecks.Add(new Vector2(1, 0));
                spotChecks.Add(new Vector2(1, 1));
                break;
            case RoomType.KITCHEN:
                spotChecks.Add(new Vector2(0, 1));
                spotChecks.Add(new Vector2(1, 0));
                break;
            case RoomType.TOILET:
                spotChecks.Add(new Vector2(0, 1));
                break;
            case RoomType.BED:
                spotChecks.Add(new Vector2(0, 1));
                spotChecks.Add(new Vector2(1, 0));
                spotChecks.Add(new Vector2(1, 1));
                break;

            default:
                throw new Exception("Forgot to add type to spotchecks");
        }

        return spotChecks;
    }

}
