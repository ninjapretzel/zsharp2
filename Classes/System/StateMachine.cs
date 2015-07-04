using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// StateMachine class
/// </summary>
/// <typeparam name="T">Object type operate on</typeparam>
public class StateMachine<T>  {

	/// <summary> Current, active state </summary>
	public State<T> currentState;
	/// <summary> The state that was just switched away from. </summary>
	public State<T> previousState;
	/// <summary> Object which the StateMachine is attached to. State objects switched to will recieve this object as their target. </summary>
	public T owner;

	private bool switchedLastFrame = false;
	private bool doneSwitching = false;

	/// <summary> Construct a StateMachine with a target object. </summary>
	public StateMachine(T target) {
		currentState = State<T>.baseInstance;
		owner = target;
		currentState.Enter();
	}
	
	/// <summary> Construct a StateMachine with a starting state and target object. </summary>
	public StateMachine(State<T> initialState, T target) {
		currentState = initialState;
		owner = target;
		currentState.Enter();
	}

	/// <summary> Attempt to switch to a different state. Returns if the state was actually switched or not. </summary>
	public bool Switch(State<T> s) {
		if (s == null) { return Switch(State<T>.baseInstance); }
		if (s == currentState) { return false; }
		s.target = owner;
		
		previousState = currentState;
		currentState = s;
		previousState.Exit();
		currentState.Enter();		
		
		switchedLastFrame = true;
		doneSwitching = false;
		
		return true;
	}

	#region General Functions
	/// <summary> 
	/// Update the state machine.
	/// If the object switched last frame, calls the EnterFrame() for the active state.
	/// Calls the Update() for the current state
	/// Calls the OffUpdate() for the previous state (if it exists)
	/// </summary>
	public void Update() { 
		if (doneSwitching) { doneSwitching = false; switchedLastFrame = false; }
		if (switchedLastFrame) { currentState.EnterFrame(); doneSwitching = true; }
		currentState.Update();
		if (previousState != null) { previousState.OffUpdate(); }
	}

	/// <summary> 
	/// Calls LateUpdate() for the current state. 
	/// Calls OffLateUpdate() for the previous state.
	/// </summary>
	public void LateUpdate() { 
		currentState.LateUpdate(); 
		if (previousState != null) { previousState.OffLateUpdate(); }
		
	}

	/// <summary> 
	/// Calls FixedUpdate() for the current state.
	/// Calls OffFixedUpdate for the previous state.
	/// </summary>
	public void FixedUpdate() { 
		currentState.FixedUpdate();
		if (previousState != null) { previousState.OffFixedUpdate(); }
		
	}

	/// <summary> 
	/// Calls the OnGUI() for the current state.
	/// If the object switched last frame, calls the EnterGUI() of the current state, and de-focuses any GUI controls.
	/// Calls the OffGUI() for the previous state.
	/// </summary>
	public void OnGUI() {
		currentState.OnGUI(); 
		if (switchedLastFrame) { currentState.EnterGUI(); GUI.FocusControl("nothing"); }
		if (previousState != null) { previousState.OffGUI(); }
	}

	#endregion


	#region Optional UnityEngine.MonoBehaviour pass-through methods
	/// <summary> Calls the OnCollisionEnter() for the current state. </summary>
	public void OnCollisionEnter(Collision c) { currentState.OnCollisionEnter(c); }
	/// <summary> Calls the OnCollisionStay() for the current state. </summary>
	public void OnCollisionStay(Collision c) { currentState.OnCollisionStay(c); }
	/// <summary> Calls the OnCollisionExit() for the current state. </summary>
	public void OnCollisionExit(Collision c) { currentState.OnCollisionExit(c); }

	/// <summary> Calls the TriggerEnter() for the current state. </summary>
	public void OnTriggerEnter(Collider c) { currentState.OnTriggerEnter(c); }
	/// <summary> Calls the TriggerStay() for the current state. </summary>
	public void OnTriggerStay(Collider c) { currentState.OnTriggerStay(c); }
	/// <summary> Calls the TriggerExit() for the current state. </summary>
	public void OnTriggerExit(Collider c) { currentState.OnTriggerExit(c); }

