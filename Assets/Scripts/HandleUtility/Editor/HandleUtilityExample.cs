using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace EditorTeaching
{
    [CustomEditor(typeof(Transform))]
    public class HandleUtilityExample : Editor
    {
        #region Variables
        private bool showDistanceTools = true;
        private bool showPickingTools = true;
        private bool showRayTools = true;
        private bool showPointTools = true;
        private Vector3 lastMousePosition;
        private List<Vector3> points = new List<Vector3>();
        private int selectedPointIndex = -1;
        private float handleSize = 0.1f;
        #endregion

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            Transform targetTransform = target as Transform;
            if (targetTransform == null) return;

            Handles.BeginGUI();
            {
                GUILayout.BeginArea(new Rect(10, 10, 200, 300));
                {
                    EditorGUILayout.LabelField("HandleUtility Examples", EditorStyles.boldLabel);
                    EditorGUILayout.Space();

                    showDistanceTools = EditorGUILayout.Foldout(showDistanceTools, "Distance Tools");
                    if (showDistanceTools)
                    {
                        DrawDistanceTools(targetTransform);
                    }

                    showPickingTools = EditorGUILayout.Foldout(showPickingTools, "Picking Tools");
                    if (showPickingTools)
                    {
                        DrawPickingTools(targetTransform);
                    }

                    showRayTools = EditorGUILayout.Foldout(showRayTools, "Ray Tools");
                    if (showRayTools)
                    {
                        DrawRayTools(sceneView);
                    }

                    showPointTools = EditorGUILayout.Foldout(showPointTools, "Point Tools");
                    if (showPointTools)
                    {
                        DrawPointTools(targetTransform);
                    }
                }
                GUILayout.EndArea();
            }
            Handles.EndGUI();

            // Handle scene view input
            HandleSceneViewInput(sceneView);

            // Draw points and handles
            DrawPointsAndHandles(targetTransform);
        }

        private void DrawDistanceTools(Transform transform)
        {
            // Distance to camera
            float distToCamera = HandleUtility.GetHandleSize(transform.position);
            EditorGUILayout.LabelField("Distance to Camera:", distToCamera.ToString("F2"));

            // Distance to last mouse position
            Vector3 mousePosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            float distToMouse = HandleUtility.DistancePointLine(
                transform.position,
                ray.origin,
                ray.origin + ray.direction * 100f
            );
            EditorGUILayout.LabelField("Distance to Mouse Ray:", distToMouse.ToString("F2"));

            // Perpendicular distance
            if (points.Count >= 2)
            {
                float perpDist = HandleUtility.DistancePointToLine(
                    transform.position,
                    points[0],
                    points[1]
                );
                EditorGUILayout.LabelField("Perpendicular Distance:", perpDist.ToString("F2"));
            }
        }

        private void DrawPickingTools(Transform transform)
        {
            EditorGUILayout.LabelField("Nearest Point:", GetNearestPoint(transform).ToString("F2"));

            if (GUILayout.Button("Add Point at Mouse"))
            {
                Vector3 mousePosition = Event.current.mousePosition;
                Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
                float distance = 0f;
                if (new Plane(Vector3.up, transform.position).Raycast(ray, out distance))
                {
                    points.Add(ray.GetPoint(distance));
                }
            }

            if (GUILayout.Button("Clear Points"))
            {
                points.Clear();
                selectedPointIndex = -1;
            }
        }

        private void DrawRayTools(SceneView sceneView)
        {
            Ray currentRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            EditorGUILayout.LabelField("Mouse Ray Origin:", currentRay.origin.ToString("F2"));
            EditorGUILayout.LabelField("Mouse Ray Direction:", currentRay.direction.ToString("F2"));

            // Display camera-related info
            float cameraDistance = HandleUtility.GetHandleSize(sceneView.camera.transform.position);
            EditorGUILayout.LabelField("Camera Handle Size:", cameraDistance.ToString("F2"));

            // Convert a world point to GUI point
            Vector2 guiPoint = HandleUtility.WorldToGUIPoint(sceneView.camera.transform.position);
            EditorGUILayout.LabelField("Camera GUI Position:", guiPoint.ToString("F2"));
        }

        private void DrawPointTools(Transform transform)
        {
            handleSize = EditorGUILayout.Slider("Handle Size", handleSize, 0.01f, 1f);

            if (selectedPointIndex >= 0 && selectedPointIndex < points.Count)
            {
                EditorGUILayout.LabelField("Selected Point:", points[selectedPointIndex].ToString("F2"));
            }
        }

        private void HandleSceneViewInput(SceneView sceneView)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseMove)
            {
                lastMousePosition = e.mousePosition;
                sceneView.Repaint();
            }

            // Handle point selection
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                float nearestDist = float.MaxValue;
                int nearestIndex = -1;

                for (int i = 0; i < points.Count; i++)
                {
                    Vector2 guiPoint = HandleUtility.WorldToGUIPoint(points[i]);
                    float dist = Vector2.Distance(guiPoint, e.mousePosition);
                    if (dist < 10f && dist < nearestDist) // 10 pixels threshold
                    {
                        nearestDist = dist;
                        nearestIndex = i;
                    }
                }

                selectedPointIndex = nearestIndex;
                if (nearestIndex >= 0)
                {
                    e.Use();
                }
            }
        }

        private void DrawPointsAndHandles(Transform transform)
        {
            Handles.color = Color.white;

            // Draw points
            for (int i = 0; i < points.Count; i++)
            {
                Color pointColor = (i == selectedPointIndex) ? Color.yellow : Color.white;
                Handles.color = pointColor;

                // Draw point handle
                float size = HandleUtility.GetHandleSize(points[i]) * handleSize;
                if (Handles.Button(points[i], Quaternion.identity, size, size, Handles.SphereHandleCap))
                {
                    selectedPointIndex = i;
                }

                // Draw point index
                Handles.Label(points[i] + Vector3.up * size, $"Point {i}");
            }

            // Draw lines between points
            if (points.Count >= 2)
            {
                Handles.color = Color.cyan;
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Handles.DrawLine(points[i], points[i + 1]);
                }
            }

            // Draw distance to selected point
            if (selectedPointIndex >= 0 && selectedPointIndex < points.Count)
            {
                Handles.color = Color.yellow;
                Handles.DrawDottedLine(transform.position, points[selectedPointIndex], 5f);
                Vector3 midPoint = (transform.position + points[selectedPointIndex]) * 0.5f;
                float distance = Vector3.Distance(transform.position, points[selectedPointIndex]);
                Handles.Label(midPoint, $"Distance: {distance:F2}");
            }
        }

        private Vector3 GetNearestPoint(Transform transform)
        {
            if (points.Count == 0) return transform.position;

            Vector3 nearestPoint = points[0];
            float nearestDistance = Vector3.Distance(transform.position, nearestPoint);

            for (int i = 1; i < points.Count; i++)
            {
                float distance = Vector3.Distance(transform.position, points[i]);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPoint = points[i];
                }
            }

            return nearestPoint;
        }
    }
}