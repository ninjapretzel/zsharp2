using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FancyBar : Bar {
	
	public static float timeScale = .1f;
	public static float sinTimeScale = 3f;
	
	public static float sinPowerA = .2f;
	public static float sinPowerB = .3f;
	
	public static float xOffsetScaleA = .1333f;
	public static float yOffsetScaleA = .215f;
	public static float xOffsetScaleB = 1.715f;
	public static float yOffsetScaleB = .415f;
	
	public static float repeatScaleA = .15f;
	public static float repeatScaleB = .225f;
	
	
	public static Texture2D fancyGraphicA;
	public static Texture2D fancyGraphicB;
	
	public Color fancyColorA = Color.clear;
	public Color fancyColorB = Color.clear;
	public int offset = 0;
	public float speed = 1;
	
	public FancyBar(Texture2D front, Texture2D back, Color mainColor) {
		frontGraphic = front;
		backGraphic = back;
		frontColor = mainColor;
		backColor = new Color(0, 0, 0, .5f);
		
	}
	
	public new void Draw(Rect area, float fill) {
		//Color oldBackColor = backColor;
		//Debug.Log("fancy draw");
		DrawNormal(area, fill);
		
		float time = Time.time * timeScale * speed;
		float sinTime = Mathf.Sin(time * sinTimeScale);
		//float pad = Screen.height * .002f;
		
		int i = offset * offset * offset;
		float aspect = area.Aspect();
		float x, y, w, h;
		x = i * xOffsetScaleA - time % 1;
		y = i * yOffsetScaleA + sinTime * sinPowerA;
		w = aspect * repeatScaleA;
		h = repeatScaleA;
		
		repeat = new Rect(x, y, w, h);
		if (fancyColorA != Color.clear) { GUI.color = fancyColorA; }
		else { SetDefaultFancyColor(); }
		//GUI.color = Color.white;
		GUI.DrawTextureWithTexCoords(area, fancyGraphicA, repeat);
		
		//Debug.Log(fancyColorA);
		x = i * xOffsetScaleB - time % 1;
		y = i * yOffsetScaleB + sinTime * sinPowerB;
		w = aspect * repeatScaleB;
		h = repeatScaleB;
		
		repeat = new Rect(x, y, w, h);
		if (fancyColorB != Color.clear) { GUI.color = fancyColorB; }
		else { SetDefaultFancyColor(); }
		//GUI.color = Color.white;
		GUI.DrawTextureWithTexCoords(area, fancyGraphicB, repeat);
		
		
		
	}
	
	void SetDefaultFancyColor() {
		Color c = frontColor;
		c.a *= .35f;
		GUI.color = c;
	}
	
}
