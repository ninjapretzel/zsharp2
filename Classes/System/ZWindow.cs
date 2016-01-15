using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary> Unity Legacy GUI Window </summary>
public class ZWindow {

	/// <summary> Title of window </summary>
	public string name;
	/// <summary> Area on screen of window </summary>
	public Rect area;

	/// <summary> Skin of window. Stores to _skin, and creates an invisible version of the skin as well. </summary>
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

	/// <summary> Cached skin </summary>
	GUISkin _skin;
	/// <summary> Cached invisible skin </summary>
	GUISkin _invisibleSkin;

	/// <summary> Cache of skins mapped to their 'invisible' versions. </summary>
	static Dictionary<GUISkin, GUISkin> skinCache = new Dictionary<GUISkin, GUISkin>();

	/// <summary> Is the window currently open? </summary>
	public bool open;
	/// <summary> Is the background 'invisible' ? </summary>
	public bool invisibleBackground;

	/// <summary> Can this window be moved with a drag? </summary>
	public bool dragable;
	/// <summary> Does the window background eat clicks? </summary>
	public bool eatClicks;

	/// <summary> Is the close button of this window visible? </summary>
	public bool hasCloseButton;
	/// <summary> Is the minimize button of this winow visible? </summary>
	public bool hasMiniButton;
	/// <summary> Can this window be resized? </summary>
	public bool resizable;

	/// <summary> Minimum size of window </summary>
	public Vector2 minSize;
	/// <summary> Maximum size of window </summary>
	public Vector2 maxSize;

	/// <summary> Internal window id </summary>
	private int id;
	/// <summary> window ID counter </summary>
	public static int next_id = 10000;

	/// <summary> ID of last focused window </summary>
	public static int lastFocused = 0;

	/// <summary> Size of close button </summary>
	public float closeButtonSize = 18;
	/// <summary> Size of resize control </summary>
	public float resizeAreaSize = 18;

	/// <summary> Is this given window the last one focused? </summary>
	public bool focused { get { return lastFocused == id; } }

	/// <summary> Wraps through to area.x </summary>
	public float x { get { return area.x; } set { area.x = value; } }
	/// <summary> Wraps through to area.y </summary>
	public float y { get { return area.y; } set { area.y = value; } }
	/// <summary> Wraps through to area.width </summary>
	public float width { get { return area.width; } set { area.width = value; } }
	/// <summary> Wraps through to area.height </summary>
	public float height { get { return area.height; } set { area.height = value; } }

	/// <summary> Point on window of last click-down on resize control </summary>
	static Vector3 resizeClickPoint;
	/// <summary> Base size of last window being resized </summary>
	static Vector3 resizeBaseSize;
	/// <summary> What window is currently being resized? </summary>
	static ZWindow resizing = null;
	/// <summary> Style of resize control </summary>
	static GUIStyle resizeStyle;

	/// <summary> Color of close buttons </summary>
	static Color closeButtonColor = new Color(.75f, 0, 0);

	/// <summary> Was this window opened or closed last frame? Used to have callbacks happen on the frame a window is closed or opened </summary>
	bool lastOpenedState = false;

	/// <summary> Blank GUISkin </summary>
	public GUISkin blank { get { return Resources.Load<GUISkin>("blank"); } }
	/// <summary> Area of the screen the window can be dragged inside of </summary>
	public Rect draggableArea {
		get { return new Rect(0, 0 ,1000, 20); }
	}

	/// <summary> Area of the window to show the close button. Upper-Right corner. </summary>
	public Rect closeButtonArea {
		get {
			float size = closeButtonSize;
			if (size < 10) { size = 10; }
			return new Rect(width - size - 1, 1, size, size); 
		}
	}

	/// <summary> Area of the window to show the resize control. Bottom-Right corner. </summary>
	public Rect resizeArea {
		get {
			float size = resizeAreaSize;
			if (size < 10) { size = 10; }
			return new Rect(width - size - 1,height - size - 1, size, size);
		}
	}

	/// <summary> Area on Screen to show the resize control. Adjusted for screen coords, rather than local coords.  </summary>
	public Rect screenResizeArea {
		get {
			Rect r = resizeArea;
			r.x += x;
			r.y += y;
			return r;
		}
	}

	/// <summary> Constructor </summary>
	public ZWindow() { Init(); }

	/// <summary> Chain method to set area of window </summary>
	public ZWindow Area(Rect r) { area = r; return this; }

	/// <summary> Chain method to set title of window </summary>
	public ZWindow Named(string n) { name = n; return this; }
	/// <summary> Chain method to set title of window </summary>
	public ZWindow Titled(string n) { name = n; return this; }
	/// <summary> Chain method to change skin of window </summary>
	public ZWindow Skinned(GUISkin s) { skin = s; return this; }
	/// <summary> Chain method to change skin of window by name </summary>
	public ZWindow Skinned(string s) { skin = Resources.Load<GUISkin>(s); return this; }

