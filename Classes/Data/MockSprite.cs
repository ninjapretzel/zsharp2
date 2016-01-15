using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary> Unity likes to choke up when sprites are created with really small sizes.
/// This class is there to create information for sprites without actually creating the sprites and causing Unity to choke. </summary>
public class SpriteInfo {

	/// <summary> Texture for the sprite </summary>
	public Texture2D texture { get; private set; }

	/// <summary> Pixel Rectangle of area of texture this sprite occupies. </summary>
	public Rect rect { get; private set; }
	/// <summary> Normalized Rectangle of area of texture this sprite occupies. </summary>
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
		float w = tex.width; 
		float h = tex.height;
		nRect = new Rect(sub.x / w, sub.y / h, sub.width / w, sub.height / h);
	}
	
}
