using UnityEngine;
using UnityEditor;

namespace EditorTeaching
{
    // The component that will be edited in the scene
    public class SceneGUITarget : MonoBehaviour
    {
        // Basic Transform Controls
        public Vector3 customPosition;
        public Quaternion customRotation = Quaternion.identity;
        public Vector3 customScale = Vector3.one;

        // Shape Controls
        public float radius = 5f;
        public Vector3 offset = Vector3.zero;
        public Color areaColor = Color.yellow;
        public bool showWireSphere = true;

        // Additional Controls
        public float discAngle = 45f;
        public float coneAngle = 30f;
        public float coneRange = 5f;
        public Vector2 rectangleSize = new Vector2(2f, 3f);

        // Drawing Options
        public bool showBasicHandles = true;
        public bool showShapeHandles = true;
        public bool showAdvancedHandles = true;
        public bool showMeasurements = true;
    }

    // The custom editor with scene GUI functionality
    [CustomEditor(typeof(SceneGUITarget))]
    public class SceneGUIExample : Editor
    {
        private const float handleSize = 0.5f;
        private GUIStyle labelStyle;
        private bool isRotating = false;
        private Quaternion initialRotation;

        private void OnEnable()
        {
            labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.white;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fontSize = 12;
            labelStyle.fontStyle = FontStyle.Bold;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                "This component demonstrates various Handle features:\n" +
                "1. Basic Transform Handles\n" +
                "   - Position (Move Tool)\n" +
                "   - Rotation (Rotate Tool)\n" +
                "   - Scale (Scale Tool)\n\n" +
                "2. Shape Handles\n" +
                "   - Sphere/Disc\n" +
                "   - Rectangle\n" +
                "   - Cone\n\n" +
                "3. Advanced Handles\n" +
                "   - Free Move\n" +
                "   - Custom Measurements\n" +
                "   - Direction Indicators",
                MessageType.Info);

            SerializedObject so = new SerializedObject(target);
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Display Options", EditorStyles.boldLabel);
            SerializedProperty showBasicHandles = so.FindProperty("showBasicHandles");
            SerializedProperty showShapeHandles = so.FindProperty("showShapeHandles");
            SerializedProperty showAdvancedHandles = so.FindProperty("showAdvancedHandles");
            SerializedProperty showMeasurements = so.FindProperty("showMeasurements");

            EditorGUILayout.PropertyField(showBasicHandles);
            EditorGUILayout.PropertyField(showShapeHandles);
            EditorGUILayout.PropertyField(showAdvancedHandles);
            EditorGUILayout.PropertyField(showMeasurements);

            EditorGUILayout.Space(10);
            DrawDefaultInspector();

            if (EditorGUI.EndChangeCheck())
            {
                so.ApplyModifiedProperties();
            }
        }

        private void OnSceneGUI()
        {
            SceneGUITarget target = (SceneGUITarget)this.target;
            Transform transform = target.transform;

            if (target.showBasicHandles)
            {
                DrawBasicTransformHandles(target);
            }

            if (target.showShapeHandles)
            {
                DrawShapeHandles(target, transform);
            }

            if (target.showAdvancedHandles)
            {
                DrawAdvancedHandles(target, transform);
            }

            if (target.showMeasurements)
            {
                DrawMeasurements(target, transform);
            }
        }

        private void DrawBasicTransformHandles(SceneGUITarget target)
        {
            // Position Handle
            EditorGUI.BeginChangeCheck();
            Vector3 newPosition = Handles.PositionHandle(target.customPosition, target.customRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change Position");
                target.customPosition = newPosition;
            }

            // Rotation Handle
            EditorGUI.BeginChangeCheck();
            Quaternion newRotation = Handles.RotationHandle(target.customRotation, target.customPosition);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change Rotation");
                target.customRotation = newRotation;
            }

