using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Oscillator))]
public class OscillatorDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		label = EditorGUI.BeginProperty(position, label, property);
		Rect contentPosition = EditorGUI.PrefixLabel(position, label);
		contentPosition.width *= .25f;
		EditorGUI.indentLevel = 0;
		EditorGUIUtility.labelWidth = 14f;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("mode"), new GUIContent("M"));

		contentPosition.x += contentPosition.width;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("maxTime"), new GUIContent("T"));

		contentPosition.x += contentPosition.width;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("minVal"), new GUIContent("R"));

		contentPosition.x += contentPosition.width;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("maxVal"), new GUIContent("..."));
		
		EditorGUI.EndProperty();


	}

}
