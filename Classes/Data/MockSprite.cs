using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteInfo {
	public Texture2D texture { get; private set; }

	public Rect rect { get; private set; }
	public Rect nRect { get; private set; }

	public SpriteInfo() { 
		texture = null; 
		rect = new Rect(0, 0, 1, 1); 
		nRect = rect;
	}
		
	public SpriteInfo(Texture2D tex) { 
		texture = tex; 
		rect = new Rect(0, 0, tex.width, tex.height); 
		nRect = new Rect(0, 0, 1, 1);
	}

	public SpriteInfo(Texture2D tex, Rect sub) { 
		texture = tex; 
		rect = sub; 
		float w = tex.width; float h = tex.height;
		nRect = new Rect(sub.x / w, sub.y / h, sub.width / w, sub.height / h);
	}
	
}