            // Scale Handle
            EditorGUI.BeginChangeCheck();
            Vector3 newScale = Handles.ScaleHandle(target.customScale, target.customPosition, target.customRotation, handleSize * 2);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change Scale");
                target.customScale = newScale;
            }
        }

        private void DrawShapeHandles(SceneGUITarget target, Transform transform)
        {
            // Wire Sphere
            Handles.color = target.areaColor;
            if (target.showWireSphere)
            {
                Handles.DrawWireDisc(transform.position + target.offset, Vector3.up, target.radius);
                Handles.DrawWireArc(transform.position + target.offset, Vector3.right, Vector3.up, target.discAngle, target.radius);
                Handles.DrawWireArc(transform.position + target.offset, Vector3.forward, Vector3.up, target.discAngle, target.radius);
            }

            // Rectangle Handle
            Handles.color = Color.white;
            Vector3[] corners = new Vector3[]
            {
                new Vector3(-target.rectangleSize.x, 0, -target.rectangleSize.y),
                new Vector3(target.rectangleSize.x, 0, -target.rectangleSize.y),
                new Vector3(target.rectangleSize.x, 0, target.rectangleSize.y),
                new Vector3(-target.rectangleSize.x, 0, target.rectangleSize.y)
            };

            for (int i = 0; i < 4; i++)
            {
                corners[i] = target.customPosition + target.customRotation * corners[i];
            }

            Handles.DrawSolidRectangleWithOutline(corners, new Color(1, 1, 1, 0.1f), Color.white);

            // Cone Handle
            Vector3 coneDirection = target.customRotation * Vector3.forward;
            Handles.color = new Color(1, 0.92f, 0.016f, 0.5f);
            Handles.DrawSolidArc(target.customPosition, Vector3.up, Vector3.forward, target.coneAngle, target.coneRange);
            Handles.DrawSolidArc(target.customPosition, Vector3.up, Vector3.forward, -target.coneAngle, target.coneRange);

            // Radius Handle for Cone
            EditorGUI.BeginChangeCheck();
            float newRange = Handles.RadiusHandle(Quaternion.identity, target.customPosition, target.coneRange);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change Cone Range");
                target.coneRange = newRange;
            }
        }

        private void DrawAdvancedHandles(SceneGUITarget target, Transform transform)
        {
            // Free Move Handle
            EditorGUI.BeginChangeCheck();
            Vector3 freePosition = Handles.FreeMoveHandle(
                target.customPosition + target.offset + Vector3.up * 2,
                handleSize,
                Vector3.zero,
                Handles.SphereHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Move Free Handle");
                target.offset = freePosition - target.customPosition - Vector3.up * 2;
            }

            // Button Handle
            if (Handles.Button(
                target.customPosition + Vector3.up * 3,
                Quaternion.LookRotation(Vector3.up),
                handleSize,
                handleSize,
                Handles.ConeHandleCap))
            {
                Debug.Log("Button Handle Clicked!");
            }

            // 2D Slider Handle
            EditorGUI.BeginChangeCheck();
            Vector3 slider2DPosition = Handles.Slider2D(
                target.customPosition + Vector3.up * 4,
                Vector3.up,
                Vector3.right,
                Vector3.forward,
                handleSize,
                Handles.RectangleHandleCap,
                0.1f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Move 2D Slider");
                target.customPosition = slider2DPosition - Vector3.up * 4;
            }
        }

        private void DrawMeasurements(SceneGUITarget target, Transform transform)
        {
            // Draw measurements and labels
            Handles.color = Color.white;

            // Distance measurement
            Vector3 startPoint = target.customPosition;
            Vector3 endPoint = target.customPosition + target.offset;
            Handles.DrawDottedLine(startPoint, endPoint, 2f);
            Vector3 midPoint = (startPoint + endPoint) * 0.5f;
            float distance = Vector3.Distance(startPoint, endPoint);
            Handles.Label(midPoint + Vector3.up * 0.5f, $"Distance: {distance:F2}m", labelStyle);

            // Angle measurement
            Handles.color = new Color(0, 1, 0, 0.5f);
            Vector3 angleStart = target.customRotation * Vector3.forward;
            Handles.DrawWireArc(target.customPosition, Vector3.up, angleStart, target.discAngle, 2f);
            Vector3 angleLabelPos = target.customPosition + Quaternion.Euler(0, target.discAngle * 0.5f, 0) * angleStart * 2.5f;
            Handles.Label(angleLabelPos, $"Angle: {target.discAngle:F1}°", labelStyle);

            // Scale visualization
            Handles.color = new Color(1, 0.5f, 0, 0.8f);
            Vector3 scaleVisualization = Vector3.Scale(Vector3.one, target.customScale);
            Handles.DrawWireCube(target.customPosition, scaleVisualization);
            Handles.Label(target.customPosition + scaleVisualization * 0.5f,
                $"Scale: ({target.customScale.x:F1}, {target.customScale.y:F1}, {target.customScale.z:F1})",
                labelStyle);

            // Draw cardinal directions
            DrawCardinalDirections(target.customPosition + target.offset, target.radius);
        }

        private void DrawCardinalDirections(Vector3 center, float radius)
        {
            Vector3[] directions = new Vector3[]
            {
                Vector3.forward,
                Vector3.right,
                Vector3.back,
                Vector3.left
            };

            string[] labels = new string[] { "N", "E", "S", "W" };

            Handles.color = Color.white;
            for (int i = 0; i < directions.Length; i++)
            {
                Vector3 pos = center + directions[i] * radius;
                Handles.DrawLine(center, pos);
                Handles.Label(pos, labels[i], labelStyle);
            }
        }
    }

    // Add menu item to create the component
    public class SceneGUIExampleMenu
    {
        [MenuItem("Editor/Teaching/Scene GUI Example")]
        static void CreateSceneGUIExample()
        {
            GameObject go = new GameObject("Scene GUI Example");
            go.AddComponent<SceneGUITarget>();
            Selection.activeGameObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create Scene GUI Example");
        }
    }
}