using UnityEngine;
using UnityEditor;

namespace EditorTeaching
{
    public class EditorGUILayoutExample : EditorWindow
    {
        #region Variables
        private bool showBasicControls = true;
        private bool showAdvancedControls = true;
        private bool showLayoutGroups = true;
        private Vector2 scrollPosition;

        // Basic Controls
        private string textField = "Text Field";
        private string textArea = "Text Area\nMultiple Lines";
        private bool toggleValue = true;
        private float floatField = 1.0f;
        private int intField = 100;
        private Vector2 vector2Field = Vector2.one;
        private Vector3 vector3Field = Vector3.one;
        private Vector4 vector4Field = Vector4.one;
        private Color colorField = Color.white;
        private Object objectField;
        private LayerMask layerMask = 0;

        // Advanced Controls
        private int selectedToolbar = 0;
        private string[] toolbarOptions = new string[] { "Option 1", "Option 2", "Option 3" };
        private int popupIndex = 0;
        private string[] popupOptions = new string[] { "Item 1", "Item 2", "Item 3" };
        private float sliderValue = 0.5f;
        private Vector2 minMaxSliderValue = new Vector2(0.25f, 0.75f);
        private RuntimePlatform platformEnum = RuntimePlatform.WindowsEditor;
        private float rangeValue = 5f;
        private string searchField = "";
        private string delayedTextField = "Delayed";
        private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        private Gradient gradient = new Gradient();

        // Foldout States
        private bool[] foldouts = new bool[5];
        private Vector2 scrollViewPosition;
        #endregion

        [MenuItem("Editor/EditorGUILayout Example")]
        public static void ShowWindow()
        {
            var window = GetWindow<EditorGUILayoutExample>();
            window.titleContent = new GUIContent("Layout Controls");
            window.Show();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawTitle("EditorGUILayout Examples");
            EditorGUILayout.Space(10);

            #region Basic Controls Section
            showBasicControls = EditorGUILayout.BeginFoldoutHeaderGroup(showBasicControls, "Basic Controls");
            if (showBasicControls)
            {
                EditorGUI.indentLevel++;
                DrawBasicControls();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            EditorGUILayout.Space(10);

            #region Advanced Controls Section
            showAdvancedControls = EditorGUILayout.BeginFoldoutHeaderGroup(showAdvancedControls, "Advanced Controls");
            if (showAdvancedControls)
            {
                EditorGUI.indentLevel++;
                DrawAdvancedControls();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            EditorGUILayout.Space(10);

            #region Layout Groups Section
            showLayoutGroups = EditorGUILayout.BeginFoldoutHeaderGroup(showLayoutGroups, "Layout Groups");
            if (showLayoutGroups)
            {
                EditorGUI.indentLevel++;
                DrawLayoutGroups();
                EditorGUI.indentLevel--;
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

        private void DrawBasicControls()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                // Text Controls
                EditorGUILayout.LabelField("Label Field", "This is a label");
                textField = EditorGUILayout.TextField("Text Field", textField);
                textArea = EditorGUILayout.TextArea(textArea, GUILayout.Height(60));
                EditorGUILayout.Space();

                // Numeric Controls
                intField = EditorGUILayout.IntField("Int Field", intField);
                floatField = EditorGUILayout.FloatField("Float Field", floatField);
                EditorGUILayout.Space();

                // Vector Controls
                vector2Field = EditorGUILayout.Vector2Field("Vector2", vector2Field);
                vector3Field = EditorGUILayout.Vector3Field("Vector3", vector3Field);
                vector4Field = EditorGUILayout.Vector4Field("Vector4", vector4Field);
                EditorGUILayout.Space();

                // Toggle and Color
                toggleValue = EditorGUILayout.Toggle("Toggle", toggleValue);
                colorField = EditorGUILayout.ColorField("Color", colorField);
                EditorGUILayout.Space();

                // Object Fields
                objectField = EditorGUILayout.ObjectField("Object", objectField, typeof(Object), true);
                layerMask = EditorGUILayout.LayerField("Layer", layerMask);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawAdvancedControls()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                // Toolbar and Popup
                selectedToolbar = GUILayout.Toolbar(selectedToolbar, toolbarOptions);
                popupIndex = EditorGUILayout.Popup("Popup", popupIndex, popupOptions);
                EditorGUILayout.Space();

                // Sliders
                sliderValue = EditorGUILayout.Slider("Slider", sliderValue, 0f, 1f);
                EditorGUILayout.MinMaxSlider("Min/Max Slider", ref minMaxSliderValue.x, ref minMaxSliderValue.y, 0f, 1f);
                EditorGUILayout.LabelField($"Min: {minMaxSliderValue.x:F2}, Max: {minMaxSliderValue.y:F2}");
                EditorGUILayout.Space();

                // Enum and Range
                platformEnum = (RuntimePlatform)EditorGUILayout.EnumPopup("Platform", platformEnum);
                rangeValue = EditorGUILayout.IntSlider("Range", (int)rangeValue, 0, 10);
                EditorGUILayout.Space();

                // Search and Delayed
                searchField = EditorGUILayout.TextField("Search", searchField, EditorStyles.toolbarSearchField);
                delayedTextField = EditorGUILayout.DelayedTextField("Delayed", delayedTextField);
                EditorGUILayout.Space();

                // Curves
                curve = EditorGUILayout.CurveField("Animation Curve", curve);
                gradient = EditorGUILayout.GradientField("Gradient", gradient);
                EditorGUILayout.Space();

                // Help Box
                EditorGUILayout.HelpBox("This is an info message", UnityEditor.MessageType.Info);
                EditorGUILayout.HelpBox("This is a warning message", UnityEditor.MessageType.Warning);
                EditorGUILayout.HelpBox("This is an error message", UnityEditor.MessageType.Error);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawLayoutGroups()
        {
            // Horizontal Layout Example
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Horizontal Group", GUILayout.Width(100));
                if (GUILayout.Button("Button 1", GUILayout.Width(100))) { }
                if (GUILayout.Button("Button 2", GUILayout.Width(100))) { }
                GUILayout.FlexibleSpace();
                EditorGUILayout.FloatField("Float", 1.0f, GUILayout.Width(100));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Vertical Layout with Scroll View
            EditorGUILayout.LabelField("Scroll View Example:");
            scrollViewPosition = EditorGUILayout.BeginScrollView(scrollViewPosition,
                GUILayout.Height(150));
            {
                for (int i = 0; i < 5; i++)
                {
                    foldouts[i] = EditorGUILayout.Foldout(foldouts[i], $"Foldout {i + 1}");
                    if (foldouts[i])
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.LabelField($"Content {i + 1}");
                        EditorGUILayout.LabelField($"More Content {i + 1}");
                        EditorGUI.indentLevel--;
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            // Toggle Group Example
            EditorGUILayout.BeginToggleGroup("Toggle Group", toggleValue);
            {
                EditorGUILayout.TextField("Dependent Field 1");
                EditorGUILayout.TextField("Dependent Field 2");
            }
            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.Space();

            // Scope Examples
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Horizontal Scope");
                GUILayout.Button("Scope Button 1");
                GUILayout.Button("Scope Button 2");
            }

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Vertical Scope");
                GUILayout.Button("Scope Button 3");
                GUILayout.Button("Scope Button 4");
            }
        }
    }
}