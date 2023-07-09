using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Random = System.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public class RoomManager : MonoBehaviour
{
    public enum RoomType { FLOOR1, FLOOR2, PANTRY, LIVING, KITCHEN, TOILET, BED };

    public Character _character;
    public Room[] _roomPrefabs;
    public List<Room> ActiveRooms { get; set; } = new();

    public Score _score;

    AudioSource source;

    public AnimationCurve _curve;

    int _nrCount = 0;

    public void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void Start()
    {
        //source.Play();
    }

    public async Task CreateRoom()
    {
        CreateRoomSequence();
        await Task.Delay(1500);
    }

    public Room GetRoomFromRoomType(RoomType type)
    {
        foreach (Room room in ActiveRooms.Where(room => room.RoomType == type))
        {
            return room;
        }

        throw new Exception("room of type missing");
    }

    public Room GetRoomToDestroy(List<Room> remainingRooms, RoomType type)
    {
        Room roomToDestroy = null;

        int failSave = 0; 
        while (failSave <= 10000)
        {
            failSave++;
            roomToDestroy = remainingRooms[UnityEngine.Random.Range(0, remainingRooms.Count)];

            if (roomToDestroy.RoomType == type)
            {
                continue;
            }

            break;

        }

        if (failSave >= 10000)
        {
            throw new Exception("escaped endless loop");
        }

        return roomToDestroy;
    }

    public async Task RemoveRoom(Room roomToDestroy)
    {
        await roomToDestroy.Disintegrate(_curve);

        if (!Application.isPlaying)
        {
            return;
        }

        // Kill dependencies
        foreach (RoomConnection connections in roomToDestroy.RoomConnections)
        {
            if (connections.ConnectingRoom != null)
            {
                connections.ConnectingRoom.RemoveFromConnections(roomToDestroy);
                connections.ConnectingRoom = null;
            }
        }

        Room room = _character.CalculateCurrentRoom();


        if (room != null && roomToDestroy == room)
        {
            _score.ActivateEndScreen();
            Time.timeScale = 0;
            return;
        }

        ActiveRooms.Remove(roomToDestroy);


        Destroy(roomToDestroy.gameObject);
    }

    public RoomType DetermineType()
    {
        // Random, aber jeder Type darf nur ein mal vorhanden sein

        List<RoomType> allTypes = EnumToList<RoomType>();

        foreach (Room room in ActiveRooms)
        {
            allTypes.Remove(room.RoomType);
        }

        if (allTypes.Count == 0)
        {
            Debug.Log("All types have been used up");
            List<RoomType> fullList = EnumToList<RoomType>();

            return GetRandomType(fullList);
        }

        return GetRandomType(allTypes);
    }

    public RoomType GetRandomType(List<RoomType> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }


    public static List<T> EnumToList<T>()
    {
        Type enumType = typeof(T);

        if (!enumType.IsEnum)
        {
            throw new ArgumentException("T must be an enum type.");
        }

        return new List<T>((T[])Enum.GetValues(enumType));
    }


    public RoomConnection CreateRoomSequence()
    {
        RoomConnection foundConnection = null;
        int breakOut = 0;

        while (breakOut <= 1000)
        {
            breakOut++;

            RoomType type = DetermineType();

            Room roomToAddTo = FindRoomToAddTo();

            // find random connection
            foundConnection = FindConnectionForRoom(roomToAddTo);

            if (foundConnection == null)
            {
                continue;
            }

            bool notCollided = InitializeNewRoom(foundConnection, type);

            if (notCollided)
            {
                break;
            }
        }

        if (breakOut >= 1000)
        {
            throw new Exception("Broke out of endless loop");
        }

        return foundConnection;
    }

    public Room FindRoomToAddTo()
    {
        return ActiveRooms[UnityEngine.Random.Range(0, ActiveRooms.Count)];
    }

    public RoomConnection FindConnectionForRoom(Room baseRoom)
    {
        // wir nehmen von BaseRoom eine Connection die noch nicht besetzt ist

        // test ob dieser Raum noch was frei hat.
        bool isAllFull = true;

        foreach (RoomConnection room in baseRoom.RoomConnections)
        {
            if (room.ConnectingRoom == null)
            {
                isAllFull = false;
                break;
            }
        }

        if (isAllFull)
        {
            return null;
        }

        RoomConnection usedConnection = null;
        int breakOut = 0;
        while (usedConnection == null && breakOut < 10000)
        {
            RoomConnection testedConnection = baseRoom.RoomConnections[UnityEngine.Random.Range(0, baseRoom.RoomConnections.Count)];

            // einen Raum gefunden, der noch nicht gefüllt ist (hier muss noch validiert werden ob der zu platzierende Raum überhaupt platziert werden kann.
            if (testedConnection.ConnectingRoom == null)
            {
                usedConnection = testedConnection;
            }

            breakOut++;
        }

        if (breakOut >= 10000)
        {
            throw new Exception("Broken out of endless loop");
        }

        // kann besetzt werden
        return usedConnection;
    }


    public bool TestNoCollisions(RoomType type, RoomConnection newRoomConnection)
    {
        foreach (Vector2 spotCheck in Room.GetSpotChecksPerType(type))
        {
            Vector3 spotCheck3D = new Vector3(spotCheck.x, 0, spotCheck.y);

            foreach (Room room in ActiveRooms)
            {
                foreach (Collider collider in room._colliders)
                {
                    if (collider.bounds.Contains(newRoomConnection.Room.transform.position + newRoomConnection.Room.transform.rotation * spotCheck3D))
                    {
                        return false;
                    }
                }

                
            }
        }
        
        return true;
    }

    public void PlaceRoomFromConnection(RoomConnection existingConnection, RoomConnection newConnection)
    {
        // wir nutzen bisher nur 1x1 Räume

        float spacesFromStart = (int) Vector2.Distance(existingConnection.GetConnectionPosition(), existingConnection.Room.WalkPoint);
        float spacesFromEnd = (int)Vector2.Distance(newConnection.GetConnectionPosition(), newConnection.Room.WalkPoint);

        float totalSpaces = spacesFromStart + spacesFromEnd + 1;

        Vector2 startPosition = existingConnection.Room.WalkPoint;
        Vector2 direction = (existingConnection.GetConnectionPosition() - existingConnection.Room.WalkPoint).normalized;

        Vector2 end = startPosition + direction * totalSpaces;

        newConnection.Room.transform.position = new Vector3(end.x, 0, end.y);
    }

    public Room GetRoomPrefabFromType(RoomType type)
    {
        return _roomPrefabs[(int)type];
    }

    public bool InitializeNewRoom(RoomConnection existingConnection, RoomType type)
    {
        Room newRoom = Instantiate(GetRoomPrefabFromType(type), Vector3.zero, Quaternion.identity);
        newRoom.gameObject.name = type.ToString();
        _nrCount++;
        newRoom.InstantiateConnections();

        // room drehen, sodass eine der Connections in die richtige Richtung zeigt.
        // 1. Connection auswählen

        if (existingConnection != null)
        {
            RoomConnection newRoomConnection = newRoom.RoomConnections[UnityEngine.Random.Range(0, newRoom.RoomConnections.Count)];

            // Place in room
            PlaceRoomFromConnection(existingConnection, newRoomConnection);

            RotateNewRoom(newRoomConnection, existingConnection.Room);

            // test ob die Kollision verursachen würde

            if (!TestNoCollisions(type, newRoomConnection))
            {
                Destroy(newRoom.gameObject);
                return false;
            }

            _ = newRoom.Integrate();


            // in beide Richtungen verbinden! -> vorher auch feststellen mit welcher existingConnection ich den raum verbinden will.
            newRoomConnection.ConnectingRoom = existingConnection.Room;
            existingConnection.ConnectingRoom = newRoom;
        }

        AddRoom(newRoom);
        return true;
    }

    void RotateNewRoom(RoomConnection newRoomConnection, Room existingRoom)
    {
        // unrotated
        Vector2 toConnection = newRoomConnection.GetConnectionPosition() - newRoomConnection.Room.WalkPoint;

        // In Richtung drehen damit's eine passende Connection wird!
        Vector2 forward2D = existingRoom.WalkPoint - newRoomConnection.Room.WalkPoint;
        Vector3 forward3D = new Vector3(forward2D.x, 0, forward2D.y);

        Quaternion lookTowardsRotation = Quaternion.LookRotation(forward3D, Vector3.up);
        float angle = Vector2.SignedAngle(Vector2.up, toConnection);

        Quaternion rotateForRoomRotation = Quaternion.Euler(0, angle, 0);

        newRoomConnection.Room.transform.rotation = lookTowardsRotation * rotateForRoomRotation;
    }

    public bool WalksTowardsRoom(Room goalRoom, Room currentRoom, Room previousRoom)
    {
        if (goalRoom == currentRoom)
        {
            return true;
        }

        foreach (RoomConnection neighbourRoomConnection in currentRoom.RoomConnections)
        {
            if (neighbourRoomConnection == null)
            {
                continue;
            }

            if (neighbourRoomConnection.ConnectingRoom == null)
            {
                continue;
            }

            if (previousRoom == neighbourRoomConnection.ConnectingRoom)
            {
                continue;
            }

            if (WalksTowardsRoom(goalRoom, neighbourRoomConnection.ConnectingRoom, currentRoom))
            {
                return true;
            }
        }

        return false;
    }

    public void AddRoom(Room room)
    {
        ActiveRooms.Add(room);
    }
}
