using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineSpawner : MonoBehaviour {
	public Transform obj;
	public Vector3 offset = Vector3.right;
	public int numInLine = 2;
	
	public float chance = 1;
	
	public bool parentSpawnedObjects = true;
	public bool spawnCenter = true;
	public bool localOffset = true;
	public bool onStart = false;
	public bool mirror = true;
	
	void Awake() {
		if (!onStart) {
			Spawn();
		}
		
	}
	
	void Start() {
		if (onStart) {
			Spawn();
			
		}
		
	}
	
	void Update() {
		
	}
	
	public void Spawn() {
		int numPoints = numInLine;
		Vector3 off = offset;
		if (localOffset) { off = transform.rotation * off; }
		
		Vector3 start = transform.position;
		Vector3 end = start + numInLine * off;
		
		if (mirror) { 
			numPoints *= 2; 
			start -= numInLine * off;
		}
		
		Vector3 pt = start;
		if (off.magnitude < .01f) { return; }
		for (int i = 0; i < numPoints; i++) {
			if (pt.DistanceTo(transform.position) < .01f) {
				if (spawnCenter) { SpawnAt(pt); }
				i--; 
				pt += off; 
				continue; 
			}
			
			SpawnAt(pt);
			pt += off;
			
		}
		
		Destroy(gameObject);
	}
	
	void SpawnAt(Vector3 pos) {
		if (Random.value > chance) { return; }
		
		Transform spawned = Instantiate(obj, pos, Quaternion.identity) as Transform;
		if (parentSpawnedObjects) {
			spawned.parent = transform.parent;
		}
	}
	
	
	
	
	void OnDrawGizmos() {
		Gizmos.color = new Color(1, .96f, 0, .4f);
		Gizmos.DrawSphere(transform.position, 1);
	}
		
	void OnDrawGizmosSelected() {
		Gizmos.color = new Color(1, .96f, 0, .4f);
		int numPoints = numInLine;
		Vector3 off = offset;
		if (localOffset) { off = transform.rotation * off; }
		
		Vector3 start = transform.position;
		Vector3 end = start + numInLine * off;
		
		if (mirror) { 
			numPoints *= 2; 
			start -= numInLine * off;
		}
		
		Gizmos.DrawLine(start, end);
		
		Vector3 pt = start;
		if (off.magnitude < .01) { return; }
		for (int i = 0; i < numPoints; i++) {
			if (pt.DistanceTo(transform.position) < .01) { i--; pt += off; continue; }
			Gizmos.DrawWireCube(pt, Vector3.one);
			pt += off;
			
		}
		
		
		
	}
	
	
	
	
}
