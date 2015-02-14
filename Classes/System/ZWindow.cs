using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ZWindow {
	
	public string name;
	public Rect area;
	public int padding = 0;
	public GUISkin skin;
	
	
	private int id;
	public static int next_id = 10000;
	
	public float x { get { return area.x; } set { area.x = value; } }
	public float y { get { return area.y; } set { area.y = value; } }
	public float width { get { return area.width; } set { area.width = value; } }
	public float height { get { return area.height; } set { area.height = value; } } 
	
	public Rect draggableArea {
		get { return new Rect(0, 0 ,1000, 20); }
	}
	
	
	public ZWindow() { Init(); }
	public ZWindow Named(string n) { name = n; return this; }
	public ZWindow Titled(string n) { name = n; return this; }
	public ZWindow Skinned(GUISkin s) { skin = s; return this; }
	
	public void Init() {
		id = next_id++;
		area = Screen.MiddleCenter(.5f, .5f);
		name = "New Window";
		skin = Resources.Load<GUISkin>("Standard");
	}
	
	public void Center() {
		area.x = (Screen.width - area.width)/2f;
		area.y = (Screen.height - area.height)/2f;
	}
	
	public void Draw() {
		GUI.skin = skin;
		area = GUI.Window(id, area, DrawWindow, name);
	}
	
	public void DrawWindow(int windowID) {
		
		Window();
		
		GUI.DragWindow(draggableArea);
	}
	
	public virtual void Window() { }
	
}
