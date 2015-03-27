using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUITooltip : MonoBehaviour {
	
	public static JsonObject tooltip;
	
	public static float tooltip_width = 240;
	void OnGUI() {
		GUI.depth = -100;
		GUI.skin = Resources.Load<GUISkin>("Standard");
		if (tooltip != null) {
			DrawTooltip(tooltip);
			
			tooltip = null;
		}
	}
	
	void DrawTooltip(JsonObject tooltip) {
		float x = tooltip.GetFloat("x");
		float y = tooltip.GetFloat("y");
		
		string content = tooltip.GetString("content");
		
		float width = tooltip_width;
		if (tooltip.ContainsKey("width")) {
			width = tooltip.GetFloat("width");
		}
		float height = GUI.skin.box.CalcHeight(new GUIContent(content), width);
		
		if (y + height > Screen.height) { 
			y = Screen.height - height;
		}
		if (x + width > Screen.width) {
			x = Screen.width - width;
		}
		
		Rect brush = new Rect(x, y, width, height);
		GUI.Box(brush, content);
		
		
		
	}
	
}
