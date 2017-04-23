#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class DatastructureAccessorWindow : ZEditorWindow {
	
	public static string sourcePath { get { return Application.dataPath + "/Data/Accessors/"; } }
	public static string targetPath { get { return Application.dataPath + "/Scripts/Partials/"; } }
	
	[System.NonSerialized] List<string> list;
	[System.NonSerialized] int timeout;
	Vector2 scroll;
	
	[MenuItem ("ZSharp/Legacy/Accessors")]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(DatastructureAccessorWindow));
		
		
	}
	
	public DatastructureAccessorWindow() : base() {
		
		
	}

	void OnEnable() {
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
		foreach (string file in Directory.GetFiles(sourcePath, "*.txt")) {
			list.Add(file);
		}
		
	}
	
	void Build() {
		foreach (string filePath in list) { Build(filePath); }
		
		AssetDatabase.Refresh();
	}
	
	void Build(string filePath) {
		if (File.Exists(filePath)) {
			
			string name = filePath.FromLast('/').UpToFirst('.');
			string content = File.ReadAllText(filePath);
			Settings settings = new Settings(content);
			
			//Debug.Log(settings.ToString());
			
			StreamWriter sr = File.CreateText(targetPath + name + ".cs");
			sr.Write(settings.ToString());
			sr.Close();
			
			Debug.Log("wrote file " + name + ".cs");
			
		
		}
		
	}
	
	
	
	void Update() {
		timeout += 1;
		if (timeout > 100) { RefreshList(); }
		
	}	
	
	void OnGUI() {
		
		BeginVertical("box"); {
			Label(list.Count + " File(s) Found");
			Label("Looking in " + sourcePath);
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
	
	class Settings {
		const string imports = "using UnityEngine;\n\n";
		public string header;
		//Holds associations of variable names to some table
		//An example:
		//("public string float stats", ("level", "hp", "str", "dex", "vit"))
		public Dictionary<string, List<string>> associations;
		
		string activeAssociation = "";
		bool justReadHeader = false;
		
		public Settings() {
			header = "";
			associations = new Dictionary<string, List<string>>();
		}
		
		public Settings(string content) {
			header = "";
			associations = new Dictionary<string, List<string>>();
			
			foreach (string l in content.Split('\n')) {
				string line = l.Trim();
				ReadLine(line);
				
			}
			
			
		}
		
		public override string ToString() {
			StringBuilder str = new StringBuilder("");
			
			//Debug.Log(associations.Count);
			
			str.Append(imports);
			str.Append("//This is an auto generated file\n\n");
			str.Append(header + " {\n");
			foreach (string collection in associations.Keys) {
				str.Append("\n" + GetLines(collection) + "\n");
			}
			
			str.Append("}\n\n");
			
			return str.ToString();
		}
		
		//Get a all lines of a collection's fields
		public string GetLines(string collection) {
			string[] mods = collection.Split(' ');
			bool isStatic = false;
			bool isPublic = false;
			string collectionName = mods[mods.Length-1];
			string typeTo = mods[mods.Length-2];
			string typeFrom = mods[mods.Length-3];
			
			string castTo = "";
			if (typeTo.IndexOf(":") > 0) {
				string[] typesTo = typeTo.Split(':');
				
				typeTo = typesTo[0];
				castTo = typesTo[1];
			}
			
			for (int i = 0; i < mods.Length; i++) {
				string mod = mods[i];
				
				if (mod == "static") { isStatic = true; }
				if (mod == "public") { isPublic = true; }
				
			}
			
			StringBuilder str = new StringBuilder(2047);
			List<string> things = associations[collection];
			bool stringIndex = (typeFrom == "string");
			
			foreach (string thing in things) {
				mods = thing.Split(',');
				string name = mods[0];
				string index = name;
				if (mods.Length > 1) { index = mods[1]; }
				
				string id = collectionName;
				
				if (typeFrom != "thru") {
					id += "[";
					if (stringIndex) { id += "\""; }
					id += index;
					if (stringIndex) { id += "\""; }
					id += "]";
				} else {
					id += "." + index;
				}
				
				StringBuilder line = new StringBuilder(511);
				line.Append("\t");
				if (isPublic) { line.Append("public "); }
				if (isStatic) { line.Append("static "); }
				
				if (castTo == "") {
					line.Append(typeTo + " ");
				} else {
					line.Append(castTo + " ");
				}
				line.Append(name + " ");
				
				//line.Append("{ get { return " + id + "; } set { " + id + " = value; } }\n");
				line.Append("{ get { return " + CastGetLogic(typeTo, castTo, id) + " } ");
				line.Append("set { " + CastSetLogic(typeTo, castTo, id) + " } }\n");
				
				str.Append(line.ToString());
				
			}
			
			
			return str.ToString();
		}
		
		public void Associate(string name, string stuff) {
			List<string> list;
			if (associations.ContainsKey(name)) {
				list = associations[name];

			} else {
				list = new List<string>();
				associations.Add(name, list);
			}
			list.Add(stuff);
			
		}
		
		public string CastSetLogic(string type, string cast, string id) {
			//if (cast == "") { return id + " = value;"; }
			if (type == "float" && cast == "bool") {
				return id + " = value ? 1 : 0;";
			}
			
			if (type == "int" && cast == "bool") {
				return id + " = value ? 1 : 0;";
			}
			
			return id + " = value; ";
		}
		
		public string CastGetLogic(string type, string cast, string id) {
			if (type == "float" && cast == "bool") {
				return "" + id + " == 1f;";
			}
			
			if (type == "int" && cast == "bool") {
				return "" + id + " != 0;";
			}
			
			return id + ";";
		}
		
		public bool ReadLine(string line) {
			
			if (line.Length < 1) { return false; }
			if (line[0] == '#') { return false; }
			
			if (header == "") {
				header = line;
				justReadHeader = true;
				//Debug.Log("<" + line + "> Became header");
				return true;
			}
			
			
			if (line == "{") { 
				justReadHeader = false;
				return true; 
			}
			
			if (line == "}") {
				activeAssociation = ""; 
				//Debug.Log("<" + line + "> Cleared activeAssociation");
				justReadHeader = false;
				return true;
			}
				
			if (activeAssociation == "" || justReadHeader) {
				activeAssociation = line;
				//Debug.Log("<" + line + "> Became activeAssociation");
				justReadHeader = false;
				return true;
			} else {
				Associate(activeAssociation, line);
				//Debug.Log("<" + line + "> Associated");
				justReadHeader = false;
				return true;
			}
			
			//return false;
		}
		
		
	}
	
}


#endif
