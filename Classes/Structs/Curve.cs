using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Curve {
	public float baseValue = 100;
	public List<Term> entries;
	
	[System.Serializable]
	public class Term {
		public enum Type { Poly, Exp, Log }
		public Type type = Type.Poly;
		public float primary = 1;
		public float secondary = 1;
		
		public Term() {
			type = Type.Poly;
			primary = 1;
			secondary = 1;
		}
		
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
	
	
	public float Eval(float value) {
		float f = baseValue;
		foreach (Term t in entries) {
			f += t.Eval(value);
		}
		return f;
	}
	
	
	
}
