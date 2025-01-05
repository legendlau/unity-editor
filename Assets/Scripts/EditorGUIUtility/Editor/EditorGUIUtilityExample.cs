using UnityEngine;
using UnityEditor;

namespace EditorTeaching
{
    public class EditorGUIUtilityExample : EditorWindow
    {
        #region Variables
        private Vector2 scrollPosition;
        private bool showSystemInfo = true;
        private bool showIconsDemo = true;
        private bool showUtilityDemo = true;
        private bool showStateDemo = true;
        private bool isDragging = false;
        private Rect dragArea;
        private GUIStyle customStyle;
        private Texture2D customIcon;
        private string selectedIconName = "";
        private float currentProgress = 0f;
        private bool isProcessing = false;
        #endregion

        [MenuItem("Editor/EditorGUIUtility Example")]
        public static void ShowWindow()
        {
            var window = GetWindow<EditorGUIUtilityExample>();
            window.titleContent = new GUIContent("GUI Utility Demo", EditorGUIUtility.FindTexture("UnityEditor.SceneView"));
            window.Show();
        }

        private void OnEnable()
        {
            customStyle = new GUIStyle(EditorStyles.helpBox);
            customStyle.padding = new RectOffset(10, 10, 10, 10);
            customStyle.margin = new RectOffset(5, 5, 5, 5);

            // 创建自定义图标
            customIcon = new Texture2D(16, 16);
            Color[] colors = new Color[256];
            for (int i = 0; i < 256; i++)
            {
                colors[i] = new Color(Random.value, Random.value, Random.value, 1f);
            }
            customIcon.SetPixels(colors);
            customIcon.Apply();
        }

