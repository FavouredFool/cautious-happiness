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
        RoomType type;

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
