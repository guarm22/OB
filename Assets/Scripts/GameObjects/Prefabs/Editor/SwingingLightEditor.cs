using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SwingingLight))]
public class SwingingLightEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        SwingingLight light = (SwingingLight)target;

        if (GUILayout.Button("Reset Animation State"))
        {
            Undo.RecordObject(light, "Reset SwingingLight Animation State");
            Undo.RecordObject(light.transform, "Reset SwingingLight Animation State");
            light.ResetAnimationStateOnly();
            EditorUtility.SetDirty(light);
            EditorUtility.SetDirty(light.transform);
        }

        if (GUILayout.Button("Reset Animation + Position Properties"))
        {
            Undo.RecordObject(light, "Reset SwingingLight Properties");
            Undo.RecordObject(light.transform, "Reset SwingingLight Properties");
            light.ResetAnimationAndPositionProperties();
            EditorUtility.SetDirty(light);
            EditorUtility.SetDirty(light.transform);
        }
    }
}
