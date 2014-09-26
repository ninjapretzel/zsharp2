using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/**
DTables are a version of Table that uses doubles instead of floats.

DTables can be added, multiplied, and whatnot, just like Tables.
Each of these operations constructs a new DTable.
When DTables (a, b) are added/subtracted, the new DTable has (a.Keys U b.Keys) elements.
When DTables (a, b) are multiplied/divided, the new DTable has (a.Keys I b.Keys) elements
There are also functions to add a single value to the whole DTable (or multiply/divide)

As well as functions to do so randomly, making this quite a useful class to easily calculate stats
Desks are a Dictionary<string, DTable>, which can be used to store all the calculations needed
To calculate various stats.

Example:
In an RPG, lets say we have a large list of stats. We split these up into two parts:
Base Stats:
STR, DEX, VIT
Combat Stats:
Health, Attack, Defense, Accuracy, Dodge, etc

What we can do, is we can construct a Desk to store the information on how to derive these stats.
Lets say the player has this DTable as its 'stats' DTable:
DTable {
	"STR":10,
	"DEX":30,
	"VIT":15,
	"WIS":20
}

This desk might look something like this:
Desk {
	"Health" : DTable { "STR":2, "VIT":5 },
	"Attack" : DTable { "STR":4, "DEX":2 },
	"Defense" : DTable { "VIT":2, "STR":1 }
}

Then we can simply loop through all of the keys of the desk, and create new DTables
from multiplying the stats DTable with each of the 'instructional' DTables.




*/

[System.Serializable]
public class DTable : Dictionary<string, double> {
	
	////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	//Constructors and Loading
	#region
	public DTable() : base() {}
	public DTable(int capacity) : base(capacity) {}
	public DTable(string csv) : base() { LoadCSV(csv); }
	public DTable(string csv, char separator) : base() { LoadCSV(csv, separator); }
	public DTable(TextAsset textAsset) : base() { LoadCSV(textAsset.text); }
	public DTable(TextAsset textAsset, char separator) : base() { LoadCSV(textAsset.text, separator); }
	public DTable(Dictionary<string, double> source) : base() { 
		foreach (var pair in source) { this.Add(pair.Key, pair.Value); }
	}
	public DTable(DTable source) : base() { 
		//Debug.Log("Cloning DTable with " + source.Count + " elements");
		foreach (var pair in source) { 
			//Debug.Log(s);
			this.Add(pair.Key, pair.Value);
			//this[s] = source[s];
		}
	}
	
	public static DTable LoadTextAsset(string name) { return LoadTextAsset(name, ','); }
	public static DTable LoadTextAsset(string name, char delim) {
		TextAsset asset = Resources.Load(name, typeof(TextAsset)) as TextAsset;
		if (asset == null) { return null; }
		return new DTable(asset, delim);
	}
	
	public DTable Clone() { return new DTable(this); }
	public static DTable CreateFromLine(string line) { return CreateFromLine(line, ','); }
	public static DTable CreateFromLine(string line, char separator) { 
		DTable tb = new DTable();
		tb.LoadLine(line, separator);
		return tb;
	}
	
	public void LoadLine(string line) { LoadLine(line, ','); }
	public void LoadLine(string line, char separator) {
		if (line.IndexOf('\n') != -1) {
			Debug.Log("DTable.LoadLine passed in a multi-line string. Loading it as a normal CSV.");
			LoadCSV(line);
			return;
		}
		
		Clear();
		string[] content = line.Split(separator);
		if (line.Length == 0) { 
			//Debug.LogWarning("DTable.LoadLine passed blank string, DTable cleared.");
			return;
		}
		
		for (int i = 0; i < content.Length; i += 2) {
			this[content[i]] = double.Parse(content[i+1]);
		}
		
	}
	
	public static DTable CreateFromCSV(string csv) { return CreateFromCSV(csv, ','); }
	public static DTable CreateFromCSV(string csv, char separator) {
		DTable tb = new DTable();
		tb.LoadCSV(csv, separator);
		return tb;
	}
	
	
	
