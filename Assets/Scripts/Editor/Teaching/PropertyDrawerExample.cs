using UnityEngine;
using UnityEditor;

namespace EditorTeaching
{
    // Custom attribute for read-only fields
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }

    // Custom attribute for range with step
    public class StepSliderAttribute : PropertyAttribute
    {
        public float step;
        public StepSliderAttribute(float step)
        {
            this.step = step;
        }
    }

    // Example MonoBehaviour that uses these attributes
    public class PropertyDrawerTarget : MonoBehaviour
    {
        [ReadOnly]
        public string identifier = System.Guid.NewGuid().ToString();

        [StepSlider(0.5f)]
        [Range(0, 10)]
        public float stepValue = 0f;

        [Multiline(3)]
        public string description;
    }

    // Property drawer for ReadOnly attribute
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Store the original GUI enabled state
            bool previousGUIState = GUI.enabled;

            // Disable the GUI
            GUI.enabled = false;

            // Draw the property
            EditorGUI.PropertyField(position, property, label);

            // Restore the original GUI state
            GUI.enabled = previousGUIState;
        }
    }

    // Property drawer for StepSlider attribute
    [CustomPropertyDrawer(typeof(StepSliderAttribute))]
    public class StepSliderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Float)
            {
                EditorGUI.LabelField(position, label.text, "Use StepSlider with float.");
                return;
            }

            StepSliderAttribute stepAttr = (StepSliderAttribute)attribute;
            RangeAttribute rangeAttr = (RangeAttribute)fieldInfo.GetCustomAttributes(typeof(RangeAttribute), true)[0];

            // Create a slider control
            EditorGUI.BeginChangeCheck();
            float value = EditorGUI.Slider(position, label, property.floatValue, rangeAttr.min, rangeAttr.max);
            if (EditorGUI.EndChangeCheck())
            {
                // Round to the nearest step
                value = Mathf.Round(value / stepAttr.step) * stepAttr.step;
                property.floatValue = value;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }

    // Example of a custom editor that uses these property drawers
    [CustomEditor(typeof(PropertyDrawerTarget))]
    public class PropertyDrawerTargetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                "This component demonstrates custom property drawers and attributes:\n" +
                "- ReadOnly: Makes a field read-only in the inspector\n" +
                "- StepSlider: Creates a slider with custom step values",
                MessageType.Info);

            DrawDefaultInspector();
        }
    }
}