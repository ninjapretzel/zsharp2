using UnityEngine;
using System.Collections;

public static class Curves {
	
	public static float Eval(this AnimationCurve a, float f) { return a.Evaluate(f); } 
	public static Vector3 Eval(this AnimationCurve a, Vector3 p) {
		Vector3 v = Vector3.zero;
		v.x = a.Eval(p.x);
		v.y = a.Eval(p.y);
		v.z = a.Eval(p.z);
		return v;
	}
	
	public static AnimationCurve SineCurve() { return SineCurve(1, 1); }
	public static AnimationCurve SineCurve(float length, float magnitude) {
		AnimationCurve a = new AnimationCurve();
			a.preWrapMode = WrapMode.Loop;
			a.postWrapMode = WrapMode.Loop;
			a.AddKey(0				,	0);
			a.AddKey(.25f * length	,	-magnitude);
			a.AddKey(.5f * length	,	0);
			a.AddKey(.75f * length	,	magnitude);
			a.AddKey(length			,	0);
			return a;
	}
	
}