        private void OnDisable()
        {
            if (customIcon != null)
            {
                DestroyImmediate(customIcon);
            }
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawTitle("EditorGUIUtility Examples");
            EditorGUILayout.Space(10);

            #region System Information Section
            showSystemInfo = EditorGUILayout.BeginFoldoutHeaderGroup(showSystemInfo, "System Information");
            if (showSystemInfo)
            {
                EditorGUILayout.BeginVertical(customStyle);
                {
                    // 系统信息
                    EditorGUILayout.LabelField("System Info:", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Current Editor Skin:", EditorGUIUtility.isProSkin ? "Pro" : "Personal");
                    EditorGUILayout.LabelField("System Copy Buffer:", EditorGUIUtility.systemCopyBuffer);
                    EditorGUILayout.LabelField("Single Line Height:", EditorGUIUtility.singleLineHeight.ToString());
                    EditorGUILayout.LabelField("Standard Vertical Spacing:", EditorGUIUtility.standardVerticalSpacing.ToString());
                    EditorGUILayout.LabelField("Current View Width:", EditorGUIUtility.currentViewWidth.ToString());
                    EditorGUILayout.LabelField("Label Width:", EditorGUIUtility.labelWidth.ToString());
                    EditorGUILayout.LabelField("Field Width:", EditorGUIUtility.fieldWidth.ToString());
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            EditorGUILayout.Space(10);

            #region Built-in Icons Section
            showIconsDemo = EditorGUILayout.BeginFoldoutHeaderGroup(showIconsDemo, "Built-in Icons Demo");
            if (showIconsDemo)
            {
                EditorGUILayout.BeginVertical(customStyle);
                {
                    EditorGUILayout.LabelField("Common Built-in Icons:", EditorStyles.boldLabel);
                    EditorGUILayout.BeginHorizontal();
                    {
                        string[] commonIcons = new string[]
                        {
                            "console.infoicon",
                            "console.warnicon",
                            "console.erroricon",
                            "Favorite",
                            "Folder Icon",
                            "Assembly Icon",
                            "ScriptableObject Icon",
                            "PreMatQuad",
                            "PreMatCube",
                            "PreMatSphere",
                            "PreMatCylinder",
                            "_Help"
                        };

                        foreach (string iconName in commonIcons)
                        {
                            Texture icon = EditorGUIUtility.FindTexture(iconName);
                            if (icon != null)
                            {
                                if (GUILayout.Button(new GUIContent(icon, iconName), GUILayout.Width(32), GUILayout.Height(32)))
                                {
                                    selectedIconName = iconName;
                                    EditorGUIUtility.systemCopyBuffer = iconName;
                                    ShowNotification(new GUIContent($"Copied: {iconName}"));
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (!string.IsNullOrEmpty(selectedIconName))
                    {
                        EditorGUILayout.LabelField($"Selected Icon: {selectedIconName}");
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            EditorGUILayout.Space(10);

            #region Utility Features Section
            showUtilityDemo = EditorGUILayout.BeginFoldoutHeaderGroup(showUtilityDemo, "Utility Features Demo");
            if (showUtilityDemo)
            {
                EditorGUILayout.BeginVertical(customStyle);
                {
                    // 进度条示例
                    EditorGUILayout.LabelField("Progress Bar Example:", EditorStyles.boldLabel);
                    if (GUILayout.Button("Start Progress"))
                    {
                        isProcessing = true;
                        EditorApplication.update += UpdateProgress;
                    }

                    if (isProcessing)
                    {
                        Rect progressRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                        EditorGUI.ProgressBar(progressRect, currentProgress, $"Processing... {(currentProgress * 100):F0}%");
                    }

                    EditorGUILayout.Space();

                    // 拖拽区域示例
                    EditorGUILayout.LabelField("Drag Area Example:", EditorStyles.boldLabel);
                    dragArea = EditorGUILayout.GetControlRect(false, 100);
                    EditorGUI.DrawRect(dragArea, new Color(0.5f, 0.5f, 0.5f, 0.2f));

                    if (Event.current.type == EventType.MouseDown && dragArea.Contains(Event.current.mousePosition))
                    {
                        isDragging = true;
                        GUI.changed = true;
                    }
                    else if (Event.current.type == EventType.MouseUp)
                    {
                        isDragging = false;
                        GUI.changed = true;
                    }

                    if (isDragging)
                    {
                        EditorGUI.DrawRect(dragArea, new Color(0.5f, 0.8f, 0.5f, 0.3f));
                        EditorGUILayout.LabelField("Dragging...", EditorStyles.boldLabel);
                    }
                    else
                    {
                        EditorGUI.LabelField(dragArea, "Click and drag here", EditorStyles.centeredGreyMiniLabel);
                    }

                    EditorGUILayout.Space();

                    // 自定义图标示例
                    EditorGUILayout.LabelField("Custom Icon Example:", EditorStyles.boldLabel);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(customIcon, GUILayout.Width(32), GUILayout.Height(32));
                        if (GUILayout.Button("Regenerate Icon"))
                        {
                            RegenerateCustomIcon();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            EditorGUILayout.Space(10);

            #region State Management Section
            showStateDemo = EditorGUILayout.BeginFoldoutHeaderGroup(showStateDemo, "State Management Demo");
            if (showStateDemo)
            {
                EditorGUILayout.BeginVertical(customStyle);
                {
                    EditorGUILayout.LabelField("State Management:", EditorStyles.boldLabel);

                    // 控制ID示例
                    int controlID = GUIUtility.GetControlID(FocusType.Passive);
                    EditorGUILayout.LabelField($"Current Control ID: {controlID}");

                    // 热区示例
                    Rect hotArea = EditorGUILayout.GetControlRect(false, 30);
                    EditorGUI.DrawRect(hotArea, new Color(0.8f, 0.8f, 0.8f, 0.2f));

                    if (Event.current.type == EventType.MouseMove && hotArea.Contains(Event.current.mousePosition))
                    {
                        EditorGUIUtility.AddCursorRect(hotArea, MouseCursor.Link);
                        Repaint();
                    }

                    EditorGUI.LabelField(hotArea, "Hover here to see cursor change", EditorStyles.centeredGreyMiniLabel);

                    // 键盘焦点示例
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Keyboard Focus Example:", EditorStyles.boldLabel);
                    if (GUILayout.Button("Set Keyboard Focus"))
                    {
                        GUIUtility.keyboardControl = controlID;
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            EditorGUILayout.EndScrollView();
        }

        private void DrawTitle(string title)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        private void UpdateProgress()
        {
            currentProgress += 0.01f;
            if (currentProgress >= 1f)
            {
                currentProgress = 0f;
                isProcessing = false;
                EditorApplication.update -= UpdateProgress;
                ShowNotification(new GUIContent("Process Complete!"));
            }
            Repaint();
        }

        private void RegenerateCustomIcon()
        {
            Color[] colors = new Color[256];
            for (int i = 0; i < 256; i++)
            {
                colors[i] = new Color(Random.value, Random.value, Random.value, 1f);
            }
            customIcon.SetPixels(colors);
            customIcon.Apply();
        }
    }
}