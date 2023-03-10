using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/PlayerManager/Weapon")]
public class Weapon : Item
{
    [SerializeField] public GameObject myPrefab;
    //[SerializeField] public Animation myAnimation;

    //private void Start()
    //{
    //    myAnimator = GetComponent<Animator>();
    //}

    //public void UseWeaponTrigger(string animationBool)
    //{
    //    myAnimator.SetTrigger(animationBool);
    //}


}
