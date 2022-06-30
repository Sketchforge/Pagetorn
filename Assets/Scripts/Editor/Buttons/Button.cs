#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace ButtonAttributeEditor
{
    public abstract class Button
    {
        public readonly MethodInfo Method;
        public readonly string DisplayName;
        private readonly bool _disabled;
        private readonly int _spacing;

        // Create a new button
        internal static Button Create(MethodInfo method, ButtonAttribute buttonAttribute) {
            var parameters = method.GetParameters();

            if (parameters.Length == 0) {
                return new ButtonWithoutParams(method, buttonAttribute);
            }
            return new ButtonWithParams(method, buttonAttribute, parameters);
        }

        // Constructor used by inherited classes (this is abstract)
        protected Button(MethodInfo method, ButtonAttribute buttonAttribute) {
            this.Method = method;
            DisplayName = string.IsNullOrEmpty(buttonAttribute.ButtonName) ? ObjectNames.NicifyVariableName(method.Name) : buttonAttribute.ButtonName;

            _spacing = buttonAttribute.Spacing;
            switch (buttonAttribute.Mode) {
                case ButtonMode.Always:
                    _disabled = false;
                    break;
                case ButtonMode.InPlayMode:
                    _disabled = !EditorApplication.isPlaying;
                    break;
                case ButtonMode.NotInPlayMode:
                    _disabled = EditorApplication.isPlaying;
                    break;
                default:
                    _disabled = true;
                    break;
            }
        }

        // Draw the button
        public void Draw(IEnumerable<object> targets) {
            EditorGUI.BeginDisabledGroup(_disabled);
            DrawInternal(targets, _spacing);
            EditorGUI.EndDisabledGroup();
        }

        protected abstract void DrawInternal(IEnumerable<object> targets, int spacing);
    }
}
#endif