using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : PlayerInteractable
{
    [SerializeField] List<GameObject> _myLoot;
    [SerializeField] Transform _lootContainer;
    [SerializeField] public int _indexObjectToSpawn;
    [SerializeField] float upwardsModifier;
    [SerializeField] float throwForce;
    [SerializeField] Animator _animator;
    [SerializeField] Animation _dropAnimation;
    [SerializeField] GameObject _objectToStopRendering;
    public bool _collectedItem;
    Vector3 throwDirection;

    void Start()
    {
        throwDirection = transform.forward + Vector3.up * upwardsModifier;

    }

    void Update()
    {
        throwDirection = transform.forward + Vector3.up * upwardsModifier;
    }

    public override void Interact()
    {
        //DropItems();
        if (!_collectedItem)
            DropRandomItem();
        
    }

    [Button]
    public void DropItems()
    {
        for (int i = 0; i <= _indexObjectToSpawn; i++)
        {
            var lootDropped = Instantiate(_myLoot[i], _lootContainer, false);
            Debug.Log("Instantiated " + lootDropped);
            Rigidbody _lootRigidbody = lootDropped.GetComponent<Rigidbody>();
            _lootRigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            //Animator lootAnim = lootDropped.GetComponent<Animator>();
            //lootAnim.animation
        }
    }

    public void DropRandomItem()
    {
        int i = Random.Range(0, _myLoot.Count);
        var lootDropped = Instantiate(_myLoot[i], _lootContainer, false);
        Debug.Log("Instantiated " + lootDropped);
        Rigidbody _lootRigidbody = lootDropped.GetComponent<Rigidbody>();
        _lootRigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        _collectedItem = true;
        _objectToStopRendering.SetActive(false);
            //Animator lootAnim = lootDropped.GetComponent<Animator>();
            //lootAnim.animation
    


    }

    public void RefreshSelf()
    {
        _collectedItem = false;
        _objectToStopRendering.SetActive(true);
    }
}
