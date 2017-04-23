using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawsGizmo : MonoBehaviour {
	public bool wireframe = false;
	public bool sphere = false;
	public bool matchTransformRotation = false;
	public bool matchTransformScale = false;
	public Vector3 size = Vector3.one;
	public Vector3 offset = Vector3.zero;
	
	public float sizeScale = 1;
	public Color color = new Color(1, 0, 0, .5f);
	public float alphaScale = .5f;

	
	void OnDrawGizmos() {
		Color c = color;
		c.a *= alphaScale;
		Gizmos.color = c;
		
		DrawGizmo();
	}
	
	void OnDrawGizmosSelected() {
		Gizmos.color = color;
		DrawGizmo();
	}
	
	void DrawGizmo() {
		Vector3 center = transform.position + offset;
		Vector3 size = this.size * sizeScale;
		
		if (matchTransformRotation) {
			Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
			center = Vector3.zero;
		} else {
			Gizmos.matrix = Matrix4x4.identity;
		}
		
		if (matchTransformScale) {
			size = Vector3.Scale(size, transform.lossyScale);
		}

		if (wireframe) {
			if (sphere) {
				Gizmos.DrawWireSphere(center, size.magnitude/2f);
			} else {
				Gizmos.DrawWireCube(center, size);
			}
		} else {
			if (sphere) {
				Gizmos.DrawSphere(center, size.magnitude/2f);
			} else {
				Gizmos.DrawCube(center, size);
			}
		}
	}
	
}
