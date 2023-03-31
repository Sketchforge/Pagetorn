using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField, Range(0, 31)] private int _roomBoundsLayer;
    [SerializeField] private LayerMask _roomBoundsLayerMask;
    [SerializeField] private float _roomHeight = 10;
    [SerializeField] private Vector2 _halfRoomSize = new Vector2(10, 10);
    [SerializeField] private RandomRoomRotation _randomRotation = RandomRoomRotation.Any;

    [Header("Door Settings")]
    [SerializeField] private float _doorHeight = 4;
    [SerializeField] private float _doorWidth = 4;
    [SerializeField] private bool _posZDoor = true;
    [SerializeField, ShowIf("_posZDoor")] private GameObject _posZDoorGeo;
    [SerializeField] private bool _negZDoor = true;
    [SerializeField, ShowIf("_negZDoor")] private GameObject _negZDoorGeo;
    [SerializeField] private bool _posXDoor = true;
    [SerializeField, ShowIf("_posXDoor")] private GameObject _posXDoorGeo;
    [SerializeField] private bool _negXDoor = true;
    [SerializeField, ShowIf("_negXDoor")] private GameObject _negXDoorGeo;

    [Header("Connected Rooms")]
    [SerializeField, ShowIf("_posZDoor")] private Room _posZConnectedRoom;
    [SerializeField, ShowIf("_negZDoor")] private Room _negZConnectedRoom;
    [SerializeField, ShowIf("_posXDoor")] private Room _posXConnectedRoom;
    [SerializeField, ShowIf("_negXDoor")] private Room _negXConnectedRoom;

    [Header("References")]
    [SerializeField] private BoxCollider _roomBounds;
    [SerializeField] private Transform _art;
    [SerializeField] private Transform _wallParent;
    [SerializeField] private Transform _floor;
    [SerializeField] private List<Transform> _walls = new List<Transform>();

    [Header("External References")]
    [SerializeField] private Transform _cube;
    [SerializeField] private List<Room> _validNeighbors = new List<Room>();

    public float RoomHeight => _roomHeight;
    public Vector2 HalfRoomSize => _halfRoomSize;

    public void OnPlayerEnter()
    {
        CheckGenerateNeighbors();
        if (_posZConnectedRoom) _posZConnectedRoom.CheckGenerateNeighbors();
        if (_negZConnectedRoom) _negZConnectedRoom.CheckGenerateNeighbors();
        if (_posXConnectedRoom) _posXConnectedRoom.CheckGenerateNeighbors();
        if (_negXConnectedRoom) _negXConnectedRoom.CheckGenerateNeighbors();
    }

    public void OnPlayerLeave()
    {
    }

    private void Start()
    {
        switch (_randomRotation)
        {
            case RandomRoomRotation.Flip:
                _art.localRotation *= Quaternion.Euler(0, Random.Range(0, 2) * 180f, 0);
                break;
            case RandomRoomRotation.Any:
                _art.localRotation *= Quaternion.Euler(0, Random.Range(0, 4) * 90f, 0);
                break;
        }
    }

    [Button]
    public void CheckGenerateNeighbors()
    {
        if (_posZDoor && !_posZConnectedRoom)
        {
            (bool value, Room room) = CheckCreateNeighbor(true, false);
            if (room)
            {
                _posZConnectedRoom = room;
                room._negZConnectedRoom = this;
                room._negZDoorGeo.SetActive(false);
            }
            _posZDoorGeo.SetActive(!room && !value);
        }
        if (_negZDoor && !_negZConnectedRoom)
        {
            (bool value, Room room) = CheckCreateNeighbor(true, true);
            if (room)
            {
                _negZConnectedRoom = room;
                room._posZConnectedRoom = this;
                room._posZDoorGeo.SetActive(false);
            }
            _negZDoorGeo.SetActive(!room && !value);
        }
        if (_posXDoor && !_posXConnectedRoom)
        {
            (bool value, Room room) = CheckCreateNeighbor(false, false);
            if (room)
            {
                _posXConnectedRoom = room;
                room._negXConnectedRoom = this;
                room._negXDoorGeo.SetActive(false);
            }
            _posXDoorGeo.SetActive(!room && !value);
        }
        if (_negXDoor && !_negXConnectedRoom)
        {
            (bool value, Room room) = CheckCreateNeighbor(false, true);
            if (room)
            {
                _negXConnectedRoom = room;
                room._posXConnectedRoom = this;
                room._posXDoorGeo.SetActive(false);
            }
            _negXDoorGeo.SetActive(!room && !value);
        }
    }

    private (bool, Room) CheckCreateNeighbor(bool z, bool neg)
    {
        var m = neg ? -1 : 1;
        var prefab = GetValidRoom(z, neg);
        if (prefab)
        {
            var room = Instantiate(prefab, transform.parent);
            room.transform.position = transform.position + new Vector3(z ? 0 : (_halfRoomSize.x + room._halfRoomSize.x) * m, 0, z ? (_halfRoomSize.y + room._halfRoomSize.y) * m : 0);
            return (false, room);
        }
        var pos = transform.position + new Vector3(z ? 0 : (_halfRoomSize.x + 1f) * m, -1, z ? (_halfRoomSize.y + 1f) * m : 0);
        
        if (!Physics.CheckBox(pos, Vector3.one * 0.5f, Quaternion.identity, _roomBoundsLayerMask, QueryTriggerInteraction.Collide))
        {
            return (true, null);
        }
        /*
        foreach (var hit in hitObjs)
        {
            var col = hit.collider;
            Debug.Log("Collider" + col.transform.parent.gameObject.name, col.gameObject);
            var colRoom = col.transform.parent.GetComponent<Room>();
            if (colRoom && colRoom != this) return colRoom;
        }*/
        return (false, null);
    }

    private Room GetValidRoom(bool z, bool neg)
    {
        var pos = new Vector3(z ? 0 : _halfRoomSize.x, 0, z ? _halfRoomSize.y : 0);
        if (neg)
        {
            pos.x = -pos.x;
            pos.z = -pos.z;
        }
        pos += transform.position;

        var xDir = z ? 0 : neg ? -1 : 1;
        var zDir = z ? neg ? -1 : 1 : 0;

        var c = _validNeighbors.Count;
        int start = Random.Range(0, c);
        for (var i = 0; i < c; i++)
        {
            var index = (i + start) % c;
            Room room = _validNeighbors[index];
            if (z && neg && room._posZDoor || z && !neg && room._negZDoor || !z && neg && room._posXDoor || !z && !neg && room._negXDoor)
            {
                var halfSize = new Vector3(room._halfRoomSize.x, room._roomHeight * 0.5f, room._halfRoomSize.y);
                if (CanGenerateRoom(pos + Vector3.up * room._roomHeight * 0.5f, xDir, zDir, halfSize)) return room;
            }
        }
        return null;
    }

    private bool CanGenerateRoom(Vector3 pos, float xDir, float zDir, Vector3 halfSize)
    {
        pos += new Vector3(xDir * halfSize.x, 0, zDir * halfSize.z);
        halfSize -= Vector3.one * 0.1f;
        return !Physics.CheckBox(pos, halfSize, Quaternion.identity, _roomBoundsLayerMask, QueryTriggerInteraction.Collide);
    }

