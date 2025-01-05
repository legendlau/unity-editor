using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace EditorTeaching
{
    public class SceneViewExample : EditorWindow
    {
        #region Variables
        private Vector2 scrollPosition;
        private bool showCameraControls = true;
        private bool showViewStates = true;
        private bool showSceneTools = true;
        private bool showCustomDrawing = true;
        private bool isOrthographic = false;
        private float orthographicSize = 5f;
        private Vector3 pivotPoint = Vector3.zero;
        private Vector3 cameraOffset = new Vector3(0, 5, -10);
        private float rotationSpeed = 1f;
        private bool autoRotate = false;
        private Color drawColor = Color.green;
        private List<Vector3> customPoints = new List<Vector3>();
        private GUIStyle customStyle;
        #endregion

        [MenuItem("Editor/SceneView Example")]
        public static void ShowWindow()
        {
            var window = GetWindow<SceneViewExample>();
            window.titleContent = new GUIContent("SceneView Demo");
            window.Show();
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            customStyle = new GUIStyle(EditorStyles.helpBox);
            customStyle.padding = new RectOffset(10, 10, 10, 10);
            customStyle.margin = new RectOffset(5, 5, 5, 5);

            // Initialize with current scene view settings
            if (SceneView.lastActiveSceneView != null)
            {
                isOrthographic = SceneView.lastActiveSceneView.orthographic;
                orthographicSize = SceneView.lastActiveSceneView.size;
                pivotPoint = SceneView.lastActiveSceneView.pivot;
            }
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            if (autoRotate)
            {
                EditorApplication.update -= AutoRotateCamera;
            }
        }

        private void OnGUI()
        {
            if (SceneView.lastActiveSceneView == null)
            {
                EditorGUILayout.HelpBox("No active Scene View found. Please open a Scene View window.", UnityEditor.MessageType.Warning);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawTitle("SceneView Examples");
            EditorGUILayout.Space(10);

            #region Camera Controls Section
            showCameraControls = EditorGUILayout.BeginFoldoutHeaderGroup(showCameraControls, "Camera Controls");
            if (showCameraControls)
            {
                EditorGUILayout.BeginVertical(customStyle);
                {
                    // Orthographic toggle
                    bool newIsOrthographic = EditorGUILayout.Toggle("Orthographic", isOrthographic);
                    if (newIsOrthographic != isOrthographic)
                    {
                        isOrthographic = newIsOrthographic;
                        SceneView.lastActiveSceneView.orthographic = isOrthographic;
                        SceneView.lastActiveSceneView.Repaint();
                    }

                    // Orthographic size
                    if (isOrthographic)
                    {
                        float newSize = EditorGUILayout.Slider("Orthographic Size", orthographicSize, 1f, 20f);
                        if (newSize != orthographicSize)
                        {
                            orthographicSize = newSize;
                            SceneView.lastActiveSceneView.size = orthographicSize;
                            SceneView.lastActiveSceneView.Repaint();
                        }
                    }

                    // Pivot point
                    Vector3 newPivot = EditorGUILayout.Vector3Field("Pivot Point", pivotPoint);
                    if (newPivot != pivotPoint)
                    {
                        pivotPoint = newPivot;
                        SceneView.lastActiveSceneView.pivot = pivotPoint;
                        SceneView.lastActiveSceneView.Repaint();
                    }

                    // Camera offset
                    cameraOffset = EditorGUILayout.Vector3Field("Camera Offset", cameraOffset);

                    // Auto-rotate controls
                    EditorGUILayout.BeginHorizontal();
                    {
                        autoRotate = EditorGUILayout.Toggle("Auto Rotate", autoRotate);
                        if (autoRotate)
                        {
                            rotationSpeed = EditorGUILayout.Slider("Speed", rotationSpeed, 0.1f, 5f);
                            EditorApplication.update -= AutoRotateCamera;
                            EditorApplication.update += AutoRotateCamera;
                        }
                        else
                        {
                            EditorApplication.update -= AutoRotateCamera;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (GUILayout.Button("Frame Selected"))
                    {
                        SceneView.lastActiveSceneView.FrameSelected();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            EditorGUILayout.Space(10);

            #region View States Section
            showViewStates = EditorGUILayout.BeginFoldoutHeaderGroup(showViewStates, "View States");
            if (showViewStates)
            {
                EditorGUILayout.BeginVertical(customStyle);
                {
                    if (GUILayout.Button("Top View"))
                    {
                        SetSceneViewDirection(Vector3.down, Vector3.forward);
                    }
                    if (GUILayout.Button("Side View"))
                    {
                        SetSceneViewDirection(Vector3.right, Vector3.up);
                    }
                    if (GUILayout.Button("Front View"))
                    {
                        SetSceneViewDirection(Vector3.forward, Vector3.up);
                    }
                    if (GUILayout.Button("Perspective View"))
                    {
                        SetSceneViewDirection(new Vector3(-1, -1, -1).normalized, Vector3.up);
                    }

                    EditorGUILayout.Space();

                    if (GUILayout.Button("Save Camera State"))
                    {
                        SaveCameraState();
                    }
                    if (GUILayout.Button("Restore Camera State"))
                    {
                        RestoreCameraState();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            EditorGUILayout.Space(10);

            #region Scene Tools Section
            showSceneTools = EditorGUILayout.BeginFoldoutHeaderGroup(showSceneTools, "Scene Tools");
            if (showSceneTools)
            {
                EditorGUILayout.BeginVertical(customStyle);
                {
                    EditorGUILayout.LabelField("Active Tool:", Tools.current.ToString());

                    if (GUILayout.Button("Toggle 2D Mode"))
                    {
                        SceneView.lastActiveSceneView.in2DMode = !SceneView.lastActiveSceneView.in2DMode;
                    }

                    if (GUILayout.Button("Toggle Gizmos"))
                    {
                        SceneView.lastActiveSceneView.drawGizmos = !SceneView.lastActiveSceneView.drawGizmos;
                    }

                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Scene View States:");
                    EditorGUILayout.Toggle("Is 2D Mode", SceneView.lastActiveSceneView.in2DMode);
                    EditorGUILayout.Toggle("Draw Gizmos", SceneView.lastActiveSceneView.drawGizmos);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            EditorGUILayout.Space(10);

            #region Custom Drawing Section
            showCustomDrawing = EditorGUILayout.BeginFoldoutHeaderGroup(showCustomDrawing, "Custom Drawing");
            if (showCustomDrawing)
            {
                EditorGUILayout.BeginVertical(customStyle);
                {
                    drawColor = EditorGUILayout.ColorField("Draw Color", drawColor);

                    if (GUILayout.Button("Add Point at Mouse"))
                    {
                        AddPointAtMouse();
                    }

                    if (GUILayout.Button("Clear Points"))
                    {
                        customPoints.Clear();
                        SceneView.lastActiveSceneView.Repaint();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Click in the Scene View to add points. Points will be connected with lines.", UnityEditor.MessageType.Info);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            EditorGUILayout.EndScrollView();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            // Draw custom points and lines
            if (customPoints.Count > 0)
            {
                Handles.color = drawColor;

                // Draw points
                for (int i = 0; i < customPoints.Count; i++)
                {
                    Handles.SphereHandleCap(
                        i,
                        customPoints[i],
                        Quaternion.identity,
                        HandleUtility.GetHandleSize(customPoints[i]) * 0.1f,
                        EventType.Repaint
                    );
                }

                // Draw lines between points
                if (customPoints.Count > 1)
                {
                    Handles.DrawPolyLine(customPoints.ToArray());
                }
            }

            // Handle mouse events for point placement
            HandleMouseEvents(sceneView);
        }

        private void HandleMouseEvents(SceneView sceneView)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0 && e.control)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    customPoints.Add(hit.point);
                    sceneView.Repaint();
                }
                else
                {
                    // If no hit, add point at a fixed distance
                    customPoints.Add(ray.origin + ray.direction * 10f);
                    sceneView.Repaint();
                }

                e.Use();
            }
        }

        private void AddPointAtMouse()
        {
            if (SceneView.lastActiveSceneView != null)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    customPoints.Add(hit.point);
                }
                else
                {
                    customPoints.Add(ray.origin + ray.direction * 10f);
                }

                SceneView.lastActiveSceneView.Repaint();
            }
        }

        private void AutoRotateCamera()
        {
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView view = SceneView.lastActiveSceneView;
                view.rotation *= Quaternion.Euler(0, rotationSpeed, 0);
                view.Repaint();
            }
        }

        private void SetSceneViewDirection(Vector3 direction, Vector3 upDirection)
        {
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.LookAt(
                    pivotPoint,
                    Quaternion.LookRotation(direction, upDirection),
                    5f
                );
            }
        }

        private Vector3 savedPosition;
        private Quaternion savedRotation;
        private bool savedOrthographic;
        private float savedSize;

        private void SaveCameraState()
        {
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView view = SceneView.lastActiveSceneView;
                savedPosition = view.pivot;
                savedRotation = view.rotation;
                savedOrthographic = view.orthographic;
                savedSize = view.size;
            }
        }

        private void RestoreCameraState()
        {
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView view = SceneView.lastActiveSceneView;
                view.pivot = savedPosition;
                view.rotation = savedRotation;
                view.orthographic = savedOrthographic;
                view.size = savedSize;
                view.Repaint();
            }
        }

        private void DrawTitle(string title)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
    }
}