	public void LoadCSV(string csv) { LoadCSV(csv, ','); }
	public void LoadCSV(string csv, char separator) {
		Clear();
		string[] lines = csv.Split('\n');
		
		for (int i = 0; i < lines.Length; i++) {
			if (lines[i].Length < 3) { continue; }
			if (lines[i][0] == '#') { continue; }
			string[] content = lines[i].Split(separator);
			for (int j = 0; j < content.Length; j += 2) {
				
				this[content[j]] = double.Parse(content[j+1]);
			}
		}
		
	}
	#endregion
	
	
	////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	//Accessor
	#region
	public new double this[string key] {
		get {
			Dictionary<string, double> goy = this;
			if (!goy.ContainsKey(key)) { return 0; }
			return goy[key];
		}
		
		set {
			Dictionary<string, double> goy = this;
			goy[key] = value;
		}
	}
	#endregion
	
	////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	//Operators and Operator-like functions
	#region
	public static DTable operator +(double a, DTable b) { return b + a; }
	public static DTable operator +(DTable a, double b) {
		DTable c = new DTable(a.Count);
		foreach (var pair in a) { c[pair.Key] = pair.Value + b; }
		return c;
	}
	
	public static DTable operator -(double a, DTable b) { return b - a; }
	public static DTable operator -(DTable a, double b) {
		DTable c = new DTable(a.Count);
		foreach (var pair in a) { c[pair.Key] = pair.Value - b; }
		return c;
	}
	
	public static DTable operator *(double a, DTable b) { return b * a; }
	public static DTable operator *(DTable a, double b) {
		DTable c = new DTable(a.Count);
		foreach (var pair in a) { c[pair.Key] = pair.Value * b; }
		return c;
	}
	
	public static DTable operator /(double a, DTable b) { return b / a; }
	public static DTable operator /(DTable a, double b) {
		if (b == 0) { Debug.LogWarning("Trying to divide DTable by zero..."); return a; }
		DTable c = new DTable(a.Count);
		foreach (var pair in a) { c[pair.Key] = pair.Value / b; }
		return c;
	}
	
	public static DTable operator +(DTable a, DTable b) {
		DTable c = new DTable((a.Count > b.Count) ? a.Count : b.Count);
		foreach (var pair in a) { c[pair.Key] += pair.Value; }
		foreach (var pair in b) { c[pair.Key] += pair.Value; }
		return c;
	}
	
	public static DTable operator -(DTable a, DTable b) {
		DTable c = new DTable((a.Count > b.Count) ? a.Count : b.Count);
		foreach (var pair in a) { c[pair.Key] += pair.Value; }
		foreach (var pair in b) { c[pair.Key] -= pair.Value; }
		return c;
	}
	
	public static DTable operator *(DTable a, DTable b) {
		DTable c = new DTable((a.Count > b.Count) ? a.Count : b.Count);
		foreach (var pair in b) { 
			if (a.ContainsKey(pair.Key)) { c[pair.Key] = a[pair.Key] * pair.Value; }
		}
		return c;
	}
	
	public static DTable operator /(DTable a, DTable b) {
		DTable c = new DTable((a.Count > b.Count) ? a.Count : b.Count);
		foreach (var pair in b) {
			if (a.ContainsKey(pair.Key)) { c[pair.Key] = a[pair.Key] / pair.Value; }
		}
		return c;
	}
	
	
	public new void Add(string s, double f) { this[s] = f; }
	public void Add(double f, string s) { this[s] = f; }
	public void Add(string s) { this[s] = 0; }
	
	public void Add(double f) { foreach (var pair in this) { this[pair.Key] = pair.Value + f; } }
	public void Add(DTable t) { foreach (var pair in t) { this[pair.Key] += pair.Value; } }
	
	public void Subtract(double f) { foreach (var pair in this) { this[pair.Key] = pair.Value - f; } }
	public void Subtract(DTable t) { foreach (var pair in t) { this[pair.Key] -= pair.Value; } }
	
	public void AddRandomly(double f) { foreach (var pair in this) { this[pair.Key] = pair.Value + f * Random.value; } }
	public void AddRandomly(DTable t) { foreach (var pair in t) { this[pair.Key] += pair.Value * Random.value; } }
	
