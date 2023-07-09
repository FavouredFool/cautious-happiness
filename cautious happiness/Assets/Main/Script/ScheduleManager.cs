using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using static RoomManager;
using Debug = UnityEngine.Debug;

public class ScheduleManager : MonoBehaviour
{
    RoomManager _roomManager;

    public Clock _clock;

    public void Awake()
    {
        _roomManager = GetComponent<RoomManager>();
    }


    public Room GetGoalRoomFromTValue()
    {
        int currentHour = (int) (12 * _clock.t);

        if (currentHour == 12) currentHour = 0;

        RoomType type;

        switch (currentHour)
        {
            case 0:
                type = RoomType.BED;
                break;
            case 1:
                type = RoomType.BED;
                break;
            case 2:
                type = RoomType.BED;
                break;
            case 3:
                type = RoomType.PANTRY;
                break;
            case 4:
                type = RoomType.TOILET;
                break;
            case 5:
                type = RoomType.TOILET;
                break;
            case 6:
                type = RoomType.KITCHEN;
                break;
            case 7:
                type = RoomType.KITCHEN;
                break;
            case 8:
                type = RoomType.LIVING;
                break;
            case 9:
                type = RoomType.LIVING;
                break;
            case 10:
                type = RoomType.LIVING;
                break;
            case 11:
                type = RoomType.PANTRY;
                break;
            default:
                throw new Exception("shouldnt be here");
                type = RoomType.BED;
                break;
        }

        return _roomManager.GetRoomFromRoomType(type);
    }
    


}
