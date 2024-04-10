using UnityEngine;
using UnityEditor;

namespace NaxtorGames.LightLODSystem.EditorScripts
{
    [CustomEditor(typeof(LightLODSourceMono))]
    public sealed class LightLODSourceEditor : Editor
    {
        private static GUIContent _lightGUIContent = null;
        private static GUIContent _settingsGUIContent = null;
        private static GUIContent _rangeIsThresholdGUIContent = null;
        private static GUIContent _dotThresholdGUIContent = null;
        private static GUIContent _dotThresholdRangeGUIContent = null;
        private static GUIContent _debug_dotDirectionGUIContent = null;

        private SerializedProperty _scriptProperty = null;
        private SerializedProperty _lightProperty = null;
        private SerializedProperty _settingsProperty = null;
        private SerializedProperty _rangeIsThresholdProperty = null;
        private SerializedProperty _dotThresholdProperty = null;
        private SerializedProperty _dotDirectionProductProperty = null;


        private void OnEnable()
        {
            LightLODSourceEditor.FillGUIContents();
            _scriptProperty = serializedObject.FindProperty("m_Script");
            _lightProperty = serializedObject.FindProperty("_light");
            _settingsProperty = serializedObject.FindProperty("_settings");
            _rangeIsThresholdProperty = serializedObject.FindProperty("_rangeIsThreshold");
            _dotThresholdProperty = serializedObject.FindProperty("_dotThreshold");
            _dotDirectionProductProperty = serializedObject.FindProperty("DEBUG_directionDot");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_scriptProperty);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(3.0f);

            EditorGUILayout.LabelField("References", EditorStyles.boldLabel);

            serializedObject.Update();

            EditorGUILayout.PropertyField(_lightProperty, _lightGUIContent);

            Light lightReference = _lightProperty.objectReferenceValue != null ? _lightProperty.objectReferenceValue as Light : null;
            if (lightReference == null)
            {
                EditorGUILayout.HelpBox("Missing Light reference!", MessageType.Error);
            }
            else
            {
                if (lightReference.type == LightType.Directional)
                {
                    EditorGUILayout.HelpBox("While Directional lights might work, be aware that the transforms position is depending what settings will be applied wich is not how directional lights work.", MessageType.Warning);
                }
                if (lightReference.lightmapBakeType == LightmapBakeType.Baked)
                {
                    EditorGUILayout.HelpBox("Lightmap Bake Type cannot be 'Baked'", MessageType.Error);
                }
                if (lightReference.type == LightType.Area || lightReference.type == LightType.Rectangle || lightReference.type == LightType.Disc)
                {
                    EditorGUILayout.HelpBox("Area Lights are not supported", MessageType.Error);
                }
            }

            EditorGUILayout.PropertyField(_settingsProperty, _settingsGUIContent);

            if (_settingsProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Missing Settings reference!\n\nTo create a Light LOD Settings Scriptable Object go to:\n'Create -> NaxtorGames -> Light LOD System -> Light LOD Settings'", MessageType.Error);
            }

            EditorGUILayout.Space(3.0f);

            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_rangeIsThresholdProperty, _rangeIsThresholdGUIContent);

            if (_rangeIsThresholdProperty.boolValue)
            {
                if (lightReference != null)
                {
                    EditorGUILayout.HelpBox("Dot Threshold will be overriden by the lights range.", MessageType.Info);
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.FloatField(_dotThresholdRangeGUIContent, lightReference.range);
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.FloatField(_dotThresholdRangeGUIContent, 0.0f);
                    EditorGUI.EndDisabledGroup();
                }
            }
            else
            {
                EditorGUILayout.PropertyField(_dotThresholdProperty, _dotThresholdGUIContent);
            }

            if (Application.isPlaying)
            {
                EditorGUILayout.Space(3.0f);
                EditorGUILayout.LabelField("DEBUG (Read only)", EditorStyles.boldLabel);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(_dotDirectionProductProperty, _debug_dotDirectionGUIContent);
                EditorGUI.EndDisabledGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void FillGUIContents()
        {
            _lightGUIContent ??= new GUIContent(
                "Light Source",
                "The controlled light source"
                );

            _settingsGUIContent ??= new GUIContent(
                "LOD Settings",
                "The light lod settings (Distances, Light and Shadow quality)"
                );

            _rangeIsThresholdGUIContent ??= new GUIContent(
                "Range Is Threshold",
                "If the lights 'range' property should direct the dot product threshold"
                );

            _dotThresholdGUIContent ??= new GUIContent(
                "Dot Threshold",
                "The overriden dot threshold when 'Range Is Threshold' is disabled"
                );

            _dotThresholdRangeGUIContent ??= new GUIContent(
                "Dot Threshold (Light range)",
                "The range value of the light source"
                );

            _debug_dotDirectionGUIContent ??= new GUIContent(
                "Dot Direction",
                "The current dot product to the active 'Light LOD Camera'"
                );
        }
    }
}
