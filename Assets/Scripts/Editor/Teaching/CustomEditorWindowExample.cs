using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace EditorTeaching
{
    public class CustomEditorWindowExample : EditorWindow
    {
        private string searchString = "";
        private Vector2 scrollPosition;
        private GameObject selectedObject;
        private Color customColor = Color.white;
        private List<bool> foldouts = new List<bool>();

        [MenuItem("Window/Teaching/Custom Editor Window Example")]
        public static void ShowWindow()
        {
            var window = GetWindow<CustomEditorWindowExample>();
            window.titleContent = new GUIContent("Custom Window");
            window.Show();
        }

        private void OnEnable()
        {
            // Initialize any data when the window is opened
            if (foldouts.Count == 0)
            {
                foldouts.Add(true);  // Scene Objects
                foldouts.Add(false); // Settings
            }
        }

        private void OnGUI()
        {
            DrawToolbar();
            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Scene Objects Section
            foldouts[0] = EditorGUILayout.Foldout(foldouts[0], "Scene Objects");
            if (foldouts[0])
            {
                EditorGUI.indentLevel++;
                DrawSceneObjectsList();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // Settings Section
            foldouts[1] = EditorGUILayout.Foldout(foldouts[1], "Settings");
            if (foldouts[1])
            {
                EditorGUI.indentLevel++;
                DrawSettings();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndScrollView();

            DrawFooter();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
            {
                Repaint();
            }

            searchString = EditorGUILayout.TextField(searchString, EditorStyles.toolbarSearchField);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSceneObjectsList()
        {
            GameObject[] sceneObjects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in sceneObjects)
            {
                if (string.IsNullOrEmpty(searchString) || obj.name.ToLower().Contains(searchString.ToLower()))
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.ObjectField(obj, typeof(GameObject), true);

                    if (GUILayout.Button("Select", GUILayout.Width(60)))
                    {
                        Selection.activeGameObject = obj;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void DrawSettings()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            selectedObject = (GameObject)EditorGUILayout.ObjectField("Selected Object",
                selectedObject, typeof(GameObject), true);

            customColor = EditorGUILayout.ColorField("Custom Color", customColor);

            if (GUILayout.Button("Apply Color"))
            {
                if (selectedObject != null)
                {
                    Renderer renderer = selectedObject.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        Undo.RecordObject(renderer.material, "Change Material Color");
                        renderer.material.color = customColor;
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawFooter()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Status: Ready", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
        }
    }
}