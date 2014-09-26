using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Set<T> : List<T> {
	
	public Set() {
		
	}
	
	public Set(List<T> source) {
		foreach(T element in source) { Add(element); }
	}
	
	public Set(T[] source) {
		foreach(T element in source) { Add(element); }
	}
	
	
	public Set<T> Clone() {
		Set<T> d = new Set<T>();
		d.Capacity = Count;
		foreach (T element in this) { d.Add(element); }
		return d;
	}
	
	public new void Add(T value) {
		if (!Contains(value)) { 
			List<T> goy = this;
			goy.Add(value);
		}
	}
	
	
	/////////////////////////////////////////////////////////////////////
	//Union
	public static Set<T> operator +(Set<T> a, T b) {
		Set<T> c = a.Clone();
		c.Add(b);
		return c;
	}
	
	public static Set<T> operator +(Set<T> a, List<T> b) {
		Set<T> c = a.Clone();
		foreach (T element in b) { c.Add(element); }
		return c;
	}
	
	public static Set<T> operator +(Set<T> a, T[] b) { 
		Set<T> c = a.Clone();
		foreach (T element in b) { c.Add(element); }
		return c;
	}
	/////////////////////////////////////////////////////////////////////
	//Set Subtraction
	public static Set<T> operator -(Set<T> a, T b) {
		Set<T> c = a.Clone();
		c.Remove(b);
		return c;
	}
	
	public static Set<T> operator -(Set<T> a, List<T> b) {
		Set<T> c = a.Clone();
		foreach (T element in b) { c.Remove(element); }
		return c;
	}
	
	public static Set<T> operator -(Set<T> a, T[] b) {
		Set<T> c = a.Clone();
		foreach (T element in b) { c.Remove(element); }
		return c;
	}
	
	/////////////////////////////////////////////////////////////////////
	//Intersect
	public static Set<T> operator *(Set<T> a, List<T> b) { return a - (new Set<T>(b) - a); }
	public static Set<T> operator *(Set<T> a, T[] b) { return a - (new Set<T>(b) - a); }
	
	public T ChooseOne() { return this[(int)(Count * Random.value * .9999f)]; }
	
}
