using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary> Map from (T, T) that 'swings both ways'. Works like a translator for two lists of items that are associated.
/// Items do NOT need to be unique. This is intended to be a 'database' of sorts, and it is queried for Miss/Match.
/// This can map One-To-One, One-To-Many, Many-To-One, and Many-To-Many. </summary>
public class BiMap<T> {
	/// <summary> List of first set of elements </summary>
	public List<T> listA;

	/// <summary> List of second set of elements </summary>
	public List<T> listB;

	/// <summary> Total number of elements in this BiMap </summary>
	public int Count { get { return listA.Count; } }

	/// <summary> Constructor </summary>
	public BiMap() {
		listA = new List<T>();
		listB = new List<T>();
	}

	/// <summary> Gets a string representation of this BiMap </summary>
	public override string ToString() { return ToString('|'); }
	/// <summary> Gets a string representation of this BiMap with a given character between entries. </summary>
	public string ToString(char delim) {
		StringBuilder str = new StringBuilder();
		for (int i = 0; i < Count; i++) {
			str.Append(listA[i]);
			str.Append(delim);
			str.Append(listB[i]);
			str.Append("\n");
		}
		return str.ToString();
	}

	/// <summary> Add pair (a, b) to the mapping function. </summary>
	public void Add(T a, T b) {
		listA.Add(a);
		listB.Add(b);
	}
	
	/// <summary> Gets all of the 'a' values in the BiMap </summary>
	public List<T> GetKeyList() {
		List<T> c = new List<T>();
		foreach (T t in listA) {
			if (!c.Contains(t)) {
				c.Add(t);
			}
		}
		return c;
	}
	
	/// <summary> Gets all of the 'b' values in the BiMap </summary>
	public List<T> GetValueList() {
		List<T> c = new List<T>();
		foreach (T t in listB) {
			if (!c.Contains(t)) {
				c.Add(t);
			}
		}
		return c;
	}
	
	/// <summary> Get the 'T' at index 'i' in 'listA' </summary>
	public T GetA(int i) { return listA[i]; }
	/// <summary> Get the 'T' at index 'i' in 'listB' </summary>
	public T GetB(int i) { return listB[i]; }

	/// <summary> Get all of the 'listB' items that have a pair matching 'key' in 'listA' </summary>
	public List<T> GetAMatch(T key) {
		List<T> c = new List<T>();
		for (int i = 0; i < Count; i++) {
			if (listA[i].Equals(key)) { c.Add(listB[i]); }
		}
		return c;
	}

	/// <summary> Get all of the 'listB' items that have a pair not matching 'key' in 'listA' </summary>
	public List<T> GetAMiss(T key) {
		List<T> c = new List<T>();
		for (int i = 0; i < Count; i++) {
			if (!listA[i].Equals(key)) { c.Add(listB[i]); }
		}
		return c;
	}

	/// <summary> Get all of the 'listA' items that have a pair matching 'key' in 'listB' </summary>
	public List<T> GetBMatch(T key) {
		List<T> c = new List<T>();
		for (int i = 0; i < Count; i++) {
			if (listB[i].Equals(key)) { c.Add(listA[i]); }
		}
		return c;
	}

	/// <summary> Get all of the 'listA' items that have a pair not matching 'key' in 'listB' </summary>
	public List<T> GetBMiss(T key) {
		List<T> c = new List<T>();
		for (int i = 0; i < Count; i++) {
			if (!listB[i].Equals(key)) { c.Add(listA[i]); }
		}
		return c;
	}
	
	/// <summary> Clear all elements from the list</summary>
	public void Clear() {
		listA.Clear();
		listB.Clear();
	}
	
	/// <summary> Returns a BiMap that _REFERENCES_THE_SAME_LISTS_ but in the opposite direction (listB and listA are swapped). </summary>
	public BiMap<T> Flopped() {
		BiMap<T> f = new BiMap<T>();
		f.listA = listB;
		f.listB = listA;
		return f;
	}
	
	
}

/// <summary> Helper class for BiMaps</summary>
public static class BiMapExtensions {
	/// <summary> Loads a CSV string into a given BiMap, with a default delimeter of ',' </summary>
	/// <param name="d">Map to load into</param>
	/// <param name="csv">CSV string to load</param>
	public static void LoadCSV(this BiMap<string> d, string csv) { d.LoadCSV(csv, ','); }
	/// <summary> Loads a CSV string into a given BiMap, with a default delimeter of ',' </summary>
	/// <param name="d">Map to load into</param>
	/// <param name="csv">CSV string to load</param>
	/// <param name="delim">CSV delimeter</param>
	public static void LoadCSV(this BiMap<string> d, string csv, char delim) {
		d.Clear();
		
		csv = csv.ConvertNewlines();
		string[] lines = csv.Split('\n');
		
		for (int i = 0; i < lines.Length; i++) {
			if (lines[i].Length < 3) { continue; }
			if (lines[i][0] == '#') { continue; }
			string[] content = lines[i].Split(delim);
			for (int j = 0; j < content.Length; j += 2) {
				d.Add(content[j], content[j+1]);
			}
		}
	}
}
