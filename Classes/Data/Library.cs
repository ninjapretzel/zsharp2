using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary> Mapping of (string, Shelf) </summary>
[System.Serializable]
public class Library : Dictionary<string, Shelf> {

	/// <summary> Returns a copy of this library. </summary>
	public Library Clone() {
		Library d = new Library();
		foreach (string key in Keys) { d[key] = this[key]; }
		return d;
	}

	/// <summary> Accessor to provide some custom behaviour. Non-existant entries are treated as a new Shelf() </summary>
	public new Shelf this[string key] {
		get {
			Dictionary<string, Shelf> goy = this;
			if (!goy.ContainsKey(key)) { return new Shelf(); }
			return goy[key];
		}
		
		set { 
			Dictionary<string, Shelf> goy = this;
			if (goy.ContainsKey(key)) { goy[key] = value; }
			else { goy.Add(key, value); }
			
		}
	}

	/// <summary> Operator that creates a new Library and adds all pairs of two libraries to it. </summary>
	public static Library operator +(Library a, Library b) {
		Library c = new Library();
		foreach (string key in a.Keys) { c[key] += a[key]; }
		foreach (string key in b.Keys) { c[key] += b[key]; }
		return c;
	}

	/// <summary> Get a sstring representation of this library, formatted as a CSV with ',' as the delimeter. </summary>
	public override string ToString() {
		StringBuilder str = new StringBuilder("#Formatted Library as .csv:");
		foreach (string key in Keys) {
			str.Append("\n");
			str.Append(key);
			Shelf collection = this[key];
			foreach (string val in collection) {
				str.Append(",");
				str.Append(val);
			}
		}
		return str.ToString();
	}

	/// <summary> Load a CSV into this library. Left column of each line becomes the key, the rest of values on the line become all the entries in that Shelf. </summary>
	public void LoadCSV(string csv) {
		Clear();
		string[] lines = csv.Split('\n');
		
		for (int i = 0; i < lines.Length; i++) {
			if (lines[i][0] == '#') { continue; }
			
			string[] content = lines[i].Split(',');
			string key = content[0];
			
			if (content.Length == 1) { continue; }
			
			this[key] = new Shelf();
			for (int j = 1; j < content.Length; j++) {
				this[key] += content[j];
			}
		}
	}

	/// <summary> Load a CSV into this library. Left column of each line becomes the key, the rest of values on the line become all the entries in that Shelf. 
	/// This version Ignores a set of strings when loading the values. </summary>
	public void LoadCSV(string csv, string[] ignore) {
		Clear();
		string[] lines = csv.Split('\n');
		
		for (int i = 0; i < lines.Length; i++) {
			if (lines[i][0] == '#') { continue; }
			
			string[] content = lines[i].Split(',');
			string key = content[0];
			
			if (content.Length == 1) { continue; }
			
			this[key] = new Shelf();
			for (int j = 1; j < content.Length; j++) {
				for (int k = 0; k < ignore.Length; k++) { 
					if (ignore[k] == content[j]) { continue; } 
				}
				this[key] += content[j];
			}
		}
	}
	
	/// <summary> Save this Library to PlayerPrefs with the key 'name' </summary>
	public void Save(string name) {
		PlayerPrefs.SetString(name, ToString());
	}
	
	/// <summary> Load into this Library from PlayerPrefs with the key 'name' </summary>
	public void Load(string name) {
		if (PlayerPrefs.HasKey(name)) {
			LoadCSV(PlayerPrefs.GetString(name));
		} else {
			Debug.Log("Tried to load non existant key as library: " + name);
		}
	}
	
	
	
}





