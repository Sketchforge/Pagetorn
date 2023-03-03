using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReactor : MonoBehaviour
{
    public float numDistanceWalked = DataManager.NumberDistanceWalked;
    public float totalTimePassed = DataManager.totalTime;

    [Header("Max Fields")]
    [SerializeField] float MAX_DISTANCE;
    [SerializeField] float MAX_TIME;

    [Header("Events")]
    [SerializeField] PostProcessingEvent _fogEvent;
    [SerializeField] private bool _inFog = false;
    [SerializeField] PostProcessingEvent _darkenEvent;


    // Update is called once per frame
    void Update()
    {
        //DEBUG - Delete before final build
        numDistanceWalked = DataManager.NumberDistanceWalked;
        totalTimePassed = DataManager.totalTime;
        //

        if (DataManager.NumberDistanceWalked > Random.Range(MAX_DISTANCE - 100f, MAX_DISTANCE + 100f) && !_inFog)
        {
            Debug.Log("Walked " + MAX_DISTANCE + " meters");
            _fogEvent.ActivateEvent();
            DataManager.TimesWalkedLargeDistances++;

            if (DataManager.TimesWalkedLargeDistances >= 5)
            {
                DataManager.bWalksLargeDistances = true;
            }

            _inFog = true;
        }

        if (_inFog)
        {
            Debug.Log(_fogEvent._currDuration -= Time.time);
            DataManager.NumberDistanceWalked = 0;
            _inFog = false;
        }

        if (DataManager.bWalksLargeDistances && DataManager.bKillsLotsOfMonsters) //if the player walks a lot and manages to kill a lot of monsters, they must find the game easy or not as scary.
        {
            //3 solutions: Event that Spawns a CHIMERA. Increase frequency of Librarian Chases. OR spawn more monsters and darken environment. For now, we do the latter.

        }
    }

}
