using System;
using System.Collections.Generic;
using System.Linq;
using CoffeyUtils;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

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

    [SerializeField] private float smallRange = 10; // if sides hit something under this, limit to a 2 gen
    [SerializeField] private float medRange = 20; // if forward hit something under this (max values may crash), kill the generation
    [SerializeField] private float largeRange = 40; // if sides hit something under this, limit to a 2 gen
    [SerializeField] private float largestRange = 60; // if forward hit something under this (max values may crash), kill the generation

    private void Start()
    {
        roomIndex = 0;
        
        // First Room <3
        RoomDetails firstRoom = Instantiate<GameObject>(initialRooms[roomIndex].gameObject, initialRooms[roomIndex].transform.position, Quaternion.identity, transform).GetComponent<RoomDetails>();
        roomsGenerated.Add(firstRoom);

        SpawnHallways(firstRoom);

        lastRoomIndex = roomIndex;
    }

    private void OnJump() => GenerateRooms(); // temporarily using input system to set up

    [Button]
    private void GenerateRooms()
	{
        Debug.Log("Generating More Rooms");

        var openRooms = roomsStillOpen.Where(room => room.AnyDoorOpen).ToList();
        roomsStillOpen.Clear();

        foreach (var lastRoom in openRooms)
        {
            RoomDetails[] roomList = FindRoomToCreate(lastRoom);

            // Randomly select a room from the given list.
            roomIndex = Random.Range(0, roomList.Length);
            Debug.Log("Room index: " + roomIndex);
            Debug.Log("Room list: " + roomList.Length);

            var newRoom = Instantiate(roomList[roomIndex].gameObject, transform).GetComponent<RoomDetails>();
            Debug.Log(newRoom);

            SpawnRoom(newRoom, lastRoom);
        }
	}

    private RoomDetails[] FindRoomToCreate(RoomDetails currentRoom)
    {
        Debug.Log("Finding rooms");
        // Get the list of rooms to instantiate from
        List<RoomDetails> roomsToUse = new List<RoomDetails>();
        // Rooms that will actually be used
        List<RoomDetails> roomsToInstantiate = new List<RoomDetails>();

        float vDistanceCheck = 0;
        float hDistanceCheck = 0;
        Vector3 hitDirection = new Vector3(0, 0, 0);

        // check where the open door is so you can offset the box on the correct side
        if (currentRoom.openDoorLocations.Up)
        {
            vDistanceCheck = currentRoom.upDistanceToDoor + 1;
            hitDirection = new Vector3(0, 0, 1);
        }
        else if (currentRoom.openDoorLocations.Down)
        {
            vDistanceCheck = -currentRoom.downDistanceToDoor - 1;
            hitDirection = new Vector3(0, 0, -1);
        }
        else if (currentRoom.openDoorLocations.Left)
        {
            hDistanceCheck = -currentRoom.leftDistanceToDoor - 1;
            hitDirection = new Vector3(-1, 0, 0);
        }
        else if (currentRoom.openDoorLocations.Right)
        {
            hDistanceCheck = currentRoom.rightDistanceToDoor + 1;
            hitDirection = new Vector3(1, 0, 0);
        }

        var pos = currentRoom.transform.position;
        // cache the position where the cast starts at (the end of the hallway)
        Vector3 endPosition = new Vector3(pos.x + hDistanceCheck, pos.y, pos.z + vDistanceCheck);

        //Debug.Log("Normal: " + endPosition + hitDirection + "Left: " + leftOffset + "Right: " + rightOffset);

        if (Physics.Raycast(endPosition, hitDirection, out RaycastHit hitRay))
        {
            // Check distance to object
            float distance = hitRay.distance;

            if (distance < smallRange) // hit before small room (close door off)
            {
                Debug.Log("Checking small room");
                foreach (RoomDetails room in currentRoom.roomTransitions)
                {
                    if (room.fullRoomSize.x < smallRange)
                    {
                        roomsToUse.Add(room);
                        Debug.Log("Added small room");
                    }
                }
            }
            else if (distance < medRange) // hit before medium room (spawn small)
            {
                Debug.Log("Checking med room");
                foreach (RoomDetails room in currentRoom.roomTransitions)
                {
                    if (room.fullRoomSize.x < medRange)
                    {
                        roomsToUse.Add(room);
                        Debug.Log("Added med room");
                    }
                }
            }
            else if (distance < largeRange) // hit before large room (spawn medium)
            {
                Debug.Log("Checking large room");
                foreach (RoomDetails room in currentRoom.roomTransitions)
                {
                    if (room.fullRoomSize.x < largeRange)
                    {
                        roomsToUse.Add(room);
                        Debug.Log("Added large room");
                    }
                }
            }
            else if (distance < largestRange) // hit before large hallway (spawn room/small hallway?)
            {
                Debug.Log("Checking largest room");
                foreach (RoomDetails room in currentRoom.roomTransitions)
                {
                    if (room.fullRoomSize.x < largestRange)
                    {
                        roomsToUse.Add(room);
                        Debug.Log("Added largest room");
                    }
                }
            }
            else // full room to use
            {
                Debug.Log("Checking all room, hit too far");
                foreach (RoomDetails room in currentRoom.roomTransitions)
                {
                    roomsToUse.Add(room);
                    Debug.Log("Added all rooms, hit too far");
                }
            }
        }
        else // raycast doesn't hit
		{
            Debug.Log("Checking all room, no hit");
            foreach (RoomDetails room in currentRoom.roomTransitions)
            {
                roomsToUse.Add(room);
                Debug.Log("Added all room, no hit");
            }
        }


        // grab the position of the point to see if it's within any circle
        float checkRoomPosition = Vector3.Distance(new Vector3(0, 0, 0), endPosition);

        // if the room is within the first area,
        if (checkRoomPosition < initialRoomDistance)
        {
            Debug.Log("Checking start room");
            foreach (RoomDetails room in roomsToUse)
            {
                if (room.roomLevel == RoomLevel.Start || room.roomLevel == RoomLevel.NONE)
                {
                    roomsToInstantiate.Add(room);
                    Debug.Log("Added start room");
                }
            }
        }
        // if the room is within the middle area,
        else if (checkRoomPosition >= initialRoomDistance && checkRoomPosition < middleRoomDistance)
        {
            Debug.Log("Checking middle room");
            foreach (RoomDetails room in roomsToUse)
            {
                if (room.roomLevel == RoomLevel.Middle || room.roomLevel == RoomLevel.NONE)
                {
                    roomsToInstantiate.Add(room);
                    Debug.Log("Added middle room");
                }
            }
        }
        // if the room is within the final area,
        else if (checkRoomPosition >= middleRoomDistance && checkRoomPosition < finalRoomDistance)
        {
            Debug.Log("Checking final room");
            foreach (RoomDetails room in roomsToUse)
            {
                if (room.roomLevel == RoomLevel.Final || room.roomLevel == RoomLevel.NONE)
                {
                    roomsToInstantiate.Add(room);
                    Debug.Log("Added final room");
                }
            }
        }
        else
        {
            Debug.Log("Checking all room pos");
            foreach (RoomDetails room in roomsToUse)
            {
                roomsToInstantiate.Add(room);
                Debug.Log("Added all room pos");
            }
        }
        return roomsToInstantiate.ToArray();
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

        var pos = currentRoom.transform.position;
        // cache the position where the cast starts at (the end of the hallway)
        Vector3 endPosition = new Vector3(pos.x + hDistanceCheck, pos.y, pos.z + vDistanceCheck);
        // push out the left/right casts in their respective direction
        hDistanceCheck = hDistanceCheck > 0 ? hDistanceCheck + 4 : hDistanceCheck < 0 ? hDistanceCheck - 4 : 0;
        vDistanceCheck = vDistanceCheck > 0 ? vDistanceCheck + 4 : vDistanceCheck < 0 ? vDistanceCheck - 4 : 0;
        Vector3 pushedPosition = new Vector3(pos.x + hDistanceCheck, pos.y, pos.z + vDistanceCheck);

        Vector3 leftDirection = Quaternion.Euler(0, -90, 0) * hitDirection;
        Vector3 rightDirection = Quaternion.Euler(0, 90, 0) * hitDirection;
        Vector3 leftPosition = pushedPosition + leftDirection;
        Vector3 rightPosition = pushedPosition + rightDirection;

        //Debug.Log("Normal: " + endPosition + hitDirection + "Left: " + leftOffset + "Right: " + rightOffset);

        // Cast a ray and check for a hit in front of the door, if the room needs to be dead
        if (Physics.Raycast(endPosition, hitDirection, out RaycastHit hitRay))
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

        foreach (RoomDetails potentialRoom in currentRoom.roomTransitions)
		{
            Collider[] hitRooms = Physics.OverlapBox(endPosition, new Vector3(checkRadius.x, 0, checkRadius.y));

            // look into each room to grab data
            foreach (Collider hitCollide in hitRooms)
            {
                RoomDetails room = hitCollide.GetComponent<RoomDetails>();
                if (room != null && room != currentRoom)
                {
                    // grab vector3 values, compare them and find how much space you have to play with, and proceed (in other words, check if you can build lol)
                }
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
        float distanceFromCenter = Vector3.Distance(new Vector3(0, 0, 0), endPosition);

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

    private void SpawnRoom(RoomDetails room, RoomDetails hallway)
    {
        bool didMove = MoveRoom(room, hallway);

        for (int j = 0; j < 4 && didMove == false; j++)
        {
            room.RotateRoom();
            didMove = MoveRoom(room, hallway);
        }
        if (didMove == false)
        {
            Debug.LogError("Couldn't move room even after rotating!");
        }
    }

    // there is totally a way to consolidate SpawnHallways and MoveRoom I know it!
    private void SpawnHallways(RoomDetails currentRoom)
    {
        int hallIndex = Random.Range(0, hallways.Length);
        Vector3 prevRoomLocation = currentRoom.transform.position;

        if (currentRoom.openDoorLocations.Up)
        {
            RoomDetails hallway = Instantiate(hallways[hallIndex].gameObject, transform).GetComponent<RoomDetails>();
            hallway.transform.position =
                new Vector3(prevRoomLocation.x, hallway.transform.position.y, prevRoomLocation.z + (currentRoom.upDistanceToDoor + hallway.downDistanceToDoor));
            hallway.openDoorLocations.Down = false;
            currentRoom.openDoorLocations.Up = false;
            roomsGenerated.Add(hallway);
            roomsStillOpen.Add(hallway);
        }
        if (currentRoom.openDoorLocations.Down)
        {
            RoomDetails hallway = Instantiate(hallways[hallIndex].gameObject, transform).GetComponent<RoomDetails>();
            hallway.transform.position =
                new Vector3(prevRoomLocation.x, hallway.transform.position.y, prevRoomLocation.z - (currentRoom.downDistanceToDoor + hallway.upDistanceToDoor));
            hallway.openDoorLocations.Up = false;
            currentRoom.openDoorLocations.Down = false;
            roomsGenerated.Add(hallway);
            roomsStillOpen.Add(hallway);
        }
        if (currentRoom.openDoorLocations.Left)
        {
            RoomDetails hallway = Instantiate(hallways[hallIndex].gameObject, transform).GetComponent<RoomDetails>();
            hallway.RotateRoom();
            hallway.transform.position =
                new Vector3(prevRoomLocation.x - (currentRoom.leftDistanceToDoor + hallway.rightDistanceToDoor), hallway.transform.position.y, prevRoomLocation.z);
            hallway.openDoorLocations.Right = false;
            currentRoom.openDoorLocations.Left = false;
            roomsGenerated.Add(hallway);
            roomsStillOpen.Add(hallway);
        }
        if (currentRoom.openDoorLocations.Right)
        {
            RoomDetails hallway = Instantiate(hallways[hallIndex].gameObject, transform).GetComponent<RoomDetails>();
            hallway.RotateRoom();
            hallway.transform.position =
                new Vector3(prevRoomLocation.x + (currentRoom.rightDistanceToDoor + hallway.leftDistanceToDoor), hallway.transform.position.y, prevRoomLocation.z);
            hallway.openDoorLocations.Left = false;
            currentRoom.openDoorLocations.Right = false;
            roomsGenerated.Add(hallway);
            roomsStillOpen.Add(hallway);
        }
    }

    private bool MoveRoom(RoomDetails currentRoom, RoomDetails prevRoom)
    {
        Vector3 prevRoomLocation = prevRoom.transform.position;

        if (prevRoom.openDoorLocations.Up && currentRoom.openDoorLocations.Down)
        {
            currentRoom.transform.position = new Vector3(prevRoomLocation.x, currentRoom.transform.position.y, prevRoomLocation.z + (prevRoom.upDistanceToDoor + currentRoom.downDistanceToDoor));
            roomsGenerated.Add(currentRoom);
            roomsStillOpen.Add(currentRoom);
            prevRoom.openDoorLocations.Up = false;
            currentRoom.openDoorLocations.Down = false;
            return true;
        }
        if (prevRoom.openDoorLocations.Down && currentRoom.openDoorLocations.Up)
        {
            currentRoom.transform.position = new Vector3(prevRoomLocation.x, currentRoom.transform.position.y, prevRoomLocation.z - (prevRoom.downDistanceToDoor + currentRoom.upDistanceToDoor));
            roomsGenerated.Add(currentRoom);
            roomsStillOpen.Add(currentRoom);
            prevRoom.openDoorLocations.Down = false;
            currentRoom.openDoorLocations.Up = false;
            return true;
        }
        if (prevRoom.openDoorLocations.Left && currentRoom.openDoorLocations.Right)
        {
            currentRoom.transform.position = new Vector3(prevRoomLocation.x - (prevRoom.leftDistanceToDoor + currentRoom.rightDistanceToDoor), currentRoom.transform.position.y, prevRoomLocation.z);
            roomsGenerated.Add(currentRoom);
            roomsStillOpen.Add(currentRoom);
            prevRoom.openDoorLocations.Left = false;
            currentRoom.openDoorLocations.Right = false;
            return true;
        }
        if (prevRoom.openDoorLocations.Right && currentRoom.openDoorLocations.Left)
        {
            currentRoom.transform.position = new Vector3(prevRoomLocation.x + (prevRoom.rightDistanceToDoor + currentRoom.leftDistanceToDoor), currentRoom.transform.position.y, prevRoomLocation.z);
            roomsGenerated.Add(currentRoom);
            roomsStillOpen.Add(currentRoom);
            prevRoom.openDoorLocations.Right = false;
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
