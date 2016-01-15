using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;


/// <summary> Desk class, using doubles instead of floats. </summary>
public class DDesk : Dictionary<string, DTable> {
	
	
	public DDesk() : base() {} 
	public DDesk(string csv) : base() { LoadCSV(csv); }
	public DDesk(string csv, char separator) : base() { LoadCSV(csv, separator); }
	public DDesk(TextAsset textAsset) : base() { LoadCSV(textAsset.text); }
	public DDesk(TextAsset textAsset, char separator) : base() { LoadCSV(textAsset.text, separator); }
	public DDesk(Dictionary<string, DTable> source) : base() {
		foreach (var pair in source) { this.Add(pair.Key, pair.Value); }
	}
	
	public DDesk Clone() {
		DDesk d = new DDesk();
		foreach (string key in Keys) { d[key] = this[key]; }
		return d;
	}
	
	public new DTable this[string key] {
		get {
			Dictionary<string, DTable> goy = this;
			if (!goy.ContainsKey(key)) { return new DTable(); }
			return goy[key];
		}
		
		set {
			Dictionary<string, DTable> goy = this;
			if (goy.ContainsKey(key)) { goy[key] = value; }
			else { goy.Add(key, value); }
		}
	}
	
	public static DTable operator *(DTable a, DDesk b) { return b * a; }
	public static DTable operator *(DDesk a, DTable b) {
		DTable c = new DTable();
		foreach (string key in a.Keys) {
			c[key] = (a[key] * b).Sum();
		}
		return c;
	}
	
	
	public override string ToString() {
		StringBuilder str = new StringBuilder("#Formatted DDesk as .csv:");
		foreach (string key in Keys) {
			str.Append("\n");
			str.Append(key);
			
			DTable t = this[key];
			foreach (string k in t.Keys) {
				str.Append(",");
				str.Append(k);
				str.Append(",");
				str.Append(t[k]);
			}
		}
		return str.ToString();
		
	}
	
	
	public static DDesk FromCSV(TextAsset t) { return FromCSV(t.text, ','); }
	public static DDesk FromCSV(TextAsset t, char delim) { return FromCSV(t.text, delim); }
	public static DDesk FromCSV(string csv) { return FromCSV(csv, ','); }
	public static DDesk FromCSV(string csv, char delim) { 
		DDesk d = new DDesk();
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
			this[key] = new DTable();
			for (int j = 1; j < content.Length; j += 2) {
				//Debug.Log(content[j]);
				this[key].Add(content[j].Trim(), float.Parse(content[j+1]));
			}
			
		}
		
		
	}
	
}
