using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Shelf : List<string> {
	
	public Shelf Clone() {
		Shelf d = new Shelf();
		d.Capacity = Count;
		foreach (string str in this) { d.Add(str); }
		return d;
	}
	
	public new void Add(string value) {
		if (!Contains(value)) { 
			List<string> goy = this;
			goy.Add(value);
		}
	}
	
	public static Shelf operator +(Shelf a, string b) {
		Shelf c = a.Clone();
		c.Add(b);
		return c;
	}
	
	public static Shelf operator +(Shelf a, List<string> b) {
		Shelf c = a.Clone();
		foreach (string str in b) { c.Add(str); }
		return c;
	}
	
	public static Shelf operator +(Shelf a, string[] b) { 
		Shelf c = a.Clone();
		foreach (string str in b) { c.Add(str); }
		return c;
	}
	
	
	public string ChooseOne() { return this[(int)(Count * Random.value * .9999f)]; }
	
	
}
