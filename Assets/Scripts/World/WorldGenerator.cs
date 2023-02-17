using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldGenerator : MonoBehaviour
{
    public RoomDetails[] roomsToInstantiate;
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
            FindRoom();
            //Debug.Log("Current room: " + roomsGenerated[roomsGenerated.Count - 1]);
        }
        if (i >= roomsToInstantiate.Length) i = 0;
    }

    private void FindRoom()
	{
        for (int j = i; j < roomsToInstantiate.Length; j++)
        {
            var prevRoom = roomsGenerated[roomsGenerated.Count - 1];
            var currentRoom = roomsToInstantiate[j];
            var prevRoomLocation = prevRoom.gameObject.transform.position;

            if (prevRoom.gameObject != currentRoom.gameObject)
            {
                //Debug.Log("Previous room: " + prevRoom.gameObject.name + ", current room: " + currentRoom.gameObject.name);
                if (prevRoom.openDoorLocations.Up && currentRoom.openDoorLocations.Down)
                {
                    currentRoom.gameObject.transform.position = 
                        new Vector3(prevRoomLocation.x, currentRoom.gameObject.transform.position.y, prevRoomLocation.z + (prevRoom.upDistanceToDoor + currentRoom.downDistanceToDoor));

                    Instantiate<GameObject>(currentRoom.gameObject, transform).SetActive(true);
                    roomsGenerated.Add(currentRoom);

                    //Debug.Log("Last up, now down. Distance: " + prevRoomLocation.z + " Plus Door distance: " + (prevRoom.upDistanceToDoor + currentRoom.downDistanceToDoor));
                    i = j;
                    break;
                }
                else if (prevRoom.openDoorLocations.Down && currentRoom.openDoorLocations.Up)
                {
                    currentRoom.gameObject.transform.position = 
                        new Vector3(prevRoomLocation.x, currentRoom.gameObject.transform.position.y, prevRoomLocation.z - (prevRoom.downDistanceToDoor + currentRoom.upDistanceToDoor));

                    Instantiate<GameObject>(currentRoom.gameObject, transform).SetActive(true);
                    roomsGenerated.Add(currentRoom);

                    //Debug.Log("Last down, now up. Distance: " + prevRoomLocation.z + " Minus Door distance: " + (prevRoom.upDistanceToDoor + currentRoom.downDistanceToDoor));
                    i = j;
                    break;
                }
                else if (prevRoom.openDoorLocations.Left && currentRoom.openDoorLocations.Right)
                {
                    currentRoom.gameObject.transform.position = 
                        new Vector3(prevRoomLocation.x - (prevRoom.leftDistanceToDoor + currentRoom.rightDistanceToDoor), currentRoom.gameObject.transform.position.y, prevRoomLocation.z);

                    Instantiate<GameObject>(currentRoom.gameObject, transform).SetActive(true);
                    roomsGenerated.Add(currentRoom);


                    //Debug.Log("Last left, now right. Distance: " + prevRoomLocation.x + " Minus Door distance: " + (prevRoom.leftDistanceToDoor + currentRoom.rightDistanceToDoor));
                    i = j;
                    break;
                }
                else if (prevRoom.openDoorLocations.Right && currentRoom.openDoorLocations.Left)
                {
                    currentRoom.gameObject.transform.position = 
                        new Vector3(prevRoomLocation.x + (prevRoom.rightDistanceToDoor + currentRoom.leftDistanceToDoor), currentRoom.gameObject.transform.position.y, prevRoomLocation.z);

                    Instantiate<GameObject>(currentRoom.gameObject, transform).SetActive(true);
                    roomsGenerated.Add(currentRoom);


                    //Debug.Log("Last right, now left. Distance: " + prevRoomLocation.x + " Plus Door distance: " + (prevRoom.leftDistanceToDoor + currentRoom.rightDistanceToDoor));
                    i = j;
                    break;
                }
            }
        }
    }

    private void GenerateRooms()
	{

	}

    private void CheckNeighbors()
	{

	}
}
