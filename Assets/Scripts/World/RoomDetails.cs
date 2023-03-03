using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDetails : MonoBehaviour
{
    [Tooltip("With UP being the Z axis (blue) and RIGHT being X (red), mark which walls have an entrance.")]
    public OpenDoors openDoorLocations;
	[SerializeField] private GameObject distanceObject;

	[Tooltip("Amount of space taken up by the room prefab (set currently to openDoorLocation's scale). X is object's X value, Y is object's Z")]
    public Vector2 fullRoomSize;

	[Tooltip("Location data of where the object is, checking up, down, left, and right areas")]
	public Vector4 roomLocationBounds;

	public float upDistanceToDoor;
	public float downDistanceToDoor;
	public float leftDistanceToDoor;
	public float rightDistanceToDoor;

	private void OnValidate()
	{
		fullRoomSize.x = distanceObject.transform.localScale.x;
		fullRoomSize.y = distanceObject.transform.localScale.z;

		if (openDoorLocations.Up)
		{
			upDistanceToDoor = fullRoomSize.y / 2;
		}
		else upDistanceToDoor = 0;
		if (openDoorLocations.Down)
		{
			downDistanceToDoor = fullRoomSize.y / 2;
		}
		else downDistanceToDoor = 0;
		if (openDoorLocations.Left)
		{
			leftDistanceToDoor = fullRoomSize.x / 2;
		}
		else leftDistanceToDoor = 0;
		if (openDoorLocations.Right)
		{
			rightDistanceToDoor = fullRoomSize.x / 2;
		}
		else rightDistanceToDoor = 0;
	}

	private void Update()
	{
		if (roomLocationBounds.x == 0 && roomLocationBounds.y == 0)
		{
			roomLocationBounds.x = gameObject.transform.position.z + (fullRoomSize.y / 2); // up location
			roomLocationBounds.y = gameObject.transform.position.z - (fullRoomSize.y / 2); // down location
			roomLocationBounds.z = gameObject.transform.position.x - (fullRoomSize.x / 2); // left location
			roomLocationBounds.w = gameObject.transform.position.x + (fullRoomSize.x / 2); // right location
		}
	}
}
