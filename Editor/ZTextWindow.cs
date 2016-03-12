#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

public class ZTextWindow : ZEditorWindow {
	
	[MenuItem("Window/ZTextWindow")]
	public static void ShowWindow() { 
		EditorWindow.GetWindow(typeof(ZTextWindow)); 
	}

	JsonObject values;
	JsonObject flags;

	[System.NonSerialized] private Font font;
	[System.NonSerialized] private Material mat;

	Vector2 scroll = Vector2.zero;


	public ZTextWindow() {
		font = null;
		mat = null;
		values = new JsonObject();
		flags = new JsonObject();

		minSize = new Vector2(NAME_WIDTH + CHANGE_WIDTH + VALUE_WIDTH + 50, 80);
		
	}


	const float NAME_WIDTH = 150;
	const float CHANGE_WIDTH = 50;
	const float VALUE_WIDTH = 100;

	void OnGUI() {
		if (Button("Apply to ONLY selected objects", Width(NAME_WIDTH + CHANGE_WIDTH + VALUE_WIDTH))) {
			ApplyToSelection();
		}
		if (Button("Apply to selected AND children", Width(NAME_WIDTH + CHANGE_WIDTH + VALUE_WIDTH))) {
			foreach (var t in Selection.transforms) { ApplyToChildren(t); }
		}


		BeginHorizontal("box"); {
			Label("Thing", Width(NAME_WIDTH));
			Label("Change", Width(CHANGE_WIDTH));
			Label("Value", Width(VALUE_WIDTH));
		} EndHorizontal();

		
		scroll = BeginScrollView(scroll); {

			font = BoolObjectField<Font>("font", font);

			BoolEnumField("fontStyle", typeof(FontStyle));

			BoolIntField("fontSize");
			BoolFloatField("lineSpacing");
			BoolBoolField("richText");

			BoolEnumField("alignment", typeof(TextAnchor));

			BoolBoolField("alignByGeometry");
			BoolEnumField("horizontalOverflow", typeof(HorizontalWrapMode));
			BoolEnumField("verticalOverflow", typeof(VerticalWrapMode));

			BoolBoolField("bestFit");

			BoolColorField("color");

			mat = BoolObjectField<Material>("material", mat);

			BoolBoolField("raycastTarget");

		} EndScrollView();
		
	}


	void ApplyToChildren(Transform target) {
		JsonObject data = values.Mask(flags);
		foreach (Text t in target.GetComponentsInChildren<Text>(true)) {
			Apply(t, data);


		}

	}
	
	void ApplyToSelection() {
		JsonObject data = values.Mask(flags);
		foreach (var tr in Selection.transforms) {
			Text t = tr.GetComponent<Text>();
			if (t != null) {
				Apply(t, data);
			}
		}

	}

	void Apply(Text t, JsonObject data) {
		Json.ReflectInto(data, t);
		if (flags["material"].boolVal) {
			t.material = mat;
		}

		if (flags["font"].boolVal) {
			t.font = font;
		}
		
		
	}


	void BoolBoolField(string field) {
		BeginHorizontal("box"); {
			Label(field, Width(NAME_WIDTH));
			flags[field] = EditorGUILayout.Toggle(flags.Get<bool>(field), Width(CHANGE_WIDTH));
			values[field] = EditorGUILayout.Toggle(values.Get<bool>(field), Width(VALUE_WIDTH));
		} EndHorizontal();
	}

	void BoolFloatField(string field) {
		BeginHorizontal("box"); {
			Label(field, Width(NAME_WIDTH));
			flags[field] = EditorGUILayout.Toggle(flags.Get<bool>(field), Width(CHANGE_WIDTH));
			values[field] = EditorGUILayout.FloatField(values.Get<float>(field), Width(VALUE_WIDTH));
		} EndHorizontal();
	}

	void BoolIntField(string field) {
		BeginHorizontal("box"); {
			Label(field, Width(NAME_WIDTH));
			flags[field] = EditorGUILayout.Toggle(flags.Get<bool>(field), Width(CHANGE_WIDTH));
			values[field] = EditorGUILayout.IntField(values.Get<int>(field), Width(VALUE_WIDTH));
		} EndHorizontal();
	}

	T BoolObjectField<T>(string label, T obj) where T : UnityEngine.Object {
		T t = default(T);
		BeginHorizontal("box"); {
			Label(label, Width(NAME_WIDTH));
			flags[label] = EditorGUILayout.Toggle(flags[label].boolVal, Width(CHANGE_WIDTH));
			 t = (T)(object)EditorGUILayout.ObjectField(obj, typeof(T), true, Width(VALUE_WIDTH));
		} EndHorizontal();
		return t;
	}

	void BoolEnumField(string field, Type enumType) {
		if (enumType.IsEnum) {

			string strVal = values.Get<string>(field);
			Enum val;
			if (strVal == null) {
				val = (Enum)Enum.ToObject(enumType, 0);
			} else {
				try {
					val = Enum.Parse(enumType, strVal, true) as Enum;
				} catch {
					val = (Enum) Enum.ToObject(enumType, 0);
				}
			}

			BeginHorizontal("box"); {
				Label(field, Width(NAME_WIDTH));
				flags[field] = EditorGUILayout.Toggle(flags.Get<bool>(field), Width(CHANGE_WIDTH));
				values[field] = EditorGUILayout.EnumPopup(val, Width(VALUE_WIDTH)).ToString();
			} EndHorizontal();

		}

	}

	void BoolColorField(string field) {
		
		Color val;
		if (values.ContainsKey(field)) {
			val = values.Get<Color>(field);
		} else {
			val = Color.white;
		}

		BeginHorizontal("box"); {
			Label(field, Width(NAME_WIDTH));
			flags[field] = EditorGUILayout.Toggle(flags.Get<bool>(field), Width(CHANGE_WIDTH));
			//values[field] = Colors.HexString(EditorGUILayout.ColorField(val, Width(VALUE_WIDTH) ) );
			val = EditorGUILayout.ColorField(val, Width(VALUE_WIDTH) );
			values[field] = Json.Reflect(val);
		} EndHorizontal();
	}

	
	void Update() { }
	void OnInspectorUpdate() { }
	
	void OnFocus() { }
	void OnLostFocus() { }

	void OnSelectionChange() { }
	void OnHierarchyChange() { }
	void OnProjectChange() { }
	
	void OnDestroy() { }
	

	void ApplyStuffToSelection() {

	}
}

#endif
