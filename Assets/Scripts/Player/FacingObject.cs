using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingObject : MonoBehaviour
{
    [SerializeField] PlayerMovementScript player;
    [SerializeField] float angle = 10;
    // Update is called once per frame
    private void Awake()
    {
        if (!player)
        {
            player = FindObjectOfType<PlayerMovementScript>();
        }
    }


    void Update()
    {
        if (Vector3.Angle(player.transform.forward, transform.position - player.transform.position) < angle)
        {
            GameManager.Data.FocusTime += Time.deltaTime;
        }
    }
}
