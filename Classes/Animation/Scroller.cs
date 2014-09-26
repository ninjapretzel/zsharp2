using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scroller {
	
	public Vector2 position;
	public Vector2 velocity;
	public float dampening = 2.3f;
	
	public Scroller() {}
	
	public static implicit operator Vector2(Scroller sc) {
		return sc.position;
	}
	
	
	public void Update() {
		position += velocity * Time.deltaTime;
		velocity = Vector2.Lerp(velocity, Vector2.zero, Time.deltaTime * dampening);
	}
	
}
