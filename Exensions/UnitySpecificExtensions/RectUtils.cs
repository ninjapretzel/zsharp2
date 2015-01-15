using UnityEngine;
using System.Collections;

public static class RectUtils {
	public static Rect unit { get { return new Rect(0, 0, 1, 1); } }
	
	public static Vector2 Coord(this Rect r, Vector2 v) {
		Vector2 d = v - r.TopLeft();
		d.x /= r.width;
		d.y /= r.height;
		return d;
	}
	
	public static Rect ToScreenArea(this Transform t, float w, float h) {
		Vector3 pos = Camera.main.WorldToScreenPoint(t.position);
		pos.y = Screen.height - pos.y;
		float width = w * Screen.width;
		float height = h * Screen.height;
		return new Rect(pos.x - .5f * width, pos.y - .5f * height, width, height);
	}
	
	public static bool Contains(this Rect r, Touch t) { return r.Contains(t.ScreenPosition()); }
	
	public static Rect Scaled(this Rect r, Vector2 s) { return r.Scaled(s.x, s.y); }
	public static Rect Scaled(this Rect r, float x, float y) { return new Rect(r.x * x, r.y * y, r.width * x, r.height * y); }
	public static Rect Denormalized(this Rect r) { return r.Scaled(Screen.width, Screen.height); }
	public static Rect Normalized(this Rect r) { return r.Scaled(1f/Screen.width, 1f/Screen.height); }
	
	public static Rect Rot90Scaled(this Rect r) { return r.Scaled(Screen.rot90size); }
	public static Rect Rot90(this Rect r) { return new Rect(r.y, r.x, r.height, r.width); }
	public static Rect FlippedHorizontal(this Rect r) { return new Rect(Screen.width - r.xMax, r.y, r.width, r.height); }
	public static Rect FlippedVertical(this Rect r) { return new Rect(r.x, Screen.height - r.yMax, r.width, r.height); }
	public static Rect FlippedDiagonal(this Rect r) { return r.FlippedVertical().FlippedHorizontal(); }
	public static Rect FromCenter(Vector2 center, Vector2 size) { return FromCenter(center, size.x, size.y); }
	public static Rect FromCenter(Vector2 center, float s) { return FromCenter(center, s, s); }
	public static Rect FromCenter(Vector2 center, float x, float y) { return new Rect(center.x - x / 2f, center.y - y / 2f, x, y); }
	
	public static Rect ScreenOverscan(Rect normalized, float overscan) {
		float ratio = Mathf.Clamp(overscan, 0, .05f);
		
		Rect overScreen = new Rect(0, 0, Screen.width, Screen.height);
		overScreen = overScreen.Trim(ratio * Screen.width, ratio * Screen.height);
		
		return new Rect(overScreen.x + overScreen.width * normalized.x, 
						overScreen.y + overScreen.height * normalized.y, 
						overScreen.width * normalized.width,
						overScreen.height * normalized.height);
	}
	
	//Gets a rect as it being shifted by pixels
	public static Rect Shift(this Rect r, Vector2 v) { return r.Shift(v.x, v.y); }
	public static Rect Shift(this Rect r, float x, float y) {
		return new Rect(r.x + x,
						r.y + y,
						r.width,
						r.height);
	}
	
	//Gets a rect as it being shifted by a percent of its width/height
	public static Rect Move(this Rect r, Vector2 v) { return r.Move(v.x, v.y); }
	public static Rect Move(this Rect r, float x, float y) {
		return new Rect(r.x + x * r.width, 
						r.y + y * r.height, 
						r.width, 
						r.height);
	}

	/*
	public static Slider Slide(this Rect r, Cardinal direction) {
		Slider s = new Slider(r);
		s.Slide(direction);
		return s;
	}
	//*/
	
	
	public static Rect ShrinkTo(this Rect r, Rect t) {
		if (r.width < t.width && r.height < t.height) { return r; }
		float hRatio = t.height / r.height;
		float wRatio = t.width / r.width;
		if (hRatio < wRatio) {
			return FromCenter(r.MiddleCenter(), hRatio * r.width, hRatio * r.height);
		} else {
			return FromCenter(r.MiddleCenter(), wRatio * r.width, wRatio * r.height);
		}
	}
	
	///Get a rectangle's aspect ration
	public static float Aspect(this Rect r) { return r.width / r.height; }
	///Resize a rect's width based on its existant height
	public static Rect Aspect(this Rect r, float ratio) { return FromCenter(r.center, r.height * ratio, r.height); }
	
	public static Rect MoveLeft(this Rect r) { return r.MoveLeft(1); }
	public static Rect MoveLeft(this Rect r, float val) { return r.Move(-val, 0); }
	
