using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;


/**
Desks and tables are best buddies.

ass

*/
public class Desk : Dictionary<string, Table> {
	
	
	public Desk() : base() {} 
	public Desk(string csv) : base() { LoadCSV(csv); }
	public Desk(string csv, char separator) : base() { LoadCSV(csv, separator); }
	public Desk(TextAsset textAsset) : base() { LoadCSV(textAsset.text); }
	public Desk(TextAsset textAsset, char separator) : base() { LoadCSV(textAsset.text, separator); }
	public Desk(Dictionary<string, Table> source) : base() {
		foreach (var pair in source) { this.Add(pair.Key, pair.Value); }
	}
	
	public Desk Clone() {
		Desk d = new Desk();
		foreach (string key in Keys) { d[key] = this[key]; }
		return d;
	}
	
	public new Table this[string key] {
		get {
			Dictionary<string, Table> goy = this;
			if (!goy.ContainsKey(key)) { return new Table(); }
			return goy[key];
		}
		
		set {
			Dictionary<string, Table> goy = this;
			if (goy.ContainsKey(key)) { goy[key] = value; }
			else { goy.Add(key, value); }
		}
	}
	
	public static Table operator *(Table a, Desk b) { return b * a; }
	public static Table operator *(Desk a, Table b) {
		Table c = new Table();
		foreach (string key in a.Keys) {
			c[key] = (a[key] * b).Sum();
		}
		return c;
	}
	
	
	public override string ToString() {
		StringBuilder str = new StringBuilder("#Formatted Desk as .csv:");
		foreach (string key in Keys) {
			str.Append("\n");
			str.Append(key);
			
			Table t = this[key];
			foreach (string k in t.Keys) {
				str.Append(",");
				str.Append(k);
				str.Append(",");
				str.Append(t[k]);
			}
		}
		return str.ToString();
		
	}
	
	
	public static Desk FromCSV(TextAsset t) { return FromCSV(t.text, ','); }
	public static Desk FromCSV(TextAsset t, char delim) { return FromCSV(t.text, delim); }
	public static Desk FromCSV(string csv) { return FromCSV(csv, ','); }
	public static Desk FromCSV(string csv, char delim) { 
		Desk d = new Desk();
		d.LoadCSV(csv, delim);
		return d;
	}
	
	public void LoadCSV(TextAsset t) { LoadCSV(t.text, ','); }
	public void LoadCSV(TextAsset t, char delim) { LoadCSV(t.text, delim); }
	public void LoadCSV(string csv) { LoadCSV(csv, ','); }
	public void LoadCSV(string csv, char delim) {
		Clear();
		string[] lines = csv.Split('\n');
		
		for (int i = 0; i < lines.Length; i++) {
			//Debug.Log(lines[i]);
			if (lines[i].Length == 0) { continue; }
			if (lines[i][0] == '#') { continue; }
			string[] content = lines[i].Split(delim);
			string key = content[0];
			this[key] = new Table();
			for (int j = 1; j < content.Length; j += 2) {
				//Debug.Log(content[j]);
				this[key].Add(content[j].Trim(), float.Parse(content[j+1]));
			}
			
		}
		
		
	}
	
}
