using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace EditorTeaching
{
    public class EditorUtilityExample : EditorWindow
    {
        #region Variables
        private Vector2 scrollPosition;
        private GameObject selectedPrefab;
        private Material selectedMaterial;
        private Texture2D selectedTexture;
        private string selectedPath;
        private bool showAssetManagement = true;
        private bool showFileOperations = true;
        private bool showProgressDemo = true;
        private bool showDialogDemo = true;
        private float simulatedProgress = 0f;
        private bool isProcessing = false;
        #endregion

        [MenuItem("Editor/EditorUtility Example")]
        public static void ShowWindow()
        {
            var window = GetWindow<EditorUtilityExample>();
            window.titleContent = new GUIContent("EditorUtility Demo");
            window.Show();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawTitle("EditorUtility Examples");
            EditorGUILayout.Space(10);

            #region Asset Management Section
            showAssetManagement = EditorGUILayout.BeginFoldoutHeaderGroup(showAssetManagement, "Asset Management");
            if (showAssetManagement)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.LabelField("Asset Management Examples:", EditorStyles.boldLabel);

                    // Prefab handling
                    selectedPrefab = EditorGUILayout.ObjectField("Select Prefab:", selectedPrefab, typeof(GameObject), false) as GameObject;
                    if (selectedPrefab != null)
                    {
                        EditorGUILayout.LabelField("Prefab Path:", AssetDatabase.GetAssetPath(selectedPrefab));
                        if (GUILayout.Button("Ping Prefab"))
                        {
                            EditorGUIUtility.PingObject(selectedPrefab);
                        }
                    }

                    EditorGUILayout.Space();

                    // Material handling
                    selectedMaterial = EditorGUILayout.ObjectField("Select Material:", selectedMaterial, typeof(Material), false) as Material;
                    if (selectedMaterial != null && GUILayout.Button("Mark Material As Dirty"))
                    {
                        EditorUtility.SetDirty(selectedMaterial);
                    }

                    EditorGUILayout.Space();

                    // Texture handling
                    selectedTexture = EditorGUILayout.ObjectField("Select Texture:", selectedTexture, typeof(Texture2D), false) as Texture2D;
                    if (selectedTexture != null)
                    {
                        if (GUILayout.Button("Copy Texture Path"))
                        {
                            EditorGUIUtility.systemCopyBuffer = AssetDatabase.GetAssetPath(selectedTexture);
                            ShowNotification(new GUIContent("Path copied to clipboard!"));
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            EditorGUILayout.Space(10);

            #region File Operations Section
            showFileOperations = EditorGUILayout.BeginFoldoutHeaderGroup(showFileOperations, "File Operations");
            if (showFileOperations)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.LabelField("File Operation Examples:", EditorStyles.boldLabel);

                    if (GUILayout.Button("Open File Dialog"))
                    {
                        string path = EditorUtility.OpenFilePanel("Select File", Application.dataPath, "");
                        if (!string.IsNullOrEmpty(path))
                        {
                            selectedPath = path;
                            ShowNotification(new GUIContent("File selected!"));
                        }
                    }

                    if (GUILayout.Button("Save File Dialog"))
                    {
                        string path = EditorUtility.SaveFilePanel("Save File", Application.dataPath, "NewFile", "txt");
                        if (!string.IsNullOrEmpty(path))
                        {
                            selectedPath = path;
                            ShowNotification(new GUIContent("Save location selected!"));
                        }
                    }

                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        EditorGUILayout.LabelField("Selected Path:", selectedPath);
                    }

                    if (GUILayout.Button("Open Folder Panel"))
                    {
                        string folder = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");
                        if (!string.IsNullOrEmpty(folder))
                        {
                            selectedPath = folder;
                            ShowNotification(new GUIContent("Folder selected!"));
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            EditorGUILayout.Space(10);

            #region Progress Display Section
            showProgressDemo = EditorGUILayout.BeginFoldoutHeaderGroup(showProgressDemo, "Progress Display");
            if (showProgressDemo)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.LabelField("Progress Display Examples:", EditorStyles.boldLabel);

                    if (!isProcessing && GUILayout.Button("Start Progress Bar"))
                    {
                        isProcessing = true;
                        simulatedProgress = 0f;
                        EditorApplication.update += UpdateProgress;
                    }

                    if (isProcessing)
                    {
                        EditorGUILayout.LabelField($"Progress: {simulatedProgress * 100:F1}%");
                        if (GUILayout.Button("Cancel"))
                        {
                            EditorUtility.ClearProgressBar();
                            isProcessing = false;
                            EditorApplication.update -= UpdateProgress;
                        }
                    }

                    EditorGUILayout.Space();

                    if (GUILayout.Button("Display Progress Bar (Instant)"))
                    {
                        EditorUtility.DisplayProgressBar("Progress Example", "This is a sample message", 0.5f);
                        EditorApplication.delayCall += () => EditorUtility.ClearProgressBar();
                    }

                    if (GUILayout.Button("Display Cancel Progress Bar"))
                    {
                        bool cancelled = EditorUtility.DisplayCancelableProgressBar("Cancelable Progress", "Click Cancel to stop", 0.7f);
                        EditorApplication.delayCall += () => EditorUtility.ClearProgressBar();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            EditorGUILayout.Space(10);

            #region Dialog Section
            showDialogDemo = EditorGUILayout.BeginFoldoutHeaderGroup(showDialogDemo, "Dialog Examples");
            if (showDialogDemo)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.LabelField("Dialog Examples:", EditorStyles.boldLabel);

                    if (GUILayout.Button("Show Dialog"))
                    {
                        EditorUtility.DisplayDialog("Information",
                            "This is a sample dialog message.", "OK");
                    }

                    if (GUILayout.Button("Show Dialog with Cancel"))
                    {
                        bool result = EditorUtility.DisplayDialog("Question",
                            "Would you like to proceed?", "Yes", "No");
                        ShowNotification(new GUIContent(result ? "Accepted!" : "Cancelled!"));
                    }

                    if (GUILayout.Button("Show Dialog with Alt"))
                    {
                        int result = EditorUtility.DisplayDialogComplex("Complex Dialog",
                            "Choose an action:", "Save", "Don't Save", "Cancel");
                        string message = result switch
                        {
                            0 => "Saving...",
                            1 => "Not saving",
                            2 => "Cancelled",
                            _ => "Unknown response"
                        };
                        ShowNotification(new GUIContent(message));
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
            simulatedProgress += 0.01f;
            EditorUtility.DisplayProgressBar("Processing",
                $"Simulating work... {simulatedProgress * 100:F1}%",
                simulatedProgress);

            if (simulatedProgress >= 1f)
            {
                EditorUtility.ClearProgressBar();
                isProcessing = false;
                simulatedProgress = 0f;
                EditorApplication.update -= UpdateProgress;
                ShowNotification(new GUIContent("Process Complete!"));
            }
        }

        private void OnDisable()
        {
            if (isProcessing)
            {
                EditorUtility.ClearProgressBar();
                isProcessing = false;
                EditorApplication.update -= UpdateProgress;
            }
        }
    }
}