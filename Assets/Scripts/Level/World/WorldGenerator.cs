using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private float initialRoomDistance = 500;
    [SerializeField] private float middleRoomDistance = 1500;
    [SerializeField] private float finalRoomDistance = 2500;

    //[Tooltip("Should NOT be referenced from Prefabs folder, but rather from objects already in the scene")]
    [SerializeField] private RoomDetails[] initialRooms;
    [SerializeField] private RoomDetails[] middleRooms;
    [SerializeField] private RoomDetails[] finalRooms;

    [SerializeField] private RoomDetails[] hallways;
    [SerializeField] private RoomDetails[] deadEnds;

    private int lastRoomIndex = 0;
    private int roomIndex = 0;
    private RoomDetails randRoom;
    private int randRotate;

    private List<RoomDetails> roomsGenerated = new List<RoomDetails>();
    private List<RoomDetails> roomsStillOpen = new List<RoomDetails>();
    //private List<RoomDetails> roomsMarkedDead = new List<RoomDetails>();

    // Check how far away from the hallway and how large an area to cover for checking potential collisions
    [SerializeField] private Vector2 checkOffset = new Vector2(0,0);
    [SerializeField] private Vector2 checkRadius = new Vector2(1,1);

    // Start is called before the first frame update
    void Start()
    {
        roomIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnJump() // temporarily using input system to set up
	{
        if (roomsGenerated.Count <= 0) // if there aren't any rooms and we're at spawn, create the starting rooms. could use a new list just for these, or place them in the scene first??
        {
            RoomDetails firstRoom = Instantiate<GameObject>(initialRooms[roomIndex].gameObject, initialRooms[roomIndex].gameObject.transform.position, Quaternion.identity, transform).GetComponent<RoomDetails>();
            roomsGenerated.Add(firstRoom);

            SpawnHallways(firstRoom);

            lastRoomIndex = roomIndex;
        } else // otherwise do the maze generation
		{
            //Debug.Log("Previous room: " + roomsGenerated[roomsGenerated.Count - 1]);
            GenerateRoom2();
                //Debug.Log("Current room: " + roomsGenerated[roomsGenerated.Count - 1]);
        }
        //if (randRoom >= initialRoomsToInstantiate.Length) randRoom = 0;
    }

    private void GenerateRoom2()
	{
        Debug.Log("Starting room");
        int upperBound = roomsStillOpen.Count;
        Debug.Log("Upperbound: " + upperBound);

        for (int i = 0; i < upperBound; i++)
        {
            Debug.Log("Loop " + i);

            // Check the rooms for potential collisions. Spawn a dead end and move on if so
            CheckNeighborIsDead(roomsStillOpen[i]);

            // Randomly select a room from the given list. To prevent duplicate rooms, stick in a while loop to make sure you get a different room.
            while (roomIndex == lastRoomIndex)
                roomIndex = Random.Range(0, initialRooms.Length);
            lastRoomIndex = roomIndex;
            //roomIndex++;
            if (roomIndex > initialRooms.Length-1) roomIndex = 0;
            Debug.Log("Room index: " + roomIndex);


            RoomDetails newRoom = Instantiate<GameObject>(initialRooms[roomIndex].gameObject, transform).GetComponent<RoomDetails>();
            Debug.Log(newRoom);

            // Rotate the room 0-3 times
            //randRotate = Random.Range((int)0, (int)3);
            //Debug.Log("Rotate amount: " + randRotate);
            //for (int j = 0; j < randRotate; j++) newRoom.RotateRoom();

            bool didMove = false;
            didMove = MoveRoom(newRoom, roomsStillOpen[i]);

            for (int j = 0; j < 4 && didMove == false; j++) // rotate room is breaking the generation??
			{
                newRoom.RotateRoom();
                didMove = MoveRoom(newRoom, roomsStillOpen[i]);
            }
            if (didMove == false)
			{
                return;
			}
            SpawnHallways(newRoom);
        }
        for (int i = 0; i < upperBound; i++)
        {
            roomsStillOpen.RemoveAt(i);
        }
	}

    private bool CheckNeighborIsDead(RoomDetails currentRoom)
    {
        float vDistanceCheck = 0;
        float hDistanceCheck = 0;

        // check where the open door is so you can offset the box on the correct side
        if (currentRoom.openDoorLocations.Up)
            vDistanceCheck = currentRoom.upDistanceToDoor + checkRadius.y/2;
        else if (currentRoom.openDoorLocations.Down)
            vDistanceCheck = -currentRoom.downDistanceToDoor - checkRadius.y/2;
        else if (currentRoom.openDoorLocations.Left)
            hDistanceCheck = -currentRoom.leftDistanceToDoor - checkRadius.x/2;
        else if (currentRoom.openDoorLocations.Right)
            hDistanceCheck = currentRoom.rightDistanceToDoor + checkRadius.x/2;

        // check for other rooms, placed where the hallway is and with the proper offset
        Collider[] hitRooms = Physics.OverlapBox(
            new Vector3(currentRoom.gameObject.transform.position.x + hDistanceCheck, currentRoom.gameObject.transform.position.y, currentRoom.gameObject.transform.position.z + vDistanceCheck),
            new Vector3(checkRadius.x, 0, checkRadius.y));

        // look into each room to grab data
        foreach (Collider hit in hitRooms)
        {
            RoomDetails room = hit.GetComponent<RoomDetails>();
            if (room != null && room != currentRoom)
            {
                // grab vector3 values, compare them and find how much space you have to play with, and proceed (in other words, check if you can build lol)
            }
        }

        return false;
    }

    // there is totally a way to consolidate SpawnHallways and MoveRoom I know it!
    private void SpawnHallways(RoomDetails currentRoom)
    {
        int hallIndex = Random.Range(0, hallways.Length);
        Vector3 prevRoomLocation = currentRoom.gameObject.transform.position;

        if (currentRoom.openDoorLocations.Up)
        {
            RoomDetails hallway = Instantiate<GameObject>(hallways[hallIndex].gameObject, transform).GetComponent<RoomDetails>();
            hallway.gameObject.transform.position =
                new Vector3(prevRoomLocation.x, hallway.gameObject.transform.position.y, prevRoomLocation.z + (currentRoom.upDistanceToDoor + hallway.downDistanceToDoor));
            hallway.openDoorLocations.Down = false;
            currentRoom.openDoorLocations.Up = false;
            roomsGenerated.Add(hallway);
            roomsStillOpen.Add(hallway);
        }
        if (currentRoom.openDoorLocations.Down)
        {
            RoomDetails hallway = Instantiate<GameObject>(hallways[hallIndex].gameObject, transform).GetComponent<RoomDetails>();
            hallway.gameObject.transform.position =
                new Vector3(prevRoomLocation.x, hallway.gameObject.transform.position.y, prevRoomLocation.z - (currentRoom.downDistanceToDoor + hallway.upDistanceToDoor));
            hallway.openDoorLocations.Up = false;
            currentRoom.openDoorLocations.Down = false;
            roomsGenerated.Add(hallway);
            roomsStillOpen.Add(hallway);
        }
        if (currentRoom.openDoorLocations.Left)
        {
            RoomDetails hallway = Instantiate<GameObject>(hallways[hallIndex].gameObject, transform).GetComponent<RoomDetails>();
            hallway.RotateRoom();
            hallway.gameObject.transform.position =
                new Vector3(prevRoomLocation.x - (currentRoom.leftDistanceToDoor + hallway.rightDistanceToDoor), hallway.gameObject.transform.position.y, prevRoomLocation.z);
            hallway.openDoorLocations.Right = false;
            currentRoom.openDoorLocations.Left = false;
            roomsGenerated.Add(hallway);
            roomsStillOpen.Add(hallway);
        }
        if (currentRoom.openDoorLocations.Right)
        {
            RoomDetails hallway = Instantiate<GameObject>(hallways[hallIndex].gameObject, transform).GetComponent<RoomDetails>();
            hallway.RotateRoom();
            hallway.gameObject.transform.position =
                new Vector3(prevRoomLocation.x + (currentRoom.rightDistanceToDoor + hallway.leftDistanceToDoor), hallway.gameObject.transform.position.y, prevRoomLocation.z);
            hallway.openDoorLocations.Left = false;
            currentRoom.openDoorLocations.Right = false;
            roomsGenerated.Add(hallway);
            roomsStillOpen.Add(hallway);
        }
    }

    private bool MoveRoom(RoomDetails currentRoom, RoomDetails prevRoom)
	{
        Vector3 prevRoomLocation = prevRoom.gameObject.transform.position;

        if (prevRoom.openDoorLocations.Up)
        {
            currentRoom.gameObject.transform.position =
                new Vector3(prevRoomLocation.x, currentRoom.gameObject.transform.position.y, prevRoomLocation.z + (prevRoom.upDistanceToDoor + currentRoom.downDistanceToDoor));
            roomsGenerated.Add(currentRoom);
            currentRoom.openDoorLocations.Down = false;
            return true;
        }
        else if (prevRoom.openDoorLocations.Down)
        {
            currentRoom.gameObject.transform.position =
                new Vector3(prevRoomLocation.x, currentRoom.gameObject.transform.position.y, prevRoomLocation.z - (prevRoom.downDistanceToDoor + currentRoom.upDistanceToDoor));
            roomsGenerated.Add(currentRoom);
            currentRoom.openDoorLocations.Up = false;
            return true;
        }
        else if (prevRoom.openDoorLocations.Left)
        {
            currentRoom.gameObject.transform.position =
                new Vector3(prevRoomLocation.x - (prevRoom.leftDistanceToDoor + currentRoom.rightDistanceToDoor), currentRoom.gameObject.transform.position.y, prevRoomLocation.z);
            roomsGenerated.Add(currentRoom);
            currentRoom.openDoorLocations.Right = false;
            return true;
        }
        else if (prevRoom.openDoorLocations.Right)
        {
            currentRoom.gameObject.transform.position =
                new Vector3(prevRoomLocation.x + (prevRoom.rightDistanceToDoor + currentRoom.leftDistanceToDoor), currentRoom.gameObject.transform.position.y, prevRoomLocation.z);
            roomsGenerated.Add(currentRoom);
            currentRoom.openDoorLocations.Left = false;
            return true;
        }
        return false;
	}


	private void OnDrawGizmos()
	{
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, transform.up, initialRoomDistance);
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, transform.up, middleRoomDistance);
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.up, finalRoomDistance);

        Handles.color = Color.green;
        Handles.DrawWireCube(new Vector3(transform.position.x + checkOffset.x, transform.position.y, transform.position.z + checkRadius.y/2 + checkOffset.y), new Vector3(checkRadius.x, 0, checkRadius.y));
    }
}
