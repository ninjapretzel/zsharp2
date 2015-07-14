using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if XtoJSON
public class GUITooltip : MonoBehaviour {
	
	public static JsonObject tooltip;
	
	public static float tooltip_width = 240;
	public static bool always_towards_center = true;

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
		float sizex = tooltip.Extract<float>("sizex", 2);
		float sizey = tooltip.Extract<float>("sizey", 2);
		
		string content = tooltip.GetString("content");
		
		float width = tooltip_width;
		if (tooltip.ContainsKey("width")) {
			width = tooltip.GetFloat("width");
		}
		float height = GUI.skin.box.CalcHeight(new GUIContent(content), width);
		
		if (always_towards_center) {
			if (x > Screen.width / 2) { x -= width + sizex; }
			else {}

			if (y > Screen.height / 2) { y -= height + sizey; } 
			else { }

		} else {
			if (y + height > Screen.height) { 
				y -= height + sizey;
			}
			if (x + width > Screen.width) {
				x -= width + sizex;
			}

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


	public static void SetTooltip(Vector2 pos, string content) {
		tooltip = new JsonObject()
			.Add("x", pos.x)
			.Add("y", pos.y)
			.Add("content", content);
	}

	public static void SetTooltip(Vector2 pos, Vector2 size, string content) {
		tooltip = new JsonObject()
			.Add("x", pos.x)
			.Add("y", pos.y)
			.Add("sizex", size.x)
			.Add("sizey", size.y)
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
