using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class TransformByDepth : IComparer {
	
	int Depth(Transform t) {
		int d = 0;
		Transform p = t.parent;
		while (p != null) {
			d++;
			p = p.parent;
		}
		return d;
	}


	public int Compare(Transform x, Transform y) {
		return Depth(y) - Depth(x);
	}
	public int Compare(object x, object y) {
		if (x is Transform && y is Transform) { return Compare(x as Transform, y as Transform); }
		return -1;
	}
}

[ExecuteInEditMode]
public class StyleChildrenWithGSS : MonoBehaviour {


	[NonSerialized] private int lastWidth = -1;
	[NonSerialized] private int lastHeight = -1;

	
	void Start() {
		ProcessAllChildren(true);
		
	}

	void Update() {
		if (GSS.restyleEverything) {
			//Debug.Log("EVERYTHING IS GETTING RESTYLED");
			ProcessAllChildren(true);
		}

		if (GSS.ReloadStyles() || lastWidth != Screen.width || lastHeight != Screen.height) {
			ProcessAllChildren(true);
			lastHeight = Screen.height;
			lastWidth = Screen.width;

		} else {
			if (!Application.isPlaying) {
				ProcessAllChildren();
			}
		}
		
	}


	void LateUpdate() {
		GSS.restyleEverything = false;
	}


	void OnEnable() {
		ProcessAllChildren(true);

	}	

	void ProcessAllChildren(bool includeInactvie = false) {
		if (!GSS.loaded) { GSS.ReloadStyles(); }

		Transform[] children = transform.GetComponentsInChildren<Transform>(includeInactvie);
		Array.Sort(children, new TransformByDepth() );

		//StringBuilder log = name + "Processing " + children.Length + " objects. ";
		//log += includeInactvie ? "Included Inactives" : "Active Only";
		for (int i = 0; i < children.Length; i++) {
			var child = children[i];
			foreach (var style in GSS.cascades) {
				//log += Process(style as JsonObject, child);
				Process(style as JsonObject, child);
			}
		}

		//Debug.Log(log);
	}

	public string Process(JsonObject style, Component c) {
		string name = c.gameObject.name;
		string namePrefix = style.Get<string>("namePrefix");
		string nameSuffix = style.Get<string>("nameSuffix");
		string nameContains = style.Get<string>("nameContains");
		string tagWith = style.Get<string>("tagWith");
		string styleWith = style.Get<string>("styleWith");

		if ((namePrefix != null && namePrefix != "" && name.StartsWith(namePrefix))
			|| (nameContains != null && nameContains != "" && name.Contains(nameContains))
			|| (nameSuffix != null && nameSuffix != "" && name.EndsWith(nameSuffix))) {


			GSS gss = c.GetComponent<GSS>();
			if (gss == null) {
				GSS.ApplyStyle(c, tagWith, styleWith);
				return "\nApplied " + tagWith + ":" + styleWith + " to " + c.GetRelativePath();
				//GSS g = c.AddComponent<GSS>();
				//g.tagClass = tagWith;
				//g.styleClass = styleWith;
			} else {
				gss.UpdateStyle();
			}
		}
		return "";
	}
	

}
