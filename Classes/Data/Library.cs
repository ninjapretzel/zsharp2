using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class Library : Dictionary<string, Shelf> {
	public string[] strings;
	public Shelf[] lists;
	
	public Library Clone() {
		Library d = new Library();
		foreach (string key in Keys) { d[key] = this[key]; }
		return d;
	}
	
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
	
	public static Library operator +(Library a, Library b) {
		Library c = new Library();
		foreach (string key in a.Keys) { c[key] += a[key]; }
		foreach (string key in b.Keys) { c[key] += b[key]; }
		return c;
	}
	
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
	
	public void Save(string name) {
		PlayerPrefs.SetString(name, ToString());
	}
	
	public void Load(string name) {
		if (PlayerPrefs.HasKey(name)) {
			LoadCSV(PlayerPrefs.GetString(name));
		} else {
			Debug.Log("Tried to load non existant key as library: " + name);
		}
	}
	
	
	
}





