using UnityEngine;

public class AnimationEventManager : MonoBehaviour
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
