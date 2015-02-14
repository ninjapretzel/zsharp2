using UnityEngine;
using System.Collections.Generic;

using UnityGUI = UnityEngine.GUI;

public static class GUI {

	public static Color ColorPicker(Rect brush, Color inColor, string title = "") {
		TextAnchor alignBackup = GUI.skin.box.alignment;
		GUI.skin.box.alignment = TextAnchor.UpperCenter;
		UnityGUI.Box(brush, title);
		GUI.skin.box.alignment = alignBackup;
		Color backup = UnityGUI.color;
		UnityGUI.color = inColor;
		UnityGUI.DrawTexture(brush.TopCenter(0.9f, 0.4f).Move(0.0f, 0.25f), GUIUtils.pixel);
		UnityGUI.color = backup;
		Rect sliderBrush = brush.TopCenter(0.9f, 0.1f).Move(0.0f, 6.0f);
		sliderBrush.height = (GUI.skin.textField.fontSize == 0 ? 18 : GUI.skin.textField.fontSize) + GUI.skin.textField.border.top + GUI.skin.textField.border.bottom;
		inColor.r = UnityGUI.HorizontalSlider(sliderBrush, inColor.r, 0.0f, 1.0f);
		sliderBrush = sliderBrush.Move(0.0f, 1.1f);
		inColor.g = UnityGUI.HorizontalSlider(sliderBrush, inColor.g, 0.0f, 1.0f);
		sliderBrush = sliderBrush.Move(0.0f, 1.1f);
		inColor.b = UnityGUI.HorizontalSlider(sliderBrush, inColor.b, 0.0f, 1.0f);
		return inColor;
	}
	
	public static Stack<GUISkin> skinStack = new Stack<GUISkin>();
	public static void PushSkin(GUISkin newSkin) { 
		skinStack.Push(skin);
		skin = newSkin;
	}
	
	public static GUISkin PopSkin() {
		if (skinStack.Count > 0) {
			GUISkin prev = skin;
			skin = skinStack.Pop();
			return prev;
		}
		return null;
	}
	public static GUISkin blankSkin { get { return Resources.Load<GUISkin>("blank"); } }
	
	
	
	#region passthroughs
	public static Color backgroundColor { get { return UnityGUI.backgroundColor; } set { UnityGUI.backgroundColor = value; } }
	public static bool changed { get { return UnityGUI.changed; } set { UnityGUI.changed = value; } }
	public static Color color { get { return UnityGUI.color; } set { UnityGUI.color = value; } }
	public static Color contentColor { get { return UnityGUI.contentColor; } set { UnityGUI.contentColor = value; } }
	public static int depth { get { return UnityGUI.depth; } set { UnityGUI.depth = value; } }
	public static bool enabled { get { return UnityGUI.enabled; } set { UnityGUI.enabled = value; } }
	public static Matrix4x4 matrix { get { return UnityGUI.matrix; } set { UnityGUI.matrix = value; } }
	public static GUISkin skin { get { return UnityGUI.skin; } set { UnityGUI.skin = value; } }
	public static string tooltip { get { return UnityGUI.tooltip; } set { UnityGUI.tooltip = value; } }

	public static void BeginGroup(Rect position) { UnityGUI.BeginGroup(position); }
	public static void BeginGroup(Rect position, string text) { UnityGUI.BeginGroup(position, text); }
	public static void BeginGroup(Rect position, Texture image) { UnityGUI.BeginGroup(position, image); }
	public static void BeginGroup(Rect position, GUIContent content) { UnityGUI.BeginGroup(position, content); }
	public static void BeginGroup(Rect position, GUIStyle style) { UnityGUI.BeginGroup(position, style); }
	public static void BeginGroup(Rect position, string text, GUIStyle style) { UnityGUI.BeginGroup(position, text, style); }
	public static void BeginGroup(Rect position, Texture image, GUIStyle style) { UnityGUI.BeginGroup(position, image, style); }
	public static void BeginGroup(Rect position, GUIContent content, GUIStyle style) { UnityGUI.BeginGroup(position, content, style); }

