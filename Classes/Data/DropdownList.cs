using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary> Legacy-GUI dropdown list. </summary>
public class DropdownList<T> : List<T> {

	/// <summary> Currently open? </summary>
	public bool open = false;
	/// <summary> Current selection index. </summary>
	public int selectedIndex { get; private set; }
	
	/// <summary> Position of scroll in drop-down</summary>
	Vector2 scrollPos;

	/// <summary> Max height as percentage of screen. </summary>
	public float maxHeight = .25f;

	/// <summary> Style for the top of the dropdown </summary>
	public GUIStyle topStyle { get { return GUI.skin.FindStyle("dropdown_top"); } }
	/// <summary> Style for the body of the dropdown </summary>
	public GUIStyle bodyStyle { get { return GUI.skin.FindStyle("dropdown_body"); } }

	/// <summary> String representation of the selected item. </summary>
	public string selectedLabel { get { return LabelOfElementAt(selectedIndex); } }
	/// <summary> Actual selected item </summary>
	public T selected { 
		get {
			if (Count == 0) { return default(T); }
			else if (selectedIndex <= 0) { return this[0]; }
			else if (selectedIndex > Count) { return this[Count-1]; }
			return this[selectedIndex];
		}
	}

	/// <summary> Callback for when an item is selected. </summary>
	public Action<List<T>> onSelected = null;

	/// <summary> Display for 'null' elements </summary>
	const string NULLSTR = "<NULL>";

	public DropdownList() : base() { Init(); }
	public DropdownList(int cap) : base(cap) { Init(); }
	public DropdownList(IEnumerable<T> collection) : base(collection) { Init(); }

	/// <summary> Common initialization </summary>
	void Init() { 
		open = false;
		selectedIndex = 0;
	}

	/// <summary> Get the label (via ToString) of an element in the list </summary>
	public string LabelOfElementAt(int index) {
		if (index < 0) { return NULLSTR; }
		else if (index >= Count) { return NULLSTR; }
		
		T sel = this[index];
		string str = NULLSTR;
		if (sel != null) { str = sel.ToString(); }
		
		return str;
	}

	/// <summary> Set the selected index to the index of a given value, or zero if that object is not in the list. </summary>
	public void SetIndexTo(object o) {
		for (int i = 0; i < Count; i++) {
			if (this[i].Equals(o)) {
				selectedIndex = i;
				return;
			}
		}
		selectedIndex = 0;
	}

	/// <summary> Set the selected index to a given number. </summary>
	public void SetIndex(int index) {
		if (this.Count - 1 >= index) {
			selectedIndex = index;
		}
	}

	/// <summary> Draw the dropdown list using the given area for the top of the dropdown. </summary>
	public void Draw(Rect baseArea) {
		if (GUI.Button(baseArea, selectedLabel, topStyle)) { open = !open; }
		
		if (open) { Selection(baseArea); }
		
	}

	/// <summary> Draw the dropdown as a part of GUILayout. </summary>
	public void Draw(params GUILayoutOption[] options) {
		if (GUILayout.Button(selectedLabel, topStyle, options)) { open = !open; }
		
		if (open) { Selection(GUILayoutUtility.GetLastRect()); }
		
	}

	/// <summary> Draw and handle the dropdown selection, given the rectangle for the top of the list. </summary>
	public void Selection(Rect r) {
		float totalHeight = r.height * Count;
		float maxPixelHeight = maxHeight * Screen.height;
		
		if (totalHeight > maxPixelHeight) {
			Rect screenArea = r.MoveDown();
			screenArea.width += 16;
			screenArea.height = maxPixelHeight;
			
			Rect totalArea = new Rect(0, 0, screenArea.width-16, totalHeight);
			
			scrollPos = GUI.BeginScrollView(screenArea, scrollPos, totalArea, false, true); 
			r = new Rect(0, 0, r.width, r.height); 
		} else {
			r = r.MoveDown();
		}
	
		for (int i = 0; i < Count; i++) {
			string str = LabelOfElementAt(i);
			if (GUI.Button(r, str, bodyStyle)) {
				selectedIndex = i;
				if (onSelected != null) { 
					onSelected(this);
				}
				open = false;
			}
			r = r.MoveDown();
		}
		
		if (totalHeight > maxPixelHeight) { GUI.EndScrollView(); }
		
		GUI.PushSkin(GUI.blankSkin);
		if (GUI.Button(Screen.all, "")) { open = false; }
		GUI.PopSkin();	
		
	}
	
	
	
	
	
}
