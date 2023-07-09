using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using AmazingAssets.AdvancedDissolve;
using static RoomManager;

public class Room : MonoBehaviour
{
    public RoomType _roomType;

    public Transform _walkPointTransform;

    public List<Transform> _connectionTransforms;

    MeshRenderer[] _meshRenderer;

    public Collider[] _colliders;

    public List<RoomConnection> RoomConnections { get; set; } = new();

    public Vector2 WalkPoint => new(_walkPointTransform.position.x, _walkPointTransform.position.z);

    public RoomType RoomType => _roomType;

    float _createTime = 1.5f;
    public static float _destroyTime = 15;

    public void Awake()
    {
        _meshRenderer = GetComponentsInChildren<MeshRenderer>();
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

    public void RemoveFromConnections(Room roomToDisconnect)
    {
        foreach (RoomConnection connection in RoomConnections)
        {
            if (connection.ConnectingRoom == roomToDisconnect)
            {
                connection.ConnectingRoom = null;
            }
        }
    }

    public async Task Integrate()
    {

        foreach (MeshRenderer meshRenderer in _meshRenderer)
        {
            Material mat = meshRenderer.material;
            AdvancedDissolveKeywords.SetKeyword(mat, AdvancedDissolveKeywords.State.Enabled, true);
        }

        float startTime = Time.time;
        float maxTime = startTime + _createTime;

        while (Time.time < maxTime && Application.isPlaying)
        {
            float t = (Time.time - startTime) / _createTime;

            foreach (MeshRenderer meshRenderer in _meshRenderer)
            {
                Material mat = meshRenderer.material;
                AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(mat, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, 1-t);
            }

            await Task.Yield();
        }
    }

    public async Task Disintegrate(AnimationCurve curve)
    {
        foreach (MeshRenderer meshRenderer in _meshRenderer)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            Material mat = meshRenderer.material;
            AdvancedDissolveKeywords.SetKeyword(mat, AdvancedDissolveKeywords.State.Enabled, true);
        }


        float startTime = Time.time;
        float maxTime = startTime + _destroyTime;

        while (Time.time < maxTime && Application.isPlaying)
        {
            float t = (Time.time - startTime) / _destroyTime;

            foreach (MeshRenderer meshRenderer in _meshRenderer)
            {
                Material mat = meshRenderer.material;
                AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(mat, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, curve.Evaluate((t)));
            }

            await Task.Yield();
        }
    }

}