	public void AddRandomNormal(double f) { foreach (var pair in this) { this[pair.Key] = pair.Value + f * Random.normal; } }
	public void AddRandomNormal(DTable t) { foreach (var pair in t) { this[pair.Key] += pair.Value * Random.normal; } }

	public void Multiply(double f) { foreach (var pair in this) { this[pair.Key] = pair.Value * f; } }
	public void Multiply(DTable t) { 
		foreach (var pair in t) {
			if (ContainsKey(pair.Key)) { this[pair.Key] *= pair.Value; }
		}
	}
	
	public void Divide(double f) { foreach (var pair in this) { this[pair.Key] = pair.Value / f; } }
	public void Divide(DTable t) { 
		foreach (var pair in t) {
			if (ContainsKey(pair.Key) && pair.Value != 0) { this[pair.Key] /= pair.Value; }
		}
	}
	
	public DTable Mask(string mask) { return Mask(mask, ','); }
	public DTable Mask(string mask, char delim) { return Mask(mask.Split(delim)); }
	public DTable Mask(string[] fields) {
		DTable t = new DTable(Count);
		foreach (string field in fields) {
			if (this[field] != 0) { t[field] = this[field]; }
		}
		return t;
	}
	
	#endregion
	
	////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	//Support for containing certain data types
	#region
	
	//Get a collection containing only the keys for 'fields', (double/Vector/Color/etc)
	public Set<string> FieldKeys {
		get {
			Set<string> ukeys = new Set<string>();
			
			foreach (string s in Keys) {
				int index = s.LastIndexOf('.');
				if (index < 0) { ukeys.Add(s); continue; }
				
				string k = s.Substring(0, index);
				if (s.Length - k.Length <= 2) { ukeys.Add(k); }
				else { ukeys.Add(s); }
				
			}
			
			return ukeys;
		}
	}
	
	//Quick check functions
	public bool ContainsColorQ(string s) { return ContainsKey(s+".r"); }
	public bool ContainsVector2Q(string s) { return ContainsKey(s+".y"); }
	public bool ContainsVector3Q(string s) { return ContainsKey(s+".z"); }
	
	//Full check functions
	public bool ContainsColor(string s) { return ContainsKey(s+".r") && ContainsKey(s+".g") && ContainsKey(s+".b") && ContainsKey(s+".a"); }
	public bool ContainsVector2(string s) { return ContainsKey(s+".x") && ContainsKey(s+".y"); }
	public bool ContainsVector3(string s) { return ContainsKey(s+".x") && ContainsKey(s+".y") && ContainsKey(s+".z"); }
		
	public Color GetColor(string s) { return new Color((float)this[s+".r"], (float)this[s+".g"], (float)this[s+".b"], (float)this[s+".a"]); }
	public Vector2 GetVector2(string s) { return new Vector2((float)this[s+".x"], (float)this[s+".y"]); }
	public Vector3 GetVector3(string s) { return new Vector3((float)this[s+".x"], (float)this[s+".y"], (float)this[s+".z"]); }
	
	public void SetColor(string s, Color c) { 
		this[s+".r"] = c.r;
		this[s+".g"] = c.g;
		this[s+".b"] = c.b;
		this[s+".a"] = c.a;
	}
	
	public void SetVector3(string s, Vector3 v) {
		this[s+".x"] = v.x;
		this[s+".y"] = v.y;
		this[s+".z"] = v.z;
	}
	
	public void SetVector2(string s, Vector2 v) {
		this[s+".x"] = v.x;
		this[s+".y"] = v.y;
	}
	#endregion
	
	
	
	////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	//Misc functions
	#region
	public string Choose() {
		List<string> names = new List<string>(Count);
		List<float> weights = new List<float>(Count);
		
		int i = 0;
		foreach (var pair in this) {
			names.Add(pair.Key);
			weights.Add((float)pair.Value);
			i++;
		}
		
		return names.Choose(weights);
		
	}
	
	public override string ToString() { return ToString(','); }
	public string ToString(char delim) {
		
		//Save a little bit of space when saving DTables to PlayerPrefs on webplayer
		StringBuilder str = new StringBuilder(
		#if !UNITY_WEBPLAYER
		"#Formatted DTable as .csv:"
		#endif
		);
		
		foreach (var pair in this) { str.Append('\n' + pair.Key + delim + pair.Value); }
		return str.ToString();
	}
	
