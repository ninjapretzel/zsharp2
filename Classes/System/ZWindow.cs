﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class ZWindow {
	public string name;
	public Rect area;
	public int padding = 0;
	public GUISkin skin;
	
	public bool open;
	public bool invisibleBackground;
	public bool dragable;
	
	public bool hasCloseButton;
	public bool hasMiniButton;
	
	private int id;
	public static int next_id = 10000;
	public static int lastFocused = 0;
	
	public float x { get { return area.x; } set { area.x = value; } }
	public float y { get { return area.y; } set { area.y = value; } }
	public float width { get { return area.width; } set { area.width = value; } }
	public float height { get { return area.height; } set { area.height = value; } } 
	
	public GUISkin blank { get { return Resources.Load<GUISkin>("blank"); } }
	public Rect draggableArea {
		get { return new Rect(0, 0 ,1000, 20); }
	}
	
	public Rect closeButtonArea {
		get { return new Rect(width - 24, 0, 20, 20); }
	}
	
	
	public ZWindow() { Init(); }
	
	public ZWindow Area(Rect r) { area = r; return this; }
	public ZWindow Named(string n) { name = n; return this; }
	public ZWindow Titled(string n) { name = n; return this; }
	public ZWindow Skinned(GUISkin s) { skin = s; return this; }
	public ZWindow InvisibleBackground() { invisibleBackground = true; return this; }
	
	public ZWindow Undragable() { dragable = false; return this; }
	public ZWindow Unclosable() { hasCloseButton = false; return this; }
	public ZWindow Minimizable() { hasMiniButton = true; return this; }
	
	public ZWindow Opened() { open = true; return this; }
	public ZWindow Closed() { open = false; return this; }
	
	public void Init() {
		id = next_id++;
		area = Screen.MiddleCenter(.5f, .5f);
		name = "New Window";
		skin = Resources.Load<GUISkin>("Standard");
		open = true;
		invisibleBackground = false;
		dragable = true;
		
		hasCloseButton = true;
		hasMiniButton = false;
	}
	
	public void Center() {
		area.x = (Screen.width - area.width)/2f;
		area.y = (Screen.height - area.height)/2f;
	}
	
	public void Draw() {
		if (open) {
			if (invisibleBackground) {
				GUI.skin = blank;
			} else {
				GUI.skin = skin;
			}
			area = GUI.Window(id, area, DrawWindow, name);	
			
		}
	}
	
	public void DrawWindow(int windowID) {
		if ((Event.current.button == 0) && (Event.current.type == EventType.MouseDown)) {
			lastFocused = id;
		}
		
		Window();
		
		if (GUI.Button(closeButtonArea, "X")) {
			open = false;
		}
		if (dragable) {
			GUI.DragWindow(draggableArea);
		}
		
	}
	
	
	public virtual void Window() { }
	
	
	#region GUI Extensions
	public static void Label(string s, params GUILayoutOption[] options) { GUILayout.Label(s, options); }
	public static void Label(Texture s, params GUILayoutOption[] options) { GUILayout.Label(s, options); }
	public static void Label(GUIContent s, params GUILayoutOption[] options) { GUILayout.Label(s, options); }
	public static void FixedLabel(string s) { Label(s, ExpandWidth(false)); }
	public static void FixedLabel(Texture s) { Label(s, ExpandWidth(false)); }
	public static void FixedLabel(GUIContent s) { Label(s, ExpandWidth(false)); }
	
	public static void Label(Rect r, string s) { GUI.Label(r, s); }
	public static void Label(Rect r, Texture s) { GUI.Label(r, s); }
	public static void Label(Rect r, GUIContent s) { GUI.Label(r, s); }
	
	public static void IconLabel(Texture s, float size) { Label(s, ExpandWidth(false), ExpandHeight(false), MaxHeight(size), MaxWidth(size), MinHeight(size), MinWidth(size)); }
	
	
	public static void Box(string s, params GUILayoutOption[] options) { GUILayout.Box(s, options); }
	public static void Box(Texture s, params GUILayoutOption[] options) { GUILayout.Box(s, options); }
	public static void Box(GUIContent s, params GUILayoutOption[] options) { GUILayout.Box(s, options); }
	public static void FixedBox(string s) { Box(s, ExpandWidth(false)); }
	public static void FixedBox(Texture s) { Box(s, ExpandWidth(false)); }
	public static void FixedBox(GUIContent s) { Box(s, ExpandWidth(false)); }
	
	public static void Box(Rect r, string s) { GUI.Box(r, s); }
	public static void Box(Rect r, Texture s) { GUI.Box(r, s); }
	public static void Box(Rect r, GUIContent s) { GUI.Box(r, s); }
	
	public static bool Button(string s, params GUILayoutOption[] options) { return GUILayout.Button(s, options); }
	public static bool Button(Texture s, params GUILayoutOption[] options) { return GUILayout.Button(s, options); }
	public static bool Button(GUIContent s, params GUILayoutOption[] options) { return GUILayout.Button(s, options); }
	public static bool FixedButton(string s) { return Button(s, ExpandWidth(false)); }
	
	public static bool Button(Rect r, string s) { return GUI.Button(r, s); }
	public static bool Button(Rect r, Texture s) { return GUI.Button(r, s); }
	public static bool Button(Rect r, GUIContent s) { return GUI.Button(r, s); }
	/*
	public static bool Button(Rect r, string s, string snd) { 
		if (GUI.Button(r, s)) { SoundMaster.Play(snd); return true; }
		else { return false; }
	}
	public static bool Button(Rect r, Texture s, string snd) { 
		if (GUI.Button(r, s)) { SoundMaster.Play(snd); return true; }
		else { return false; }
	}
	public static bool Button(Rect r, GUIContent s, string snd) { 
		if (GUI.Button(r, s)) { SoundMaster.Play(snd); return true; }
		else { return false; }
	}
	//*/
	
	
	public static bool Toggle(bool val, string s, params GUILayoutOption[] options) { return GUILayout.Toggle(val, s, options); }
	public static bool Toggle(bool val, Texture s, params GUILayoutOption[] options) { return GUILayout.Toggle(val, s, options); }
	public static bool Toggle(bool val, GUIContent s, params GUILayoutOption[] options) { return GUILayout.Toggle(val, s, options); }
	
	public static string TextField(string text, params GUILayoutOption[] options) { return GUILayout.TextField(text, options); }
	public static string PasswordField(string s, params GUILayoutOption[] options) { return GUILayout.PasswordField(s, '*', options); }
	public static float FloatField(float value, params GUILayoutOption[] options) {
		float val = value;
		string str;
		str = TextField(""+val, options);
		
		try { val = str.ParseFloat(); }
		catch { return value; }
		
		return val;
	}
	public static int IntField(int value, params GUILayoutOption[] options) {
		int val = value;
		string str;
		str = TextField(""+val, options);
		
		try { val = str.ParseInt(); }
		catch { return value; }
		
		return val;
	}
	
	public static float HorizontalSlider(float val, float left, float right, params GUILayoutOption[] options) {
		return GUILayout.HorizontalSlider(val, left, right, options);
	}
	
	public static float VerticalSlider(float val, float top, float bottom, params GUILayoutOption[] options) {
		return GUILayout.VerticalSlider(val, top, bottom, options);
	}
	
	public static void BeginArea(Rect area) { GUILayout.BeginArea(area); }
	public static void EndArea() { GUILayout.EndArea(); }
	
	public static void BeginHorizontal(params GUILayoutOption[] options) { GUILayout.BeginHorizontal(options); }
	public static void BeginHorizontal(string style, params GUILayoutOption[] options) { GUILayout.BeginHorizontal(style, options); }
	public static void EndHorizontal() { GUILayout.EndHorizontal(); }
	
	public static void BeginVertical(params GUILayoutOption[] options) { GUILayout.BeginVertical(options); }
	public static void BeginVertical(string style, params GUILayoutOption[] options) { GUILayout.BeginVertical(style, options); }
	public static void EndVertical() { GUILayout.EndVertical(); }
	
	public static Vector2 BeginScrollView(Vector2 scroll, bool alwaysShowHor = false, bool alwaysShowVer = false, params GUILayoutOption[] options) { 
		return GUILayout.BeginScrollView(scroll, alwaysShowHor, alwaysShowVer, options);
	}
	public static void EndScrollView() { GUILayout.EndScrollView(); }
	
	public static void Space(float pixels) { GUILayout.Space(pixels); }
	public static void FlexibleSpace() { GUILayout.FlexibleSpace(); }
	
	public static GUILayoutOption ExpandWidth(bool expand) { return GUILayout.ExpandWidth(expand); }
	public static GUILayoutOption ExpandHeight(bool expand) { return GUILayout.ExpandHeight(expand); }
	public static GUILayoutOption MaxHeight(float height) { return GUILayout.MaxHeight(height); }
	public static GUILayoutOption MinHeight(float height) { return GUILayout.MinHeight(height); }
	public static GUILayoutOption Height(float height) { return GUILayout.Height(height); }
	public static GUILayoutOption MaxWidth(float width) { return GUILayout.MaxWidth(width); }
	public static GUILayoutOption MinWidth(float width) { return GUILayout.MinWidth(width); }
	public static GUILayoutOption Width(float width) { return GUILayout.Width(width); }
	
	
	
	public void StringField(string label, string valueName, params GUILayoutOption[] options) {
		BeginHorizontal(); {
			Label(label, options);
			
			string value = this.GetObjectValue<string>(valueName);
			value = TextField(value, options);
			this.SetObjectValue(valueName, value);
		} EndHorizontal();
		
	}
	
	public void FloatField(string label, string valueName, params GUILayoutOption[] options) {
		BeginHorizontal(); {
			Label(label, options);
			
			float value = this.GetObjectValue<float>(valueName);
			value = FloatField(value, options);
			this.SetObjectValue(valueName, value);
		} EndHorizontal();
		
	}
	
	public void IntField(string label, string valueName, params GUILayoutOption[] options) {
		BeginHorizontal(); {
			Label(label, options);
			
			int value = this.GetObjectValue<int>(valueName);
			value = IntField(value, options);
			this.SetObjectValue(valueName, value);
		} EndHorizontal();
		
	}
	
	#endregion
	
}