	/// <summary> Chain method to set background visible </summary>
	public ZWindow VisibleBackground() { invisibleBackground = false; return this; }
	/// <summary> Chain method to set background invisible </summary>
	public ZWindow InvisibleBackground() { invisibleBackground = true; return this; }

	/// <summary> Chain method to completely lock window in place </summary>
	public ZWindow Locked() {
		dragable = false;
		resizable = false;
		hasCloseButton = false;
		hasMiniButton = false;
		return this;
	}

	/// <summary> Chain method to completely free window for user to move </summary>
	public ZWindow Unlocked() {
		dragable = true;
		resizable = true;
		hasCloseButton = true;
		hasMiniButton = true;
		return this;
	}

	/// <summary> Chain method to make window dragable </summary>
	public ZWindow Dragable() { dragable = true; return this; }
	/// <summary> Chain method to make window undragable </summary>
	public ZWindow Undragable() { dragable = false; return this; }

	/// <summary> Chain method to make window resizable </summary>
	public ZWindow Resizable() { resizable = true; return this; }
	/// <summary> Chain method to make window unresizable </summary>
	public ZWindow Unresizable() { resizable = false; return this; }

	/// <summary> Chain method to make window closable </summary>
	public ZWindow Closable() { hasCloseButton = true; return this; }
	/// <summary> Chain method to make window unclosable </summary>
	public ZWindow Unclosable() { hasCloseButton = false; return this; }

	/// <summary> Chain method to make window minimizable </summary>
	public ZWindow Minimizable() { hasMiniButton = true; return this; }
	/// <summary> Chain method to make window unminimizable </summary>
	public ZWindow Unminimizable() { hasMiniButton = false; return this; }

	/// <summary> Chain method to open window, and force OnOpen() callback </summary>
	public ZWindow Opened() { open = true; lastOpenedState = true; return this; }
	/// <summary> Chain method to close window, and force OnClose() callback </summary>
	public ZWindow Closed() { open = false; lastOpenedState = false; return this; }


	/// <summary> Initialization logic for windows </summary>
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

	/// <summary> Center the window. </summary>
	public void Center() {
		area.x = (Screen.width - area.width)/2f;
		area.y = (Screen.height - area.height)/2f;
	}

	/// <summary> Set the skin to the active GUISkin </summary>
	void SetSkin() {
		GUI.skin = skin;
		
		if (invisibleBackground) {
			GUI.skin = _invisibleSkin;
		}
		
	}

	/// <summary> Called before any drawing to ensure OnOpen() and OnClose() callbacks happen only once. </summary>
	public void Predraw() {
		if (open && !lastOpenedState) { OnOpen(); }
		if (!open && lastOpenedState) { OnClose(); }
		
		lastOpenedState = open;
	}

	/// <summary> Draw the window via GUI.Window, and apply any drags. </summary>
	public virtual void Draw() {
		Predraw();
		
		if (open) {
			SetSkin();
			Resize();
			area = GUI.Window(id, area, DrawWindow, name);	
		}
			
		
	}

	/// <summary> Function passed into GUI.Window to draw any window. </summary>
	public void DrawWindow(int windowID) {
		if ((GUIEvent.button == 0) && (GUIEvent.clickDown)) {
			lastFocused = id;
		}
		
		Window();
		
		if (hasCloseButton) {
			GUI.PushColor(closeButtonColor);
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
	}

	/// <summary> Resizing logic </summary>
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
	/// <summary> Called by GUIRoot on Update </summary>
	public virtual void Update() { }
	/// <summary> Called by GUIRoot on LateUpdate </summary>
	public virtual void LateUpdate() { }

	/// <summary> Insert custom window drawing logic </summary>
	public virtual void Window() { }
	/// <summary> Insert any logic that happens when the window is closed. </summary>
	public virtual void OnClose() { }
	/// <summary> Insert any logic that happens when the window is opened. </summary>
	public virtual void OnOpen() { }
	
	#endregion

	/// <summary> Focus this window </summary>
	public void Focus() { GUI.FocusWindow(id); lastFocused = id; }
	/// <summary> Unfocus all windows </summary>
	public void Unfocus() { GUI.FocusWindow(-1); lastFocused = -1; }

	/// <summary> Prevent this window from going off the screen. </summary>
	public void Bound() {
		if (x < -width+60) { x = -width+60; }
		if (y < 0) { y = 0; }
		if (x > Screen.width - 60) { x = Screen.width - 60; }
		if (y > Screen.height - 20) { y = Screen.height - 20; }
	}

	/// <summary> Get the area on the screen that this window exists inside of </summary>
	public Rect GetScreenRect(Rect r) { return GetScreenRect(r, Vector2.zero); }
	/// <summary> Get the area on the screen that this window exists inside of, shifted by an offset </summary>
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
