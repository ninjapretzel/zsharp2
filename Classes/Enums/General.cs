using UnityEngine;
using System.Collections;
using System;


public enum Cardinal { Up,Left,Down,Right }

public enum NumberCompare {
	LessThan,
	LessThanOrEqualTo,
	GreaterThan,
	GreaterThanOrEqualTo,
	EqualTo,
	NotEqualTo,
}

public enum UpdateType {
	Update,
	LateUpdate,
	FixedUpdate
}

public enum RandomType {
	Normal,
	Seeded,
	Perlin,
}

public enum CollisionAction {
	Enter,
	Stay,
	Exit,
}

public static class General {
	
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
