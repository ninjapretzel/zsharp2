using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/// <summary> Extension on Dictionary(string, string) with operators and custom Behaviour. 
/// NonExistant entries are treated as empty strings. </summary>
public class StringMap : Dictionary<string, string> {

	/// <summary> Indexer treats non-existant entries as empty strings. </summary>
	public new string this[string key] {
		get { 
			if (ContainsKey(key)) { return ((Dictionary<string, string>)this)[key]; }
			return "";
		}
		
		set {
			((Dictionary<string, string>)this)[key] = value;
		}
		
	}
	/// <summary> can + any IEnumerable(KeyValuePair(object, object)), using ToString() to add keys and values</summary>
	public static StringMap operator +(StringMap a, IEnumerable<KeyValuePair<object, object>> b) {
		StringMap c = a.Clone();
		foreach (var pair in b) {
			string key = pair.Key.ToString();
			if (!c.ContainsKey(key)) { c[key] = pair.Value.ToString(); }
		}
		return c;
	}
	
	public StringMap() : base() { }
	public StringMap(string s, char delim = ',' ) : base() { LoadFromString(s, delim); }

	/// <summary> Clones this object, making a new StringMap with the same data. </summary>
	public StringMap Clone() {
		StringMap copy = new StringMap();
		foreach (var pair in this) { copy[pair.Key] = pair.Value; }
		return copy;
	}

	/// <summary> Get a string representation of this object as a CSV, with a default delimeter of ',' </summary>
	public override string ToString() { return ToString(','); }
	/// <summary> Get a string representation of this object as a CSV, with the given delimeter character. </summary>
	public string ToString(char delim = ',') {
		StringBuilder str = new StringBuilder("");
		int i = 0;
		foreach (string key in Keys) {
			bool done = (++i == Count);
			str.Append(key + delim + this[key] + (done ? "" : (""+delim)));
		}
		return str.ToString();
	}

	/// <summary> Clear the map and load a CSV into this map with a given delimeter character. </summary>
	public void LoadFromCSV(string csv, char delim = ',') {
		Clear();
		string[] lines = csv.ConvertNewlines().Split('\n');
		for (int i = 0; i < lines.Length; i++) {
			Debug.Log(lines[i]);
			LoadLine(lines[i], delim);
		}
	}

	/// <summary> Clear the map and load a string into the mapping. </summary>
	public void LoadFromString(string s, char delim = ',') {
		Clear();
		LoadLine(s, delim);
	}

	/// <summary> Add pairs of elements in a string (should be one line) to the mapping. </summary>
	public void LoadLine(string s, char delim = ',' ) {
		string[] strs = s.Split(delim);
		for (int i = 0; i+1 < strs.Length; i += 2) {
			this[strs[i]] = strs[i+1];
		}
	}


	[System.Obsolete("Just use the constructor....")]
	public static StringMap CreateFromLine(string s, char delim = ',') {
		StringMap map = new StringMap();
		map.LoadFromString(s, delim);
		return map;
	}

	

	
	
	
	
}
