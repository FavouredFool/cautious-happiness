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

    List<Room> _tempRooms;

    public void Awake()
    {
        _roomManager = GetComponent<RoomManager>();
    }

    public void Start()
    {
        _roomManager.InitializeNewRoom(null, RoomManager.RoomType.BED);

        for (int i = 0; i < 6; i++)
        {
            _ = CreateRoom();
        }

        _character.InitializeCharacter();

        EndlessTimer();
    }

    public async void EndlessTimer()
    {
        await Task.Delay(2000);

        int failSave = 0;

        while (Application.isPlaying && failSave < 10000)
        {
            failSave++;
            await DestroyPhase();

            if (!Application.isPlaying) break;

            await CreatePhase();
        }
    }

    public async Task CreatePhase()
    {
        for (int i = 0; i < 5; i++)
        {
            _ = CreateRoom();
        }

        await CreateRoom();
    }

    public async Task DestroyPhase()
    {
        _tempRooms = new(_roomManager.ActiveRooms);

        for (int i = 0; i < 5; i++)
        {
            _ = DestroyRoom();
        }

        await DestroyRoom();
    }

    public async Task CreateRoom()
    {
        await _roomManager.CreateRoom();
    }

    public async Task DestroyRoom()
    {
        Room room = _roomManager.GetRoomToDestroy(_tempRooms);
        _tempRooms.Remove(room);

        await _roomManager.RemoveRoom(room);
    }
}
