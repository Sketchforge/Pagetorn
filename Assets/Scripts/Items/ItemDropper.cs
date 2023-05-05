using System.Collections.Generic;
using CoffeyUtils;
using UnityEngine;

public class ItemDropper : PlayerInteractable
{
    [SerializeField] List<GameObject> _myLoot;
    [SerializeField] Transform _lootContainer;
    [SerializeField] public int _indexObjectToSpawn;
    [SerializeField] public int _numberToSpawn;
    [SerializeField] public bool _dropRandomItem;
    [SerializeField] float upwardsModifier;
    [SerializeField] float throwForce;
    [SerializeField] Animator _animator;
    [SerializeField] Animation _dropAnimation;
    [SerializeField] GameObject _objectToStopRendering;
    [SerializeField] SfxReference _collectionSound;
    public bool _collectedItem;
    Vector3 throwDirection;

    private void Start()
    {
        throwDirection = transform.forward + Vector3.up * upwardsModifier;
    }
    
    public override void Interact()
    {
        //DropItems();
        if (_collectedItem) return;

        if (_collectionSound != null) _collectionSound.Play();

        if (_numberToSpawn <= 1)
        {
            if (_dropRandomItem)
            {
                DropRandomItem();
            }
            else
            {
                DropItems();
            }
        }
        else if (_numberToSpawn > 1)
        {
            if (_dropRandomItem)
            {
                for (int i = 0; i <= _numberToSpawn; i++)
                {
                    DropRandomItem();
                }
            }
            else
            {
                DropItems();
            }
        }
    }

    [Button]
    public void DropItems()
    {
        throwDirection = transform.forward + Vector3.up * upwardsModifier;
        for (int i = 0; i <= _numberToSpawn; i++)
        {
            var lootDropped = Instantiate(_myLoot[i], new Vector3(_lootContainer.position.x + Random.Range(-2, 2), _lootContainer.position.y, _lootContainer.position.z), Quaternion.identity, null);
            Debug.Log("Instantiated " + lootDropped);
            Rigidbody _lootRigidbody = lootDropped.GetComponent<Rigidbody>();
            _lootRigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        }
    }

    public void DropRandomItem()
    {
        throwDirection = transform.forward + Vector3.up * upwardsModifier;
        int i = Random.Range(0, _myLoot.Count);
        var lootDropped = Instantiate(_myLoot[i], new Vector3(_lootContainer.position.x + Random.Range(-2, 2), _lootContainer.position.y, _lootContainer.position.z), Quaternion.identity, null);
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
