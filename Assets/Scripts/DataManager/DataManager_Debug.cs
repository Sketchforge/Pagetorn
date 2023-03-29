using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager_Debug : MonoBehaviour
{
    [Header("DataManager Current Vals")]
    [SerializeField] bool _showRealValues = true;
    
    
    [Range(0f, 2000f)] public float NumberKnowledgePointsGathered = DataManager.NumberKnowledgePointsGathered;
    [Range(0f, 2000f)] public float NumberDistanceWalked = DataManager.NumberDistanceWalked;
    [Range(0f, 5000f)] public float AmountTimeStoodStill =  DataManager.AmountTimeStoodStill;
    [Range(0, 2000)] public int NumberMeleeAttacksDone = DataManager.NumberMeleeAttacksDone;
    [Range(0, 2000)] public int NumberSpellsDone = DataManager.NumberSpellsDone;
    [Range(0, 2000)] public int NumberItemsCollected = DataManager.NumberItemsCollected;
    [Range(0, 2000)] public int NumberBooksCollected = DataManager.NumberBooksCollected;
    [Range(0, 2000)] public int NumberRoomsFound = DataManager.NumberRoomsFound;
    [Range(0, 2000)] public int NumberMonstersKilled = DataManager.NumberMonstersKilled;
    //brainhealth center data
    [Range(0f, 2000f)] public float focusTime;
    [Range(0f, 2000f)] public float pupilDilation;
    [Range(0f, 1000f)] public float mouthQuiver;
    [SerializeField] public bool eyesClosed;

    public float totalTime;

    //player behavior
    [Range(0, 100)] public int TimesWalkedLargeDistances;//if reaches 5+, then this player walks large distances. That is their behavior.
    public bool bWalksLargeDistances; //set to true if ^^ above.
    public int NumberMonstersKilledLastHour;
    public bool bKillsLotsOfMonsters; //set to true if NumberMonstersKilled is higher than the average (which will be tested. For now, we say 20 per hour?) // Start is called before the first frame update
    public int NumberSpellsDoneLastMinute; // number of spells used in the last minute. Used in the DataReactor to determine spell behavior. void Awake()
    public bool bUsesSpellsOften; //set to true if Number spells done in last minute is higher than a set number, say 10; Set to false if plenty of time  {
    public bool bCollectsLotsofBooks; //set to true if BooksCollected within 30 minutes is higher than 100;     DataManager.NumberKnowledgePointsGathered = NumberKnowledgePointsGathered;
    public bool bExploresLotsOfRooms; //set to true if finds at least 30 different rooms.     DataManager.NumberDistanceWalked = NumberDistanceWalked;
    public bool bIsHostile;


    private void Update()
    {


        //SET TO SHOW INPUT
        if (_showRealValues)
        {
            NumberKnowledgePointsGathered = DataManager.NumberKnowledgePointsGathered;
            NumberDistanceWalked = DataManager.NumberDistanceWalked;
            AmountTimeStoodStill = DataManager.AmountTimeStoodStill;
            NumberMeleeAttacksDone = DataManager.NumberMeleeAttacksDone;
            NumberSpellsDone = DataManager.NumberSpellsDone;
            NumberItemsCollected = DataManager.NumberItemsCollected;
            NumberBooksCollected = DataManager.NumberBooksCollected;
            NumberRoomsFound = DataManager.NumberRoomsFound;
            NumberMonstersKilled = DataManager.NumberMonstersKilled;
            focusTime = DataManager.focusTime;
            pupilDilation = DataManager.pupilDilation;
            mouthQuiver = DataManager.mouthQuiver;
            eyesClosed = DataManager.eyesClosed;

            TimesWalkedLargeDistances = DataManager.TimesWalkedLargeDistances;
            bWalksLargeDistances = DataManager.bWalksLargeDistances;
            NumberMonstersKilledLastHour = DataManager.NumberMonstersKilledLastHour;
            bKillsLotsOfMonsters = DataManager.bKillsLotsOfMonsters;
            NumberSpellsDoneLastMinute = DataManager.NumberSpellsDoneLastMinute;
            bUsesSpellsOften = DataManager.bUsesSpellsOften;
            bCollectsLotsofBooks = DataManager.bCollectsLotsofBooks;
            bExploresLotsOfRooms = DataManager.bExploresLotsOfRooms;
            bIsHostile = DataManager.bIsHostile;
        }
        else
        {
            DataManager.NumberKnowledgePointsGathered = NumberKnowledgePointsGathered;
            DataManager.NumberDistanceWalked = NumberDistanceWalked;
            DataManager.AmountTimeStoodStill = AmountTimeStoodStill;
            DataManager.NumberMeleeAttacksDone = NumberMeleeAttacksDone;
            DataManager.NumberSpellsDone = NumberSpellsDone;
            DataManager.NumberItemsCollected = NumberItemsCollected;
            DataManager.NumberBooksCollected = NumberBooksCollected;
            DataManager.NumberRoomsFound = NumberRoomsFound;
            DataManager.NumberMonstersKilled = NumberMonstersKilled;
            DataManager.focusTime = focusTime;
            DataManager.pupilDilation = pupilDilation;
            DataManager.mouthQuiver = mouthQuiver;
            DataManager.eyesClosed = eyesClosed;

            DataManager.TimesWalkedLargeDistances = TimesWalkedLargeDistances;
            DataManager.bWalksLargeDistances = bWalksLargeDistances;
            DataManager.NumberMonstersKilledLastHour = NumberMonstersKilledLastHour;
            DataManager.bKillsLotsOfMonsters = bKillsLotsOfMonsters;
            DataManager.NumberSpellsDoneLastMinute = NumberSpellsDoneLastMinute;
            DataManager.bUsesSpellsOften = bUsesSpellsOften;
            DataManager.bCollectsLotsofBooks = bCollectsLotsofBooks;
            DataManager.bExploresLotsOfRooms = bExploresLotsOfRooms;
            DataManager.bIsHostile = bIsHostile;
        }
        
    }

    //public override onInspectorGUI()
    //{
    //    DrawDefaultInspector();
    //
    //    if (_showCurrentVals)
    //    {
    //        serializedObject.Update();
    //        EditorGUILayout.PropertyField(serializedObject.FindProperty())
    //        serializedObject.ApplyModifiedProperties();
    //    }
    //}

}
