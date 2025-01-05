using UnityEngine;
using UnityEditor;

namespace EditorTeaching
{
    // This is the component we want to customize
    public class CustomInspectorTarget : MonoBehaviour
    {
        public string playerName = "Player";
        public int health = 100;
        public float moveSpeed = 5f;
        [SerializeField] private bool isInvincible;
    }

    // This is the custom inspector
    [CustomEditor(typeof(CustomInspectorTarget))]
    public class CustomInspectorExample : Editor
    {
        private bool showAdvancedSettings = false;

        public override void OnInspectorGUI()
        {
            // Get the target component
            CustomInspectorTarget target = (CustomInspectorTarget)this.target;

            // Add a title
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Player Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // Custom field with validation
            EditorGUI.BeginChangeCheck();
            string newName = EditorGUILayout.TextField("Player Name", target.playerName);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change Player Name");
                target.playerName = newName;
            }

            // Slider for health
            target.health = EditorGUILayout.IntSlider("Health", target.health, 0, 200);

            // Custom field with buttons
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Move Speed");
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                target.moveSpeed = Mathf.Max(0, target.moveSpeed - 1);
            }
            target.moveSpeed = EditorGUILayout.FloatField(target.moveSpeed);
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                target.moveSpeed += 1;
            }
            EditorGUILayout.EndHorizontal();

            // Foldout for advanced settings
            showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Settings");
            if (showAdvancedSettings)
            {
                EditorGUI.indentLevel++;
                SerializedProperty invincibleProp = serializedObject.FindProperty("isInvincible");
                EditorGUILayout.PropertyField(invincibleProp);
                EditorGUI.indentLevel--;
            }

            // Help box
            EditorGUILayout.HelpBox("This is an example of a custom inspector. It shows various UI elements and how to handle them.", MessageType.Info);

            // Apply changes
            serializedObject.ApplyModifiedProperties();
        }
    }
}