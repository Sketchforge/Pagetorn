using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldGenerator : MonoBehaviour
{
    [Tooltip("Should NOT be referenced from Prefabs folder, but rather from objects already in the scene")]
    public RoomDetails[] roomsToInstantiate;
    public RoomDetails[] hallwaysToInstantiate;
    private int i = 0;

    private List<RoomDetails> roomsGenerated = new List<RoomDetails>();

    // Start is called before the first frame update
    void Start()
    {
        i = 0;
        /*for (int i = 0; i < roomsToInstantiate.Length; i++)
		{
            Instantiate<GameObject>(roomsToInstantiate[i].gameObject);
		}*/
    }

    // Update is called once per frame
    void Update()
    {
        /*if ((KeyCode.I))
        {
            Instantiate<GameObject>(roomsToInstantiate[i].gameObject).SetActive(true);
            i++;
            if (i >= roomsToInstantiate.Length) i = 0;
        }*/
    }

    private void OnJump() // temporarily using input system to set up
	{
        if (roomsGenerated.Count <= 0)
        {
            Instantiate<GameObject>(roomsToInstantiate[i].gameObject, roomsToInstantiate[i].gameObject.transform.position, Quaternion.identity, transform).SetActive(true);
            roomsGenerated.Add(roomsToInstantiate[i]);
            i++;
        } else
		{
            //Debug.Log("Previous room: " + roomsGenerated[roomsGenerated.Count - 1]);
                GenerateRoom();
                //Debug.Log("Current room: " + roomsGenerated[roomsGenerated.Count - 1]);
        }
        if (i >= roomsToInstantiate.Length) i = 0;
    }

    private void GenerateRoom()
	{
        int loopStart = 0;
        int loopEnd = 0;
        bool isHallway = false;
        if (roomsGenerated.Count % 2 == 1)
		{
            loopStart = 0;
            loopEnd = hallwaysToInstantiate.Length;
            isHallway = true;
		}
        else
		{
            loopStart = i;
            loopEnd = roomsToInstantiate.Length;
            isHallway = false;
        }

        for (int j = loopStart; j < loopEnd; j++)
        {
            var prevRoom = roomsGenerated[roomsGenerated.Count - 1];
            var currentRoom = roomsToInstantiate[j];
            if (isHallway) currentRoom = hallwaysToInstantiate[j];
            var prevRoomLocation = prevRoom.gameObject.transform.position;

            if (prevRoom.gameObject != currentRoom.gameObject)
            {
                //Debug.Log("Previous room: " + prevRoom.gameObject.name + ", current room: " + currentRoom.gameObject.name);
                if (prevRoom.openDoorLocations.Up && currentRoom.openDoorLocations.Down)
                {
                    InstanceUp(prevRoom, currentRoom, prevRoomLocation);
                    i = j;
                    return;
                }
                else if (prevRoom.openDoorLocations.Down && currentRoom.openDoorLocations.Up)
                {
                    InstanceDown(prevRoom, currentRoom, prevRoomLocation);
                    i = j;
                    return;
                }
                else if (prevRoom.openDoorLocations.Left && currentRoom.openDoorLocations.Right)
                {
                    InstanceLeft(prevRoom, currentRoom, prevRoomLocation);
                    i = j;
                    return;
                }
                else if (prevRoom.openDoorLocations.Right && currentRoom.openDoorLocations.Left)
                {
                    InstanceRight(prevRoom, currentRoom, prevRoomLocation);
                    i = j;
                    return;
                }
            }
        }
        Debug.Log("Couldn't find an object to instantiate");
    }

    private void InstanceUp(RoomDetails prevRoom, RoomDetails currentRoom, Vector3 prevRoomLocation)
	{
        currentRoom.gameObject.transform.position = 
            new Vector3(prevRoomLocation.x, currentRoom.gameObject.transform.position.y, prevRoomLocation.z + (prevRoom.upDistanceToDoor + currentRoom.downDistanceToDoor));

        currentRoom.openDoorLocations.Down = false;
        prevRoom.openDoorLocations.Up = false;
        roomsGenerated.Add(Instantiate<GameObject>(currentRoom.gameObject, transform).GetComponent<RoomDetails>());

        //Debug.Log("Last up, now down. Distance: " + prevRoomLocation.z + " Plus Door distance: " + (prevRoom.upDistanceToDoor + currentRoom.downDistanceToDoor));
	}

    private void InstanceDown(RoomDetails prevRoom, RoomDetails currentRoom, Vector3 prevRoomLocation)
	{
        currentRoom.gameObject.transform.position = 
            new Vector3(prevRoomLocation.x, currentRoom.gameObject.transform.position.y, prevRoomLocation.z - (prevRoom.downDistanceToDoor + currentRoom.upDistanceToDoor));

        currentRoom.openDoorLocations.Up = false;
        prevRoom.openDoorLocations.Down = false;
        roomsGenerated.Add(Instantiate<GameObject>(currentRoom.gameObject, transform).GetComponent<RoomDetails>());

        //Debug.Log("Last down, now up. Distance: " + prevRoomLocation.z + " Minus Door distance: " + (prevRoom.upDistanceToDoor + currentRoom.downDistanceToDoor));
	}

    private void InstanceLeft(RoomDetails prevRoom, RoomDetails currentRoom, Vector3 prevRoomLocation)
    {
        currentRoom.gameObject.transform.position = 
            new Vector3(prevRoomLocation.x - (prevRoom.leftDistanceToDoor + currentRoom.rightDistanceToDoor), currentRoom.gameObject.transform.position.y, prevRoomLocation.z);

        currentRoom.openDoorLocations.Right = false;
        prevRoom.openDoorLocations.Left = false;
        //Instantiate<GameObject>(currentRoom.gameObject, transform).SetActive(true);
        roomsGenerated.Add(Instantiate<GameObject>(currentRoom.gameObject, transform).GetComponent<RoomDetails>());
        
        //Debug.Log("Last left, now right. Distance: " + prevRoomLocation.x + " Minus Door distance: " + (prevRoom.leftDistanceToDoor + currentRoom.rightDistanceToDoor));
    }

    private void InstanceRight(RoomDetails prevRoom, RoomDetails currentRoom, Vector3 prevRoomLocation)
    {
        currentRoom.gameObject.transform.position = 
            new Vector3(prevRoomLocation.x + (prevRoom.rightDistanceToDoor + currentRoom.leftDistanceToDoor), currentRoom.gameObject.transform.position.y, prevRoomLocation.z);

        currentRoom.openDoorLocations.Left = false;
        prevRoom.openDoorLocations.Right = false;
        //Instantiate<GameObject>(currentRoom.gameObject, transform).SetActive(true);
        roomsGenerated.Add(Instantiate<GameObject>(currentRoom.gameObject, transform).GetComponent<RoomDetails>());

        //Debug.Log("Last right, now left. Distance: " + prevRoomLocation.x + " Plus Door distance: " + (prevRoom.leftDistanceToDoor + currentRoom.rightDistanceToDoor));
    }
}
