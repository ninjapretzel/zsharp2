using UnityEngine;
using System.Collections;

///Min-Max
///Holds the endpoints of a range
///Has properties that return a random value, or normally distributed random value.
///Used to simplify the process of randomization code
[System.Serializable]
public class MM {
	public float min;
	public float max;
	
	public float value { get { return Random.Range(min, max); } }
	public float normal { get { return Random.Normal(min, max); } }
	
	public MM() {
		min = 0;
		max = 1;
	}
	
	public MM(float a, float b) {
		min = Mathf.Min(a, b);
		max = Mathf.Max(a, b);
	}
	
	public MM(string s) {
		LoadFromString(s);
	}
	
	public override string ToString() { return ToString(','); }
	public string ToString(char delim) { return "" + min + delim + max; }
	
	public override bool Equals(System.Object other) {
		if (other == null) { return false; }
		if (!(GetType() == other.GetType())) { return false; }
		MM o = other as MM;
		return min == o.min && max == o.max;
	}
	
	public override int GetHashCode() {
		int a = (int)(min*1000) + 31337;
		int b = (int)(max*10000) + 375717;
		return b ^ a;
	}
	
	public void LoadFromString(string str) {
		string[] cells = str.Split(',');
		if (cells.Length != 2) { 
			Debug.LogWarning("Trying to load malformed string into MM range struct.");
			return;
		}
		
		min = cells[0].ParseFloat();
		max = cells[1].ParseFloat();
	}
	
}
