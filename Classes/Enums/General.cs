using UnityEngine;
using System.Collections;
using System;

/// <summary> Cardinal directions </summary>
public enum Cardinal { Up,Left,Down,Right }

/// <summary> Comparisons of numbers </summary>
public enum NumberCompare {
	LessThan,
	LessThanOrEqualTo,
	GreaterThan,
	GreaterThanOrEqualTo,
	EqualTo,
	NotEqualTo,
}

/// <summary> Update method hooks provided by Unity </summary>
public enum UpdateType {
	Update,
	LateUpdate,
	FixedUpdate
}

/// <summary> Randomness </summary>
public enum RandomType {
	Normal,
	Seeded,
	Perlin,
}

/// <summary> Collision callbacks provided by unity. </summary>
public enum CollisionAction {
	Enter,
	Stay,
	Exit,
}

/// <summary> Extensions for these Enums </summary>
public static class General {

	/// <summary> Convert a NumberCompare into a function that takes two numbers, compares them using that comparison, and returns the result of the comparison. </summary>
	public static Func<float, float, bool> Comparator(this NumberCompare c) {
		if (c == NumberCompare.LessThan) { return (a,b) => (a < b); }
		else if (c == NumberCompare.LessThanOrEqualTo) { return (a,b) => (a <= b); }
		else if (c == NumberCompare.GreaterThan) { return (a,b) => (a > b); }
		else if (c == NumberCompare.GreaterThanOrEqualTo) { return (a,b) => (a >= b); }
		else if (c == NumberCompare.EqualTo) { return (a,b) => (a == b); }
		else if (c == NumberCompare.NotEqualTo) { return (a,b) => (a != b); }
		return (a, b) => (a == b);
	}
	

	public static Cardinal Flipped(this Cardinal c) { return c.Flip(); }
	public static Cardinal Flip(this Cardinal c) { return (Cardinal)( ( (int)c + 2) % 4); }
	
	
	
}