	/// <summary> Calls the OnMouseEnter() for the current state. </summary>
	public void OnMouseEnter() { currentState.OnMouseEnter(); }
	/// <summary> Calls the OnMouseOver() for the current state. </summary>
	public void OnMouseOver() { currentState.OnMouseOver(); }
	/// <summary> Calls the OnMouseExit() for the current state. </summary>
	public void OnMouseExit() { currentState.OnMouseExit(); }
	/// <summary> Calls the OnMouseDown() for the current state. </summary>
	public void OnMouseDown() { currentState.OnMouseDown(); }
	/// <summary> Calls the OnMouseUp() for the current state. </summary>
	public void OnMouseUp() { currentState.OnMouseUp(); }
	/// <summary> Calls the OnMouseUpAsButton for the current state. </summary>
	public void OnMouseUpAsButton() { currentState.OnMouseUpAsButton(); }

	#endregion

}

/// <summary>
/// State[T] blueprint class for use with StateMachine[T]
/// </summary>
/// <typeparam name="T">Type the state acts with</typeparam>
public class State<T> {

	/// <summary> Empty state with no behaviour. </summary>
	public static State<T> baseInstance = new State<T>();

	/// <summary> Target object that the state is keeping track of. </summary>
	public T target;
	
	public State() { target = default(T); }

	/// <summary> Simple hook for states to respond to different 'stimulus' </summary>
	public virtual void Action(string a) { Debug.LogWarning("Missing override to Action(string a) in " + GetType()); }
	
	#region Virtuals
	
	/// <summary> Called by the state machine when it enters a new state </summary>
	public virtual void Enter() {}
	/// <summary> Called by the state machine when it transitions to a new state </summary>
	public virtual void Exit() {}

	/// <summary> Called by OnGUI() of the state machine _AFTER_ the state's OnGUI() the first frame after the state is entered. </summary>
	public virtual void EnterGUI() {}
	/// <summary> Called by Update() of the state machine _BEFORE_ the state's Update() the first frame after the state is entered.  </summary>
	public virtual void EnterFrame() {}

	/// <summary> Called by Update() of the state machine, if it is the active state. </summary>
	public virtual void Update() {}
	/// <summary> Called by LateUpdate() of the state machine, if it is the active state. </summary>
	public virtual void LateUpdate() {}
	/// <summary> Called by FixedUpdate() of the state machine, if it is the active state. </summary>
	public virtual void FixedUpdate() {}
	/// <summary> Called by OnGUI() of the state machine, if it is the active state. </summary>
	public virtual void OnGUI() {}

	/// <summary> Called by Update() of the state machine, if it is previous state. </summary>
	public virtual void OffUpdate() {}
	/// <summary> Called by LateUpdate() of the state machine, if it is the previous state. </summary>
	public virtual void OffLateUpdate() {}
	/// <summary> Called by FixedUpdate() of the state machine, if it is the previous state. </summary>
	public virtual void OffFixedUpdate() {}
	/// <summary> Called by OnGUI() of the state machine, if it is the previous state. </summary>
	public virtual void OffGUI() {}

	/// <summary> Called by OnTriggerEnter() of the state machine, if it is the active state. </summary>
	public virtual void OnTriggerEnter(Collider c) {}
	/// <summary> Called by OnTriggerStay() of the state machine, if it is the active state. </summary>
	public virtual void OnTriggerStay(Collider c) {}
	/// <summary> Called by OnTriggerExit() of the state machine, if it is the active state. </summary>
	public virtual void OnTriggerExit(Collider c) {}

	/// <summary> Called by OnCollisionEnter() of the state machine, if it is the active state. </summary>
	public virtual void OnCollisionEnter(Collision c) {}
	/// <summary> Called by OnCollisionStay() of the state machine, if it is the active state. </summary>
	public virtual void OnCollisionStay(Collision c) {}
	/// <summary> Called by OnCollisionExit() of the state machine, if it is the active state. </summary>
	public virtual void OnCollisionExit(Collision c) {}

