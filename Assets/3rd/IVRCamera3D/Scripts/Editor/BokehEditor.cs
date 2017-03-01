using UnityEngine;
using UnityEditor;

namespace Kino
{
    [CanEditMultipleObjects, CustomEditor(typeof(Bokeh))]
    public class BokehEditor : Editor
    {
        SerializedProperty _focusedObject;
        SerializedProperty _distance;
        SerializedProperty _fNumber;
        SerializedProperty _useCameraFov;
        SerializedProperty _focalLength;
        SerializedProperty _maxBlur;
        SerializedProperty _irisAngle;
        SerializedProperty _sampleCount;
        SerializedProperty _foregroundBlur;
        SerializedProperty _visualize;
        SerializedProperty _deltaFocusTime;
        SerializedProperty _minRefocusTime;
        //SerializedProperty _worldScale;

        static GUIContent _textFNumber = new GUIContent("f/");
        static GUIContent _textFocalLengthMM = new GUIContent("Focal Length (mm)");
        static GUIContent _textMaxBlurPercent = new GUIContent("Max Blur (%)");

        void OnEnable()
        {
			_focusedObject  = serializedObject.FindProperty("_focusedObject");
            _distance       = serializedObject.FindProperty("_distance");
            _fNumber        = serializedObject.FindProperty("_fNumber");
            _useCameraFov   = serializedObject.FindProperty("_useCameraFov");
            _focalLength    = serializedObject.FindProperty("_focalLength");
            _maxBlur        = serializedObject.FindProperty("_maxBlur");
            _irisAngle      = serializedObject.FindProperty("_irisAngle");
            _sampleCount    = serializedObject.FindProperty("_sampleCount");
            _foregroundBlur = serializedObject.FindProperty("_foregroundBlur");
            _visualize      = serializedObject.FindProperty("_visualize");
            _deltaFocusTime = serializedObject.FindProperty("_deltaFocusTime");
            _minRefocusTime = serializedObject.FindProperty("_minRefocusTime");
            //_worldScale     = serializedObject.FindProperty("_worldScale");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Subject/Distance
			EditorGUILayout.PropertyField(_focusedObject);
			if (_focusedObject.hasMultipleDifferentValues || _focusedObject.objectReferenceValue == null)
                EditorGUILayout.PropertyField(_distance);

            // f/
            EditorGUILayout.PropertyField(_fNumber, _textFNumber);

            // Use Camera FOV
            EditorGUILayout.PropertyField(_useCameraFov);

            // Focal Length
            if (_useCameraFov.hasMultipleDifferentValues || !_useCameraFov.boolValue)
            {
                if (_focalLength.hasMultipleDifferentValues)
                    EditorGUILayout.PropertyField(_focalLength);
                else
                {
                    EditorGUI.BeginChangeCheck();
                    var f = _focalLength.floatValue * 1000;
                    f = EditorGUILayout.Slider(_textFocalLengthMM, f, 10.0f, 300.0f);
                    if (EditorGUI.EndChangeCheck())
                        _focalLength.floatValue = f / 1000;
                }
            }

            // Max Blur
            if (_maxBlur.hasMultipleDifferentValues)
                EditorGUILayout.PropertyField(_maxBlur);
            else
            {
                EditorGUI.BeginChangeCheck();
                var blur = _maxBlur.floatValue * 100;
                blur = EditorGUILayout.Slider(_textMaxBlurPercent, blur, 1, 10);
                if (EditorGUI.EndChangeCheck())
                    _maxBlur.floatValue = blur / 100;
            }

            // Iris Angle
            EditorGUILayout.Slider(_irisAngle, 0, 90);

            // Sample Count
            EditorGUILayout.PropertyField(_sampleCount);

            // Foreground Blur
            EditorGUILayout.PropertyField(_foregroundBlur);

            // Visualize
            EditorGUILayout.PropertyField(_visualize);

            // Delta Focus Time
            EditorGUILayout.Slider(_deltaFocusTime, 0.01f, 0.8f);

            //Min Refocus Time
            EditorGUILayout.Slider(_minRefocusTime, 0.01f, 0.1f);

            //World Scale
            //EditorGUILayout.Slider(_worldScale, 0.1f, 10f);

            serializedObject.ApplyModifiedProperties();

            
        }
    }
}
