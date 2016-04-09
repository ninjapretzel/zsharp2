using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//Nasty class kept around for the few times during testing when we'd want to use the default GUI.

public static class GUIUtils {
	public static Texture2D pixel { get { return Resources.Load<Texture2D>("pixel"); } }
	
}

public static class GUIStyleF {
	
	public static float LineSize(this GUIStyle c) { return c.CalcHeight(new GUIContent("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"), 9999999f); }
	public static GUIStyle Clone(this GUIStyle c) { return new GUIStyle(c); }
	
	public static GUIStyle FontSize(this GUIStyle style, float p) { 
		GUIStyle copy = new GUIStyle(style);
		int size = (int) (p/720f * Screen.height);
		if (size <= 1) {
			copy.fontSize = 1;
		} else {
			copy.fontSize = size;
		}
		return copy;
	}
	
	public static void SetFontSize(this GUIStyle style, float p, float minFontSize = 8f) {
		style.fontSize = (int) Mathf.Max(minFontSize, (p * (float)Screen.height));	
	}
	
	public static GUIStyle ScaledFontTo(this GUIStyle style, float p, float minFontSize = 8f) {
		GUIStyle copy = new GUIStyle(style);
		copy.fontSize = (int) Mathf.Max(minFontSize, (p * (float)Screen.height));
		return copy;
	}
	
	public static GUIStyle Aligned(this GUIStyle style, TextAnchor a) {
		GUIStyle copy = new GUIStyle(style);
		copy.alignment = a;
		return copy;
	}
	
	public static GUIStyle LeftAligned(this GUIStyle style) {
		GUIStyle copy = new GUIStyle(style);
		TextAnchor a = style.alignment;
		int b = (int)a / 3;
		copy.alignment = (TextAnchor)(b * 3);
		return copy;
	}
	
	public static GUIStyle CenterAligned(this GUIStyle style) {
		GUIStyle copy = new GUIStyle(style);
		TextAnchor a = style.alignment;
		int b = (int)a / 3;
		copy.alignment = (TextAnchor)(b * 3 + 1);
		return copy;
	}	
	
	public static GUIStyle RightAligned(this GUIStyle style) {
		GUIStyle copy = new GUIStyle(style);
		TextAnchor a = style.alignment;
		int b = (int)a / 3;
		copy.alignment = (TextAnchor)(b * 3 + 2);
		return copy;
	}
	
	public static GUIStyle UpperAligned(this GUIStyle style) {
		GUIStyle copy = new GUIStyle(style);
		TextAnchor a = style.alignment;
		int b = (int)a % 3;
		copy.alignment = (TextAnchor)b;
		return copy;
	}
	
	public static GUIStyle MiddleAligned(this GUIStyle style) {
		GUIStyle copy = new GUIStyle(style);
		TextAnchor a = style.alignment;
		int b = (int)a % 3;
		copy.alignment = (TextAnchor)3 + b;
		return copy;
	}
	
	public static GUIStyle LowerAligned(this GUIStyle style) {
		GUIStyle copy = new GUIStyle(style);
		TextAnchor a = style.alignment;
		int b = (int)a % 3;
		copy.alignment = (TextAnchor)6 + b;
		return copy;
	}
	
	
}

public static class GUISkinF {
	
	public static GUISkin Clone(this GUISkin source) {
		return Object.Instantiate(source) as GUISkin;
	}

	public static void FontSize(this GUISkin skin, float size, float minFontSize = 8f) { skin.SetFontSize(size/720.0f, minFontSize); }
	public static void FontSizeFull(this GUISkin skin, float size, float minFontSize = 8f) { skin.SetFontSizeFull(size/720.0f, minFontSize); }
	

	public static void SetFontSize(this GUISkin skin, float size, float minFontSize = 8f) {
		skin.label.SetFontSize(size, minFontSize);
		skin.button.SetFontSize(size, minFontSize);
		skin.box.SetFontSize(size, minFontSize);
	}
	
