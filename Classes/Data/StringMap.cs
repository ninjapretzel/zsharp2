using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class StringMap : Dictionary<string, string> {
	
	public new string this[string key] {
		get { 
			if (ContainsKey(key)) { return ((Dictionary<string, string>)this)[key]; }
			return "";
		}
		
		set {
			((Dictionary<string, string>)this)[key] = value;
		}
		
	}
	
	public static StringMap operator +(StringMap a, StringMap b) {
		StringMap c = a.Clone();
		foreach (string key in b.Keys) {
			if (!c.ContainsKey(key)) { c[key] = b[key]; }
		}
		return c;
	}
	
	public StringMap() : base() { }
	public StringMap(string s) : base() { LoadFromString(s); }
	
	public StringMap Clone() {
		StringMap m = new StringMap();
		foreach (string key in Keys) { m[key] = this[key]; }
		return m;
	}
	
	public override string ToString() { return ToString(','); }
	public string ToString(char delim) {
		StringBuilder str = new StringBuilder("");
		int i = 0;
		foreach (string key in Keys) {
			bool done = (++i == Count);
			str.Append(key + delim + this[key] + (done ? "" : (""+delim)));
		}
		return str.ToString();
	}
	
	public void LoadFromCSV(string csv) { LoadFromString(csv, ','); }
	public void LoadFromCSV(string csv, char delim) {
		Clear();
		string[] lines = csv.ConvertNewlines().Split('\n');
		for (int i = 0; i < lines.Length; i++) {
			Debug.Log(lines[i]);
			LoadLine(lines[i], delim);
		}
	}
	
	public void LoadFromString(string s) { LoadFromString(s, ','); }
	public void LoadFromString(string s, char delim) {
		Clear();
		LoadLine(s, delim);
	}
	
	public void LoadLine(string s, char delim) {
		//Debug.Log(s);
		string[] strs = s.Split(delim);
		for (int i = 0; i+1 < strs.Length; i += 2) {
			this[strs[i]] = strs[i+1];
		}
	}
	
	public static StringMap CreateFromLine(string s) { return CreateFromLine(s, ','); }
	public static StringMap CreateFromLine(string s, char delim) {
		StringMap map = new StringMap();
		map.LoadFromString(s, delim);
		return map;
	}

	

	
	
	
	
}
