using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataManager  
{
    //data bank
    static public int NumberKnowledgePointsGathered;
    static public float NumberDistanceWalked;
    static public int AmountTimeStoodStill;
    static public int NumberMeleeAttacksDone;
    static public int NumberSpellsDone;
    static public int NumberItemsCollected;
    static public int NumberBooksCollected;
    static public int NumberRoomsFound;
    static public int NumberMonstersKilled;
    static public float totalTime;

    //player behavior
    static public int TimesWalkedLargeDistances;//if reaches 5+, then this player walks large distances. That is their behavior.
    static public bool bWalksLargeDistances; //set to true if ^^ above.
    static public bool bKillsLotsOfMonsters; //set to true if NumberMonstersKilled is higher than the average (which will be tested. For now, we say 20 per hour?)
}
