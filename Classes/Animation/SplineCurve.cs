using UnityEngine;
using System.Collections;

[System.Serializable]
public class SplineCurve {
	public Vector3[] points;
	
	public SplineCurve(Vector3[] ps) {
		points = ps;
		//Debug.Log("made a new spline");
		Smooth();
	}
	
	public void Smooth() {
		if (points.Length < 2) { return; }
		//Vector3 lastPoint = points[points.Length-1];
		
		while (points.Length < 7) { points = Smoothed(); }
		
		if (MaxCount4() < points.Length) { CutToMax(); }
		
		points = Uncusped();
	}
	
	public Vector3[] Uncusped() {
		Vector3[] ps = new Vector3[points.Length];
		
		for (int i = 0; i < points.Length; i++) { ps[i] = points[i]; }		
		for (int i = 3; i < points.Length-2; i+=3) {
			Vector3 prev = Vector3.Lerp(points[i-2], points[i-1], .5f);
			Vector3 next = Vector3.Lerp(points[i+2], points[i+1], .5f);
			ps[i] = Vector3.Lerp(prev, next, .5f);
			
		}
		
		return ps;
	}
	
	public Vector3[] Smoothed() {
		Vector3[] ps = new Vector3[points.Length * 2 - 1];
		
		int i;
		for (i = 0; i < points.Length; i++) { ps[i*2] = points[i]; }
		for (i = 1; i < ps.Length; i+=2) {
			ps[i] = Vector3.Lerp(ps[i-1], ps[i+1], .5f);
		}
		return ps;
	}
	
	public Vector3 Trace(float distance) { return Trace(distance, 100); }
	public Vector3 Trace(float distance, int segs) {
		float dd = Distance(segs);
		if (distance >= dd) { return points[points.Length-1]; }
		if (distance <= 0) { return points[0]; }
		
		float dist = 0;
		float last = 0;
		float diff = 1.0f / (0.0f+segs);
		for (int i = 0; i < segs; i++) {
			dist += (Eval(last) - Eval(last+diff)).magnitude;
			if (dist > distance) { return Eval(last); }
			last += diff;
			
		}
		return points[points.Length-1];
	}
	
	public float Distance() { return Distance(100); }
	public float Distance(int segs) {
		float dist = 0;
		float last = 0;
		float diff = 1.0f / (0.0f+segs);
		for (int i = 0; i < segs; i++) {
			dist += (Eval(last)-Eval(last+diff)).magnitude;
			last += diff;
		}
		return dist;
	}
	
	public Vector3 Eval(float f) {
		if (f <= 0) { return points[0]; }
		if (f >= 1) { return points[points.Length-1]; }
		return Eval4(Mathf.Clamp01(f));
	}
	
	public Vector3 Direction(float f) {
		if (f == 1) { return -Direction(.999f); }
		return (Eval(f+.001f) - Eval(f)).normalized;
	}
	
	public int MaxCount3() {
		if (points.Length < 3) { return -1; }
		return points.Length - (points.Length % 2);
	}
	
	public int MaxCount4() {
		if (points.Length < 7) { return -1; }
		return points.Length - ((points.Length-4)%3);
	}
	
	public void CutToMax() { CutToMax4(); }
	public void CutToMax4() {
		points[MaxCount4()-1] = points[points.Length-1];
		Vector3[] ps = new Vector3[MaxCount4()];
		for (int i = 0; i < ps.Length; i++) {
			ps[i] = points[i];
		}
		points = ps;
	}
	
	public int Segments3() { return (int)(MaxCount3() * .5f); }
	public int Segments4() { return (int)(MaxCount4() / 3.0f); }
	
	public Vector3 Eval4(float f) {
		int maxIndex = MaxCount4();
		if (maxIndex == -1) { return Vector3.zero; }
		
		int segments = Segments4();
		int start = (int)Mathf.Floor(f * segments) * 3;
		
		Vector3[] ps = new Vector3[4];
		for (int i = 0; i < 4; i++) { ps[i] = points[start+i]; }
		
		return Eval4(ps, Mathf.Repeat(f*segments, 1.0f));
	}
	
	
	public Vector3 Eval3(Vector3[] ps, float f) {
		Vector3 first = ps[0];
		Vector3 second = Vector3.Lerp(ps[1], ps[2], f);
		return Vector3.Lerp(first, second, f);
	}
	
	
	public Vector3 Eval4(Vector3[] ps, float f) {
		Vector3 first = Vector3.Lerp(ps[0], ps[1], f);
		Vector3 second = Vector3.Lerp(ps[2], ps[3], f);
		return Vector3.Lerp(first, second, f);
	}
	
	
	public void DrawGizmos() {
		if (points == null) { return; }
		Vector3 n = Vector3.zero;
		Vector3 p = Vector3.zero;
		
		for (int i = 1; i < points.Length; i++) {
			p = points[i-1];
			n = points[i];
			
			GizmoAlpha(.5f);
			Gizmos.DrawSphere(p, .5f);
			GizmoAlpha(2.0f);
			
			Gizmos.DrawLine(p, n);
		}
		
		GizmoAlpha(.5f);
		Gizmos.DrawSphere(n, .5f);
		GizmoAlpha(2.0f);
		
	}
	
	private void GizmoAlpha(float f) {
		Color c = Gizmos.color;
		c.a *= f;
		Gizmos.color = c;
	}
}
