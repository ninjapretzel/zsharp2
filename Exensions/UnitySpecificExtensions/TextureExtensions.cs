using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TextureExtensions {
	
	/// <summary> Creates a sprite using the full rect of a Texture2D </summary>
	/// <param name="tex">Texture to make a sprite from </param>
	/// <returns>A sprite containing the full texture</returns>
	public static Sprite ToSprite(this Texture2D tex) {
		Rect all = new Rect(0, 0, tex.width, tex.height);
		Vector2 pivot = new Vector2(tex.width/2f, 1);
		
		return Sprite.Create(tex, all, pivot);
	}
	
	
}
