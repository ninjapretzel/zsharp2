#if UNITY_EDITOR && !UNITY_WEBPLAYER && XtoJSON

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class JsonPropertyGenerator : ZEditorWindow {
	public static string sourcePath { get { return Application.dataPath + "/Data/Accessors/"; } }
	public static string targetPath { get { return Application.dataPath + "/Scripts/Partials/"; } }
	
	
	[System.NonSerialized] List<string> list;
	[System.NonSerialized] int timeout;
	Vector2 scroll;
	
	[MenuItem ("Window/JsonAccessors")]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(JsonPropertyGenerator));
		
	}
	
	public JsonPropertyGenerator() : base() {
		list = new List<string>();
		if (!Directory.Exists(sourcePath)) {
			Directory.CreateDirectory(sourcePath);
			AssetDatabase.Refresh();
		}
		
		if (!Directory.Exists(targetPath)) {
			Directory.CreateDirectory(targetPath);
			AssetDatabase.Refresh();
		}
		
		
		RefreshList();
		
	}
	
	
	void RefreshList() {
		list.Clear();
		foreach (string file in Directory.GetFiles(sourcePath, "*.json")) {
			list.Add(file);
		}
		
	}
	
	
	void Update() {
		timeout += 1;
		if (timeout > 100) { RefreshList(); }
		
	}
	
	void OnGUI() {
		
		BeginVertical("box"); {
			Label(list.Count + " File(s) Found");
			if (Button("Build All")) {
				Build();
			}
			
			scroll = BeginScrollView(scroll, false, true); {
				foreach (string file in list) {
					string name = file.FromLast('/');
					BeginHorizontal("box"); {
						if (Button("Build")) {
							Build(file);
						}
						FixedLabel(name);
						FlexibleSpace();
						FixedLabel("->");
						FlexibleSpace();
						FixedLabel(name.UpToFirst('.') + ".cs");
						
					} EndHorizontal();
				}
			} EndScrollView();
			
			
		} EndVertical();
		
		
	}
	
	
	void Build() {
		foreach (string filePath in list) {
			Build(filePath);
		}
		AssetDatabase.Refresh();
	}
	
	void Build(string filePath) {

		if (File.Exists(filePath)) {
			string name = filePath.FromLast('/').UpToFirst('.');
			string content = File.ReadAllText(filePath);
			
			JsonObject model = Json.Parse(content) as JsonObject;
			if (model != null) {
				string generated = Generate(name, model);
				File.WriteAllText(targetPath + name + ".cs", generated);
				Debug.Log("wrote file " + name + ".cs");
			}
			
		}
	}
	
	
	string Generate(string name, JsonObject model) {
		StringBuilder str = new StringBuilder("");
		
		str += "using UnityEngine;\n\n";
		str += "//This code is a generated code file\n\n";
		
		str += "public partial class " + name + " : JsonObject {\n";
		
		
		var data = model.GetData();
		foreach (var pair in data) {
			string type = pair.Key;
			JsonArray arr = pair.Value as JsonArray;
			if (arr != null) {
				string[] fields = arr.ToStringArray();
				foreach (string field in fields) {
					if (type == "string") {
						str += GetStringLine(field); 
					} else if (type == "float") {
						str += GetFloatLine(field);
					} else if (type == "double") {
						str += GetBooleanLine(field);
					} else if (type == "int") {
						str += GetIntLine(field);
					} else if (type == "bool") {
						str += GetBooleanLine(field);
					} else {
						str += GetObjectLine(type, field);
					}
				}
				
			}
			
		}
		
		
		str += "}";
		
		return str.ToString();
	}
	
	
	//Json.ReflectInto(this["fieldName"], new type());
	//this["fieldName"] = Json.Reflect(value);
	string GetObjectLine(string type, string fieldName) {
		return "\tpublic " + type + " " + fieldName
			+ "{get{"+type+" n=new "+type+"();Json.ReflectInto(this[\"" + fieldName+"\"]as JsonObject,n);return n;"
			+ "}set{this[\"" + fieldName
			+ "\"]=Json.Reflect(value);}}\n";
	}
	
	
	string GetStringLine(string fieldName) {
		return "\tpublic string " + fieldName
			+ "{get{return GetString(\"" + fieldName
			+ "\");}set{this[\"" + fieldName
			+ "\"]=value;}}\n";
	}
	string GetIntLine(string fieldName) {
		return "\tpublic int " + fieldName
			+ "{get{return GetInt(\"" + fieldName
			+ "\");}set{this[\"" + fieldName
			+ "\"]=value;}}\n";
	}
	string GetFloatLine(string fieldName) {
		return "\tpublic float " + fieldName
			+ "{get{return GetFloat(\"" + fieldName
			+ "\");}set{this[\"" + fieldName
			+ "\"]=value;}}\n";
	}
	string GetDoubleLine(string fieldName) {
		return "\tpublic double " + fieldName
			+ "{get{return GetNumber(\"" + fieldName
			+ "\");}set{this[\"" + fieldName
			+ "\"]=value;}}\n";
	}
	string GetBooleanLine(string fieldName) {
		return "\tpublic bool " + fieldName
			+ "{get{return GetBoolean(\"" + fieldName
			+ "\");}set{this[\"" + fieldName
			+ "\"]=value;}}\n";
	}
}










#endif


