	public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect) { return UnityGUI.BeginScrollView(position, scrollPosition, viewRect); }
	public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical) { return UnityGUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical); }
	public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar) { return UnityGUI.BeginScrollView(position, scrollPosition, viewRect, horizontalScrollbar, verticalScrollbar); }
	public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar) { return UnityGUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar); }

	public static void Box(Rect position, string text) { UnityGUI.Box(position, text); }
	public static void Box(Rect position, Texture image) { UnityGUI.Box(position, image); }
	public static void Box(Rect position, GUIContent content) { UnityGUI.Box(position, content); }
	public static void Box(Rect position, string text, GUIStyle style) { UnityGUI.Box(position, text, style); }
	public static void Box(Rect position, Texture image, GUIStyle style) { UnityGUI.Box(position, image, style); }
	public static void Box(Rect position, GUIContent content, GUIStyle style) { UnityGUI.Box(position, content, style); }

	public static void BringWindowToBack(int windowID) { UnityGUI.BringWindowToBack(windowID); }

	public static void BringWindowToFront(int windowID) { UnityGUI.BringWindowToFront(windowID); }

	public static bool Button(Rect position, string text) { return UnityGUI.Button(position, text); }
	public static bool Button(Rect position, Texture image) { return UnityGUI.Button(position, image); }
	public static bool Button(Rect position, GUIContent content) { return UnityGUI.Button(position, content); }
	public static bool Button(Rect position, string text, GUIStyle style)  { return UnityGUI.Button(position, text, style); }
	public static bool Button(Rect position, Texture image, GUIStyle style)  { return UnityGUI.Button(position, image, style); }
	public static bool Button(Rect position, GUIContent content, GUIStyle style)  { return UnityGUI.Button(position, content, style); }

	public static void DragWindow(Rect position) { UnityGUI.DragWindow(position); }

	public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode = ScaleMode.StretchToFill, bool alphaBlend = true, float imageAspect = 0) { UnityGUI.DrawTexture(position, image, scaleMode, alphaBlend, imageAspect); }

	public static void DrawTextureWithTexCoords(Rect position, Texture image, Rect texCoords, bool alphaBlend = true) { UnityGUI.DrawTextureWithTexCoords(position, image, texCoords, alphaBlend); }

	public static void EndGroup() { UnityGUI.EndGroup(); }

	public static void EndScrollView() { UnityGUI.EndScrollView(); }
	public static void EndScrollView(bool handleScrollWheel) { UnityGUI.EndScrollView(handleScrollWheel); }

	public static void FocusControl(string name) { UnityGUI.FocusControl(name); }

	public static void FocusWindow(int windowID) { UnityGUI.FocusWindow(windowID); }

	public static string GetNameOfFocusedControl() { return UnityGUI.GetNameOfFocusedControl(); }

	public static float HorizontalScrollbar(Rect position, float value, float size, float leftValue, float rightValue) { return UnityGUI.HorizontalScrollbar(position, value, size, leftValue, rightValue); }
	public static float HorizontalScrollbar(Rect position, float value, float size, float leftValue, float rightValue, GUIStyle style) { return UnityGUI.HorizontalScrollbar(position, value, size, leftValue, rightValue, style); }

	public static float HorizontalSlider(Rect position, float value, float leftValue, float rightValue) { return UnityGUI.HorizontalSlider(position, value, leftValue, rightValue); }
	public static float HorizontalSlider(Rect position, float value, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb) { return UnityGUI.HorizontalSlider(position, value, leftValue, rightValue, slider, thumb); }

	public static void Label(Rect position, string text) { UnityGUI.Label(position, text); }
	public static void Label(Rect position, Texture image) { UnityGUI.Label(position, image); }
	public static void Label(Rect position, GUIContent content) { UnityGUI.Label(position, content); }
	public static void Label(Rect position, string text, GUIStyle style) { UnityGUI.Label(position, text, style); }
	public static void Label(Rect position, Texture image, GUIStyle style) { UnityGUI.Label(position, image, style); }
	public static void Label(Rect position, GUIContent content, GUIStyle style) { UnityGUI.Label(position, content, style); }

	public static Rect ModalWindow(int id, Rect clientRect, UnityGUI.WindowFunction func, string text) { return UnityGUI.ModalWindow(id, clientRect, func, text); }
	public static Rect ModalWindow(int id, Rect clientRect, UnityGUI.WindowFunction func, Texture image) { return UnityGUI.ModalWindow(id, clientRect, func, image); }
	public static Rect ModalWindow(int id, Rect clientRect, UnityGUI.WindowFunction func, GUIContent content) { return UnityGUI.ModalWindow(id, clientRect, func, content); }
	public static Rect ModalWindow(int id, Rect clientRect, UnityGUI.WindowFunction func, string text, GUIStyle style)  { return UnityGUI.ModalWindow(id, clientRect, func, text, style); }
	public static Rect ModalWindow(int id, Rect clientRect, UnityGUI.WindowFunction func, Texture image, GUIStyle style) { return UnityGUI.ModalWindow(id, clientRect, func, image, style); }
	public static Rect ModalWindow(int id, Rect clientRect, UnityGUI.WindowFunction func, GUIContent content, GUIStyle style) { return UnityGUI.ModalWindow(id, clientRect, func, content, style); }

	public static string PasswordField(Rect position, string password, char maskChar) { return UnityGUI.PasswordField(position, password, maskChar); }
	public static string PasswordField(Rect position, string password, char maskChar, int maxLength) { return UnityGUI.PasswordField(position, password, maskChar, maxLength); }
	public static string PasswordField(Rect position, string password, char maskChar, GUIStyle style) { return UnityGUI.PasswordField(position, password, maskChar, style); }
	public static string PasswordField(Rect position, string password, char maskChar, int maxLength, GUIStyle style) { return UnityGUI.PasswordField(position, password, maskChar, maxLength, style); }

	public static bool RepeatButton(Rect position, string text) { return UnityGUI.RepeatButton(position, text); }
	public static bool RepeatButton(Rect position, Texture image) { return UnityGUI.RepeatButton(position, image); }
	public static bool RepeatButton(Rect position, GUIContent content) { return UnityGUI.RepeatButton(position, content); }
	public static bool RepeatButton(Rect position, string text, GUIStyle style) { return UnityGUI.RepeatButton(position, text, style); }
	public static bool RepeatButton(Rect position, Texture image, GUIStyle style) { return UnityGUI.RepeatButton(position, image, style); }
	public static bool RepeatButton(Rect position, GUIContent content, GUIStyle style) { return UnityGUI.RepeatButton(position, content, style); }

	public static void ScrollTo(Rect position) { UnityGUI.ScrollTo(position); }

	public static int SelectionGrid(Rect position, int selected, string[] texts, int xCount) { return UnityGUI.SelectionGrid(position, selected, texts, xCount); }
	public static int SelectionGrid(Rect position, int selected, Texture[] images, int xCount) { return UnityGUI.SelectionGrid(position, selected, images, xCount); }
	public static int SelectionGrid(Rect position, int selected, GUIContent[] content, int xCount) { return UnityGUI.SelectionGrid(position, selected, content, xCount); }
	public static int SelectionGrid(Rect position, int selected, string[] texts, int xCount, GUIStyle style) { return UnityGUI.SelectionGrid(position, selected, texts, xCount, style); }
	public static int SelectionGrid(Rect position, int selected, Texture[] images, int xCount, GUIStyle style) { return UnityGUI.SelectionGrid(position, selected, images, xCount, style); }
	public static int SelectionGrid(Rect position, int selected, GUIContent[] contents, int xCount, GUIStyle style) { return UnityGUI.SelectionGrid(position, selected, contents, xCount, style); }

	public static void SetNextControlName(string name) { UnityGUI.SetNextControlName(name); }

	public static string TextArea(Rect position, string text) { return UnityGUI.TextArea(position, text); }
	public static string TextArea(Rect position, string text, int maxLength) { return UnityGUI.TextArea(position, text, maxLength); }
	public static string TextArea(Rect position, string text, GUIStyle style) { return UnityGUI.TextArea(position, text, style); }
	public static string TextArea(Rect position, string text, int maxLength, GUIStyle style) { return UnityGUI.TextArea(position, text, maxLength, style); }

	public static string TextField(Rect position, string text) { return UnityGUI.TextField(position, text); }
	public static string TextField(Rect position, string text, int maxLength) { return UnityGUI.TextField(position, text, maxLength); }
	public static string TextField(Rect position, string text, GUIStyle style) { return UnityGUI.TextField(position, text, style); }
	public static string TextField(Rect position, string text, int maxLength, GUIStyle style) { return UnityGUI.TextField(position, text, maxLength, style); }

	public static bool Toggle(Rect position, bool value, string text) { return UnityGUI.Toggle(position, value, text); }
	public static bool Toggle(Rect position, bool value, Texture image) { return UnityGUI.Toggle(position, value, image); }
	public static bool Toggle(Rect position, bool value, GUIContent content) { return UnityGUI.Toggle(position, value, content); }
	public static bool Toggle(Rect position, bool value, string text, GUIStyle style) { return UnityGUI.Toggle(position, value, text, style); }
	public static bool Toggle(Rect position, bool value, Texture image, GUIStyle style) { return UnityGUI.Toggle(position, value, image, style); }
	public static bool Toggle(Rect position, bool value, GUIContent content, GUIStyle style) { return UnityGUI.Toggle(position, value, content, style); }

	public static int Toolbar(Rect position, int selected, string[] texts) { return UnityGUI.Toolbar(position, selected, texts); }
	public static int Toolbar(Rect position, int selected, Texture[] images) { return UnityGUI.Toolbar(position, selected, images); }
	public static int Toolbar(Rect position, int selected, GUIContent[] content) { return UnityGUI.Toolbar(position, selected, content); }
	public static int Toolbar(Rect position, int selected, string[] texts, GUIStyle style) { return UnityGUI.Toolbar(position, selected, texts, style); }
	public static int Toolbar(Rect position, int selected, Texture[] images, GUIStyle style) { return UnityGUI.Toolbar(position, selected, images, style); }
	public static int Toolbar(Rect position, int selected, GUIContent[] contents, GUIStyle style) { return UnityGUI.Toolbar(position, selected, contents, style); }

	public static void UnfocusWindow() { UnityGUI.UnfocusWindow(); }

	public static float VerticalScrollbar(Rect position, float value, float size, float topValue, float bottomValue) { return UnityGUI.VerticalScrollbar(position, value, size, topValue, bottomValue); }
	public static float VerticalScrollbar(Rect position, float value, float size, float topValue, float bottomValue, GUIStyle style) { return UnityGUI.VerticalScrollbar(position, value, size, topValue, bottomValue, style); }

	public static float VerticalSlider(Rect position, float value, float topValue, float bottomValue) { return UnityGUI.VerticalSlider(position, value, topValue, bottomValue); }
	public static float VerticalSlider(Rect position, float value, float topValue, float bottomValue, GUIStyle slider, GUIStyle thumb) { return UnityGUI.VerticalSlider(position, value, topValue, bottomValue, slider, thumb); }

	public static Rect Window(int id, Rect clientRect, UnityGUI.WindowFunction func, string text) { return UnityGUI.Window(id, clientRect, func, text); }
	public static Rect Window(int id, Rect clientRect, UnityGUI.WindowFunction func, Texture image) { return UnityGUI.Window(id, clientRect, func, image); }
	public static Rect Window(int id, Rect clientRect, UnityGUI.WindowFunction func, GUIContent content) { return UnityGUI.Window(id, clientRect, func, content); }
	public static Rect Window(int id, Rect clientRect, UnityGUI.WindowFunction func, string text, GUIStyle style) { return UnityGUI.Window(id, clientRect, func, text, style); }
	public static Rect Window(int id, Rect clientRect, UnityGUI.WindowFunction func, Texture image, GUIStyle style) { return UnityGUI.Window(id, clientRect, func, image, style); }
	public static Rect Window(int id, Rect clientRect, UnityGUI.WindowFunction func, GUIContent title, GUIStyle style) { return UnityGUI.Window(id, clientRect, func, title, style); }
	#endregion

}
