using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDetails : MonoBehaviour
{
    [Tooltip("With UP being the Z axis (blue) and RIGHT being X (red), mark which walls have an entrance.")]
    public OpenDoors openDoorLocations;
	[SerializeField] private GameObject distanceObject;
    //[SerializeField] private Vector2 fullRoomSize;

	public float upDistanceToDoor;
	public float downDistanceToDoor;
	public float leftDistanceToDoor;
	public float rightDistanceToDoor;

	private void OnValidate()
	{
		if (openDoorLocations.Up)
		{
			upDistanceToDoor = distanceObject.transform.localScale.z / 2;
		}
		if (openDoorLocations.Down)
		{
			downDistanceToDoor = distanceObject.transform.localScale.z / 2;
		}
		if (openDoorLocations.Left)
		{
			leftDistanceToDoor = distanceObject.transform.localScale.x / 2;
		}
		if (openDoorLocations.Right)
		{
			rightDistanceToDoor = distanceObject.transform.localScale.x / 2;
		}
	}
}
