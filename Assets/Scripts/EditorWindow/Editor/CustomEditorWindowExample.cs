using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
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
        private bool isDirty = false;
        private bool isPlayingOrWillChange;
        private GUIStyle customStyle;

        [MenuItem("Editor/Custom Editor Window Example")]
        public static void ShowWindow()
        {
            var window = GetWindow<CustomEditorWindowExample>();
            window.titleContent = new GUIContent("Scene Explorer");
            window.Show();
        }

        [MenuItem("Editor/Custom Editor Window Example _F12")]
        public static void ShowWindowWithHotkey()
        {
            ShowWindow();
        }

        [MenuItem("Editor/Custom Editor Window Example", true)]
        public static bool ValidateShowWindow()
        {
            return !Application.isPlaying;
        }

        private void OnEnable()
        {
            headerTexture = new Texture2D(1, 1);
            headerTexture.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f));
            headerTexture.Apply();

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Undo.undoRedoPerformed += OnUndoRedo;

            InitStyles();
        }

        private void OnDisable()
        {
            if (headerTexture != null)
            {
                DestroyImmediate(headerTexture);
            }

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        private void OnFocus()
        {
            Repaint();
        }

        private void OnLostFocus()
        {
            if (isDirty)
            {
                SaveChanges();
            }
        }

        private void OnHierarchyChange()
        {
            Repaint();
        }

        private void OnProjectChange()
        {
            Repaint();
        }

        private void OnSelectionChange()
        {
            Repaint();
        }

        private void OnInspectorUpdate()
        {
            if (isPlayingOrWillChange)
            {
                Repaint();
            }
        }

        private void InitStyles()
        {
            if (customStyle == null)
            {
                customStyle = new GUIStyle(EditorStyles.helpBox);
                customStyle.normal.textColor = Color.white;
                customStyle.fontSize = 12;
                customStyle.fontStyle = FontStyle.Bold;
                customStyle.alignment = TextAnchor.MiddleLeft;
                customStyle.padding = new RectOffset(10, 10, 5, 5);
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    isPlayingOrWillChange = false;
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    isPlayingOrWillChange = true;
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    isPlayingOrWillChange = true;
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    isPlayingOrWillChange = false;
                    break;
            }
            Repaint();
        }

        private void OnUndoRedo()
        {
            Repaint();
        }

        private void SaveChanges()
        {
            isDirty = false;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        private void OnGUI()
        {
            HandleKeyboardEvents();

            DrawHeader();
            DrawToolbar();
            DrawMainContent();
            DrawFooter();

            HandleDragAndDrop();

            if (GUI.changed)
            {
                isDirty = true;
                Repaint();
            }
        }

        private void HandleKeyboardEvents()
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                switch (e.keyCode)
                {
                    case KeyCode.F5:
                        Repaint();
                        e.Use();
                        break;
                    case KeyCode.Escape:
                        Close();
                        e.Use();
                        break;
                }
            }
        }

        private void HandleDragAndDrop()
        {
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!position.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is GameObject prefab)
                            {
                                Debug.Log($"Dragged prefab: {prefab.name}");
                            }
                        }
                    }
                    evt.Use();
                    break;
            }
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(customStyle);
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

                EditorGUILayout.HelpBox("This window helps you explore and manage scene objects.", UnityEditor.MessageType.Info);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                searchFilter = EditorGUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField);

                showGameObjects = GUILayout.Toggle(showGameObjects, "GameObjects", EditorStyles.toolbarButton);
                showComponents = GUILayout.Toggle(showComponents, "Components", EditorStyles.toolbarButton);

                backgroundColor = EditorGUILayout.ColorField(backgroundColor, GUILayout.Width(50));

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Settings", EditorStyles.toolbarButton))
                {
                    ShowSettingsMenu();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowSettingsMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Auto Refresh"), isPlayingOrWillChange, ToggleAutoRefresh);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Reset Window"), false, ResetWindow);
            menu.ShowAsContext();
        }

        private void ToggleAutoRefresh()
        {
            isPlayingOrWillChange = !isPlayingOrWillChange;
        }

        private void ResetWindow()
        {
            searchFilter = "";
            backgroundColor = Color.gray;
            showGameObjects = true;
            showComponents = true;
            scrollPosition = Vector2.zero;
            isDirty = true;
            Repaint();
        }

        private void DrawMainContent()
        {
            EditorGUILayout.BeginVertical(customStyle);
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
            EditorGUILayout.EndVertical();
        }

        private void DrawGameObjectInfo(GameObject go)
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.ObjectField(go, typeof(GameObject), true);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(go, "Modified GameObject");
                }

                bool wasActive = go.activeSelf;
                bool isActive = EditorGUILayout.Toggle(wasActive, GUILayout.Width(20));
                if (wasActive != isActive)
                {
                    Undo.RecordObject(go, "Toggle GameObject Active State");
                    go.SetActive(isActive);
                }

                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = go;
                    SceneView.FrameLastActiveSceneView();
                }

                if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        ShowGameObjectContextMenu(go);
                        Event.current.Use();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowGameObjectContextMenu(GameObject go)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete"), false, () => {
                Undo.DestroyObjectImmediate(go);
            });
            menu.AddItem(new GUIContent("Duplicate"), false, () => {
                GameObject duplicate = Instantiate(go);
                duplicate.name = go.name + " (Copy)";
                Undo.RegisterCreatedObjectUndo(duplicate, "Duplicate GameObject");
            });
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Add Component/Rigidbody"), false, () => {
                Undo.AddComponent<Rigidbody>(go);
            });
            menu.ShowAsContext();
        }

        private void DrawComponentInfo(Component component)
        {
            if (component != null && component.gameObject != null)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    EditorGUILayout.LabelField($"{component.GetType().Name} ({component.gameObject.name})");

                    if (component is Behaviour behaviour)
                    {
                        bool wasEnabled = behaviour.enabled;
                        bool isEnabled = EditorGUILayout.Toggle(wasEnabled, GUILayout.Width(20));
                        if (wasEnabled != isEnabled)
                        {
                            Undo.RecordObject(behaviour, "Toggle Component Enabled State");
                            behaviour.enabled = isEnabled;
                        }
                    }

                    if (GUILayout.Button("Select", GUILayout.Width(60)))
                    {
                        Selection.activeGameObject = component.gameObject;
                        SceneView.FrameLastActiveSceneView();
                    }

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                    {
                        if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                        {
                            ShowComponentContextMenu(component);
                            Event.current.Use();
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void ShowComponentContextMenu(Component component)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Remove Component"), false, () => {
                Undo.DestroyObjectImmediate(component);
            });

            menu.AddItem(new GUIContent("Copy Component"), false, () => {
                ComponentUtility.CopyComponent(component);
            });
            menu.AddItem(new GUIContent("Paste Component Values"),
                ComponentUtility.PasteComponentValues(component), () => {
                ComponentUtility.PasteComponentValues(component);
            });

            menu.ShowAsContext();
        }

        private void DrawFooter()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                string status = Application.isPlaying ? "Playing" : "Editing";
                EditorGUILayout.LabelField(status, GUILayout.Width(100));

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
                {
                    Repaint();
                }

                if (isDirty)
                {
                    if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                    {
                        SaveChanges();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}