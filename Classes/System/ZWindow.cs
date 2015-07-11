using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class ZWindow {
	public string name;
	public Rect area;
	public int padding = 0;
	public GUISkin skin { get { return _skin; } 
		set {
			_skin = value;
			resizeStyle = _skin.FindStyle("resize");
			if (skinCache.ContainsKey(_skin)) {
				_invisibleSkin = skinCache[_skin];	
			} else {
				_invisibleSkin = _skin.Clone();
				_invisibleSkin.window.normal.background = null;
				_invisibleSkin.window.onNormal.background = null;
				skinCache[_skin] = _invisibleSkin;
			}
		}
	}
		
	GUISkin _skin;
	GUISkin _invisibleSkin;
	static Dictionary<GUISkin, GUISkin> skinCache = new Dictionary<GUISkin, GUISkin>();
	
	public bool open;
	public bool invisibleBackground;
	public bool dragable;
	public bool eatClicks;
	
	public bool hasCloseButton;
	public bool hasMiniButton;
	public bool resizable;
	
	public Vector2 minSize;
	public Vector2 maxSize;
	
	private int id;
	public static int next_id = 10000;

	public static int lastFocused = 0;
	public float closeButtonSize = 18;
	public float resizeAreaSize = 18;
	
	public bool focused { get { return lastFocused == id; } }

	
	public float x { get { return area.x; } set { area.x = value; } }
	public float y { get { return area.y; } set { area.y = value; } }
	public float width { get { return area.width; } set { area.width = value; } }
	public float height { get { return area.height; } set { area.height = value; } } 
	
	static Vector3 resizeClickPoint;
	static Vector3 resizeBaseSize;
	static ZWindow resizing = null;
	static GUIStyle resizeStyle;
	
	bool lastOpenedState = false;
	
	public GUISkin blank { get { return Resources.Load<GUISkin>("blank"); } }
	public Rect draggableArea {
		get { return new Rect(0, 0 ,1000, 20); }
	}
	
	public Rect closeButtonArea {
		get {
			float size = closeButtonSize;
			if (size < 10) { size = 10; }
			return new Rect(width - size - 1, 1, size, size); 
		}
	}
	
	public Rect resizeArea {
		get {
			float size = resizeAreaSize;
			if (size < 10) { size = 10; }
			return new Rect(width - size - 1,height - size - 1, size, size);
		}
	}
	
	public Rect screenResizeArea {
		get {
			Rect r = resizeArea;
			r.x += x;
			r.y += y;
			return r;
		}
	}
	
	
	public ZWindow() { Init(); }
	
	public ZWindow Area(Rect r) { area = r; return this; }
	public ZWindow Named(string n) { name = n; return this; }
	public ZWindow Titled(string n) { name = n; return this; }
	public ZWindow Skinned(GUISkin s) { skin = s; return this; }
	public ZWindow Skinned(string s) { skin = Resources.Load<GUISkin>(s); return this; }
	
	public ZWindow VisibleBackground() { invisibleBackground = false; return this; }
	public ZWindow InvisibleBackground() { invisibleBackground = true; return this; }
	
	public ZWindow Locked() {
		dragable = false;
		resizable = false;
		hasCloseButton = false;
		hasMiniButton = false;
		return this;
	}
	
	public ZWindow Unlocked() {
		dragable = true;
		resizable = true;
		hasCloseButton = true;
		hasMiniButton = true;
		return this;
	}
	
	public ZWindow Dragable() { dragable = true; return this; }
	public ZWindow Undragable() { dragable = false; return this; }
	
	public ZWindow Resizable() { resizable = true; return this; }
	public ZWindow Unresizable() { resizable = false; return this; }
	
	public ZWindow Closable() { hasCloseButton = true; return this; }
	public ZWindow Unclosable() { hasCloseButton = false; return this; }
	
	public ZWindow Minimizable() { hasMiniButton = true; return this; }
	public ZWindow Unminimizable() { hasMiniButton = false; return this; }
	
	public ZWindow Opened() { open = true; lastOpenedState = true; return this; }
	public ZWindow Closed() { open = false; lastOpenedState = false; return this; }
	
	
	
	void Init() {
		id = next_id++;
		area = Screen.MiddleCenter(.5f, .5f);
		
		
		name = "New Window";
		skin = Resources.Load<GUISkin>("Standard");
		open = true;
		lastOpenedState = true;
		invisibleBackground = false;
		dragable = true;
		resizable = true;
		eatClicks = true;
		
		hasCloseButton = true;
		hasMiniButton = false;
		
		minSize = new Vector2(100, 60);
		maxSize = new Vector2(Screen.width, Screen.height);
		
	}
	
	public void Center() {
		area.x = (Screen.width - area.width)/2f;
		area.y = (Screen.height - area.height)/2f;
	}
	
	void SetSkin() {
		GUI.skin = skin;
		
		if (invisibleBackground) {
			GUI.skin = _invisibleSkin;
		}
		
	}
	
	public void Predraw() {
		if (open && !lastOpenedState) { OnOpen(); }
		if (!open && lastOpenedState) { OnClose(); }
		
		lastOpenedState = open;
	}
	
	public virtual void Draw() {
		Predraw();
		
		if (open) {
			SetSkin();
			Resize();
			area = GUI.Window(id, area, DrawWindow, name);	
		}
			
		
	}
	
	
	public void DrawWindow(int windowID) {
		//SetSkin();
		
		if ((GUIEvent.button == 0) && (GUIEvent.clickDown)) {
			lastFocused = id;
		}
		

		Window();
		
		if (hasCloseButton) {
			GUI.PushColor(Color.red);
			if (GUI.Button(closeButtonArea, "X")) {
				open = false;
			}
			GUI.PopColor();
		}
		
		if (hasMiniButton) {
			//GUI.PushColor(Color.gray);
			
			//TBD: Draw/handle minimizeButton
			
			//GUI.PopColor();
		}
		
		if (dragable) { GUI.DragWindow(draggableArea); }
		
		if (GUIEvent.clickDown && eatClicks) { GUIEvent.Use(); }
		Bound();
		//SetSkin();
	}
	
	public void Resize() {
		if (!open) { return; }
		if (resizing == null || resizing == this) {
			
			if (resizable && lastFocused == id) {
				GUI.DrawTexture(screenResizeArea, resizeStyle.normal.background);
				if (GUIEvent.clickDown) {
					if (screenResizeArea.Contains(Input.mousePosition)) {
						resizing = this;
						GUIEvent.Use();
						resizeBaseSize = new Vector3(width, height, 0);
						resizeClickPoint = Input.mousePosition;
					}
				} else if (resizing == this && GUIEvent.mouseUp) {
					resizing = null;
					GUIEvent.Use();
				} else if (GUIEvent.mouseDrag) {
					if (resizing == this) {
						Vector3 diff = Input.mousePosition - resizeClickPoint;
						width = resizeBaseSize.x + diff.x;
						height = resizeBaseSize.y + diff.y;
						
						GUIEvent.Use();
					}
				}
				
				if (width < minSize.x) { width = minSize.x; }
				if (height < minSize.y) { height = minSize.y; }
				if (width > maxSize.x) { width = maxSize.x; }
				if (height > maxSize.y) { height = maxSize.y; }
				
			}
			
		}
		
	}
	
	#region Virtual functions
	public virtual void Window() { }
	public virtual void OnClose() { }
	public virtual void OnOpen() { }
	
	#endregion
	
	public void Bound() {
		if (x < -width+60) { x = -width+60; }
		if (y < 0) { y = 0; }
		if (x > Screen.width - 60) { x = Screen.width - 60; }
		if (y > Screen.height - 20) { y = Screen.height - 20; }
	}

	public Rect GetScreenRect(Rect r) { return GetScreenRect(r, Vector2.zero); }
	public Rect GetScreenRect(Rect r, Vector2 offset) { return r.Shift(area.UpperLeft() + offset); }
	
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
	public static void Box(string s, GUIStyle style, params GUILayoutOption[] options) { GUILayout.Box(s, style, options); }
	public static void Box(Texture s, GUIStyle style, params GUILayoutOption[] options) { GUILayout.Box(s, style, options); }
	public static void Box(GUIContent s, GUIStyle style, params GUILayoutOption[] options) { GUILayout.Box(s, style, options); }
	public static void FixedBox(string s) { Box(s, ExpandWidth(false)); }
	public static void FixedBox(Texture s) { Box(s, ExpandWidth(false)); }
	public static void FixedBox(GUIContent s) { Box(s, ExpandWidth(false)); }
	
	public static void Box(Rect r, string s) { GUI.Box(r, s); }
	public static void Box(Rect r, Texture s) { GUI.Box(r, s); }
	public static void Box(Rect r, GUIContent s) { GUI.Box(r, s); }
	
	public static bool Button(string s, params GUILayoutOption[] options) { return GUILayout.Button(s, options); }
	public static bool Button(Texture s, params GUILayoutOption[] options) { return GUILayout.Button(s, options); }
	public static bool Button(GUIContent s, params GUILayoutOption[] options) { return GUILayout.Button(s, options); }
	public static bool Button(string s, GUIStyle style, params GUILayoutOption[] options) { return GUILayout.Button(s, style, options); }
	public static bool Button(Texture s, GUIStyle style, params GUILayoutOption[] options) { return GUILayout.Button(s, style, options); }
	public static bool Button(GUIContent s, GUIStyle style, params GUILayoutOption[] options) { return GUILayout.Button(s, style, options); }
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
	
	public static string TextField(string label, string text, params GUILayoutOption[] options) {
		string s;
		BeginHorizontal(); {
			Label(label, options);
			s = TextField(text, options);
		} EndHorizontal();
		return s;
	}
	
	public static float FloatField(string label, float val, params GUILayoutOption[] options) {
		float f;
		BeginHorizontal(); {
			Label(label, options);
			f = FloatField(val, options);
		} EndHorizontal();
		return f;
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
	
	
	
	public void StringReflectionField(string label, string valueName, params GUILayoutOption[] options) {
		BeginHorizontal(); {
			Label(label, options);
			
			string value = this.GetObjectValue<string>(valueName);
			value = TextField(value, options);
			this.SetObjectValue(valueName, value);
		} EndHorizontal();
		
	}
	
	public void FloatReflectionField(string label, string valueName, params GUILayoutOption[] options) {
		BeginHorizontal(); {
			Label(label, options);
			
			float value = this.GetObjectValue<float>(valueName);
			value = FloatField(value, options);
			this.SetObjectValue(valueName, value);
		} EndHorizontal();
		
	}
	
	public void IntReflectionField(string label, string valueName, params GUILayoutOption[] options) {
		BeginHorizontal(); {
			Label(label, options);
			
			int value = this.GetObjectValue<int>(valueName);
			value = IntField(value, options);
			this.SetObjectValue(valueName, value);
		} EndHorizontal();
		
	}
	
	#endregion
	
}
