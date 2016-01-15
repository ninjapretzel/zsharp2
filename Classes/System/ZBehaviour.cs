using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZBehaviour : MonoBehaviour {
	
	#region Other stuff
	/// <summary> Call a method via Unity's messaging system. </summary>
	public void SendMSG(string msg) { SendMessage(msg, SendMessageOptions.DontRequireReceiver); }
	/// <summary> Call a method via Uinity's messaging system, with a given parameter. </summary>
	public void SendMSG(string msg, System.Object val) { SendMessage(msg, val, SendMessageOptions.DontRequireReceiver); }

	/// <summary> Broadcast a message to this object and all children via Unity's messaging system. </summary>
	public void Broadcast(string msg) { BroadcastMessage(msg, SendMessageOptions.DontRequireReceiver); }
	/// <summary> Broadcast a message to this object and all children via Unity's messaging system, with a given parameter.</summary>
	public void Broadcast(string msg, System.Object val) { BroadcastMessage(msg, val, SendMessageOptions.DontRequireReceiver); }

	/// <summary> Wrap to Debug.Log </summary>
	public void Log(string msg) { Debug.Log("" + this + ": " + msg); }
	/// <summary> Wrap to Debug.LogWarning </summary>
	public void LogWarning(string msg) { Debug.LogWarning("" + this + ": " + msg); }
	
	#endregion
	
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
