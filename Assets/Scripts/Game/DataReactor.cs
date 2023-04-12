using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReactor : MonoBehaviour
{
    public float numDistanceWalked = DataManager.NumberDistanceWalked;
    public float totalTimePassed = DataManager.totalTime;
    public float averageMonstersKilledPerHour = 30;
    [SerializeField] static public float monsterSpawnRate = 40f;

    [Header("Max Fields")]
    [SerializeField] float MAX_DISTANCE;
    [SerializeField] float MAX_TIME;
    [SerializeField] float MAX_MELEEATTACKS;

    [Header("Events")]
    [SerializeField] PostProcessingEvent _fogEvent;
    [SerializeField] private bool _inFog = false;
    [SerializeField] PostProcessingEvent _darkenEvent;
    [SerializeField] InstantiateEvent _smallCrawlerSpawn;
    [SerializeField] InstantiateEvent _smallRangerSpawn;
    [SerializeField] InstantiateEvent _librarianSpawn;
    [SerializeField] SoundEvent _calmMusic;
    [SerializeField] SoundEvent _chaseMusic;
    [SerializeField] SoundEvent _creepyNoise1;

    private float spellCountTimer = 60f;
    private float monsterKillCountTimer = 3600f; // one hour
    private float _resetTime = 10f;
    bool calledSpawnEvent = false;
    bool calledLibrarianSpawnEvent = false;

    private void Awake()
    {
        _calmMusic.ActivateEvent();
    }


    // Update is called once per frame
    void Update()
    {
        //DEBUG - Delete before final build
        numDistanceWalked = DataManager.NumberDistanceWalked;
        totalTimePassed = DataManager.totalTime;
        //
        

        #region Behavior Nominator
        
        //DataManager.bCollectsLotsofBooks;
        //DataManager.bExploresLotsOfRooms;

        //WALKS LARGE DISTANCES//
        if (DataManager.TimesWalkedLargeDistances >= 5)
        {
            DataManager.bWalksLargeDistances = true;
        }

        //KILLS LOTS OF MONSTERS//
        if (!DataManager.bKillsLotsOfMonsters)
            monsterKillCountTimer -= Time.deltaTime;
        else if (DataManager.bKillsLotsOfMonsters)
            _resetTime -= Time.deltaTime;

        if (monsterKillCountTimer <= 0f) //if greater than a minute
        {
            DataManager.NumberMonstersKilledLastHour = 0; //resets to 0;
            monsterKillCountTimer = 3600f;
        }

        if (DataManager.NumberMonstersKilledLastHour >= averageMonstersKilledPerHour)
            DataManager.bKillsLotsOfMonsters = true;

        if (DataManager.NumberMonstersKilledLastHour > 15f)
        {
            _smallCrawlerSpawn.ActivateEvent();
            _smallCrawlerSpawn.ActivateEvent();
        }

        if (DataManager.NumberMonstersKilledLastHour > 30f)
        {
            _smallRangerSpawn.ActivateEvent();
        }

        if (_resetTime <= 0f)
        {
            DataManager.NumberMonstersKilledLastHour = 0;
            DataManager.bKillsLotsOfMonsters = false;
            _resetTime = 10f;
        }

        //DataManager.bKillsLotsOfMonsters;

        //USES SPELLS OFTEN//
        if (!DataManager.bUsesSpellsOften)
            spellCountTimer -= Time.deltaTime;
        //Debug.Log(Time.deltaTime - spellCountTimer);
        if (spellCountTimer <= 0f) //if greater than a minute
        {
            DataManager.NumberSpellsDoneLastMinute = 0; //resets to 0;
            spellCountTimer = 60f;
        }

        if (DataManager.NumberSpellsDoneLastMinute >= 10)
        {
            DataManager.bUsesSpellsOften = true;
        }

        #endregion

        #region Behavior Reactor

        #region Distance Walked Reaction
        if (DataManager.NumberDistanceWalked > Random.Range(MAX_DISTANCE - 50f, MAX_DISTANCE + 50f) && !_inFog)
        {
            Debug.Log("Walked " + MAX_DISTANCE + " meters");
            _fogEvent.ActivateEvent();
            DataManager.TimesWalkedLargeDistances++;



            _inFog = true;
        }

        if (_inFog)
        {
            Debug.Log(_fogEvent._currDuration -= Time.time);
            DataManager.NumberDistanceWalked = 0;
            _inFog = false;
        }
        #endregion

        if (DataManager.NumberMeleeAttacksDone >= MAX_MELEEATTACKS && !_inFog)
        {
            _darkenEvent.ActivateEvent();
            _inFog = true;
            DataManager.NumberMeleeAttacksDone = 0;
        }

        if (DataManager.bWalksLargeDistances && DataManager.bKillsLotsOfMonsters && !calledSpawnEvent) //if the player walks a lot and manages to kill a lot of monsters, they must find the game easy or not as scary.
        {
            //3 solutions: Event that Spawns a CHIMERA. Increase frequency of Librarian Chases. OR spawn more monsters and darken environment. For now, we do the latter.
            _darkenEvent.ActivateEvent();
            _smallCrawlerSpawn.ActivateEvent();
            monsterSpawnRate = 30f;
            calledSpawnEvent = true;
            DataManager.bIsHostile = true;
        } else
        {
            monsterSpawnRate = 140f;
            DataManager.bIsHostile = false;
        }
        #endregion

        if (totalTimePassed >= 10 && !calledLibrarianSpawnEvent)
        {
            Debug.Log("Creepy time!!");

            SpawnLibrarian();

        }

    }

    private void SpawnLibrarian()
    {
        _darkenEvent.ActivateEvent();
        _librarianSpawn.ActivateEvent();
        _creepyNoise1.ActivateEvent();
        calledLibrarianSpawnEvent = true;
    }


}
