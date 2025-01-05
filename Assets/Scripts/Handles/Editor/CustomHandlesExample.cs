using UnityEngine;
using UnityEditor;

namespace EditorTeaching
{
    // The component that will be edited in the scene
    public class HandlesGUITarget : MonoBehaviour
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

        // New Controls for Additional Handles Features
        [Header("Additional Handle Controls")]
        public float sliderValue = 0f;
        public Vector3 bezierStart = Vector3.zero;
        public Vector3 bezierEnd = Vector3.forward * 5f;
        public Vector3 bezierStartTangent = Vector3.up * 2f;
        public Vector3 bezierEndTangent = Vector3.up * 2f;
        public float arcRadius = 3f;
        public float arcAngle = 90f;
        public Vector3 customSnapValue = new Vector3(1f, 1f, 1f);
        public Camera customCamera;
        public Light customLight;

        // Drawing Options
        public bool showBasicHandles = true;
        public bool showShapeHandles = true;
        public bool showAdvancedHandles = true;
        public bool showMeasurements = true;
        public bool showBezierHandles = true;
        public bool showLightHandles = true;
        public bool showCameraHandles = true;
        public bool showCustomCapHandles = true;

        // Visual Settings
        public Color bezierColor = Color.cyan;
        public Color arcColor = Color.magenta;
        public float handleSize = 0.1f;
    }

    // The custom editor with scene GUI functionality
    [CustomEditor(typeof(HandlesGUITarget))]
    public class CustomHandlesExample : Editor
    {
        private const float handleSize = 0.5f;
        private GUIStyle labelStyle;
        private bool isRotating = false;
        private Quaternion initialRotation;
        private Vector3 snapSettings = Vector3.one;

        private void OnEnable()
        {
            labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.white;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fontSize = 12;
            labelStyle.fontStyle = FontStyle.Bold;

            // 设置默认的网格吸附值
            snapSettings = EditorSnapSettings.move;
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
                UnityEditor.MessageType.Info);

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
            var target = (HandlesGUITarget)this.target;

            // Draw help text in scene view
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(10, 10, 200, 100));
            EditorGUILayout.HelpBox(
                "Click and drag handles to modify values",
                UnityEditor.MessageType.Info
            );
            GUILayout.EndArea();
            Handles.EndGUI();

            Transform transform = target.transform;

            // 设置手柄大小
            HandleUtility.handleMaterial.SetPass(0);
            HandleUtility.GetHandleSize(target.customPosition);

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

            if (target.showBezierHandles)
            {
                DrawBezierHandles(target);
            }

            if (target.showLightHandles)
            {
                DrawLightHandles(target);
            }

            if (target.showCameraHandles)
            {
                DrawCameraHandles(target);
            }

            if (target.showCustomCapHandles)
            {
                DrawCustomCapHandles(target);
            }

            if (target.showMeasurements)
            {
                DrawMeasurements(target, transform);
            }

            // 处理键盘事件
            HandleKeyboardEvents(target);

            // 确保场景视图更新
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
                SceneView.RepaintAll();
            }
        }

        private void DrawBasicTransformHandles(HandlesGUITarget target)
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

        private void DrawShapeHandles(HandlesGUITarget target, Transform transform)
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

        private void DrawAdvancedHandles(HandlesGUITarget target, Transform transform)
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

        private void DrawMeasurements(HandlesGUITarget target, Transform transform)
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

        private void DrawBezierHandles(HandlesGUITarget target)
        {
            Handles.color = target.bezierColor;

            // 绘制贝塞尔曲线
            Handles.DrawBezier(
                target.customPosition + target.bezierStart,
                target.customPosition + target.bezierEnd,
                target.customPosition + target.bezierStart + target.bezierStartTangent,
                target.customPosition + target.bezierEnd + target.bezierEndTangent,
                target.bezierColor,
                null,
                2f
            );

            // 控制点手柄
            EditorGUI.BeginChangeCheck();
            Vector3 newStart = Handles.PositionHandle(target.customPosition + target.bezierStart, Quaternion.identity) - target.customPosition;
            Vector3 newEnd = Handles.PositionHandle(target.customPosition + target.bezierEnd, Quaternion.identity) - target.customPosition;
            Vector3 newStartTangent = Handles.PositionHandle(target.customPosition + target.bezierStart + target.bezierStartTangent, Quaternion.identity)
                                    - (target.customPosition + target.bezierStart);
            Vector3 newEndTangent = Handles.PositionHandle(target.customPosition + target.bezierEnd + target.bezierEndTangent, Quaternion.identity)
                                  - (target.customPosition + target.bezierEnd);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Modified Bezier Curve");
                target.bezierStart = newStart;
                target.bezierEnd = newEnd;
                target.bezierStartTangent = newStartTangent;
                target.bezierEndTangent = newEndTangent;
            }

            // 绘制控制点连线
            Handles.color = Color.white;
            Handles.DrawDottedLine(
                target.customPosition + target.bezierStart,
                target.customPosition + target.bezierStart + target.bezierStartTangent,
                2f
            );
            Handles.DrawDottedLine(
                target.customPosition + target.bezierEnd,
                target.customPosition + target.bezierEnd + target.bezierEndTangent,
                2f
            );
        }

        private void DrawLightHandles(HandlesGUITarget target)
        {
            if (target.customLight != null)
            {
                // 绘制光源范围
                Handles.color = Color.yellow;
                switch (target.customLight.type)
                {
                    case LightType.Spot:
                        Handles.DrawWireDisc(target.customLight.transform.position, target.customLight.transform.forward,
                            target.customLight.range * Mathf.Tan(target.customLight.spotAngle * Mathf.Deg2Rad * 0.5f));
                        break;
                    case LightType.Point:
                        Handles.DrawWireDisc(target.customLight.transform.position, Vector3.up, target.customLight.range);
                        Handles.DrawWireDisc(target.customLight.transform.position, Vector3.right, target.customLight.range);
                        Handles.DrawWireDisc(target.customLight.transform.position, Vector3.forward, target.customLight.range);
                        break;
                }

                // 光源强度滑块
                Handles.color = Color.white;
                EditorGUI.BeginChangeCheck();
                float newIntensity = Handles.ScaleSlider(
                    target.customLight.intensity,
                    target.customLight.transform.position,
                    target.customLight.transform.forward,
                    Quaternion.identity,
                    3f,
                    0.1f
                );
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target.customLight, "Change Light Intensity");
                    target.customLight.intensity = newIntensity;
                }
            }
        }

        private void DrawCameraHandles(HandlesGUITarget target)
        {
            if (target.customCamera != null)
            {
                // 绘制相机视锥体
                Handles.color = Color.white;
                Matrix4x4 matrix = Matrix4x4.TRS(
                    target.customCamera.transform.position,
                    target.customCamera.transform.rotation,
                    Vector3.one
                );
                Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                Handles.DrawCamera(new Rect(0, 0, 1, 1), target.customCamera);

                // 相机视野角度控制
                EditorGUI.BeginChangeCheck();
                float newFOV = Handles.ScaleSlider(
                    target.customCamera.fieldOfView,
                    target.customCamera.transform.position,
                    target.customCamera.transform.up,
                    Quaternion.identity,
                    3f,
                    0.1f
                );
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target.customCamera, "Change Camera FOV");
                    target.customCamera.fieldOfView = newFOV;
                }
            }
        }

        private void DrawCustomCapHandles(HandlesGUITarget target)
        {
            // 演示不同的手柄样式
            float size = HandleUtility.GetHandleSize(target.customPosition) * target.handleSize;
            Vector3 position = target.customPosition + Vector3.up * 2f;

            // 球形手柄
            Handles.color = Color.white;
            if (Handles.Button(position, Quaternion.identity, size, size, Handles.SphereHandleCap))
            {
                Debug.Log("Sphere Handle Clicked!");
            }

            // 立方体手柄
            position += Vector3.right * 2f;
            if (Handles.Button(position, Quaternion.identity, size, size, Handles.CubeHandleCap))
            {
                Debug.Log("Cube Handle Clicked!");
            }

            // 圆锥手柄
            position += Vector3.right * 2f;
            if (Handles.Button(position, Quaternion.identity, size, size, Handles.ConeHandleCap))
            {
                Debug.Log("Cone Handle Clicked!");
            }

            // 圆柱手柄
            position += Vector3.right * 2f;
            if (Handles.Button(position, Quaternion.identity, size, size, Handles.CylinderHandleCap))
            {
                Debug.Log("Cylinder Handle Clicked!");
            }

            // 绘制自定义手柄标签
            Handles.Label(position + Vector3.up, "Custom Handles", labelStyle);
        }

        private void HandleKeyboardEvents(HandlesGUITarget target)
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                switch (e.keyCode)
                {
                    case KeyCode.G:
                        // 切换网格吸附
                        EditorSnapSettings.move = EditorSnapSettings.move == Vector3.zero ? snapSettings : Vector3.zero;
                        e.Use();
                        break;
                    case KeyCode.R:
                        // 开始/结束旋转模式
                        isRotating = !isRotating;
                        if (isRotating)
                        {
                            initialRotation = target.customRotation;
                        }
                        e.Use();
                        break;
                }
            }
        }
    }

    // Add menu item to create the component
    public class HandlesGUIExampleMenu
    {
        [MenuItem("Editor/Handles GUI Example")]
        static void CreateHandlesGUIExample()
        {
            GameObject go = new GameObject("Handles GUI Example");
            go.AddComponent<HandlesGUITarget>();
            Selection.activeGameObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create Handles GUI Example");
        }
    }
}