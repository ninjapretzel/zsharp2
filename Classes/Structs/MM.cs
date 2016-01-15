using UnityEngine;
using System.Collections;

///<summary> Min-Max struct.
///Holds the endpoints of a range.
///Has properties that return a random value, or normally distributed random value.
///Used to simplify the process of randomization code. </summary>
[System.Serializable]
public struct MM {
	///<summary> Min value of randomization </summary>
	public float min;
	///<summary> Max value of randomization </summary>
	public float max;

	///<summary> Equal distribution </summary>
	public float value { get { return Random.Range(min, max); } }
	///<summary> Normal distribution </summary>
	public float normal { get { return Random.Normal(min, max); } }

	///<summary> normal constructor </summary>
	public MM(float a, float b) {
		min = Mathf.Min(a, b);
		max = Mathf.Max(a, b);
	}

	///<summary> string constructor </summary>
	public MM(string s) {
		string[] cells = s.Split(',');
		if (cells.Length != 2) {
			Debug.LogWarning("MM.Constructor: Trying to load malformed string into MM range struct.");
			min = 0; 
			max = 0;
		} else {
			float a = cells[0].ParseFloat();
			float b = cells[1].ParseFloat();
			min = Mathf.Min(a, b);
			max = Mathf.Max(a, b);
		}
	}

	///<summary> Make a string representation of this object </summary>
	public override string ToString() { return ToString(','); }
	public string ToString(char delim) { return "" + min + delim + max; }

	///<summary> Equals method </summary>
	public override bool Equals(object other) {
		if (other == null) { return false; }
		if (!(typeof(MM) == other.GetType())) { return false; }
		MM o = (MM) other;
		return min == o.min && max == o.max;
	}
	
	///<summary> Hashcode for object </summary>
	public override int GetHashCode() {
		int a = (int)(min*1000) + 31337;
		int b = (int)(max*10000) + 375717;
		return b ^ a;
	}

	///<summary> Load a comma separated string holding two number values into this object's fields </summary>
	public void LoadFromString(string str) {
		string[] cells = str.Split(',');
		if (cells.Length != 2) { 
			Debug.LogWarning("MM.LoadFromString: Trying to load malformed string into MM range struct.");
			return;
		}
		
		min = cells[0].ParseFloat();
		max = cells[1].ParseFloat();
	}
	
}