	public static Rect MoveRight(this Rect r) { return r.MoveRight(1); }
	public static Rect MoveRight(this Rect r, float val) { return r.Move(val, 0); }
	
	public static Rect MoveUp(this Rect r) { return r.MoveUp(1); }
	public static Rect MoveUp(this Rect r, float val) { return r.Move(0, -val); }
	
	public static Rect MoveDown(this Rect r) { return r.MoveDown(1); }
	public static Rect MoveDown(this Rect r, float val) { return r.Move(0, val); }
	
	public static Rect Pad(this Rect r, float p) { return r.Pad(p, p); }
	public static Rect Pad(this Rect r, Vector2 v) { return r.Pad(v.x, v.y); }
	public static Rect Pad(this Rect r, float x, float y) {
		return new Rect(r.x - x, r.y - y, r.width + 2 * x, r.height + 2 * y);
	}
	
	public static Rect Trim(this Rect r, float p) { return r.Trim(p, p); }
	public static Rect Trim(this Rect r, Vector2 v) { return r.Trim(v.x, v.y); }
	public static Rect Trim(this Rect r, float x, float y) {
		return new Rect(r.x +x, r.y + y, r.width - 2 * x, r.height - 2 * y);
	}
	
	public static Vector2 Point(this Rect r, Vector2 v) { return r.Point(v.x, v.y); }
	public static Vector2 Point(this Rect r, float x, float y) { return new Vector2(r.x + r.width * x, r.y + r.height * y); }
	
	public static Vector2 Size(this Rect r, Vector2 v) { return r.Size(v.x, v.y); }
	public static Vector2 Size(this Rect r, float x, float y) { return new Vector2(r.width * x, r.height * y); }
	
	public static Rect Craft(Vector2 pos, Vector2 size) { return new Rect(pos.x, pos.y, size.x, size.y); }
	public static Rect Make(Vector2 pos, Vector2 size) { return new Rect(pos.x, pos.y, size.x, size.y); }
	
	public static Rect Top(this Rect r, float p) { return r.UpperLeft(1, p); }
	public static Rect Upper(this Rect r, float p) { return r.UpperLeft(1, p); }
	public static Rect Middle(this Rect r, float p) { return r.MiddleLeft(1, p); }
	public static Rect Bottom(this Rect r, float p) { return r.BottomLeft(1, p); }
	public static Rect Lower(this Rect r, float p) { return r.BottomLeft(1, p); }
	
	public static Rect Left(this Rect r, float p) { return r.UpperLeft(p, 1); }
	public static Rect Center(this Rect r, float p) { return r.UpperCenter(p, 1); }
	public static Rect Right(this Rect r, float p) { return r.UpperRight(p, 1); }

	public static Rect Left(this Rect r, float offset, float p) {
		return new Rect(r.x + r.width * offset, r.y, r.width * p, r.height);
	}
	public static Rect Top(this Rect r, float offset, float p) {
		return new Rect(r.x, r.y + r.height * offset, r.width, r.height * p);
	}
	public static Rect Right(this Rect r, float offset, float p) {
		return new Rect(r.x + r.width - (r.width * offset) - (r.width * p), r.y, (r.width * p), r.height);
	}
	public static Rect Bottom(this Rect r, float offset, float p) {
		return new Rect(r.x, r.y + r.height - (r.height * offset) - (r.height * p), r.width, (r.height * p));
	}
	
	public static Vector2 TopLeft(this Rect r) { return r.UpperLeft(); }
	public static Rect TopLeft(this Rect r, float f) { return r.UpperLeft(f, f); }
	public static Rect TopLeft(this Rect r, Vector2 v) { return r.UpperLeft(v); }
	public static Rect TopLeft(this Rect r, float width, float height) { return r.UpperLeft(width, height); }
	
	public static Vector2 UpperLeft(this Rect r) { return r.Point(0, 0); }
	public static Rect UpperLeft(this Rect r, float f) { return r.UpperLeft(f, f); }
	public static Rect UpperLeft(this Rect r, Vector2 v) { return r.UpperLeft(v.x, v.y); }
	public static Rect UpperLeft(this Rect r, float width, float height) { 
		return Craft(r.Point(0, 0), r.Size(width,height)); 
	}
	
	public static Vector2 TopCenter(this Rect r) { return r.UpperCenter(); }
	public static Rect TopCenter(this Rect r, float f) { return r.UpperCenter(f, f); }
	public static Rect TopCenter(this Rect r, Vector2 v) { return r.UpperCenter(v); }
	public static Rect TopCenter(this Rect r, float width, float height) { return r.UpperCenter(width, height); }
	
