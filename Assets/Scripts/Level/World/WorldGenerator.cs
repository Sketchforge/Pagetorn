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
    [SerializeField] private RoomDetails[] winRoom;

    [SerializeField] private RoomDetails[] twoRooms;
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

    [SerializeField] private float twoRange = 105; // if sides hit something under this, limit to a 2 gen
    [SerializeField] private float deadRange = 125; // if forward hit something under this (max values may crash), kill the generation

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
            RoomDetails newRoom;
            RoomDetails[] roomList = new RoomDetails[0];

            // Check the rooms for potential collisions. Spawn a dead end and move on if so
            int checkRoom = CheckNeighborRoom(roomsStillOpen[i]);

            if (checkRoom == 0) // raycast marked room as dead
			{
                //newRoom = Instantiate<GameObject>(deadEnds[roomIndex].gameObject, transform).GetComponent<RoomDetails>();
                //SpawnRoom(newRoom, i);
                roomList = deadEnds;
            }
            else if (checkRoom == 1) // raycast marked room as 2
			{
                roomList = twoRooms;
			}
            
            // raycast didn't mark, but did find which area to instantiate from
            else if (checkRoom == 2) // initial area
			{
                roomList = initialRooms;
			}
            else if (checkRoom == 3) // middle area
			{
                roomList = middleRooms;
			}
            else if (checkRoom == 4) // final area
			{
                roomList = finalRooms;
			}
            else if (checkRoom == 5) // past final area, where you spawn dead ends and the final room
			{
                if (roomsGenerated.Contains(winRoom[0]))
                {
                    roomList = initialRooms;
                }
                else
                {
                    roomList = winRoom;
                }
			}

            // Randomly select a room from the given list. To prevent duplicate rooms, stick in a while loop to make sure you get a different room.
            while (roomIndex == lastRoomIndex)
                roomIndex = Random.Range(0, roomList.Length);
            lastRoomIndex = roomIndex;
            if (roomIndex > roomList.Length-1) roomIndex = 0;
            Debug.Log("Room index: " + roomIndex);


            newRoom = Instantiate<GameObject>(roomList[roomIndex].gameObject, transform).GetComponent<RoomDetails>();
            Debug.Log(newRoom);

            // Rotate the room 0-3 times
            //randRotate = Random.Range((int)0, (int)3);
            //Debug.Log("Rotate amount: " + randRotate);
            //for (int j = 0; j < randRotate; j++) newRoom.RotateRoom();

            SpawnRoom(newRoom, i);
            if (checkRoom != 0) SpawnHallways(newRoom);
        }
        for (int i = 0; i < upperBound; i++)
        {
            roomsStillOpen.RemoveAt(i);
        }
	}


    private int CheckNeighborRoom(RoomDetails currentRoom)
    {
        float vDistanceCheck = 0;
        float hDistanceCheck = 0;
        Vector3 hitDirection = new Vector3(0,0,0);

        // check where the open door is so you can offset the box on the correct side
        if (currentRoom.openDoorLocations.Up) {
            vDistanceCheck = currentRoom.upDistanceToDoor + 1;
            hitDirection = new Vector3(0, 0, 1);
        }
        else if (currentRoom.openDoorLocations.Down) {
            vDistanceCheck = -currentRoom.downDistanceToDoor - 1;
            hitDirection = new Vector3(0, 0, -1);
        }
        else if (currentRoom.openDoorLocations.Left) {
            hDistanceCheck = -currentRoom.leftDistanceToDoor - 1;
            hitDirection = new Vector3(-1, 0, 0);
        }
        else if (currentRoom.openDoorLocations.Right) {
            hDistanceCheck = currentRoom.rightDistanceToDoor + 1;
            hitDirection = new Vector3(1, 0, 0);
        }

        // cache the position where the cast starts at (the end of the hallway)
        Vector3 endPostion = new Vector3(currentRoom.gameObject.transform.position.x + hDistanceCheck, currentRoom.gameObject.transform.position.y, currentRoom.gameObject.transform.position.z + vDistanceCheck);
        // push out the left/right casts in their respective direction
        hDistanceCheck = hDistanceCheck > 0 ? hDistanceCheck + 4 : hDistanceCheck < 0 ? hDistanceCheck - 4 : 0;
        vDistanceCheck = vDistanceCheck > 0 ? vDistanceCheck + 4 : vDistanceCheck < 0 ? vDistanceCheck - 4 : 0;
        Vector3 pushedPostion = new Vector3(currentRoom.gameObject.transform.position.x + hDistanceCheck, currentRoom.gameObject.transform.position.y, currentRoom.gameObject.transform.position.z + vDistanceCheck);

        Vector3 leftDirection = Quaternion.Euler(0, -90, 0) * hitDirection;
        Vector3 rightDirection = Quaternion.Euler(0, 90, 0) * hitDirection;
        Vector3 leftPosition = pushedPostion + leftDirection;
        Vector3 rightPosition = pushedPostion + rightDirection;

        //Debug.Log("Normal: " + endPostion + hitDirection + "Left: " + leftOffset + "Right: " + rightOffset);

        // Cast a ray and check for a hit in front of the door, if the room needs to be dead
        RaycastHit hitRay;
        if (Physics.Raycast(endPostion, hitDirection, out hitRay))
		{
            // Check distance to object
            float distance = hitRay.distance;
            if (distance < deadRange)
			{
                // mark for dead
                return 0;
			}
		}

        // Cast a ray and check for a hit on the sides of the door, if the room needs to be a 2 way room
        if (Physics.Raycast(leftPosition, leftDirection, out hitRay))
        {
            // Check distance to object
            float distance = hitRay.distance;
            if (distance < twoRange)
            {
                // mark for a 2 room
                return 1;
            }
        }
        if (Physics.Raycast(rightPosition, rightDirection, out hitRay))
        {
            // Check distance to object
            float distance = hitRay.distance;
            if (distance < twoRange)
            {
                // mark for a 2 room
                return 1;
            }
        }

        // check for other rooms using a physics box collider, placed where the hallway is and with the proper offset
        /*Collider[] hitRooms = Physics.OverlapBox(endPostion, new Vector3(checkRadius.x, 0, checkRadius.y));

        // look into each room to grab data
        foreach (Collider hitCollide in hitRooms)
        {
            RoomDetails room = hitCollide.GetComponent<RoomDetails>();
            if (room != null && room != currentRoom)
            {
                // grab vector3 values, compare them and find how much space you have to play with, and proceed (in other words, check if you can build lol)
            }
        }*/

        // grab the position of the point to see if it's within any circle
        float distanceFromCenter = Vector3.Distance(new Vector3(0, 0, 0), endPostion);

        // if the room is within the first area,
        if (distanceFromCenter < initialRoomDistance)
        {
            return 2;
        }
        // if the room is within the middle area,
        if (distanceFromCenter >= initialRoomDistance && distanceFromCenter < middleRoomDistance)
		{
            return 3;
        }
        // if the room is within the final area,
        if (distanceFromCenter >= middleRoomDistance && distanceFromCenter < finalRoomDistance)
        {
            return 4;
        }

        // and if the room is outside the final area
        return 5;
    }

    private void SpawnRoom(RoomDetails room, int index)
    {
        bool didMove = false;
        didMove = MoveRoom(room, roomsStillOpen[index]);

        for (int j = 0; j < 4 && didMove == false; j++)
        {
            room.RotateRoom();
            didMove = MoveRoom(room, roomsStillOpen[index]);
        }
        if (didMove == false)
        {
            Debug.LogError("Couldn't move room even after rotating!");
            return;
        }
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
        //Handles.DrawWireCube(new Vector3(transform.position.x + checkOffset.x, transform.position.y, transform.position.z + checkRadius.y / 2 + checkOffset.y), new Vector3(checkRadius.x, 0, checkRadius.y));
    }
}
