using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary> Translates back and forth between A and B.
///A and B shouldn't  be the same type, since that will cause ambiguity. </summary>
public class Translator<A, B>  : IEnumerable<KeyValuePair<A,B>> {
	
	/// <summary> 'A' to 'B' dictionary </summary>
	protected Dictionary<A, B> atb;
	/// <summary> 'B' to 'A' dictionary </summary>
	protected Dictionary<B, A> bta;

	/// <summary> Constructor </summary>
	public Translator() {
		atb = new Dictionary<A, B>();
		bta = new Dictionary<B, A>();
	}

	/// <summary> Quick accessor to the pairs in the Translator </summary>
	public Dictionary<A, B> pairs { get { return atb; } }

	/// <summary> Index into this translator with an A to get a B, or Add/Set an a to a given B </summary>
	public B this[A a] { get { return atb[a]; } set { Put(a, value); } }
	/// <summary> Index into this translator with a B to get an A, or Add/Set a b to a given A </summary>
	public A this[B b] { get { return bta[b]; } set { Put(value, b); } }

	/// <summary> For IEnumerable interface </summary>
	IEnumerator IEnumerable.GetEnumerator() { return atb.GetEnumerator(); }

	/// <summary> for IEnumerable(KeyValuePair(A, B)) </summary>
	public IEnumerator<KeyValuePair<A,B>> GetEnumerator() { return atb.GetEnumerator(); }

	/// <summary> Same as Indexer, Index this translator with an A to get a B </summary>
	public B Get(A a) { return atb[a]; }
	/// <summary> Same as Indexer, Index this translator with a B to get an A </summary>
	public A Get(B b) { return bta[b]; }

	/// <summary> Does this translator contain a given A </summary>
	public bool Contains(A a) { return atb.ContainsKey(a); }
	/// <summary> Does this translator contain a given B </summary>
	public bool Contains(B b) { return atb.ContainsValue(b); }

	/// <summary> Strictly tries to add a pair of values.
	///If either 'a' or 'b' already exists, the pair is not added. 
	///Returns true if the pair was added </summary>
	public bool Add(A a, B b) {
		if (Contains(a) || Contains(b)) {
			Debug.LogWarning("Translator.Add: Already contains one of the values. Pair was not added.");
			return false;
		}
		
		atb.Add(a, b);
		bta.Add(b, a);
		return true;
	}
	
	/// <summary> Add a pair, or replace whatever values are already mapped with a given pair. </summary>
	public void Put(A a, B b) {
		Remove(a);
		Remove(b);
		Add(a, b);
	}

	/// <summary> Get a string representation of this translator. </summary>
	public override string ToString() {
		StringBuilder s = new StringBuilder("");
		foreach (var pair in pairs) {
			s = s + "(" + pair.Key + ") <--|--> (" + pair.Value + ")\n";
		}
		return s;
	}

	/// <summary> Removes a pair by a given 'A' key </summary>
	public void Remove(A a) {
		if (Contains(a)) {
			B b = Get(a);
			atb.Remove(a);
			bta.Remove(b);
		}
	}

	/// <summary> Removes a pair by a given 'B' key </summary>
	public void Remove(B b) {
		if (Contains(b)) {
			A a = Get(b);
			atb.Remove(a);
			bta.Remove(b);
		}
	}
	
}