	public static void SetFontSizeFull(this GUISkin skin, float size, float minFontSize = 8f) {
		skin.label.SetFontSize(size, minFontSize);
		skin.button.SetFontSize(size, minFontSize);
		skin.box.SetFontSize(size, minFontSize);
		skin.textField.SetFontSize(size, minFontSize);
		skin.textArea.SetFontSize(size, minFontSize);
		skin.toggle.SetFontSize(size, minFontSize);
		skin.window.SetFontSize(size, minFontSize);
		skin.horizontalSlider.SetFontSize(size, minFontSize);
		skin.horizontalSliderThumb.SetFontSize(size, minFontSize);
		skin.verticalSlider.SetFontSize(size, minFontSize);
		skin.verticalSliderThumb.SetFontSize(size, minFontSize);
		skin.horizontalScrollbar.SetFontSize(size, minFontSize);
		skin.horizontalScrollbarThumb.SetFontSize(size, minFontSize);
		skin.horizontalScrollbarLeftButton.SetFontSize(size, minFontSize);
		skin.horizontalScrollbarRightButton.SetFontSize(size, minFontSize);
		skin.verticalScrollbar.SetFontSize(size, minFontSize);
		skin.verticalScrollbarThumb.SetFontSize(size, minFontSize);
		skin.verticalScrollbarUpButton.SetFontSize(size, minFontSize);
		skin.verticalScrollbarDownButton.SetFontSize(size, minFontSize);
		skin.scrollView.SetFontSize(size, minFontSize);
		if (skin.customStyles != null) {
			foreach (GUIStyle s in skin.customStyles) { s.SetFontSize(size, minFontSize); }
		}
	}
	
	public static void ScaleFontTo(this GUISkin skin, float size) {
		skin.label 							= skin.label.ScaledFontTo(size);
		skin.button 						= skin.button.ScaledFontTo(size);
		skin.box 							= skin.box.ScaledFontTo(size);
		skin.textField 						= skin.textField.ScaledFontTo(size);
		skin.textArea						= skin.textArea.ScaledFontTo(size);
		skin.toggle 						= skin.toggle.ScaledFontTo(size);
		skin.window 						= skin.window.ScaledFontTo(size);
		skin.horizontalSlider 				= skin.horizontalSlider.ScaledFontTo(size);
		skin.horizontalSliderThumb 			= skin.horizontalSliderThumb.ScaledFontTo(size);
		skin.verticalSlider 				= skin.verticalSlider.ScaledFontTo(size);
		skin.verticalSliderThumb 			= skin.verticalSliderThumb.ScaledFontTo(size);
		skin.horizontalScrollbar 			= skin.horizontalScrollbar.ScaledFontTo(size);
		skin.horizontalScrollbarThumb 		= skin.horizontalScrollbarThumb.ScaledFontTo(size);
		skin.horizontalScrollbarLeftButton 	= skin.horizontalScrollbarLeftButton.ScaledFontTo(size);
		skin.horizontalScrollbarRightButton = skin.horizontalScrollbarRightButton.ScaledFontTo(size);
		skin.verticalScrollbar 				= skin.verticalScrollbar.ScaledFontTo(size);
		skin.verticalScrollbarThumb 		= skin.verticalScrollbarThumb.ScaledFontTo(size);
		skin.verticalScrollbarUpButton 		= skin.verticalScrollbarUpButton.ScaledFontTo(size);
		skin.verticalScrollbarDownButton 	= skin.verticalScrollbarDownButton.ScaledFontTo(size);
		skin.scrollView 					= skin.scrollView.ScaledFontTo(size);
		
		if (skin.customStyles != null) {
			for (int i = 0; i < skin.customStyles.Length; i++) {
				skin.customStyles[i] = skin.customStyles[i].ScaledFontTo(size);
			}
		}
		
	}
	
	public static GUISettings Clone(this GUISettings sets) {
		GUISettings clone = new GUISettings();
		clone.doubleClickSelectsWord = sets.doubleClickSelectsWord;
		clone.tripleClickSelectsLine = sets.tripleClickSelectsLine;
		clone.cursorColor = sets.cursorColor;
		clone.cursorFlashSpeed = sets.cursorFlashSpeed;
		clone.selectionColor = sets.selectionColor;
		return clone;
	}
	
	public static void CloneValues(this GUISettings sets, GUISettings other) {
		sets.doubleClickSelectsWord = other.doubleClickSelectsWord;
		sets.tripleClickSelectsLine = other.tripleClickSelectsLine;
		sets.cursorColor = other.cursorColor;
		sets.cursorFlashSpeed = other.cursorFlashSpeed;
		sets.selectionColor = other.selectionColor;
	}
	
}