	public string ToLine() { return ToLine(','); }
	public string ToLine(char delim) {
		StringBuilder str = new StringBuilder();
		foreach (var pair in this) {
			if (str.Length > 0) { str.Append(delim); }
			str.Append(pair.Key + delim + pair.Value);
		}
		//Debug.Log("ToLine: [" + str.ToString() + "]");
		return str.ToString();
	}
	
	
	public double Sum() {
		double f = 0;
		foreach (var pair in this) { f += pair.Value; }
		return f;
	}
	
	//Gets the first key that matches value
	public string GetKey(double value) {
		foreach (var pair in this) {
			if (pair.Value == value) { return pair.Key; }
		}
		return "";
	}
	
	public void Set(DTable t) {
		foreach (var pair in t) { this[pair.Key] = pair.Value; }
	}
	
	public void Save(string name) {
		PlayerPrefs.SetString(name, ToLine());
		
		/*
		#if UNITY_WEBPLAYER
		#else
		PlayerPrefs.SetString(name, ToString());
		#endif
		*/
	}
	
	public void Load(string name) {
		string str = PlayerPrefs.GetString(name);
		LoadLine(str);
		/*
		#if UNITY_WEBPLAYER
		#else
		if (str.Length > 2) {
			LoadCSV(str);
		} else {
			Debug.Log("Unable to load DTable from CSV from player pref: " + name + "\nLoaded:\n" + str);
		}
		#endif
		*/
	}
	
	
	#endregion
	
}

public class ConvertsToDTable {

	public DTable asDTable {
		get { return this.ToDTable(); }
		set { this.SetDTable(value); }
	}
	
}


public static class DTableHelper {
	public static DTable ToDTable(this object obj) {
		DTable t = new DTable();
			
		FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
		for (int i = 0; i < fields.Length; i++) {
			FieldInfo field = fields[i];
			if (field.FieldType == typeof(double)) {
				t[field.Name] = (double) field.GetValue(obj);
			}
			
			if (field.FieldType == typeof(double)) {
				t[field.Name] = (double) (double) field.GetValue(obj);
				
			}
			
			if (field.FieldType == typeof(int)) {
				t[field.Name] = (double) (int) field.GetValue(obj);
			}
				
			if (field.FieldType == typeof(bool)) {
				t[field.Name] = ((bool)field.GetValue(obj)) ? 1f : 0f;
			}
			
			if (field.FieldType == typeof(Vector2)) {
				t.SetVector2(field.Name, (Vector2)field.GetValue(obj));
			}
			
			if (field.FieldType == typeof(Vector3)) {
				t.SetVector3(field.Name, (Vector3)field.GetValue(obj));
			}
			
			if (field.FieldType == typeof(Color)) {
				t.SetColor(field.Name, (Color)field.GetValue(obj));
			}
			
			
		}
		
		return t;
		
	}
	
	
	public static void SetDTable(this object obj, DTable DTable) {
		foreach (string s in DTable.FieldKeys) {
			FieldInfo field = obj.GetType().GetField(s, BindingFlags.Public | BindingFlags.Instance);
			if (field != null) {
				if (field.FieldType == typeof(double)) {
					field.SetValue(obj, DTable[s]);
					continue;
				}
				
				if (field.FieldType == typeof(double)) {
					field.SetValue(obj, (double)DTable[s]);
					continue;
				}
				
				if (field.FieldType == typeof(int)) {
					field.SetValue(obj, (int)DTable[s]);
					continue;
				}
				
				if (field.FieldType == typeof(bool)) {
					field.SetValue(obj, (DTable[s] == 1f) ? true : false);
					continue;
				}
				
				if (field.FieldType == typeof(Vector2)) {
					field.SetValue(obj, DTable.GetVector2(s));
				}
				
				if (field.FieldType == typeof(Vector3)) {
					field.SetValue(obj, DTable.GetVector3(s));
				}
				
				if (field.FieldType == typeof(Color)) {
					field.SetValue(obj, DTable.GetColor(s));
				}
				
			}
		}
	}
	
}


