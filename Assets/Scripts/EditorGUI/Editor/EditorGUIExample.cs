using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace EditorTeaching
{
    public class EditorGUIExample : EditorWindow
    {
        #region Variables
        private Vector2 scrollPosition;
        private string textField = "Edit me";
        private string textArea = "Multiline\ntext area";
        private bool toggleValue = true;
        private int intValue = 42;
        private float floatValue = 3.14f;
        private Color colorValue = Color.white;
        private Vector2 vector2Value = Vector2.one;
        private Vector3 vector3Value = Vector3.one;
        private Vector4 vector4Value = Vector4.one;
        private Rect rectValue = new Rect(0, 0, 100, 100);
        private Bounds boundsValue = new Bounds(Vector3.zero, Vector3.one);
        private AnimationCurve curveValue;
        private int selectedToolbar = 0;
        private int selectedPopup = 0;
        private int selectedMask = 0;
        private float sliderValue = 0.5f;
        private float minSliderValue = 0f;
        private float maxSliderValue = 1f;
        private int selectedTab = 0;
        private bool showBasicControls = true;
        private bool showAdvancedControls = true;
        private bool showLayoutControls = true;
        private bool showMiscControls = true;
        private bool isEnabled = true;
        private GUIStyle customStyle;
        #endregion

        [MenuItem("Editor/EditorGUI Example")]
        public static void ShowWindow()
        {
            var window = GetWindow<EditorGUIExample>();
            window.titleContent = new GUIContent("EditorGUI Demo");
            window.Show();
        }

        private void OnEnable()
        {
            curveValue = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
            customStyle = new GUIStyle(EditorStyles.helpBox);
            customStyle.padding = new RectOffset(10, 10, 10, 10);
            customStyle.margin = new RectOffset(5, 5, 5, 5);
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawTitle("EditorGUI Examples");
            EditorGUILayout.Space(10);

            #region Basic Controls Section
            showBasicControls = EditorGUILayout.BeginFoldoutHeaderGroup(showBasicControls, "Basic Controls");
            if (showBasicControls)
            {
                EditorGUILayout.BeginVertical(customStyle);
                {
                    EditorGUILayout.LabelField("Text Controls:", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;

                    // Text controls
                    textField = EditorGUI.TextField(GetControlRect(), "Text Field:", textField);
                    textArea = EditorGUI.TextArea(GetControlRect(60), textArea);
                    EditorGUI.LabelField(GetControlRect(), "Label Field", "Value");
                    EditorGUI.SelectableLabel(GetControlRect(), "Selectable Label (Copy me!)");

                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Numeric Controls:", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;

                    // Numeric controls
                    intValue = EditorGUI.IntField(GetControlRect(), "Int Field:", intValue);
                    floatValue = EditorGUI.FloatField(GetControlRect(), "Float Field:", floatValue);
                    sliderValue = EditorGUI.Slider(GetControlRect(), "Slider:", sliderValue, 0f, 1f);
                    EditorGUI.MinMaxSlider(GetControlRect(), "Min/Max Slider:", ref minSliderValue, ref maxSliderValue, 0f, 1f);

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            EditorGUILayout.Space(10);

            #region Advanced Controls Section
            showAdvancedControls = EditorGUILayout.BeginFoldoutHeaderGroup(showAdvancedControls, "Advanced Controls");
            if (showAdvancedControls)
            {
                EditorGUILayout.BeginVertical(customStyle);
                {
                    EditorGUILayout.LabelField("Vector Controls:", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;

                    // Vector controls
                    vector2Value = EditorGUI.Vector2Field(GetControlRect(), "Vector2:", vector2Value);
                    vector3Value = EditorGUI.Vector3Field(GetControlRect(), "Vector3:", vector3Value);
                    vector4Value = EditorGUI.Vector4Field(GetControlRect(), "Vector4:", vector4Value);

                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Complex Controls:", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;

                    // Complex controls
                    rectValue = EditorGUI.RectField(GetControlRect(), "Rect:", rectValue);
                    boundsValue = EditorGUI.BoundsField(GetControlRect(), "Bounds:", boundsValue);
                    colorValue = EditorGUI.ColorField(GetControlRect(), "Color:", colorValue);
                    curveValue = EditorGUI.CurveField(GetControlRect(), "Curve:", curveValue);

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            EditorGUILayout.Space(10);

            #region Layout Controls Section
            showLayoutControls = EditorGUILayout.BeginFoldoutHeaderGroup(showLayoutControls, "Layout Controls");
            if (showLayoutControls)
            {
                EditorGUILayout.BeginVertical(customStyle);
                {
                    EditorGUILayout.LabelField("Layout Examples:", EditorStyles.boldLabel);

                    // Toolbar
                    string[] toolbarOptions = { "Option 1", "Option 2", "Option 3" };
                    selectedToolbar = GUI.Toolbar(GetControlRect(25), selectedToolbar, toolbarOptions);

                    EditorGUILayout.Space();

                    // Popup and Mask
                    string[] popupOptions = { "First", "Second", "Third" };
                    selectedPopup = EditorGUI.Popup(GetControlRect(), "Popup:", selectedPopup, popupOptions);
                    selectedMask = EditorGUI.MaskField(GetControlRect(), "Mask:", selectedMask, popupOptions);

                    EditorGUILayout.Space();

                    // Tabs using toggle group
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Toggle(selectedTab == 0, "Tab 1", EditorStyles.toolbarButton)) selectedTab = 0;
                        if (GUILayout.Toggle(selectedTab == 1, "Tab 2", EditorStyles.toolbarButton)) selectedTab = 1;
                        if (GUILayout.Toggle(selectedTab == 2, "Tab 3", EditorStyles.toolbarButton)) selectedTab = 2;
                    }
                    EditorGUILayout.EndHorizontal();

                    // Tab content
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        switch (selectedTab)
                        {
                            case 0:
                                EditorGUILayout.LabelField("Content of Tab 1");
                                break;
                            case 1:
                                EditorGUILayout.LabelField("Content of Tab 2");
                                break;
                            case 2:
                                EditorGUILayout.LabelField("Content of Tab 3");
                                break;
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            EditorGUILayout.Space(10);

            #region Miscellaneous Controls Section
            showMiscControls = EditorGUILayout.BeginFoldoutHeaderGroup(showMiscControls, "Miscellaneous Controls");
            if (showMiscControls)
            {
                EditorGUILayout.BeginVertical(customStyle);
                {
                    EditorGUILayout.LabelField("Miscellaneous Examples:", EditorStyles.boldLabel);

                    // Enable/Disable group
                    isEnabled = EditorGUI.ToggleLeft(GetControlRect(), "Enable Controls", isEnabled);
                    EditorGUI.BeginDisabledGroup(!isEnabled);
                    {
                        EditorGUI.TextField(GetControlRect(), "Disabled when unchecked:", "Try to edit me");
                        if (GUI.Button(GetControlRect(25), "Disabled Button"))
                        {
                            Debug.Log("Button clicked!");
                        }
                    }
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.Space();

                    // Foldout with indentation
                    EditorGUI.indentLevel++;
                    toggleValue = EditorGUI.Foldout(GetControlRect(), toggleValue, "Foldout Example");
                    if (toggleValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUI.LabelField(GetControlRect(), "Nested Content 1");
                        EditorGUI.LabelField(GetControlRect(), "Nested Content 2");
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;

                    EditorGUILayout.Space();

                    // Help box
                    EditorGUI.HelpBox(GetControlRect(50),
                        "This is a help box with useful information for the user.",
                        UnityEditor.MessageType.Info);
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

        private Rect GetControlRect(float height = 20)
        {
            return EditorGUILayout.GetControlRect(false, height);
        }
    }
}