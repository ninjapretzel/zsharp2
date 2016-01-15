using UnityEngine;
using System.Collections;
using System.Collections.Generic;


///<summary> Growth Curve for RPG-type growth mechanics. </summary>
[System.Serializable]
public class Curve {
	///<summary> Base value of curve. </summary>
	public float baseValue = 100;
	///<summary> List of terms to crunch for the curve.</summary>
	public List<Term> entries;
	
	[System.Serializable]
	public class Term {
		///<summary> Curve Term Type enumeration </summary>
		public enum Type { Poly, Exp, Log }
		///<summary> Type of Curve Term </summary>
		public Type type = Type.Poly;

		///<summary> Primary growth value (directly connected to input) </summary>
		public float primary = 1;
		///<summary> Secondary growth value (applied based on the Term Type) </summary>
		public float secondary = 1;

		///<summary> Default Constructor </summary>
		public Term() {
			type = Type.Poly;
			primary = 1;
			secondary = 1;
		}

		///<summary> Evaluate this term with a given input value. </summary>
		public float Eval(float value) {
			if (type == Type.Poly) {
				return Mathf.Pow(value * primary, secondary);
			} else if (type == Type.Exp) {
				return secondary * Mathf.Pow(value, primary);
			} else if (type == Type.Log) {
				return  Mathf.Log(secondary * value, primary);
			}
			
			return value;	
			
		}
		
	}

	///<summary> Evaluate the curve with a given input value. </summary>
	public float Eval(float value) {
		float f = baseValue;
		foreach (Term t in entries) {
			f += t.Eval(value);
		}
		return f;
	}
	
	
	
}
