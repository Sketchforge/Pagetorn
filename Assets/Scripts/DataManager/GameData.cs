using System.Collections.Generic;
using CoffeyUtils;
using UnityEngine;

public class GameData : ScriptableObject
{
    [field: Header("Data Bank")]
    [field: SerializeField] public float NumberKnowledgePointsGathered { get; set; }
    [field: SerializeField] public float NumberDistanceWalked { get; set; }
    [field: SerializeField] public float NumberDistanceRun { get; set; }
    [field: SerializeField] public float AmountTimeStoodStill { get; set; }
    [field: SerializeField] public int NumberMeleeAttacksDone { get; set; }
    [field: SerializeField] public int NumberSpellsDone { get; set; }
    [field: SerializeField] public int NumberItemsCollected { get; set; }
    [field: SerializeField] public int NumberBooksCollected { get; set; }
    [field: SerializeField] public int NumberRoomsFound { get; set; }
    [field: SerializeField] public int NumberMonstersKilled { get; set; }
    [field: SerializeField] public int NumberTimesHitLibrarian { get; set; }

    [field: Header("Brain Health Center Data")]
    [field: SerializeField] public float FocusTime { get; set; }
    [field: SerializeField] public float PupilDilation { get; set; }
    [field: SerializeField] public float MouthQuiver { get; set; }
    [field: SerializeField] public bool EyesClosed { get; set; }
    
    [field: SerializeField] public float TotalTime { get; set; }

    [field: Header("Player Behavior")]
    [field: SerializeField] public int TimesWalkedLargeDistances { get; set; }//if reaches 5+, then this player walks large distances. That is their behavior.
    [field: SerializeField] public bool BWalksLargeDistances { get; set; } //set to true if ^^ above.
    [field: SerializeField] public int NumberMonstersKilledLastHour { get; set; }
    [field: SerializeField] public bool BKillsLotsOfMonsters { get; set; } //set to true if NumberMonstersKilled is higher than the average (which will be tested. For now, we say 20 per hour?)
    [field: SerializeField] public int NumberSpellsDoneLastMinute { get; set; } // number of spells used in the last minute. Used in the DataReactor to determine spell behavior.
    [field: SerializeField] public bool BUsesSpellsOften { get; set; } //set to true if Number spells done in last minute is higher than a set number, say 10 { get; set; } Set to false if plenty of time 
    [field: SerializeField] public bool BCollectsLotsOfBooks { get; set; } //set to true if BooksCollected within 30 minutes is higher than 100 { get; set; }
    [field: SerializeField] public bool BExploresLotsOfRooms { get; set; } //set to true if finds at least 30 different rooms.
    [field: SerializeField] public bool BIsHostile { get; set; }
    [field: SerializeField] public bool BStandsStillALot { get; set; }
    [field: SerializeField] public bool BUsesMeleeALot { get; set; }

    [field: Header("Misc")]
    [field: SerializeField] public Room CurrentRoom { get; set; }
    [field: SerializeField] public bool ChaseThemePlaying { get; set; }
    [field: SerializeField] public List<EnemyBase> MonstersWatchingPlayer { get; private set; } = new List<EnemyBase>();

    public void AddMonsterSeePlayer(EnemyBase enemy)
    {
        if (!MonstersWatchingPlayer.Contains(enemy)) MonstersWatchingPlayer.Add(enemy);
    }
    
    public void RemoveMonsterSeePlayer(EnemyBase enemy)
    {
        if (MonstersWatchingPlayer.Contains(enemy)) MonstersWatchingPlayer.Remove(enemy);
    }
    
    [Button]
    public void ResetAll()
    {
        NumberKnowledgePointsGathered = 0;
        NumberDistanceWalked = 0;
        NumberDistanceRun = 0;
        AmountTimeStoodStill = 0;
        NumberMeleeAttacksDone = 0;
        NumberSpellsDone = 0;
        NumberItemsCollected = 0;
        NumberBooksCollected = 0;
        NumberRoomsFound = 0;
        NumberMonstersKilled = 0;
        NumberTimesHitLibrarian = 0;
        FocusTime = 0;
        PupilDilation = 0;
        MouthQuiver = 0;
        EyesClosed = false;
        TotalTime = 0;
        TimesWalkedLargeDistances = 0;
        BWalksLargeDistances = false;
        NumberMonstersKilledLastHour = 0;
        BKillsLotsOfMonsters = false;
        NumberSpellsDoneLastMinute = 0;
        BUsesSpellsOften = false;
        BCollectsLotsOfBooks = false;
        BExploresLotsOfRooms = false;
        BIsHostile = false;
        BStandsStillALot = false;
        BUsesMeleeALot = false;
        CurrentRoom = null;
        ChaseThemePlaying = false;
        MonstersWatchingPlayer = new List<EnemyBase>();
    }
}
