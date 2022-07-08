#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// TODO: Lists do not work. The attribute is applied individually to each element instead of the list as a whole...

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfAttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        bool show = ShouldShow(property);
        if (attribute is ShowIfAttribute showIfAttribute && !show && showIfAttribute.Output == ShowIfOutput.DontDraw) return 0;
        return base.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool show = ShouldShow(property);
        if (show)
        {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        if (attribute is ShowIfAttribute { Output: ShowIfOutput.JustDisable })
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }

    private bool ShouldShow(SerializedProperty property)
    {
        if (attribute is not ShowIfAttribute showIfAttribute) return true;
        var target = property.serializedObject.targetObject;

        FieldInfo conditionField = GetField(target, showIfAttribute.Target);
        if (conditionField != null && conditionField.FieldType == typeof(bool))
        {
            return (bool)conditionField.GetValue(target);
        }
        return true;
    }

    private static FieldInfo GetField(object target, string fieldName)
    {
        return GetAllFields(target, f => f.Name.Equals(fieldName, StringComparison.InvariantCulture)).FirstOrDefault();
    }

    private static IEnumerable<FieldInfo> GetAllFields(object target, Func<FieldInfo, bool> predicate)
    {
        var types = new List<Type>
        {
            target.GetType()
        };

        while (types.Last().BaseType != null)
        {
            types.Add(types.Last().BaseType);
        }

        for (int i = types.Count - 1; i >= 0; i--)
        {
            var bind = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;
            var fieldInfos = types[i].GetFields(bind).Where(predicate);

            foreach (var fieldInfo in fieldInfos)
            {
                yield return fieldInfo;
            }
        }
    }
}
#endif