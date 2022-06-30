using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Pagetorn/Editor/Add Inventory Items")]
public class AddInventoryItems : ScriptableObject
{
    public List<string> InventoryItems;
}

#if UNITY_EDITOR
[CustomEditor(typeof(AddInventoryItems))]
public class AddInventoryItemsEditor : Editor
{
    private AddInventoryItems _collection;
    private const string FilePath = "Assets/Scripts/Data/";
    private const string FileName = "InventoryItems";

    private void OnEnable()
    {
        _collection = (AddInventoryItems)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(15);
        if (GUILayout.Button("Save"))
        {
            EditorMethods.WriteToEnum(FilePath, FileName, _collection.InventoryItems);
        }
    }
}
#endif