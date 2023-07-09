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

    RoomManager.RoomType _lastRoom = RoomManager.RoomType.BED;

    public int score;

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
        await Task.Delay(3000);

        int failSave = 0;

        Room._destroyTime = 25f;

        while (Application.isPlaying && failSave < 10000)
        {
            failSave++;

            List<RoomManager.RoomType> list = RoomManager.EnumToList<RoomManager.RoomType>();
            list.Remove(RoomManager.RoomType.FLOOR1);
            list.Remove(RoomManager.RoomType.FLOOR2);
            list.Remove(_lastRoom);

            RoomManager.RoomType roomType = _roomManager.GetRandomType(list);
            _lastRoom = roomType;

            await DestroyPhase(roomType);

            score++;

            if (score < 3)
            {
                Room._destroyTime = 26 - (score * 2f);
            }

            Room._destroyTime = 18 - ((score - 3) * 1f);
            

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

    public async Task DestroyPhase(RoomManager.RoomType type)
    {
        _tempRooms = new(_roomManager.ActiveRooms);

        for (int i = 0; i < 5; i++)
        {
            _ = DestroyRoom(type);
        }

        await DestroyRoom(type);
    }

    public async Task CreateRoom()
    {
        await _roomManager.CreateRoom();
    }

    public async Task DestroyRoom(RoomManager.RoomType type)
    {
        Room room = _roomManager.GetRoomToDestroy(_tempRooms, type);
        _tempRooms.Remove(room);

        await _roomManager.RemoveRoom(room);
    }
}