	public static Vector2 UpperCenter(this Rect r) { return r.Point(.5f, 0); }
	public static Rect UpperCenter(this Rect r, float f) { return r.UpperCenter(f, f); }
	public static Rect UpperCenter(this Rect r, Vector2 v) { return r.UpperCenter(v.x, v.y); }
	public static Rect UpperCenter(this Rect r, float width, float height) {
		return new Rect(r.x + r.width / 2.0f - r.width * width / 2.0f,
						r.y,
						r.width * width,
						r.height * height);
	}
	
	public static Vector2 TopRight(this Rect r) { return r.UpperRight(); }
	public static Rect TopRight(this Rect r, float f) { return r.UpperRight(f, f); }
	public static Rect TopRight(this Rect r, Vector2 v) { return r.UpperRight(v); }
	public static Rect TopRight(this Rect r, float width, float height) { return r.UpperRight(width, height); }
	
	public static Vector2 UpperRight(this Rect r) { return r.Point(1, 0); }
	public static Rect UpperRight(this Rect r, float f) { return r.UpperRight(f, f); }
	public static Rect UpperRight(this Rect r, Vector2 v) { return r.UpperRight(v.x, v.y); }
	public static Rect UpperRight(this Rect r, float width, float height) {
		return new Rect(r.x + r.width - r.width * width,
						r.y,
						r.width * width,
						r.height * height);
	}
	
	public static Vector2 MiddleLeft(this Rect r) { return r.Point(0, .5f); }
	public static Rect MiddleLeft(this Rect r, float f) { return r.MiddleLeft(f, f); }
	public static Rect MiddleLeft(this Rect r, Vector2 v) { return r.MiddleLeft(v.x, v.y); }
	public static Rect MiddleLeft(this Rect r, float width, float height) {
		return new Rect(r.x, 
						r.y + r.height / 2.0f - r.height * height / 2.0f, 
						r.width * width, 
						r.height * height);
	}
	
	public static Vector2 MiddleCenter(this Rect r) { return r.Point(.5f, .5f); }
	public static Rect MiddleCenter(this Rect r, float f) { return r.MiddleCenter(f, f); }
	public static Rect MiddleCenter(this Rect r, Vector2 v) { return r.MiddleCenter(v.x, v.y); }
	public static Rect MiddleCenter(this Rect r, float width, float height) {
		return new Rect(r.x + r.width / 2.0f - r.width * width / 2.0f,
						r.y + r.height / 2.0f - r.height * height / 2.0f, 
						r.width * width,
						r.height * height);
	}

	public static Vector2 MiddleRight(this Rect r) { return r.Point(1, .5f); }
	public static Rect MiddleRight(this Rect r, float f) { return r.MiddleRight(f, f); }
	public static Rect MiddleRight(this Rect r, Vector2 v) { return r.MiddleRight(v.x, v.y); }
	public static Rect MiddleRight(this Rect r, float width, float height) {
		return new Rect(r.x + r.width - r.width * width,
						r.y + r.height/2.0f - r.height * height / 2.0f, 
						r.width * width,
						r.height * height);
	}
	

	
	public static Vector2 LowerLeft(this Rect r) { return r.BottomLeft(); }
	public static Rect LowerLeft(this Rect r, float f) { return r.BottomLeft(f, f); }
	public static Rect LowerLeft(this Rect r, Vector2 v) { return r.BottomLeft(v); }
	public static Rect LowerLeft(this Rect r, float width, float height) { return r.BottomLeft(width, height); }
	
	public static Vector2 BottomLeft(this Rect r) { return r.Point(0, 1); }
	public static Rect BottomLeft(this Rect r, float f) { return r.BottomLeft(f, f); }
	public static Rect BottomLeft(this Rect r, Vector2 v) { return r.BottomLeft(v.x, v.y); }
	public static Rect BottomLeft(this Rect r, float width, float height) {
		return new Rect(r.x, 
						r.y + r.height - r.height * height, 
						r.width * width, 
						r.height * height);
	}
	
	
	public static Vector2 LowerCenter(this Rect r) { return r.BottomCenter(); }
	public static Rect LowerCenter(this Rect r, float f) { return r.LowerCenter(f, f); }
	public static Rect LowerCenter(this Rect r, Vector2 v) { return r.BottomCenter(v); }
	public static Rect LowerCenter(this Rect r, float width, float height) { return r.BottomCenter(width, height); }
	
	public static Vector2 BottomCenter(this Rect r) { return r.Point(.5f, 1); }
	public static Rect BottomCenter(this Rect r, float f) { return r.BottomCenter(f, f); }
	public static Rect BottomCenter(this Rect r, Vector2 v) { return r.BottomCenter(v.x, v.y); }
	public static Rect BottomCenter(this Rect r, float width, float height) {
		return new Rect(r.x + r.width / 2.0f - r.width * width / 2.0f,
						r.y + r.height - r.height * height,  
						r.width * width,
						r.height * height);
	}
	
