using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class RaycastHitExtensions {
	
	public static Vector3 TrueNormal(this RaycastHit rayhit) {
		Debug.LogWarning("RaycastHit.TrueNormal does not work properly yet");
		if (rayhit.collider != null) {
			MeshCollider mcollider = rayhit.collider as MeshCollider;
			if (mcollider != null) {
				Mesh mesh = mcollider.sharedMesh;
				int triangle = rayhit.triangleIndex * 3;
				int[] triangles = mesh.triangles;
				Vector3 v1 = mesh.vertices[triangles[triangle + 0]];
				Vector3 v2 = mesh.vertices[triangles[triangle + 1]];
				Vector3 v3 = mesh.vertices[triangles[triangle + 2]];
				Plane p = new Plane(v2, v1, v3);
				
				return p.normal;
			}
			
		}
		
		
		return rayhit.normal;
	}
	
	
}
