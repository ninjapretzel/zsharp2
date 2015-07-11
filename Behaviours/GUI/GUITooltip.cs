using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if XtoJSON
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
	
	
	public static void SetTooltip(string content) {
		tooltip = new JsonObject()
			.Add("x", Input.mousePosition.x)
			.Add("y", Input.mousePosition.y)
			.Add("content", content);
	}
	
	public static void SetTooltip(float x, float y, string content) {
		tooltip = new JsonObject()
			.Add("x", x)
			.Add("y", y)
			.Add("content", content);
	}
		
	
}
#endif
