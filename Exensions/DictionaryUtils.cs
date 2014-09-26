using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Extensded function classes with lots of useful functions for dictionaries

//Dictionary float 
public static class DictionaryUtils {
	
	public static List<T> GetKeyList<T, K>(this Dictionary<T, K> d) {
		List<T> l = new List<T>();
		foreach (T t in d.Keys) { l.Add(t); }
		return l;
	}
	
	public static Set<T> GetKeySet<T, K>(this Dictionary<T, K> d) {
		Set<T> s = new Set<T>();
		foreach (T t in d.Keys) { s.Add(t); }
		return s;
	}
	
	
	public static Set<T> Choose<T, K>(this Dictionary<T, K> d, int num) {
		Set<T> s = new Set<T>();
		Set<T> keys = d.GetKeySet<T, K>();
		while (s.Count < num && keys.Count > 0) {
			T t = keys.Choose();
			s.Add(t);
			keys.Remove(t);
		}
		return s;
	}
	
	public static T RandomKey<T, K>(this Dictionary<T, K> d) {
		int i = 0;
		int chosen = (int)Random.Range(0, d.Count);
		foreach (T t in d.Keys) {
			if (i == chosen) { return t; }
			i++;
		}
		return default(T);
	}
	
	public static void LoadCSV(this Dictionary<string, string> d, string csv) { d.LoadCSV(csv, ','); }
	public static void LoadCSV(this Dictionary<string, string> d, string csv, char delim) { 
		d.Clear();
		string[] lines = csv.Split('\n');
		
		for (int i = 0; i < lines.Length; i++) {
			if (lines[i].Length == 0) { continue; }
			if (lines[i][0] == '#') { continue; }
			string[] content = lines[i].Split(delim);
			for (int j = 0; j < content.Length; j += 2) {
				d.Add(content[j], content[j+1]);
			}
		}
	}
	
	public static void LoadCSV(this Dictionary<string, Set<string>> d, string csv) { d.LoadCSV(csv, ','); }
	public static void LoadCSV(this Dictionary<string, Set<string>> d, string csv, char delim) { 
		d.Clear();
		string[] lines = csv.Split('\n');
		
		for (int i = 0; i < lines.Length; i++) {
			if (lines[i][0] == '#') { continue; }
			if (lines[i].Length == 0) { continue; }
			string[] content = lines[i].Split(delim);
			Set<string> stringset = new Set<string>();
			for (int j = 1; j < content.Length; j++) {
				stringset.Add(content[j]);
			}
			d.Add(content[0], stringset);
		}
	}


	public static void AddAll(this Dictionary<string, float> d, Dictionary<string, float> other) {
		foreach (string k in other.Keys) {
			if (d.ContainsKey(k)) { d[k] += other[k]; }
			else { d.Add(k, other[k]); }
		}
	}
	
	public static void Add(this Dictionary<string, float> d, float amt, string type) {
		if (d.ContainsKey(type)) { d[type] += amt; } 
		else { d.Add(type, amt); }
	}
	
	public static void Set(this Dictionary<string, float> d, float amt, string type) { d.Set(type, amt); }
	public static void Set(this Dictionary<string, float> d, string type, float amt) {
		if (d.ContainsKey(type)) { d[type] = amt; }
		else { d.Add(type, amt); }
	}
	
	public static float Sum(this Dictionary<string, float> d) {
		float total = 0;
		foreach (string k in d.Keys) {
			total += d[k];
		}
		return total;
	}
	
	public static void SaveToPlayerPrefs(this Dictionary<string, float> d, string name) {
		int i = 0;
		PlayerPrefs.SetInt(name + "_count", d.Count);
		foreach (string k in d.Keys) { 
			PlayerPrefs.SetString(name + "_" + i + "_key", k);
			PlayerPrefs.SetFloat(name + "_" + i + "_float", d[k]);
			i++;
		}
	}
	
	public static void LoadFromPlayerPrefs(this Dictionary<string, float> d, string name) {
		//d.Clear();
		if (!PlayerPrefs.HasKey(name + "_count")) { Debug.Log("Dictionary " + name + " does not exist in PlayerPrefs"); return; }
		int count = PlayerPrefs.GetInt(name + "_count");
		
		for (int i = 0; i < count; i++) {
			string key = PlayerPrefs.GetString(name + "_" + i + "_key");
			float val = PlayerPrefs.GetFloat(name + "_" + i + "_float");
			if (d.ContainsKey(key)) { d[key] = val; }
			else { d.Add(key, val); }
		}
	}
	
	public static string ToXML(this Dictionary<string, float> d) {
		string s = "<Dictionary Count=" + d.Count + ">";
		foreach (string k in d.Keys) { s += "\n<Entry key=" + k + " value=" +d[k] + "> </Entry>"; }
		s += "\n</Dictionary>";
		return s;
	}
	
	
	
	public static void Save(this Table d, string name) { d.SaveToPlayerPrefs(name); }
	public static void Load(this Table d, string name) {
		if (!PlayerPrefs.HasKey(name + "_count")) { Debug.Log("Dictionary " + name + " does not exist in PlayerPrefs"); return; }
		int count = PlayerPrefs.GetInt(name + "_count");
		
		for (int i = 0; i < count; i++) {
			string key = PlayerPrefs.GetString(name + "_" + i + "_key");
			float val = PlayerPrefs.GetFloat(name + "_" + i + "_float");
			d[key] = val;
		}
	}
	
}




















