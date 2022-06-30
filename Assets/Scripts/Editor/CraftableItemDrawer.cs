using UnityEditor;
using UnityEngine;

namespace Scripts.Structs.Editor
{
    [CustomPropertyDrawer(typeof(ItemAmount))]
    public class RecipeCostDrawer : PropertyDrawer
    {
        private const float Padding = 5;
        private const float AmountSize = 0.2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typeProperty = property.FindPropertyRelative("Item");
            var amountProperty = property.FindPropertyRelative("Amount");

            EditorGUI.BeginProperty(position, label, property);

            float amountWidth = (position.width - Padding) * AmountSize;
            float amountHeight = EditorGUI.GetPropertyHeight(amountProperty);
            Rect amountRect = new Rect(position.x, position.y, amountWidth, amountHeight);
            EditorGUI.PropertyField(amountRect, amountProperty, GUIContent.none);

            float typeStart = position.x + amountWidth + Padding;
            float typeWidth = (position.width - Padding) * (1 - AmountSize);
            float typeHeight = EditorGUI.GetPropertyHeight(typeProperty);
            Rect typeRect = new Rect(typeStart, position.y, typeWidth, typeHeight);
            EditorGUI.PropertyField(typeRect, typeProperty, GUIContent.none);
            
            EditorGUI.EndProperty();
        }
    }
}