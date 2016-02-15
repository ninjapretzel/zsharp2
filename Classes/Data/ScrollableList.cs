using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary> Legacy GUI scrollable selection list. </summary>
public class ScrollableList<T> : List<T> {

	/// <summary> Height of one element </summary>
	public float height = 30;
	/// <summary> Style of the body. Uses the same style as dropdowns for consistancy. </summary>
	public GUIStyle bodyStyle { get { return GUI.skin.FindStyle("dropdown_body"); } }

	/// <summary> Color of the selected element </summary>
	public Color highlightColor = Color.white;
	/// <summary> Color of regular elements </summary>
	public Color normalColor = new Color(.75f, .75f, .75f);

	/// <summary> Currently selected index </summary>
	int selectedIndex = 0;
	/// <summary> Scroll Position </summary>
	Vector2 scrollPos;

	/// <summary> Label of selected element </summary>
	public string selectedLabel { get { return LabelOfElementAt(selectedIndex); } }
	/// <summary> Actual selected element </summary>
	public T selected { 
		get {
			if (Count == 0) { return default(T); }
			else if (selectedIndex <= 0) { return this[0]; }
			else if (selectedIndex > Count) { return this[Count-1]; }
			return this[selectedIndex];
		}
	}

	/// <summary> Callback for when an element is selected. </summary>
	public Action<List<T>> onSelected = null;

	/// <summary> Display for 'null' elements </summary>
	const string NULLSTR = "<NULL>";
	
	public ScrollableList() : base() { Init(); }
	public ScrollableList(int cap) : base(cap) { Init(); }
	public ScrollableList(IEnumerable<T> collection) : base(collection) { Init(); }

	/// <summary> Common initialization </summary>
	public void Init() {
		selectedIndex = 0;
	}

	/// <summary> Get the label (via ToString) of an element at a given index </summary>
	public string LabelOfElementAt(int index) {
		if (index < 0) { return NULLSTR; }
		else if (index >= Count) { return NULLSTR; }
		
		T sel = this[index];
		string str = NULLSTR;
		if (sel != null) { str = sel.ToString(); }
		
		return str;
	}

	/// <summary> Set the selected index based to the index of the given object, or zero if that object is not in the list. </summary>
	public void SetIndexTo(object o) {
		for (int i = 0; i < Count; i++) {
			if (this[i].Equals(o)) {
				selectedIndex = i;
				return;
			}
		}
		selectedIndex = 0;
	}

	public void Draw() { DrawOptions(null); }
	public void DrawOptions(params GUILayoutOption[] options) {
		if (options == null) {
			options = new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(80) };
		}
		GUILayout.Box("", options);
		Draw(GUILayoutUtility.GetLastRect());
	}

	/// <summary> Draw and handle selection in a given area on the screen. </summary>
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
