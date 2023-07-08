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
        _roomManager.InitializeNewRoom(null, RoomManager.RoomType.BED);

        for (int i = 0; i < 6; i++)
        {
            CreateRoom();
        }

        _character.InitializeCharacter();

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

    public async void Phase()
    {
        for (int i = 0; i < 6; i++)
        {
            DestroyRoom();
        }

        await Task.Delay(200);

        for (int i = 0; i < 6; i++)
        {
            CreateRoom();
        }
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
