using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDetails : MonoBehaviour
{
    [Tooltip("With UP being the Z axis (blue) and RIGHT being X (red), mark which walls have an entrance.")]
    public OpenDoors openDoorLocations;
    public BoxCollider roomCollider;

	[Tooltip("Amount of space taken up by the room prefab (set currently to openDoorLocation's scale). X is object's X value, Y is object's Z")]
    public Vector2 fullRoomSize;

	[Tooltip("Location data of where the object is, checking up, down, left, and right areas")]
	public Vector4 roomLocationBounds;

	public float upDistanceToDoor;
	public float downDistanceToDoor;
	public float leftDistanceToDoor;
	public float rightDistanceToDoor;

	private int roomRotation = 0;

	public bool AnyDoorOpen => openDoorLocations.Up || openDoorLocations.Down || openDoorLocations.Left || openDoorLocations.Right;

	private void OnValidate()
	{
		if (!roomCollider) roomCollider = GetComponentInChildren<BoxCollider>();
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

	public void RotateRoom()
	{
		roomRotation++;
		roomRotation %= 4;

		//Debug.Log(gameObject.transform.rotation);
		this.gameObject.transform.Rotate(gameObject.transform.rotation.x, gameObject.transform.rotation.y + 90, gameObject.transform.rotation.z);

		// value to store the new rotated rooms' door locations
		OpenDoors newDoors;
		newDoors.Up = false;
		newDoors.Down = false;
		newDoors.Left = false;
		newDoors.Right = false;

		// get the new rotated open doors' locations
		if (openDoorLocations.Up)
			newDoors.Right = true;
		if (openDoorLocations.Down)
			newDoors.Left = true;
		if (openDoorLocations.Left)
			newDoors.Up = true;
		if (openDoorLocations.Right)
			newDoors.Down = true;

		// deactivate the locations previously
		if (!newDoors.Up)
			openDoorLocations.Up = false;
		if (!newDoors.Down)
			openDoorLocations.Down = false;
		if (!newDoors.Left)
			openDoorLocations.Left = false;
		if (!newDoors.Right)
			openDoorLocations.Right = false;

		// and set the new locations
		if (newDoors.Up)
			openDoorLocations.Up = true;
		if (newDoors.Down)
			openDoorLocations.Down = true;
		if (newDoors.Left)
			openDoorLocations.Left = true;
		if (newDoors.Right)
			openDoorLocations.Right = true;

		fullRoomSize.x = fullRoomSize.y;
		fullRoomSize.y = fullRoomSize.x;
		
		upDistanceToDoor = openDoorLocations.Up ? fullRoomSize.y * 0.5f : 0;
		downDistanceToDoor = openDoorLocations.Down ? fullRoomSize.y * 0.5f : 0;
		leftDistanceToDoor = openDoorLocations.Left ? fullRoomSize.x * 0.5f : 0;
		rightDistanceToDoor = openDoorLocations.Right ? fullRoomSize.x * 0.5f : 0;
	}

	[Button]
	private void SetSizes()
	{
		var b = roomCollider.bounds;
		fullRoomSize.x = b.size.x;
		fullRoomSize.y = b.size.z;
		

		upDistanceToDoor = openDoorLocations.Up ? b.extents.z : 0;
		downDistanceToDoor = openDoorLocations.Down ? b.extents.z : 0;
		leftDistanceToDoor = openDoorLocations.Left ? b.extents.x : 0;
		rightDistanceToDoor = openDoorLocations.Right ? b.extents.x : 0;
	}
}
