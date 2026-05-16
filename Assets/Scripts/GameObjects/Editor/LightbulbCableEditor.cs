using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightbulbCable))]
[CanEditMultipleObjects]
public class LightbulbCableEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		EditorGUILayout.Space();

		if (GUILayout.Button("Regenerate (Clear Existing)")) {
			foreach (var selected in targets) {
				if (selected is not LightbulbCable cable) {
					continue;
				}

				Undo.RecordObject(cable, "Regenerate Lightbulb Cable");
				cable.ClearGenerated();
				cable.Regenerate();
				EditorUtility.SetDirty(cable);
			}
		}
	}
}
