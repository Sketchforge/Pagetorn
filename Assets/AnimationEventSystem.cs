using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSystem : MonoBehaviour
{
    private PlayerActionManager playerAttack;

    // Update is called once per frame
    void Update()
    {
        if (playerAttack == null)
        {
            //Debug.Log("Hammer is searching for player..");
            playerAttack = FindObjectOfType<PlayerActionManager>();
        }
    }

    public void CancelStart()
	{
        playerAttack.SetAttack(true);
	}
    public void CancelEnd()
    {
        playerAttack.SetAttack(false);
    }
}
