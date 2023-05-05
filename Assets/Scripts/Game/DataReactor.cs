using System.Collections;
using System.Collections.Generic;
using CoffeyUtils;
using UnityEngine;

public class DataReactor : MonoBehaviour
{
    public float numDistanceWalked;
    public float totalTimePassed;
    public float averageMonstersKilledPerHour = 15;
    [SerializeField, ReadOnly] int _monsterWatchingCount;
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
    [SerializeField] SfxReference _noiseExhaustion;
    [SerializeField] SfxReference _noiseClose1;
    [SerializeField] SfxReference _noiseLowBoom;
    [SerializeField] SfxReference _noiseWhispers1;
    [SerializeField] SfxReference _noiseWhispers2;
    [SerializeField] SfxReference _noiseWhispers3;

    private float spellCountTimer = 60f;
    private float monsterKillCountTimer = 3600f; // one hour
    private float _resetTime = 10f;
    bool walkCounted = false;
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
        numDistanceWalked = GameManager.Data.NumberDistanceWalked;
        totalTimePassed = GameManager.Data.TotalTime;
        _monsterWatchingCount = GameManager.Data.MonstersWatchingPlayer.Count;
        //
        

        #region Behavior Nominator
        
        //GameManager.Data.bCollectsLotsofBooks;
        //GameManager.Data.bExploresLotsOfRooms;

        //WALKS LARGE DISTANCES//
        if (GameManager.Data.TimesWalkedLargeDistances >= 5)
        {
            GameManager.Data.BWalksLargeDistances = true;
        }

        //KILLS LOTS OF MONSTERS//
        if (!GameManager.Data.BKillsLotsOfMonsters)
            monsterKillCountTimer -= Time.deltaTime;
        else if (GameManager.Data.BKillsLotsOfMonsters)
            _resetTime -= Time.deltaTime;
        if (monsterKillCountTimer <= 0f) //if greater than a minute
        {
            GameManager.Data.NumberMonstersKilledLastHour = 0; //resets to 0;
            monsterKillCountTimer = 3600f;
        }
        if (GameManager.Data.NumberMonstersKilledLastHour >= averageMonstersKilledPerHour)
            GameManager.Data.BKillsLotsOfMonsters = true;
        if (GameManager.Data.NumberMonstersKilledLastHour > 15f)
        {
            _spawnSmallCrawler.ActivateEvent();
            _spawnSmallCrawler.ActivateEvent();
        }
        if (GameManager.Data.NumberMonstersKilledLastHour > 30f)
        {
            _spawnSmallRanger.ActivateEvent();
        }
        if (_resetTime <= 0f)
        {
            GameManager.Data.NumberMonstersKilledLastHour = 0;
            GameManager.Data.BKillsLotsOfMonsters = false;
            _resetTime = 10f;
        }

        //STANDS STILL A LOT//
        if (GameManager.Data.AmountTimeStoodStill >= MAX_TIME_STOOD_STILL)
        {
            GameManager.Data.BStandsStillALot = true;
        }

        //USES SPELLS OFTEN//
        if (!GameManager.Data.BUsesSpellsOften)
            spellCountTimer -= Time.deltaTime;
        //Debug.Log(Time.deltaTime - spellCountTimer);
        if (spellCountTimer <= 0f) //if greater than a minute
        {
            GameManager.Data.NumberSpellsDoneLastMinute = 0; //resets to 0;
            spellCountTimer = 60f;
        }
        if (GameManager.Data.NumberSpellsDoneLastMinute >= 10)
        {
            GameManager.Data.BUsesSpellsOften = true;
        }
        
        //EXPLORES A LOT OF ROOMS//
        if (GameManager.Data.NumberRoomsFound > MAX_ROOMS_FOUND)
        {
            GameManager.Data.BExploresLotsOfRooms = true;
        }

        #endregion

        #region Behavior Reactor

        #region Distance Walked Reaction
        if (GameManager.Data.NumberDistanceWalked >= Random.Range(MAX_DISTANCE - 50f, MAX_DISTANCE + 50f) && !walkCounted)
        {
          //  Debug.Log("Walked " + MAX_DISTANCE + " meters");
            _eventFog.ActivateEvent(ResetWalkDistance);
            GameManager.Data.TimesWalkedLargeDistances++;
            walkCounted = true;
        }
      
        #endregion

        if (GameManager.Data.NumberMeleeAttacksDone >= MAX_MELEEATTACKS)
        {
            GameManager.Data.BUsesMeleeALot = true;
            _eventDarken.ActivateEvent(ResetMeleeAttackNumber);
            _aiManager.makeAgentsCircleTarget();

        }

