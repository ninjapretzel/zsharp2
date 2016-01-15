using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary> An extension on List(string) that acts as a set, and has + operators </summary>
[System.Serializable]
public class Shelf : List<string> {
	
	/// <summary> Creates a copy of the given Shelf </summary>
	public Shelf Clone() {
		Shelf d = new Shelf();
		d.Capacity = Count;
		foreach (string str in this) { d.Add(str); }
		return d;
	}
	/// <summary> Overrides the Add() function for Shelf to prevent duplicates. </summary>
	public new void Add(string value) {
		if (!Contains(value)) { 
			List<string> goy = this;
			goy.Add(value);
		}
	}

	/// <summary> Operator to + one shelf with a string. Does not mutate the original object. </summary>
	public static Shelf operator +(Shelf a, string b) {
		Shelf c = a.Clone();
		c.Add(b);
		return c;
	}

	/// <summary> Operator to + a shelf with a collection of strings. Does not mutate the original object. </summary>
	public static Shelf operator +(Shelf a, IEnumerable<string> b) {
		Shelf c = a.Clone();
		foreach (string str in b) { c.Add(str); }
		return c;
	}

	/// <summary> Grab a random string from the shelf. </summary>
	public string ChooseOne() { return this[(int)(Count * Random.value)]; }
	
	
}
