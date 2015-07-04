using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class GUIDrag : MonoBehaviour {
	
#if XtoJSON

	public static JsonObject dragging;
	public static void StartDrag(JsonObject obj) {
		dragging = obj;
	}

	public static void OnDragRelease(System.Action<JsonObject> act) {
		if (dragging != null && GUIEvent.leftClickUp) {
			act(dragging);
			Event.current.Use();
			dragging = null;
		}
	}

	void OnGUI() {
		GUI.depth = 99999999;
		
		OnDragRelease(Eat);
	}
	void Eat(JsonObject drag) {
		Debug.Log("GUIRoot ate " + drag.PrettyPrint());
	}
	
#endif



}
