using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class StyleChildrenWithGSS : MonoBehaviour {


	[NonSerialized] private int lastWidth = -1;
	[NonSerialized] private int lastHeight = -1;

	void Awake() {
		ProcessAllChildren(true);
		

	}

	void Update() {
		
		if (GSS.ReloadStyles() || lastWidth != Screen.width || lastHeight != Screen.height) {
			ProcessAllChildren(true);
			lastHeight = Screen.height;
			lastWidth = Screen.width;

		} else {
			ProcessAllChildren();
		}

#if !UNITY_EDITOR
		if (Application.isPlaying) { 
			Destroy(this);	
		}
#endif

	}

	void ProcessAllChildren(bool includeInactvie = false) {
		Transform[] children = transform.GetComponentsInChildren<Transform>(includeInactvie);

		for (int i = children.Length-1; i >= 0; i--) {
			var child = children[i];
			foreach (var style in GSS.cascades) {
				Process(style as JsonObject, child);
			}
		}

	}

	public void Process(JsonObject style, Component c) {
		string name = c.gameObject.name;
		string namePrefix = style.Get<string>("namePrefix");
		string nameSuffix = style.Get<string>("nameSuffix");
		string nameContains = style.Get<string>("nameContains");
		string tagWith = style.Get<string>("tagWith");
		string styleWith = style.Get<string>("styleWith");

		if ((namePrefix != null && namePrefix != "" && name.StartsWith(namePrefix))
			|| (nameContains != null && nameContains != "" && name.Contains(nameContains))
			|| (nameSuffix != null && nameSuffix != "" && name.EndsWith(nameSuffix))) {

			if (c.GetComponent<GSS>() == null) {
				GSS.ApplyStyle(c, tagWith, styleWith);

				//GSS g = c.AddComponent<GSS>();
				//g.tagClass = tagWith;
				//g.styleClass = styleWith;
			}
		}
	}
	

}
