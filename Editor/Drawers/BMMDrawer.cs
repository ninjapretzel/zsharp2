using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BMM))]
public class BMMDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		label = EditorGUI.BeginProperty(position, label, property);
		Rect contentPosition = EditorGUI.PrefixLabel(position, label);

		contentPosition.width *= 0.1f;
		EditorGUI.indentLevel = 0;
		EditorGUIUtility.labelWidth = 14f;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("randomize"), new GUIContent(""));

		contentPosition.x += contentPosition.width;
		contentPosition.width *= 4.5f;
		EditorGUIUtility.labelWidth = 14f;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("min"), new GUIContent("R"));
		
		contentPosition.x += contentPosition.width;
		EditorGUIUtility.labelWidth = 14f;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("max"), new GUIContent("..."));
		
		EditorGUI.EndProperty();
	}

}
