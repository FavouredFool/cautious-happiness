using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static RoomManager;

public class ScheduleManager : MonoBehaviour
{
    RoomManager _roomManager;

    public void Awake()
    {
        _roomManager = GetComponent<RoomManager>();
    }

    public List<Room> GetRoomOrderList()
    {
        List<Room> roomOrder = _roomManager.ActiveRooms;

        roomOrder.Sort(new RoomComparer());

        return roomOrder;
    }

    public static int GetImportanceFromRoomType(RoomType type)
    {
        return type switch
        {
            RoomType.BED => 1,
            RoomType.TOILET => 2,
            RoomType.FLOOR1 => 3,
            RoomType.FLOOR2 => 4,
            RoomType.KITCHEN => 5,
            RoomType.PANTRY => 6,
            RoomType.LIVING => 7,
            _ => -1
        };
    }

    public class RoomComparer : IComparer<Room>
    {
        public int Compare(Room x, Room y)
        {
            int xImportance = GetImportanceFromRoomType(x.RoomType);
            int yImportance = GetImportanceFromRoomType(y.RoomType);

            if (xImportance < yImportance)
            {
                return -1;
            }
            else if (xImportance > yImportance)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }


}
