using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ScrollableList<T> : List<T> {
	
	public float height = 30;
	public GUIStyle bodyStyle { get { return GUI.skin.FindStyle("dropdown_body"); } }
	public Color highlightColor = Color.white;
	public Color normalColor = new Color(.75f, .75f, .75f);
	
	int selectedIndex = 0;
	Vector2 scrollPos;
	
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
	
	public ScrollableList() : base() { Init(); }
	public ScrollableList(int cap) : base(cap) { Init(); }
	public ScrollableList(IEnumerable<T> collection) : base(collection) { Init(); }
	
	
	public void Init() {
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
	
	public void SetIndexTo(object o) {
		for (int i = 0; i < Count; i++) {
			if (this[i].Equals(o)) {
				selectedIndex = i;
				return;
			}
		}
		selectedIndex = 0;
	}
	
	public void Draw(Rect area) {
		GUIStyle style = bodyStyle;
		
		Vector2 size = style.CalcSize(new GUIContent("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"));
		Rect brush = new Rect(0, 0, area.width - 20, size.y);
		Rect viewRect = brush;
		viewRect.height *= Count;
		
		GUI.PushColor(normalColor);
		scrollPos = GUI.BeginScrollView(area, scrollPos, viewRect); {
			for (int i = 0; i < Count; i++) {
				if (selectedIndex == i) { GUI.PushColor(highlightColor); }
				if (GUI.Button(brush, LabelOfElementAt(i), style)) {
					selectedIndex = i;
					if (onSelected != null) {
						onSelected(this);
					}
				}
				if (selectedIndex == i) { GUI.PopColor(); }
				brush = brush.MoveDown();
				
			}
		} GUI.EndScrollView();
		GUI.PopColor();
	}
	
}
