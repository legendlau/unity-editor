using UnityEngine;
using UnityEditor;
using EditorTeaching.Inspector;

namespace EditorTeaching.Inspector
{
    /// <summary>
    /// CustomInspectorTarget 的自定义检查器
    /// </summary>
    [CustomEditor(typeof(CustomInspectorTarget))]
    public class CustomInspectorExample : UnityEditor.Editor
    {
        private bool showAdvancedSettings = false;
        private bool showDebugSettings = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Basic Settings
            EditorGUILayout.LabelField("Basic Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            SerializedProperty nameProperty = serializedObject.FindProperty("playerName");
            SerializedProperty healthProperty = serializedObject.FindProperty("health");
            SerializedProperty speedProperty = serializedObject.FindProperty("moveSpeed");

            EditorGUILayout.PropertyField(nameProperty);
            EditorGUILayout.PropertyField(healthProperty);
            EditorGUILayout.PropertyField(speedProperty);

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            // Advanced Settings
            showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Settings", true);
            if (showAdvancedSettings)
            {
                EditorGUI.indentLevel++;

                SerializedProperty invincibleProp = serializedObject.FindProperty("isInvincible");
                EditorGUILayout.PropertyField(invincibleProp);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // Debug Settings
            showDebugSettings = EditorGUILayout.Foldout(showDebugSettings, "Debug Settings", true);
            if (showDebugSettings)
            {
                EditorGUI.indentLevel++;

                SerializedProperty isInvincibleProperty = serializedObject.FindProperty("isInvincible");
                EditorGUILayout.PropertyField(isInvincibleProperty);

                if (isInvincibleProperty.boolValue)
                {
                    EditorGUILayout.HelpBox("Warning: Invincibility is enabled!", UnityEditor.MessageType.Warning);
                }

                EditorGUI.indentLevel--;
            }

            // Apply changes
            serializedObject.ApplyModifiedProperties();

            // Add buttons at the bottom
            EditorGUILayout.Space();
            if (GUILayout.Button("Reset Values"))
            {
                ResetValues();
            }

            if (GUILayout.Button("Randomize Values"))
            {
                RandomizeValues();
            }
        }

        private void ResetValues()
        {
            var target = serializedObject.targetObject as CustomInspectorTarget;
            if (target != null)
            {
                Undo.RecordObject(target, "Reset Values");

                target.playerName = "Player";
                target.health = 100;
                target.moveSpeed = 5f;

                SerializedProperty invincibleProp = serializedObject.FindProperty("isInvincible");
                invincibleProp.boolValue = false;

                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        private void RandomizeValues()
        {
            var target = serializedObject.targetObject as CustomInspectorTarget;
            if (target != null)
            {
                Undo.RecordObject(target, "Randomize Values");

                target.playerName = "Player" + UnityEngine.Random.Range(1, 1000);
                target.health = UnityEngine.Random.Range(0, 200);
                target.moveSpeed = UnityEngine.Random.Range(0, 10);

                SerializedProperty invincibleProp = serializedObject.FindProperty("isInvincible");
                invincibleProp.boolValue = UnityEngine.Random.Range(0, 2) == 0;

                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }
}