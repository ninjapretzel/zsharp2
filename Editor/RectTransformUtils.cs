using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class RectTransformUtils : ZEditorWindow {
	
	public static RectTransformUtils window;

	[MenuItem("Utilities/UGUI Rect Utils")]
	public static void ShowWindow() {
		window = (RectTransformUtils) EditorWindow.GetWindow(typeof(RectTransformUtils));
	}

	[MenuItem("Macros/UGUIToggleRectUtils &a")]
	public static void ToggleAnchorMode() {
		anchorMode = !anchorMode;
	}

	private static bool anchorMode = false;

	RectTransform rt;
	Vector2 anchoredPos		{ get { return (rt == null) ? Vector2.zero : rt.anchoredPosition; } }
	Vector2 anchorMax		{ get { return (rt == null) ? Vector2.zero : rt.anchorMax; } }
	Vector2 anchorMin		{ get { return (rt == null) ? Vector2.zero : rt.anchorMin; } }
	Vector2 offsetMax		{ get { return (rt == null) ? Vector2.zero : rt.offsetMax; } }
	Vector2 offsetMin		{ get { return (rt == null) ? Vector2.zero : rt.offsetMin; } }
	Vector2 pivot			{ get { return (rt == null) ? Vector2.zero : rt.pivot; } }
	Rect rect				{ get { return (rt == null) ? new Rect(0,0,0,0) : rt.rect; } }
	Vector2 sizeDelta		{ get { return (rt == null) ? Vector2.zero : rt.sizeDelta; } }

	public RectTransformUtils() {
		
	}

	void Update() {
		GameObject selected = Selection.activeGameObject;
		if (selected != null) {
			rt = selected.GetComponent<RectTransform>();
		} else {
			rt = null;
		}

		if (anchorMode) {
			
			if (rt != null && selected.GetComponent<Canvas>() == null && (rt.offsetMax != Vector2.zero || rt.offsetMin != Vector2.zero)) {
				Undo.RecordObject(rt, "Force rect offsets to zero");
				rt.rotation = Quaternion.identity;
				// TODO: figure out how to adjust anchors when these values are nonzero
				//Vector2 omax = rt.offsetMax;
				//Vector2 omin = rt.offsetMin;
				rt.offsetMax = Vector2.zero;
				rt.offsetMin = Vector2.zero;
				rt.pivot = new Vector2(0.5f, 0.5f);

			}
			Undo.IncrementCurrentGroup();
		}

		Repaint();
	}

	void OnGUI() {
		anchorMode = Toggle(anchorMode, "Anchor Mode");

		Label("apos " + anchoredPos);
		Label("amax " + anchorMax);
		Label("amin " + anchorMin);
		Label("omax " + offsetMax);
		Label("omin " + offsetMin);
		Label("pivot " + pivot);
		Label("rect " + rect);
		Label("sizeDelta " + sizeDelta);
	}

}
