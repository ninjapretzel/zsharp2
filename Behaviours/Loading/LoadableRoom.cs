using UnityEngine;
using System.Collections;
using System.Collections.Generic;


///Attach this to a room which can by loaded.
///It will automatically try to grab all exits in its children.
///Will also try to grab and store all of the exits by tag.
///Since most rooms won't have too many exits, it should never be a huge issue when loading.
///It can also be specified if it initializes on awake or start.
public class LoadableRoom : MonoBehaviour {
	
	public LoadTrigger[] exits;
	public Dictionary<string, LoadTrigger> taggedExits;
	
	void Awake() {
		if (exits.Length == 0) { exits = GetComponentsInChildren<LoadTrigger>(); }
		taggedExits = new Dictionary<string, LoadTrigger>();
		foreach (LoadTrigger trigger in exits) {
			if (trigger.handle != "") {
				taggedExits[trigger.handle] = trigger;
			}
		}
		
		
	}
	
}