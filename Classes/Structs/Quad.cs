using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

///Struct for 'quadruple precision' float, focusing more on exponent space than significants
///Intended for numbers in 'clicker' games that want to go really, really high.
///Uses a long for exponent, and double for 'significants'.
///
///Anything with an exponent in the range of a long is valid
///The number is 'normalized' to be in the range [1.0000, 10.0) after any operation
///
///
///Not fully implemented yet...

public struct quad {
	
	public long exponent;
	public double number;
	
	public quad(double num = 0.0d, long exp = 0L) {
		exponent = exp;
		number = num;
	}
	
	///Convert quad implicitly to a double value
	public static implicit operator double(quad q) {
		return q.number * Math.Pow(10, (double)q.exponent);
	}
	
	///Convert double values implicitly to quad values
	public static implicit operator quad(double d) {
		quad q = new quad(d);
		q.Normalize();
		return q;
	}
	
	public override string ToString() {
		return "" + number + " * 10 ^ " + exponent;
	}
	

	
	public void Normalize() {
		double log = Math.Log10(number);
		double change = log.Floor();
		double factor = Math.Pow(10, -change);
		exponent += (long)change;
		number *= factor;
		if (number < 1d) { 
			number *= 10f;
			exponent += 1;
		}
		
	}
	
}












