using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField, DrawSO] private SurvivalStat Health;
    [SerializeField, DrawSO] private SurvivalStat Hunger;
    [SerializeField, DrawSO] private SurvivalStat Hydration;

    [SerializeField, DrawSO] private Inventory _inventory;
    [SerializeField, DrawSO] private Magic _magic;

    private PlayerMovementScript _movementScript;
    private PlayerActions _playerActions;

    //private static Player _Instance;
    //public static Player Instance
    //{
    //    get
    //    {
    //        if (!_Instance)
    //        {
    //            // NOTE: read docs to see directory requirements for Resources.Load!
    //            var prefab = Resources.Load<GameObject>("Assets/Prefabs/PlayerFabs/Player");
    //            // create the prefab in your scene
    //            var inScene = Instantiate<GameObject>(prefab);
    //            // try find the instance inside the prefab
    //            _Instance = inScene.GetComponentInChildren<Player>();
    //            // guess there isn't one, add one
    //            if (!_Instance) _Instance = inScene.AddComponent<Player>();
    //            // mark root as DontDestroyOnLoad();
    //            DontDestroyOnLoad(_Instance.transform.root.gameObject);
    //        }
    //        return _Instance;
    //    }
    //}

    private void Awake()
    {

        _movementScript = GetComponent<PlayerMovementScript>();
        _playerActions = GetComponent<PlayerActions>();
    }

}