        if (GameManager.Data.BWalksLargeDistances && GameManager.Data.BKillsLotsOfMonsters && !calledSpawnEvent) //if the player walks a lot and manages to kill a lot of monsters, they must find the game easy or not as scary.
        {
            //3 solutions: Event that Spawns a CHIMERA. Increase frequency of Librarian Chases. OR spawn more monsters and darken environment. For now, we do the latter.
            _eventDarken.ActivateEvent();
            _spawnSmallCrawler.ActivateEvent();
            monsterSpawnRate = 10f;
            calledSpawnEvent = true;
            GameManager.Data.BIsHostile = true; //TODO: Turn off hostility after certain amount of time
        } //else
          //{
          //    monsterSpawnRate = 140f;
          //    GameManager.Data.bIsHostile = false;
          //}

        if (totalTimePassed >= MAX_TIME_TIL_LIBRARIAN && !calledLibrarianSpawnEvent && !GameManager.Data.CurrentRoom.Hallway)
        {
            Debug.Log("Spawned Librarian");

            SpawnLibrarian();

        }

        //SCARE 1//
        if ((GameManager.Data.BStandsStillALot || GameManager.Data.BKillsLotsOfMonsters) && !calledScare1)
        {
            _eventHarshDarken.ActivateEvent();
            StartCoroutine(CloseScare());
            calledScare1 = true;

            //Reset Values
            float max_time_addon = MAX_TIME_STOOD_STILL * amountTimesScare1;
            amountTimesScare1++;
            MAX_TIME_STOOD_STILL += max_time_addon;
            GameManager.Data.BStandsStillALot = false;
        }

        //SCARE 2//
        if ((GameManager.Data.BExploresLotsOfRooms || totalTimePassed >= MAX_TIME_TIL_LIBRARIAN/3) && !calledScare2)
        {
            _eventFog.ActivateEvent(ResetScare2);
            _noiseLowBoom.Play();
            calledScare2 = true;
        }
        //|| totalTimePassed >= MAX_TIME_TIL_LIBRARIAN - 3

        //SCARE 3//
        if (((GameManager.Data.CurrentRoom.HalfRoomSize.x * 2 >= 30f && GameManager.Data.CurrentRoom.HalfRoomSize.y * 2 >= 20f) && !calledWhispers1) && GameManager.Data.TotalTime > 1000)
        {
            _noiseWhispers1.Play();
            calledWhispers1 = true;
        }

        if (GameManager.Data.FocusTime >= MAX_FOCUS_LIBRARIAN_AGGRO)
        {
            GameManager.Data.FocusTime = 0;
            ForceTeleportLibrarian(false);
        }



        #endregion

        #region Stat Responses

        if (_playerStats.IsStatLow(SurvivalStatEnum.Health))
        {
            _eventHurtVignette.ActivateEvent(ResetHeartbeat);
            if (!_heartbeatIsPlaying)
            {
                _noiseHeartbeat.ActivateEvent(ResetHeartbeat);
                _heartbeatIsPlaying = true;
            }
        }
        if (_playerStats.IsStatLow(SurvivalStatEnum.Hunger))
        {
            _eventHurtVignette.ActivateEvent(ResetHeartbeat);
            PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Health, 0.001f);
            if (!_heartbeatIsPlaying)
            {
                _noiseExhaustion.Play();
                _heartbeatIsPlaying = true;
            }
        }
        if (_playerStats.IsStatLow(SurvivalStatEnum.Hydration))
        {
            _eventHurtVignette.ActivateEvent(ResetHeartbeat);
            _eventDarken.ActivateEvent();
            PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Health, 0.01f);
            if (!_heartbeatIsPlaying)
            {
                _noiseExhaustion.Play();
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

    public void ForceTeleportLibrarian(bool wasForced)
    {
 
        if (!_heartbeatIsPlaying)
        {
        _noiseHeartbeat.ActivateEvent(ResetHeartbeat);
        _heartbeatIsPlaying = true;
        }
        
        _librarianRef.Teleport();
        _eventHurtVignette.ActivateEvent(ResetHeartbeat);
        
        if (wasForced) GameManager.Data.NumberTimesHitLibrarian += 1;
        if (GameManager.Data.NumberTimesHitLibrarian >= 3)
        {
            GameManager.Data.BIsHostile = true;
            _spawnSmallCrawler.ActivateEvent();
            GameManager.Data.NumberTimesHitLibrarian = 0;
        }
       
    }

    private void ResetWalkDistance()
    {
        GameManager.Data.NumberDistanceWalked = 0;
        walkCounted = false;
    }

    private void ResetMeleeAttackNumber()
    {
        GameManager.Data.NumberMeleeAttacksDone = 0;
    }

    private void ResetScare2()
    {
        calledScare2 = false;
        GameManager.Data.BExploresLotsOfRooms = false;
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

        GameManager.Data.AmountTimeStoodStill = 0;
    }


}
