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

        //EndlessTimer();
    }

    public async void EndlessTimer()
    {
        await Task.Delay(_intervalTimeMS);

        while (Application.isPlaying)
        {
            await DestroyPhase();

            for (int i = 0; i < 6; i++)
            {
                CreateRoom();
            }

            await Task.Delay(_intervalTimeMS);
        }
    }

    public async Task DestroyPhase()
    {
        
        for (int i = 0; i < 5; i++)
        {
            _ = DestroyRoom();
        }
        await DestroyRoom();
    }

    public void CreateRoom()
    {
        _roomManager.CreateRoom();
    }

    public async Task DestroyRoom()
    {
        await _roomManager.DestroyRoom();
    }
}