	public static Vector2 LowerRight(this Rect r) { return r.BottomRight(); }
	public static Rect LowerRight(this Rect r, float f) { return r.BottomRight(f, f); }
	public static Rect LowerRight(this Rect r, Vector2 v) { return r.BottomRight(v); }
	public static Rect LowerRight(this Rect r, float width, float height) { return r.BottomRight(width, height); }
	
	public static Vector2 BottomRight(this Rect r) { return r.Point(1, 1); }
	public static Rect BottomRight(this Rect r, float f) { return r.BottomRight(f, f); }
	public static Rect BottomRight(this Rect r, Vector2 v) { return r.BottomRight(v.x, v.y); }
	public static Rect BottomRight(this Rect r, float width, float height) {
		return new Rect(r.x + r.width - r.width * width,
						r.y + r.height - r.height * height, 
						r.width * width,
						r.height * height);
	}
	
	public static Rect UpperLeftSquare(this Rect r, float side, bool sideIsFractionOfWidth = false) {
		if(sideIsFractionOfWidth) {
			return new Rect(r.x, r.y, r.width * side, r.width * side);
		} else {
			return new Rect(r.x, r.y, r.height * side, r.height * side);
		}
	}
	
	public static Rect UpperCenterSquare(this Rect r, float side, bool sideIsFractionOfWidth = false) {
		if(sideIsFractionOfWidth) {
			return new Rect(r.x + r.width / 2.0f - r.width * side / 2.0f, r.y, r.width * side, r.width * side);
		} else {
			return new Rect(r.x + r.width / 2.0f - r.height * side / 2.0f, r.y, r.height * side, r.height * side);
		}
	}
	
	public static Rect UpperRightSquare(this Rect r, float side, bool sideIsFractionOfWidth = false) {
		if(sideIsFractionOfWidth) {
			return new Rect(r.x + r.width - r.width * side, r.y, r.width * side, r.width * side);
		} else {
			return new Rect(r.x + r.width - r.height * side, r.y, r.height * side, r.height * side);
		}
	}
	
	public static Rect MiddleLeftSquare(this Rect r, float side, bool sideIsFractionOfWidth = false) {
		if(sideIsFractionOfWidth) {
			return new Rect(r.x, r.y + r.height/2.0f - r.width*side/2.0f, r.width * side, r.width * side);
		} else {
			return new Rect(r.x, r.y + r.height/2.0f - r.height*side/2.0f, r.height * side, r.height * side);
		}
	}
	
	public static Rect MiddleCenterSquare(this Rect r, float side, bool sideIsFractionOfWidth = false) {
		if(sideIsFractionOfWidth) {
			return new Rect(r.x + r.width / 2.0f - r.width * side / 2.0f, r.y + r.height/2.0f - r.width*side/2.0f, r.width * side, r.width * side);
		} else {
			return new Rect(r.x + r.width / 2.0f - r.height * side / 2.0f, r.y + r.height/2.0f - r.height*side/2.0f, r.height * side, r.height * side);
		}
	}
	
	public static Rect MiddleRightSquare(this Rect r, float side, bool sideIsFractionOfWidth = false) {
		if(sideIsFractionOfWidth) {
			return new Rect(r.x + r.width - r.width * side, r.y + r.height/2.0f - r.width*side/2.0f, r.width * side, r.width * side);
		} else {
			return new Rect(r.x + r.width - r.height * side, r.y + r.height/2.0f - r.height*side/2.0f, r.height * side, r.height * side);
		}
	}
	
	public static Rect BottomLeftSquare(this Rect r, float side, bool sideIsFractionOfWidth = false) {
		if(sideIsFractionOfWidth) {
			return new Rect(r.x, r.y + r.height - r.width*side, r.width * side, r.width * side);
		} else {
			return new Rect(r.x, r.y + r.height - r.height*side, r.height * side, r.height * side);
		}
	}
	
	public static Rect BottomCenterSquare(this Rect r, float side, bool sideIsFractionOfWidth = false) {
		if(sideIsFractionOfWidth) {
			return new Rect(r.x + r.width / 2.0f - r.width * side / 2.0f, r.y + r.height - r.width*side, r.width * side, r.width * side);
		} else {
			return new Rect(r.x + r.width / 2.0f - r.height * side / 2.0f, r.y + r.height - r.height*side, r.height * side, r.height * side);
		}
	}
	
	public static Rect BottomRightSquare(this Rect r, float side, bool sideIsFractionOfWidth = false) {
		if(sideIsFractionOfWidth) {
			return new Rect(r.x + r.width - r.width * side, r.y + r.height - r.width*side, r.width * side, r.width * side);
		} else {
			return new Rect(r.x + r.width - r.height * side, r.y + r.height - r.height*side, r.height * side, r.height * side);
		}
	}
	
}