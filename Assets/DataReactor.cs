using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReactor : MonoBehaviour
{
    public float numDistanceWalked = DataManager.NumberDistanceWalked;
    public float totalTimePassed = DataManager.totalTime;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //DEBUG - Delete before final build
        numDistanceWalked = DataManager.NumberDistanceWalked;
        totalTimePassed = DataManager.totalTime;
        //

        if (DataManager.NumberDistanceWalked > 300f)
        {
            Debug.Log("Walked 300 meters");
            DataManager.NumberDistanceWalked = 0;
        }
    }

}
