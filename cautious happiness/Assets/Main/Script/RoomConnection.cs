using UnityEngine;

public class RoomConnection
{
    int _connectionIndex;

    public RoomConnection(int connectionIndex, Room room)
    {
        _connectionIndex = connectionIndex;
        Room = room;
    }

    public Room Room { get; set; }

    public Room ConnectingRoom { get; set; } = null;

    public Vector2 GetConnectionPosition()
    {
        return Room.GetConnectionFromTransform(_connectionIndex);
    }
}