#if UNITY_EDITOR
    [Button(Spacing = 20)]
    private void RefreshRoom()
    {
        if (!_roomBounds) _roomBounds = transform.GetComponentInChildren<BoxCollider>();
        if (!_roomBounds)
        {
            _roomBounds = new GameObject("RoomBounds", typeof(BoxCollider)).GetComponent<BoxCollider>();
            _roomBounds.transform.SetParent(transform, false);
        }
        _roomBounds.isTrigger = true;
        _roomBounds.gameObject.layer = _roomBoundsLayer;
        if (!_wallParent) _wallParent = transform.Find("WallParent");
        if (!_wallParent)
        {
            _wallParent = new GameObject("WallParent").transform;
            _wallParent.SetParent(transform, false);
        }
        if (!_art) _art = transform.Find("Art");
        if (!_art)
        {
            _art = new GameObject("Art").transform;
            _art.SetParent(transform, false);
        }

        _roomBounds.center = new Vector3(0, _roomHeight * 0.5f, 0);
        _roomBounds.size = new Vector3(_halfRoomSize.x * 2, _roomHeight, _halfRoomSize.y * 2);

        if (!_floor) _floor = Instantiate(_cube, _wallParent);
        _floor.localPosition = new Vector3(0, -0.05f, 0);
        _floor.localScale = new Vector3(_halfRoomSize.x * 2, 0.1f, _halfRoomSize.y * 2);
        foreach (Transform t in _walls.Where(t => t))
        {
            if (UnityEditor.EditorApplication.isPlaying) Destroy(t.gameObject);
            else DestroyImmediate(t.gameObject);
        }
        _walls.Clear();

        _posZDoorGeo = GenerateDoorWall(true, false, _posZDoor);
        _negZDoorGeo = GenerateDoorWall(true, true, _negZDoor);
        _posXDoorGeo = GenerateDoorWall(false, false, _posXDoor);
        _negXDoorGeo = GenerateDoorWall(false, true, _negXDoor);

        GameObject GenerateDoorWall(bool z, bool neg, bool door)
        {
            var pos = new Vector3(z ? 0 : _halfRoomSize.x - 0.05f, _roomHeight * 0.5f, z ? _halfRoomSize.y - 0.05f : 0);
            if (neg)
            {
                pos.x = -pos.x;
                pos.z = -pos.z;
            }
            if (door)
            {
                var doorWall = Instantiate(_cube, _wallParent);
                pos.y = _doorHeight * 0.5f;
                doorWall.localPosition = pos;
                doorWall.localScale = new Vector3(z ? _doorWidth : 0.1f, _doorHeight, z ? 0.1f : _doorWidth);
                _walls.Add(doorWall);

                var centerWall = Instantiate(_cube, _wallParent);
                pos.y = (_roomHeight + _doorHeight) * 0.5f;
                centerWall.localPosition = pos;
                centerWall.localScale = new Vector3(z ? _doorWidth : 0.1f, _roomHeight - _doorHeight, z ? 0.1f : _doorWidth);
                _walls.Add(centerWall);

                var leftWall = Instantiate(_cube, _wallParent);
                var offset = z ? _halfRoomSize.x : _halfRoomSize.y;
                offset -= (offset - _doorWidth * 0.5f) * 0.5f;
                if (z) pos.x -= offset;
                else pos.z -= offset;
                pos.y = _roomHeight * 0.5f;
                var s = (z ? _halfRoomSize.x : _halfRoomSize.y) - _doorWidth * 0.5f;
                leftWall.localPosition = pos;
                leftWall.localScale = new Vector3(z ? s : 0.1f, _roomHeight, z ? 0.1f : s);
                _walls.Add(leftWall);

                var rightWall = Instantiate(_cube, _wallParent);
                if (z) pos.x += offset * 2;
                else pos.z += offset * 2;
                rightWall.localPosition = pos;
                rightWall.localScale = new Vector3(z ? s : 0.1f, _roomHeight, z ? 0.1f : s);
                _walls.Add(rightWall);

                doorWall.gameObject.SetActive(false);
                return doorWall.gameObject;
            }
            var wall = Instantiate(_cube, _wallParent);
            wall.localPosition = pos;
            wall.localScale = new Vector3(z ? _halfRoomSize.x * 2 : 0.1f, _roomHeight, z ? 0.1f : _halfRoomSize.y * 2);
            _walls.Add(wall);
            return null;
        }
    }
#endif
}

public enum RandomRoomRotation
{
    None,
    Flip,
    Any
}