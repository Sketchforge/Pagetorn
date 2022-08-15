using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    [SerializeField] private GameObject _container;
    
    public void SetActive(bool active) => _container.SetActive(active);
}
