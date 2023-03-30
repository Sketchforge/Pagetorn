using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField, Range(0, 31)] private int _roomBoundsLayer;
    [SerializeField] private LayerMask _roomBoundsLayerMask;
    [SerializeField] private float _roomHeight = 10;
    [SerializeField] private Vector2 _halfRoomSize = new Vector2(10, 10);

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
    [SerializeField] private Transform _floor;
    [SerializeField] private List<Transform> _walls = new List<Transform>();

    [Header("External References")]
    [SerializeField] private Transform _cube;
    [SerializeField] private List<Room> _validNeighbors = new List<Room>();

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

    [Button]
    public void CheckGenerateNeighbors()
    {
        if (_posZDoor && !_posZConnectedRoom)
        {
            var room = CheckCreateNeighbor(true, false);
            if (room)
            {
                _posZConnectedRoom = room;
                room._negZConnectedRoom = this;
            }
            _posZDoorGeo.SetActive(!room);
        }
        if (_negZDoor && !_negZConnectedRoom)
        {
            var room = CheckCreateNeighbor(true, true);
            if (room)
            {
                _negZConnectedRoom = room;
                room._posZConnectedRoom = this;
            }
            _negZDoorGeo.SetActive(!room);
        }
        if (_posXDoor && !_posXConnectedRoom)
        {
            var room = CheckCreateNeighbor(false, false);
            if (room)
            {
                _posXConnectedRoom = room;
                room._negXConnectedRoom = this;
            }
            _posXDoorGeo.SetActive(!room);
        }
        if (_negXDoor && !_negXConnectedRoom)
        {
            var room = CheckCreateNeighbor(false, true);
            if (room)
            {
                _negXConnectedRoom = room;
                room._posXConnectedRoom = this;
            }
            _negXDoorGeo.SetActive(!room);
        }
    }
    
    private Room CheckCreateNeighbor(bool z, bool neg)
    {
        var m = neg ? -1 : 1;
        var prefab = GetValidRoom(z, neg);
        if (prefab)
        {
            var room = Instantiate(prefab, transform.parent);
            room.transform.position = transform.position + new Vector3(z ? 0 : (_halfRoomSize.x + room._halfRoomSize.x) * m, 0, z ? (_halfRoomSize.y + room._halfRoomSize.y) * m : 0);
            return room;
        }
        var pos = transform.position + new Vector3(z ? 0 : (_halfRoomSize.x + 0.25f) * m, _doorHeight * 0.5f, z ? (_halfRoomSize.y + 0.25f) * m : 0); ;
        var colliders = Physics.OverlapBox(pos, Vector3.one * 0.025f, Quaternion.identity, _roomBoundsLayerMask);
        foreach (var col in colliders)
        {
            var colRoom = col.transform.parent.GetComponent<Room>();
            if (colRoom && colRoom != this) return colRoom;
        }
        return null;
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

        var xDir = z ? 0 : (neg ? -1 : 1);
        var zDir = z ? (neg ? -1 : 1) : 0;
        
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
        return !Physics.CheckBox(pos, halfSize, Quaternion.identity, _roomBoundsLayerMask);
    }
    
#if UNITY_EDITOR
    [Button(Spacing = 20)]
    private void RefreshRoom()
    {
        if (!_roomBounds) _roomBounds = transform.GetComponentInChildren<BoxCollider>();
        if (!_roomBounds)
        {
            _roomBounds = new GameObject("RoomBounds", typeof(BoxCollider)).GetComponent<BoxCollider>();
            _roomBounds.transform.SetParent(transform);
        }
        _roomBounds.isTrigger = true;
        _roomBounds.gameObject.layer = _roomBoundsLayer;
        if (!_art) _art = transform.Find("Art");
        if (!_art)
        {
            _art = new GameObject("Art").transform;
            _art.SetParent(transform);
        }
        
        _roomBounds.center = new Vector3(0, _roomHeight * 0.5f, 0);
        _roomBounds.size = new Vector3(_halfRoomSize.x * 2, _roomHeight, _halfRoomSize.y * 2);

        if (!_floor) _floor = Instantiate(_cube, _art);
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
                var doorWall = Instantiate(_cube, _art);
                pos.y = _doorHeight * 0.5f;
                doorWall.localPosition = pos;
                doorWall.localScale = new Vector3(z ? _doorWidth : 0.1f, _doorHeight, z ? 0.1f : _doorWidth);
                _walls.Add(doorWall);

                var centerWall = Instantiate(_cube, _art);
                pos.y = (_roomHeight + _doorHeight) * 0.5f;
                centerWall.localPosition = pos;
                centerWall.localScale = new Vector3(z ? _doorWidth : 0.1f, _roomHeight - _doorHeight, z ? 0.1f : _doorWidth);
                _walls.Add(centerWall);
                
                var leftWall = Instantiate(_cube, _art);
                var offset = z ? _halfRoomSize.x : _halfRoomSize.y;
                offset -= (offset - _doorWidth * 0.5f) * 0.5f;
                if (z) pos.x -= offset;
                else pos.z -= offset;
                pos.y = _roomHeight * 0.5f;
                var s = (z ? _halfRoomSize.x : _halfRoomSize.y) - _doorWidth * 0.5f;
                leftWall.localPosition = pos;
                leftWall.localScale = new Vector3(z ? s : 0.1f, _roomHeight, z ? 0.1f : s);
                _walls.Add(leftWall);
                
                var rightWall = Instantiate(_cube, _art);
                if (z) pos.x += offset * 2;
                else pos.z += offset * 2;
                rightWall.localPosition = pos;
                rightWall.localScale = new Vector3(z ? s : 0.1f, _roomHeight, z ? 0.1f : s);
                _walls.Add(rightWall);

                doorWall.gameObject.SetActive(false);
                return doorWall.gameObject;
            }
            var wall = Instantiate(_cube, _art);
            wall.localPosition = pos;
            wall.localScale = new Vector3(z ? _halfRoomSize.x * 2 : 0.1f, _roomHeight, z ? 0.1f : _halfRoomSize.y * 2);
            _walls.Add(wall);
            return null;
        }
    }
#endif
}
