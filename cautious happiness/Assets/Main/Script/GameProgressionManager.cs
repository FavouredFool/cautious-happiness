using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameProgressionManager : MonoBehaviour
{
    public Character _character;

    public int _intervalTimeMS;

    RoomManager _roomManager;

    public void Awake()
    {
        _roomManager = GetComponent<RoomManager>();
    }

    public void Start()
    {
        _roomManager.InitializeNewRoom(null, RoomManager.RoomType.FLOOR2);

        for (int i = 0; i < 5; i++)
        {
            CreateRoom();
        }

        _character.RecalibratePath();

        EndlessTimer();
    }

    public async void EndlessTimer()
    {
        await Task.Delay(_intervalTimeMS);

        while (Application.isPlaying)
        {
            Phase();
            await Task.Delay(_intervalTimeMS);
        }
    }

    public void Phase()
    {
        DestroyRoom();
        DestroyRoom();
        DestroyRoom();
        CreateRoom();
        CreateRoom();
        CreateRoom();
    }

    public void CreateRoom()
    {
        _roomManager.CreateRoom();
    }

    public void DestroyRoom()
    {
        _roomManager.DestroyRoom();
    }
}
