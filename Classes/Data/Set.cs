using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary> Sets are like lists, but do not allow duplicates </summary>
[System.Serializable]
public class Set<T> : List<T> {

	/// <summary> Base constructor </summary>
	public Set() : base() {}

	/// <summary> Collection constructor </summary>
	public Set(IEnumerable<T> source) : this() {
		foreach(T element in source) { Add(element); }
	}

	/// <summary> Create a copy of the Set</summary>
	public Set<T> Clone() { return new Set<T>(this); }

	/// <summary> Overrides default behaviour, Add a single value, but not if there is a duplicate. </summary>
	public new void Add(T value) {
		if (!Contains(value)) { 
			List<T> goy = this;
			goy.Add(value);
		}
	}
	
	
	/////////////////////////////////////////////////////////////////////
	//Union
	/// <summary> Operator to add a single element. Does not modify the original collection. </summary>
	public static Set<T> operator +(Set<T> a, T b) {
		Set<T> c = a.Clone();
		c.Add(b);
		return c;
	}

	/// <summary> Operator to add all the elements in a collection. Does not modify the original collection. </summary>
	public static Set<T> operator +(Set<T> a, IEnumerable<T> b) {
		Set<T> c = a.Clone();
		foreach (T element in b) { c.Add(element); }
		return c;
	}

	/////////////////////////////////////////////////////////////////////
	//Set Subtraction
	/// <summary> Operator to remove one element from the set. Does not modify the original collection. </summary>
	public static Set<T> operator -(Set<T> a, T b) {
		Set<T> c = a.Clone();
		c.Remove(b);
		return c;
	}

	/// <summary> Operator to remove all elements of a collection from the set. Does not modify the original collection. </summary>
	public static Set<T> operator -(Set<T> a, IEnumerable<T> b) {
		Set<T> c = a.Clone();
		foreach (T element in b) { c.Remove(element); }
		return c;
	}
	
	/////////////////////////////////////////////////////////////////////
	//Intersect
	/// <summary> Operator to get the intersection of this Set and a given collection. Does not modify the original collection. </summary>
	public static Set<T> operator *(Set<T> a, IEnumerable<T> b) { return a - (new Set<T>(b) - a); }
	
	public T ChooseOne() { return this[(int)(Count * Random.value * .9999f)]; }
	
}
