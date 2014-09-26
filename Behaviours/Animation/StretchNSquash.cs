using UnityEngine;
using System.Collections;

///Uses two osciallators to animate the scaling of a transform.
///Basically automates the 'stretch n squash' animation.
public class StretchNSquash : MonoBehaviour {
	public Oscillator horizontal = new Oscillator(0.3f, 1.5f, 1.0f);
	public Oscillator vertical = new Oscillator(0.3f, 1.5f, 2.0f);
	Vector3 baseScale;
	
	void Start() {
		baseScale = transform.localScale;
	}
	
	void Update() {
		horizontal.Update();
		vertical.Update();
		
		Vector3 scale = baseScale;
		
		scale.y *= vertical.value;
		scale.x *= horizontal.value;
		scale.z *= horizontal.value;
		
		transform.localScale = scale;
		
	}
	
}