	/// <summary> Called by OnMouseEnter() of the state machine, if it is the active state. </summary>
	public virtual void OnMouseEnter() {}
	/// <summary> Called by OnMouseOver() of the state machine, if it is the active state. </summary>
	public virtual void OnMouseOver() {}
	/// <summary> Called by OnMouseExit() of the state machine, if it is the active state. </summary>
	public virtual void OnMouseExit() {}
	/// <summary> Called by OnMouseDown() of the state machine, if it is the active state. </summary>
	public virtual void OnMouseDown() {}
	/// <summary> Called by OnMouseUp() of the state machine, if it is the active state. </summary>
	public virtual void OnMouseUp() {}
	/// <summary> Called by OnMouseUpAsButton() of the state machine, if it is the active state. </summary>
	public virtual void OnMouseUpAsButton() {}
	
	#endregion
	
	#region GUILayoutStuff
	
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
	#endregion
	
	///Depreciated functions
	[Obsolete("No longer need to pass in the owner. Use Enter() ")] public virtual void Enter(T owner) {}
	[Obsolete("No longer need to pass in the owner. Use Exit() ")] public virtual void Exit(T owner) {}
	[Obsolete("No longer need to pass in the owner. Use EnterGUI() ")] public virtual void EnterGUI(T owner) {}
	[Obsolete("No longer need to pass in the owner. Use EnterFrame() ")] public virtual void EnterFrame(T owner) {}
	[Obsolete("No longer need to pass in the owner. Use Update() ")] public virtual void Update(T owner) {}
	[Obsolete("No longer need to pass in the owner. Use LateUpdate() ")] public virtual void LateUpdate(T owner) {}
	[Obsolete("No longer need to pass in the owner. Use FixedUpdate() ")] public virtual void FixedUpdate(T owner) {}
	[Obsolete("No longer need to pass in the owner. Use OnGUI() ")] public virtual void OnGUI(T owner) {}
	[Obsolete("No longer need to pass in the owner. Use OnTriggerEnter(Collider c) ")] public virtual void OnTriggerEnter(T owner, Collider c) {}
	[Obsolete("No longer need to pass in the owner. Use OnTriggerStay(Collider c) ")] public virtual void OnTriggerStay(T owner, Collider c) {}
	[Obsolete("No longer need to pass in the owner. Use OnTriggerExit(Collider c) ")] public virtual void OnTriggerExit(T owner, Collider c) {}
	[Obsolete("No longer need to pass in the owner. Use OnCollisionEnter(Collision c) ")] public virtual void OnCollisionEnter(T owner, Collision c) {}
	[Obsolete("No longer need to pass in the owner. Use OnCollisionStay(Collision c) ")] public virtual void OnCollisionStay(T owner, Collision c) {}
	[Obsolete("No longer need to pass in the owner. Use OnCollisionExit(Collision c) ")] public virtual void OnCollisionExit(T owner, Collision c) {}
	[Obsolete("No longer need to pass in the owner. Use OnMouseEnter() ")] public virtual void OnMouseEnter(T owner) {}
	[Obsolete("No longer need to pass in the owner. Use OnMouseOver() ")] public virtual void OnMouseOver(T owner) {}
	[Obsolete("No longer need to pass in the owner. Use OnMouseExit() ")] public virtual void OnMouseExit(T owner) {}
	[Obsolete("No longer need to pass in the owner. Use OnMouseDown() ")] public virtual void OnMouseDown(T owner) {}
	[Obsolete("No longer need to pass in the owner. Use OnMouseUp() ")] public virtual void OnMouseUp(T owner) {}
	[Obsolete("No longer need to pass in the owner. Use OnMouseUpAsButton() ")] public virtual void OnMouseUpAsButton(T owner) {}
	
}







