using UnityEngine;

public class RoomConnection
{
    public RoomConnection(Vector2 position, Room room)
    {
        ConnectionPosition = position;
        Room = room;
    }

    public Room Room { get; set; }

    public Vector2 ConnectionPosition { get; }

    public Room ConnectingRoom { get; set; } = null;
}
