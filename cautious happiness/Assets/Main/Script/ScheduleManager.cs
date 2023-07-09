using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static RoomManager;

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
                type = RoomType.PANTRY;
                break;
            case 3:
                type = RoomType.TOILET;
                break;
            case 4:
                type = RoomType.TOILET;
                break;
            case 5:
                type = RoomType.KITCHEN;
                break;
            case 6:
                type = RoomType.KITCHEN;
                break;
            case 7:
                type = RoomType.LIVING;
                break;
            case 8:
                type = RoomType.LIVING;
                break;
            case 9:
                type = RoomType.LIVING;
                break;
            case 10:
                type = RoomType.PANTRY;
                break;
            case 11:
                type = RoomType.BED;
                break;
        }

        if (_clock.t < 0.3f)
        {
            type = RoomType.BED;
        }
        else if (_clock.t < 0.35f)
        {
            type = RoomType.TOILET;
        }
        else if (_clock.t < 0.5f)
        {
            type = RoomType.KITCHEN;
        }
        else if (_clock.t < 0.6f)
        {
            type = RoomType.PANTRY;
        }
        else if (_clock.t < 0.8f)
        {
            type = RoomType.LIVING;
        }
        else if (_clock.t < 0.9f)
        {
            type = RoomType.TOILET;
        }
        else if (_clock.t < 1f)
        {
            type = RoomType.BED;
        }
        else
        {
            type = RoomType.BED;
        }

        return _roomManager.GetRoomFromRoomType(type);
    }
    


}
