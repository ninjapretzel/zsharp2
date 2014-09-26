using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Translates back and forth between A and B
//A and B shouldn't  be the same type, since that will cause ambiguity.
public class Translator<A, B> {
	
	Dictionary<A, B> atb;
	Dictionary<B, A> bta;
	
	public Translator() {
		atb = new Dictionary<A, B>();
		bta = new Dictionary<B, A>();
		
	}
	
	public Dictionary<A, B> pairs { get { return atb; } }
	
	
	//Accessors
	//No this accessor, explicitness is necessary when using this class.
	public B Get(A a) { return atb[a]; }
	public A Get(B b) { return bta[b]; }
	
	public bool Contains(A a) { return atb.ContainsKey(a); }
	public bool Contains(B b) { return atb.ContainsValue(b); }
	
	//Strictly tries to add a pair of values.
	//If either param already exists, it is not added.
	public bool Add(A a, B b) {
		if (Contains(a) || Contains(b)) {
			Debug.Log("Translator already contains one of the values. Pair was not added.");
			return false;
		}
		
		atb.Add(a, b);
		bta.Add(b, a);
		return true;
	}
	
	//Tries to add or replace a pair of values
	public void Put(A a, B b) {
		if (Contains(a) || Contains(b)) {
			atb[a] = b;
			bta[b] = a;
		} else {
			Add(a, b);
		}
		
	}
	
	public override string ToString() {
		string s = "";
		foreach (A a in atb.Keys) {
			s += "(" + a + ") <--|--> (" + atb[a] + ")\n";
		}
		return s;
	}
	
	//Removes a given pair by one of the keys
	public void Remove(A a) {
		if (Contains(a)) {
			B b = Get(a);
			atb.Remove(a);
			bta.Remove(b);
		}
	}
	
	public void Remove(B b) {
		if (Contains(b)) {
			A a = Get(b);
			atb.Remove(a);
			bta.Remove(b);
		}
	}
	
	
	//
	
}
