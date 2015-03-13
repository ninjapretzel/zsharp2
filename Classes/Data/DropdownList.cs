using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class DropdownList<T> : List<T> {
	
	public bool open = false;
	int selectedIndex = 0;
	Vector2 scrollPos;
	public float maxHeight = .25f;
	
	public GUIStyle topStyle { get { return GUI.skin.FindStyle("dropdown_top"); } }
	public GUIStyle bodyStyle { get { return GUI.skin.FindStyle("dropdown_body"); } }
	
	public string selectedLabel { get { return LabelOfElementAt(selectedIndex); } }
	public T selected { 
		get {
			if (Count == 0) { return default(T); }
			else if (selectedIndex <= 0) { return this[0]; }
			else if (selectedIndex > Count) { return this[Count-1]; }
			return this[selectedIndex];
		}
	}
	
	public Action<List<T>> onSelected = null;
	
	const string NULLSTR = "<NULL>";

	public DropdownList() : base() { Init(); }
	public DropdownList(int cap) : base(cap) { Init(); }
	public DropdownList(IEnumerable<T> collection) : base(collection) { Init(); }
	
	void Init() { 
		open = false;
		selectedIndex = 0;
	}
	
	public string LabelOfElementAt(int index) {
		if (index < 0) { return NULLSTR; }
		else if (index >= Count) { return NULLSTR; }
		
		T sel = this[index];
		string str = NULLSTR;
		if (sel != null) { str = sel.ToString(); }
		
		return str;
	}
	
	
	public void Draw(Rect baseArea) {
		if (GUI.Button(baseArea, selectedLabel, topStyle)) { open = !open; }
		
		if (open) { Selection(baseArea); }
		
	}
	
	public void Draw(params GUILayoutOption[] options) {
		if (GUILayout.Button(selectedLabel, topStyle, options)) { open = !open; }
		
		if (open) { Selection(GUILayoutUtility.GetLastRect()); }
		
	}
	
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