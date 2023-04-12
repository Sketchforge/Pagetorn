using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataManager  
{
    //data bank
    static public float NumberKnowledgePointsGathered;
    static public float NumberDistanceWalked;
    static public float NumberDistanceRun;
    static public float AmountTimeStoodStill;
    static public int NumberMeleeAttacksDone;
    static public int NumberSpellsDone;
    static public int NumberItemsCollected;
    static public int NumberBooksCollected;
    static public int NumberRoomsFound;
    static public int NumberMonstersKilled;


    
    //brainhealth center data
    static public float focusTime;
    static public float pupilDilation;
    static public float mouthQuiver;
    static public bool eyesClosed;


    static public float totalTime;

    //player behavior
    static public int TimesWalkedLargeDistances;//if reaches 5+, then this player walks large distances. That is their behavior.
    static public bool bWalksLargeDistances; //set to true if ^^ above.
    static public int NumberMonstersKilledLastHour;
    static public bool bKillsLotsOfMonsters; //set to true if NumberMonstersKilled is higher than the average (which will be tested. For now, we say 20 per hour?)
    static public int NumberSpellsDoneLastMinute; // number of spells used in the last minute. Used in the DataReactor to determine spell behavior.
    static public bool bUsesSpellsOften; //set to true if Number spells done in last minute is higher than a set number, say 10; Set to false if plenty of time 
    static public bool bCollectsLotsofBooks; //set to true if BooksCollected within 30 minutes is higher than 100;
    static public bool bExploresLotsOfRooms; //set to true if finds at least 30 different rooms.
    static public bool bIsHostile;

    static public Room currentRoom;
}
