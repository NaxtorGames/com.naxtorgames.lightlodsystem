using UnityEngine;
using UnityEditor;

namespace NaxtorGames.LightLODSystem.EditorScripts
{
    [CustomEditor(typeof(LightLODControllerMono))]
    public sealed class LightLODControllerEditor : Editor
    {
        private static GUIContent _updateTimeGUIContent = null;
        private static GUIContent _currentUpdateTimeGUIContent = null;
        private static GUIContent _considerDirectionGUIContent = null;
        private static GUIContent _manyCamerasSelectGUIContent = null;
        private static GUIContent _manyCamerasToggleGUIContent = null;

        private SerializedProperty _scriptProperty = null;
        private SerializedProperty _updateTickProperty = null;
        private SerializedProperty _considerDirectionProperty = null;
        private SerializedProperty _currentUpdateTickProperty = null;

        private bool _hasMoreThanOneLODCamera = false;
        private LightLODControllerMono[] _allLODCameras = null;

        private void OnEnable()
        {
            LightLODControllerEditor.FillGUIContents();
            _scriptProperty = serializedObject.FindProperty("m_Script");
            _updateTickProperty = serializedObject.FindProperty("_updateTick");
            _considerDirectionProperty = serializedObject.FindProperty("_considerDirection");
            _currentUpdateTickProperty = serializedObject.FindProperty("_currentTick");

            if (!Application.isPlaying)
            {
                _allLODCameras = GameObject.FindObjectsOfType<LightLODControllerMono>(true);
                _hasMoreThanOneLODCamera = _allLODCameras.Length > 1;
            }
            else
            {
                _hasMoreThanOneLODCamera = false;
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_scriptProperty);
            EditorGUI.EndDisabledGroup();

            serializedObject.Update();

            EditorGUILayout.PropertyField(_updateTickProperty, _updateTimeGUIContent);
            EditorGUILayout.PropertyField(_considerDirectionProperty, _considerDirectionGUIContent);
            serializedObject.ApplyModifiedProperties();

            ShowLODCameras();

            if (Application.isPlaying)
            {
                EditorGUILayout.Space(6.0f);

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.FloatField(_currentUpdateTimeGUIContent, _currentUpdateTickProperty.floatValue);
                EditorGUI.EndDisabledGroup();
            }
        }

        private void ShowLODCameras()
        {
            if (!_hasMoreThanOneLODCamera)
            {
                return;
            }

            EditorGUILayout.Space(6.0f);

            EditorGUILayout.HelpBox("There is more than one active LOD Camera", MessageType.Warning);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Select all", "Select all LOD Cameras")))
            {
                GameObject[] cameraGameObjects = new GameObject[_allLODCameras.Length];
                for (int i = 0; i < cameraGameObjects.Length; i++)
                {
                    cameraGameObjects[i] = _allLODCameras[i].gameObject;
                }
                Selection.objects = cameraGameObjects;
            }

            if (GUILayout.Button(new GUIContent("Destroy all but this", "Destroy all LOD Cameras except the current selected.")))
            {
                int selection = EditorUtility.DisplayDialogComplex("Destroy LOD Cameras", "What should be destroyed?", "Component", "Cancel", "Game Object");
                if (selection == 0 || selection == 2)
                {
                    GameObject currentSelectedGameObject = Selection.activeGameObject;
                    int selectedIndex = 0;
                    for (int i = 0; i < _allLODCameras.Length; i++)
                    {
                        if (_allLODCameras[i].gameObject == currentSelectedGameObject)
                        {
                            selectedIndex = i;
                            continue;
                        }

                        if (selection == 0)
                        {
                            Undo.DestroyObjectImmediate(_allLODCameras[i]);
                        }
                        else if (selection == 2)
                        {
                            Undo.DestroyObjectImmediate(_allLODCameras[i].gameObject);
                        }
                    }

                    _allLODCameras = new LightLODControllerMono[] { _allLODCameras[selectedIndex] };
                    _hasMoreThanOneLODCamera = false;
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(3.0f);

            ListAllLODCameraOptions();
        }

        private void ListAllLODCameraOptions()
        {
            EditorGUILayout.BeginVertical();

            GameObject currentSelectedGameObject = Selection.activeGameObject;
            for (int i = 0; i < _allLODCameras.Length; i++)
            {
                LightLODControllerMono lightLODCamera = _allLODCameras[i];
                if (lightLODCamera.gameObject == currentSelectedGameObject)
                {
                    continue;
                }

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button(_manyCamerasSelectGUIContent, GUILayout.Width(75.0f)))
                {
                    Selection.activeGameObject = lightLODCamera.gameObject;
                }

                bool currentState = lightLODCamera.enabled;
                lightLODCamera.enabled = EditorGUILayout.Toggle(currentState, GUILayout.Width(15.0f));
                if (currentState != lightLODCamera.enabled)
                {
                    EditorUtility.SetDirty(lightLODCamera);
                }

                GUILayout.Label(_manyCamerasToggleGUIContent, GUILayout.Width(50.0f));
                GUILayout.Label(lightLODCamera.gameObject.name + (lightLODCamera.gameObject.activeInHierarchy ? "" : "  (Game Object disabled)"));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private static void FillGUIContents()
        {
            _updateTimeGUIContent ??= new GUIContent(
                "Update Tick",
                "The frequence the lights where updated."
                );

            _currentUpdateTimeGUIContent ??= new GUIContent(
                "Current Update Tick",
                "The current tick"
                );

            _considerDirectionGUIContent ??= new GUIContent(
                "Consider Direction",
                "If the direction of this controller should be considered to disable lights that are behind this object"
                );

            _manyCamerasSelectGUIContent ??= new GUIContent(
                "Select",
                "Select the Light LOD Cameras game object"
                );

            _manyCamerasToggleGUIContent ??= new GUIContent(
                "Enabled",
                "The component enabled state"
                );
        }
    }
}
