using UnityEditor;
using UnityEngine;

namespace ntk.GeospatialCoordinates.Editor
{
    [CustomEditor(typeof(CoordinateConverterComponent))]
    internal sealed class CoordinateConverterComponentEditor : UnityEditor.Editor
    {
        private SerializedProperty originLatitude;
        private SerializedProperty originLongitude;
        private SerializedProperty originHeight;
        private SerializedProperty mode;
        private SerializedProperty japanZone;
        private SerializedProperty japanDatum;
        private SerializedProperty currentLatitude;
        private SerializedProperty currentLongitude;
        private SerializedProperty currentHeight;
        private SerializedProperty targetProperty;
        private SerializedProperty applyOnStart;

        private void OnEnable()
        {
            originLatitude = serializedObject.FindProperty("originLatitudeDegrees");
            originLongitude = serializedObject.FindProperty("originLongitudeDegrees");
            originHeight = serializedObject.FindProperty("originHeightMeters");
            mode = serializedObject.FindProperty("mode");
            japanZone = serializedObject.FindProperty("japanZone");
            japanDatum = serializedObject.FindProperty("japanDatum");
            currentLatitude = serializedObject.FindProperty("currentLatitudeDegrees");
            currentLongitude = serializedObject.FindProperty("currentLongitudeDegrees");
            currentHeight = serializedObject.FindProperty("currentHeightMeters");
            targetProperty = serializedObject.FindProperty("target");
            applyOnStart = serializedObject.FindProperty("applyCurrentCoordinateOnStart");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Transformation", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(mode);
            if ((CoordinateTransformationMode)mode.enumValueIndex == CoordinateTransformationMode.JapanPlaneRectangular)
            {
                EditorGUILayout.PropertyField(japanZone);
                EditorGUILayout.PropertyField(japanDatum, new GUIContent("Datum"));
            }
            else
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(japanZone, new GUIContent("Japan Zone"));
                    EditorGUILayout.PropertyField(japanDatum, new GUIContent("Datum"));
                }
            }
            EditorGUI.indentLevel--;

            if ((CoordinateTransformationMode)mode.enumValueIndex == CoordinateTransformationMode.LocalEnu)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Origin", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(originLatitude, new GUIContent("Latitude", "WGS84 latitude in degrees."));
                EditorGUILayout.PropertyField(originLongitude, new GUIContent("Longitude", "WGS84 longitude in degrees."));
                EditorGUILayout.PropertyField(originHeight, new GUIContent("Ellipsoidal Height", "Ellipsoidal height in metres."));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Current GPS Coordinate", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(currentLatitude, new GUIContent("Latitude", "WGS84 latitude in degrees."));
            EditorGUILayout.PropertyField(currentLongitude, new GUIContent("Longitude", "WGS84 longitude in degrees."));
            EditorGUILayout.PropertyField(currentHeight, new GUIContent("Ellipsoidal Height", "Ellipsoidal height in metres."));
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Application", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(targetProperty, new GUIContent("Target", "Optional. Uses this GameObject when empty."));
            EditorGUILayout.PropertyField(applyOnStart, new GUIContent("Apply On Start"));
            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
