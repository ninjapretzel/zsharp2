using UnityEngine;
using System.Collections;

///Animates a fade on a LineRenderer by changing the color properties of that LineRenderer
///
///Make sure that objects this script is attached to have a parent that will destroy itself after a time
///Or have another component that will remove the object after a delay.
public class LineRendererFader : MonoBehaviour {
	public Color startColor;
	public Color endColor;
	
	public float time;
	public float timeout;
	
	private LineRenderer lineRenderer;
	
	
	void Start() {
		lineRenderer = GetComponent<LineRenderer>();
		if (lineRenderer == null) { Destroy(this); }
	}
	
	void Update() {
		timeout += Time.deltaTime;
		if (timeout >= time) { Destroy(gameObject); }
		
		float alpha = 1 - (timeout/time);
		Color c1 = startColor;
		Color c2 = endColor;
		
		c1.a = alpha;
		c2.a = alpha;
		
		lineRenderer.startColor = c1;
		lineRenderer.endColor = c2;
		
	}
	
	
	
}
