using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

#if XtoJSON

public class JsonEditor : ZEditorWindow {
	
	[System.NonSerialized] private List<string> layout;
	[System.NonSerialized] protected Dictionary<string, JsonValue> defaults;
	[System.NonSerialized] protected JsonObject model;
	Vector2 editScroll;
	
	const char SEP = '|';
	
	public JsonEditor() : base() {
		ClearLayout();
	}
	
	public void ClearLayout() {
		layout = new List<string>();
		defaults = new Dictionary<string, JsonValue>();
		model = new JsonObject();
	}
	
	public void AddSpace() { layout.Add("Space"); }
	public void AddField(string name, JsonValue defaultVal) {
		layout.Add("Field" + SEP + name);
		defaults[name] = defaultVal;
		//Debug.Log("added field " + defaults.Count + " - " + name + " : " + defaultVal.ToString());
	}
	public void AddSetColor(string color) { layout.Add("Color" + SEP + color); }
	
	
	public void AddRange(string name, JsonNumber defaultMin, JsonNumber defaultMax) {
		layout.Add("Range" + SEP + name);
		defaults["min" + name] = defaultMin;
		defaults["max" + name] = defaultMax;
	}
	
	public void StartGroup() { layout.Add("StartGroup"); }
	public void StartGroup(string label) { layout.Add("StartGroup"); AddLabel(label); }
	public void EndGroup() { layout.Add("EndGroup"); }
	public void AddLabel(string str) { layout.Add("Label" + SEP + str); }
	public void AddBox(string str) { layout.Add("Box" + SEP + str); }
	
	public void ResetModel() {
		model.Clear();
		foreach (var pair in defaults) {
			model[pair.Key] = pair.Value;
		}
		
	}
	
	public void PatchModel() {
		foreach (var pair in defaults) {
			if (!model.ContainsKey(pair.Key)) {
				model[pair.Key] = pair.Value;
			}
		}
	}
	
	public void ModelEditor() {
		GUI.color = Color.white;
		editScroll = BeginScrollView(editScroll, false, true); {
			BeginVertical("box"); {
				foreach (string line in layout) {
					Do(line);
				}
			} EndVertical();
		} EndScrollView();
	}
	
	void Do(string line) {
		string[] content = line.Split(SEP);
		if (content.Length == 0) { return; }
		
		if (content[0] == "Color") { ChangeColor(content[1]); }
		if (content[0] == "Label") { Label(content[1]); }
		if (content[0] == "Field") { Field(content[1]); }
		if (content[0] == "Range") { Range(content[1]); }
		if (content[0] == "Box") { Box(content[1]); }
		if (content[0] == "Space") { Space(15); }
		if (content[0] == "StartGroup") { BeginVertical("box"); }
		if (content[0] == "EndGroup") { EndVertical(); }
		
	}
	
	void ChangeColor(string color) {
		GUI.color = Color.white;
		if (typeof(Color).HasStaticProperty(color)) {
			Color c = typeof(Color).GetStaticPropertyValue<Color>(color);
			GUI.color = c;
		} else {
			string[] splits = color.Split(',');
			if (splits.Length >= 3) {
				Color c = new Color(0, 0, 0, 1);
				float f;
				
				if (float.TryParse(splits[0], out f)) { c.r = f; }
				if (float.TryParse(splits[1], out f)) { c.g = f; }
				if (float.TryParse(splits[2], out f)) { c.b = f; }
				if (splits.Length >= 4) {
					if (float.TryParse(splits[3], out f)) { c.a = f; }
				}
					
				GUI.color = c;
			}
			
			//Debug.Log("Color " + color + " not found");
		}
	}
	
	void ColoredBox(string color) {
		ChangeColor(color);
		BeginHorizontal("box"); {
			Space(200);
		} EndHorizontal();
	}
	
	void Range(string name) {
		string minKey = "min"+name;
		string maxKey = "max"+name;
		JsonValue minDef = defaults[minKey];
		JsonValue maxDef = defaults[maxKey];
		
		if (minDef != null && maxDef != null) {
			if (model.ContainsAllKeys(minKey, maxKey)) {
				JsonValue minVal = model[minKey];
				JsonValue maxVal = model[maxKey];
				
				if (minVal.isNumber && maxVal.isNumber) {
					BeginHorizontal("box"); {
						Label(name + ": ");
						model[minKey] = FloatField((float)minVal.numVal);
						Label("-");
						model[maxKey] = FloatField((float)maxVal.numVal);
					} EndHorizontal();
					
					
				}
				
			}
		}
		
	}
	
	void Field(string name) {
		JsonValue def = defaults[name];
		if (def != null) {
			if (model.ContainsKey(name)) {
				JsonValue modelVal = model[name];
//				JsonType type = def.JsonType;
				JsonValue backValue = null;
				
				if (def.isBool) {
					if (!modelVal.isBool) { modelVal = false; }
					backValue = Toggle(modelVal.boolVal, name);
				}
				
				if (def.isNumber) {
					if (!modelVal.isNumber) { modelVal = 0; }
					backValue = FloatField(name, (float)modelVal.numVal, .25f);
				}
				
				if (def.isString) {
					if (!modelVal.isString) { modelVal = ""; }
					backValue = TextField(name, modelVal.stringVal, .25f);
				}
				
				model[name] = backValue;
			} else {
				Label("Missing Field: " + name);
			}
		} else {
			Label("Undefined Field: " + name);
		}
		
	}
	
}



#endif