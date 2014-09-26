using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

///Map from (T, T) that 'swings both ways'.
///Works like a translator for two lists of items that are associated.
public class BiMap<T> {
	public List<T> listA;
	public List<T> listB;
	
	public int Count { get { return listA.Count; } } 
	
	public BiMap() {
		listA = new List<T>();
		listB = new List<T>();
	}
	
	public override string ToString() { return ToString('|'); }
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
	
	public void Add(T a, T b) {
		listA.Add(a);
		listB.Add(b);
	}
	
	public List<T> GetKeyList() {
		List<T> c = new List<T>();
		foreach (T t in listA) {
			if (!c.Contains(t)) {
				c.Add(t);
			}
		}
		return c;
	}
	
	public List<T> GetValueList() {
		List<T> c = new List<T>();
		foreach (T t in listB) {
			if (!c.Contains(t)) {
				c.Add(t);
			}
		}
		return c;
	}
	
	public T GetA(int i) { return listA[i]; }
	public T GetB(int i) { return listB[i]; }
	
	public List<T> GetAMatch(T key) {
		List<T> c = new List<T>();
		for (int i = 0; i < Count; i++) {
			if (listA[i].Equals(key)) { c.Add(listB[i]); }
		}
		return c;
	}
	
	public List<T> GetAMiss(T key) {
		List<T> c = new List<T>();
		for (int i = 0; i < Count; i++) {
			if (!listA[i].Equals(key)) { c.Add(listB[i]); }
		}
		return c;
	}
	
	public List<T> GetBMatch(T key) {
		List<T> c = new List<T>();
		for (int i = 0; i < Count; i++) {
			if (listB[i].Equals(key)) { c.Add(listA[i]); }
		}
		return c;
	}
	
	public List<T> GetBMiss(T key) {
		List<T> c = new List<T>();
		for (int i = 0; i < Count; i++) {
			if (!listB[i].Equals(key)) { c.Add(listA[i]); }
		}
		return c;
	}
	
	public void Clear() {
		listA.Clear();
		listB.Clear();
	}
	
	public BiMap<T> Flopped() {
		BiMap<T> f = new BiMap<T>();
		f.listA = listB;
		f.listB = listA;
		return f;
	}
	
	
}


public static class BiMapExtensions {
	public static void LoadCSV(this BiMap<string> d, string csv) { d.LoadCSV(csv, ','); }
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