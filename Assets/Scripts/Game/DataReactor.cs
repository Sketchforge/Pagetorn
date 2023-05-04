using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReactor : MonoBehaviour
{
    public float numDistanceWalked = DataManager.NumberDistanceWalked;
    public float totalTimePassed = DataManager.totalTime;
    public float averageMonstersKilledPerHour = 15;
    [SerializeField] static public float monsterSpawnRate = 90f;

    [SerializeField] LibrarianBehavior _librarianRef;
    [SerializeField] AIManager _aiManager;

    [SerializeField] Survival _playerStats;

    [Header("Max Fields")]
    [SerializeField] float MAX_DISTANCE;
    [SerializeField] float MAX_TIME;
    [SerializeField] float MAX_TIME_STOOD_STILL;
    [SerializeField] float MAX_MELEEATTACKS;
    [SerializeField] float MAX_ROOMS_FOUND;
    [SerializeField] float MAX_TIME_TIL_LIBRARIAN;
    [SerializeField] float MAX_FOCUS_LIBRARIAN_AGGRO = 30;

    [Header("Events")]
    [SerializeField] PostProcessingEvent _eventFog;
    [SerializeField] PostProcessingEvent _eventDarken;
    [SerializeField] PostProcessingEvent _eventHarshDarken;
    [SerializeField] PostProcessingEvent _eventHurtVignette;
    [SerializeField] InstantiateEvent _spawnSmallCrawler;
    [SerializeField] InstantiateEvent _spawnSmallRanger;
    [SerializeField] InstantiateEvent _spawnLibrarian;

    [Header("Music")]
    [SerializeField] SoundEvent _musicCalm;
    [SerializeField] SoundEvent _musicChase;

    [Header("SFX")]
    [SerializeField] SoundEvent _noiseHeartbeat;
    [SerializeField] SfxReference _noiseClose1;
    [SerializeField] SfxReference _noiseLowBoom;
    [SerializeField] SfxReference _noiseWhispers1;
    [SerializeField] SfxReference _noiseWhispers2;
    [SerializeField] SfxReference _noiseWhispers3;

    private float spellCountTimer = 60f;
    private float monsterKillCountTimer = 3600f; // one hour
    private float _resetTime = 10f;
    bool _heartbeatIsPlaying = false;
    bool calledSpawnEvent = false;
    bool calledLibrarianSpawnEvent = false;
    bool calledScare1 = false;
    bool calledScare2 = false;
    bool calledWhispers1 = false;
    int amountTimesScare1 = 0;

    private void Awake()
    {
        _musicCalm.ActivateEvent();

        if (!_playerStats) _playerStats = FindObjectOfType<Survival>();

        //SessionLicense sessionLicense = new SessionLicense("a34e698a-e959-4f23-9c6d-f9e92f23aa16", "gcpkIKqvJJJPpfZal3vdPPIucbx5OVlpgYzzM1ZG4XSPEEZ2U26mSHRKJkKv29xs", LicensingModel.Rev_Share, Application.isEditor);
        //
        //m_isConnected = false;
        //Debug.Log("Connecting to service");
        //m_connectTask = Task.Run( () =>
        //{
        //    m_gliaClient = new Glia("UnityClient", sessionLicense);
        //    m_gliaValCache = new GliaValueCache(m_gliaClient.Connection);
        //    Debug.Log("Connected To Omnicept Runtime");
        //}
        //);

    }

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
            _spawnSmallCrawler.ActivateEvent();
            _spawnSmallCrawler.ActivateEvent();
        }
        if (DataManager.NumberMonstersKilledLastHour > 30f)
        {
            _spawnSmallRanger.ActivateEvent();
        }
        if (_resetTime <= 0f)
        {
            DataManager.NumberMonstersKilledLastHour = 0;
            DataManager.bKillsLotsOfMonsters = false;
            _resetTime = 10f;
        }

        //STANDS STILL A LOT//
        if (DataManager.AmountTimeStoodStill >= MAX_TIME_STOOD_STILL)
        {
            DataManager.bStandsStillALot = true;
        }

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
        
        //EXPLORES A LOT OF ROOMS//
        if (DataManager.NumberRoomsFound > MAX_ROOMS_FOUND)
        {
            DataManager.bExploresLotsOfRooms = true;
        }

        #endregion

        #region Behavior Reactor

        #region Distance Walked Reaction
        if (DataManager.NumberDistanceWalked >= Random.Range(MAX_DISTANCE - 50f, MAX_DISTANCE + 50f))
        {
          //  Debug.Log("Walked " + MAX_DISTANCE + " meters");
            _eventFog.ActivateEvent(ResetWalkDistance);
            DataManager.TimesWalkedLargeDistances++;
        }
      
        #endregion

        if (DataManager.NumberMeleeAttacksDone >= MAX_MELEEATTACKS)
        {
            DataManager.bUsesMeleeALot = true;
            _eventDarken.ActivateEvent(ResetMeleeAttackNumber);
            _aiManager.makeAgentsCircleTarget();

        }

        if (DataManager.bWalksLargeDistances && DataManager.bKillsLotsOfMonsters && !calledSpawnEvent) //if the player walks a lot and manages to kill a lot of monsters, they must find the game easy or not as scary.
        {
            //3 solutions: Event that Spawns a CHIMERA. Increase frequency of Librarian Chases. OR spawn more monsters and darken environment. For now, we do the latter.
            _eventDarken.ActivateEvent();
            _spawnSmallCrawler.ActivateEvent();
            monsterSpawnRate = 10f;
            calledSpawnEvent = true;
            DataManager.bIsHostile = true; //TODO: Turn off hostility after certain amount of time
        } //else
          //{
          //    monsterSpawnRate = 140f;
          //    DataManager.bIsHostile = false;
          //}

        if (totalTimePassed >= MAX_TIME_TIL_LIBRARIAN && !calledLibrarianSpawnEvent)
        {
            Debug.Log("Spawned Librarian");

            SpawnLibrarian();

        }

        //SCARE 1//
        if ((DataManager.bStandsStillALot || DataManager.bKillsLotsOfMonsters) && !calledScare1)
        {
            _eventHarshDarken.ActivateEvent();
            StartCoroutine(CloseScare());
            calledScare1 = true;

            //Reset Values
            float max_time_addon = MAX_TIME_STOOD_STILL * amountTimesScare1;
            amountTimesScare1++;
            MAX_TIME_STOOD_STILL += max_time_addon;
            DataManager.bStandsStillALot = false;
        }

        //SCARE 2//
        if ((DataManager.bExploresLotsOfRooms || totalTimePassed >= MAX_TIME_TIL_LIBRARIAN/4 || totalTimePassed >= MAX_TIME_TIL_LIBRARIAN - 3) && !calledScare2)
        {
            _eventFog.ActivateEvent(ResetScare2);
            _noiseLowBoom.Play();
            calledScare2 = true;
        }

        //SCARE 3//
        if (((DataManager.currentRoom.HalfRoomSize.x * 2 >= 30f && DataManager.currentRoom.HalfRoomSize.y * 2 >= 20f) && !calledWhispers1) && DataManager.totalTime > 1000)
        {
            _noiseWhispers1.Play();
            calledWhispers1 = true;
        }

        if (DataManager.focusTime > MAX_FOCUS_LIBRARIAN_AGGRO)
        {
            _noiseHeartbeat.ActivateEvent();
            _heartbeatIsPlaying = true;
            _librarianRef.Teleport();
            _eventHurtVignette.ActivateEvent(ResetHeartbeat);
            _spawnSmallCrawler.ActivateEvent();
            DataManager.focusTime = 0;
        }



        #endregion

        #region Stat Responses

        if (_playerStats.IsStatLow(SurvivalStatEnum.Health))
        {
            Debug.Log("Health Low!");
            if (!_heartbeatIsPlaying)
            {
                _noiseHeartbeat.ActivateEvent();
                _eventHurtVignette.ActivateEvent(ResetHeartbeat);
                _heartbeatIsPlaying = true;
            }
        }




        #endregion


    }

    private void SpawnLibrarian()
    {
        _eventDarken.ActivateEvent();
        _spawnLibrarian.ActivateEvent();
        _librarianRef = FindObjectOfType<LibrarianBehavior>();
        _noiseHeartbeat.ActivateEvent();
        calledLibrarianSpawnEvent = true;
    }

    private void ResetWalkDistance()
    {
        DataManager.NumberDistanceWalked = 0;
    }

    private void ResetMeleeAttackNumber()
    {
        DataManager.NumberMeleeAttacksDone = 0;
    }

    private void ResetScare2()
    {
        calledScare2 = false;
        DataManager.bExploresLotsOfRooms = false;
    }

    private void ResetHeartbeat()
    {
        _heartbeatIsPlaying = false;
    }

    private IEnumerator CloseScare()
    {
        //Debug.Log("monster sounds nearby");

        //_eventDarken.ActivateEvent();

        yield return new WaitForSeconds(2f);

        PlayerMovementScript _player = FindObjectOfType<PlayerMovementScript>();
        if (_player)
            _noiseClose1.PlayAtParentAndFollow(_player.transform);

        yield return new WaitForSeconds(2f);

        DataManager.AmountTimeStoodStill = 0;
    }


}
