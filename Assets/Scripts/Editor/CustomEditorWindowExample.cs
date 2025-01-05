using UnityEngine;
using UnityEditor;
using System.Linq;

namespace EditorTeaching
{
    public class CustomEditorWindowExample : EditorWindow
    {
        private Vector2 scrollPosition;
        private bool showGameObjects = true;
        private bool showComponents = true;
        private string searchFilter = "";
        private Color backgroundColor = Color.gray;
        private Texture2D headerTexture;

        [MenuItem("Window/Custom Editor Window Example")]
        public static void ShowWindow()
        {
            var window = GetWindow<CustomEditorWindowExample>();
            window.titleContent = new GUIContent("Scene Explorer");
            window.Show();
        }

        private void OnEnable()
        {
            headerTexture = new Texture2D(1, 1);
            headerTexture.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f));
            headerTexture.Apply();
        }

        private void OnDisable()
        {
            if (headerTexture != null)
            {
                DestroyImmediate(headerTexture);
            }
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawToolbar();
            DrawMainContent();
            DrawFooter();

            if (GUI.changed)
            {
                Repaint();
            }
        }

        private void DrawHeader()
        {
            GUILayout.Box(headerTexture, GUILayout.ExpandWidth(true), GUILayout.Height(30));
            var headerRect = GUILayoutUtility.GetLastRect();
            EditorGUI.LabelField(headerRect, "Scene Explorer", new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            });
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            searchFilter = EditorGUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField);
            showGameObjects = GUILayout.Toggle(showGameObjects, "GameObjects", EditorStyles.toolbarButton);
            showComponents = GUILayout.Toggle(showComponents, "Components", EditorStyles.toolbarButton);
            backgroundColor = EditorGUILayout.ColorField(backgroundColor, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawMainContent()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            GUI.backgroundColor = backgroundColor;

            if (showGameObjects)
            {
                EditorGUILayout.LabelField("Scene GameObjects", EditorStyles.boldLabel);
                var gameObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                    .Where(go => string.IsNullOrEmpty(searchFilter) ||
                                go.name.ToLower().Contains(searchFilter.ToLower()));

                foreach (var go in gameObjects)
                {
                    DrawGameObjectInfo(go);
                }
            }

            if (showComponents)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Scene Components", EditorStyles.boldLabel);
                var components = Object.FindObjectsByType<Component>(FindObjectsSortMode.None)
                    .Where(c => string.IsNullOrEmpty(searchFilter) ||
                               c.GetType().Name.ToLower().Contains(searchFilter.ToLower()));

                foreach (var component in components)
                {
                    DrawComponentInfo(component);
                }
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndScrollView();
        }

        private void DrawGameObjectInfo(GameObject go)
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.ObjectField(go, typeof(GameObject), true);
            go.SetActive(EditorGUILayout.Toggle(go.activeSelf, GUILayout.Width(20)));

            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                Selection.activeGameObject = go;
                SceneView.FrameLastActiveSceneView();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawComponentInfo(Component component)
        {
            if (component != null && component.gameObject != null)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.LabelField($"{component.GetType().Name} ({component.gameObject.name})");

                if (component is Behaviour behaviour)
                {
                    behaviour.enabled = EditorGUILayout.Toggle(behaviour.enabled, GUILayout.Width(20));
                }

                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = component.gameObject;
                    SceneView.FrameLastActiveSceneView();
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawFooter()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
            {
                Repaint();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}