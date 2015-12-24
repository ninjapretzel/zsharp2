using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class LegacySlider {
	public Rect baseArea;
	
	public Rect IN { get { return slideIn.Denormalized(); } }
	public Rect In { get { return slideIn.Denormalized(); } }
	public Rect ON { get { return slideIn.Denormalized(); } }
	public Rect On { get { return slideIn.Denormalized(); } }
	
	public Rect ON90 { get { return slideIn.Scaled(Screen.rot90size); } }
	public Rect On90 { get { return slideIn.Scaled(Screen.rot90size); } }
	public Rect IN90 { get { return slideIn.Scaled(Screen.rot90size); } }
	public Rect In90 { get { return slideIn.Scaled(Screen.rot90size); } }
	
	public Rect OUT { get { return slideOut.Denormalized(); } }
	public Rect Out { get { return slideOut.Denormalized(); } }
	public Rect OFF { get { return slideOut.Denormalized(); } }
	public Rect Off { get { return slideOut.Denormalized(); } }
	
	public Rect OUT90 { get { return slideOut.Scaled(Screen.rot90size); } }
	public Rect Out90 { get { return slideOut.Scaled(Screen.rot90size); } }
	public Rect OFF90 { get { return slideOut.Scaled(Screen.rot90size); } }
	public Rect Off90 { get { return slideOut.Scaled(Screen.rot90size); } }
	
	
	public Rect slideIn;
	public Rect slideOn { get { return slideIn; } }
	
	public Rect slideOut;
	public Rect slideOff { get { return slideOut; } }
	
	public float dampening = 5;
	public float thresh = .04f;
	
	public bool done { get { return slideChange.magnitude < thresh; } }
	public Vector2 slideChange = Vector2.zero;
	
	//Constructors
	public LegacySlider() { baseArea = RectUtils.unit; } // unit = (0, 0, 1, 1)
	public LegacySlider(Rect area) { baseArea = area; }
	//Normalized Factory
	public static LegacySlider Normalized(Rect area) {
		Rect a = new Rect(area);
		return new LegacySlider(a);
	}
	
	
	public static LegacySlider Up(Rect area) { return Up(area, 1); }
	public static LegacySlider Down(Rect area) { return Down(area, 1); }
	public static LegacySlider Left(Rect area) { return Left(area, 1); }
	public static LegacySlider Right(Rect area) { return Right(area, 1); }
	
	public static LegacySlider Up(Rect area, float power) { LegacySlider s = Normalized(area); s.SlideUp(power); return s; }
	public static LegacySlider Down(Rect area, float power) { LegacySlider s = Normalized(area); s.SlideDown(power); return s; }
	public static LegacySlider Left(Rect area, float power) { LegacySlider s = Normalized(area); s.SlideLeft(power); return s; }
	public static LegacySlider Right(Rect area, float power) { LegacySlider s = Normalized(area); s.LegacySlideright(power); return s; }
	
	public static LegacySlider Up(Rect area, float power, float dampening) { LegacySlider s = Normalized(area); s.SlideUp(power); s.dampening = dampening; return s; }
	public static LegacySlider Down(Rect area, float power, float dampening) { LegacySlider s = Normalized(area); s.SlideDown(power); s.dampening = dampening; return s; }
	public static LegacySlider Left(Rect area, float power, float dampening) { LegacySlider s = Normalized(area); s.SlideLeft(power); s.dampening = dampening; return s; }
	public static LegacySlider Right(Rect area, float power, float dampening) { LegacySlider s = Normalized(area); s.LegacySlideright(power); s.dampening = dampening; return s; }
	
	
	public void Update() { Update(Time.deltaTime); }
	public void FixedUpdate() { Update(.02f); }
	public void Update(float t) {
		float time = t;
		if (time > .1) { time = .1f; }
		slideIn.x += slideChange.x * time * dampening;
		slideIn.y += slideChange.y * time * dampening;
		slideOut.x += slideChange.x * time * dampening;
		slideOut.y += slideChange.y * time * dampening;
		
		slideChange -= slideChange * time * dampening;
	}
	
	public void Slide(Cardinal direction) { Slide(direction, 1); }
	public void Slide(Cardinal direction, float power) {
		if (direction == Cardinal.Up) { SlideUp(power); }
		else if (direction == Cardinal.Down) { SlideDown(power); }
		else if (direction == Cardinal.Left) { SlideLeft(power); }
		else if (direction == Cardinal.Right) { LegacySlideright(power); }
	}
	
	public void Slide(Vector2 v) { Slide(v.x, v.y); }
	public void Slide(float x, float y) {
		slideIn = baseArea.Move(-x, -y);
		slideOut = baseArea;
		slideChange = new Vector2(x * baseArea.width, y * baseArea.height);
	}
	
	public void SlideLeft(float f) { Slide(-f, 0f); }
	public void LegacySlideright(float f) { Slide(f, 0f); }
	public void SlideUp(float f) { Slide(0f, -f); }
	public void SlideDown(float f) { Slide(0f, f); }
	
	public void SlideLeft() { SlideLeft(1); }
	public void LegacySlideright() { LegacySlideright(1); }
	public void SlideUp() { SlideUp(1); }
	public void SlideDown() { SlideDown(1); }
	
	public void Finish() {
		slideIn.x += slideChange.x;
		slideIn.y += slideChange.y;
		slideOut.x += slideChange.x;
		slideOut.y += slideChange.y;
		slideChange = Vector2.zero;
	}
	
	
}
