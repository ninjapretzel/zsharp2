using UnityEngine;
using System.Collections;
using System.Collections.Generic;


///This behaviour is to be attached to exit triggers in rooms.
///The target room and exit must be specified to allow the room to load.
///
///A handle can be specified to refer to the exit by name.
///If a blank handle is used, the exit must be referred to by number, 
///		which depends on placement in the heiararchy
///
public class LoadTrigger : MonoBehaviour {

	public string handle = "";
	public LoadableRoom obToLoad;
	public string targetHandle = "";
	public int exitNum;
	
	public LoadableRoom loadedObject;
	
	void OnTriggerEnter(Collider c) {
		LoadTriggerer check = c.GetComponent<LoadTriggerer>();
		if (check != null) {
			if (loadedObject == null) {
				loadedObject = Instantiate(obToLoad, Vector3.zero, Quaternion.identity) as LoadableRoom;
				LoadTrigger anchor;
				
				if (targetHandle != "" && loadedObject.taggedExits.ContainsKey(targetHandle)) {
					anchor = loadedObject.taggedExits[targetHandle];
				} else {
					anchor = loadedObject.exits[exitNum];
				}
				
				anchor.SnapParent(transform, true);
				
				anchor.loadedObject = this.GetComponentAbove<LoadableRoom>();
			}
			
			
		}
	}
	
	void OnTriggerExit(Collider c) {
		LoadTriggerer check = c.GetComponent<LoadTriggerer>();
		if (check != null) {
			if (loadedObject != null) {
				Vector3 facing = transform.forward;
				Vector3 direction = transform.DirectionTo(c.transform);
				float angle = Vector3.Angle(facing, direction);
				if (angle < 90) {
					Destroy(transform.parent.gameObject);
				}
				
			}
			
			
		}
	}
	
}
