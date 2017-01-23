using UnityEngine;
using System.Collections;

///Animates a fade on a LineRenderer by changing the color properties of that LineRenderer
///
///Make sure that objects this script is attached to have a parent that will destroy itself after a time
///Or have another component that will remove the object after a delay.
public class LineRendererFader : MonoBehaviour {
	public Color startColor = Color.red;
	public Color endColor = Color.white;
	public float endLength = 1.0f;
	
	public float time = 3.0f;
	private float timeout = 0.0f;
	
	private LineRenderer lineRenderer;

	private Vector3[] initialPositions = new Vector3[10];
	private int numPos = 0;
	void Start() {
		lineRenderer = GetComponent<LineRenderer>();
		if (lineRenderer == null) { Destroy(this); }
		numPos = lineRenderer.GetPositions(initialPositions);
	}
	
	void Update() {
		timeout += Time.deltaTime;
		if (timeout >= time) { Destroy(gameObject); }
		
		float len = Mathf.Lerp(1.0f, endLength, (timeout / time));
		for (int i = 0; i < numPos; i++) {
			lineRenderer.SetPosition(i, len * initialPositions[i]);
		}
		
		float alpha = 1 - (timeout/time);
		Color c1 = startColor;
		Color c2 = endColor;
		
		c1.a = alpha;
		c2.a = alpha;
		
		lineRenderer.startColor = c1;
		lineRenderer.endColor = c2;
		
	}
	
	
	
}
