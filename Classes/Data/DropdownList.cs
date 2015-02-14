using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DropdownList<T> : List<T> {
	
	public bool open = false;
	int selectedIndex = 0;
	public T selected { 
		get {
			if (Count == 0) { return default(T); }
			else if (selectedIndex <= 0) { return this[0]; }
			else if (selectedIndex > Count) { return this[Count-1]; }
			return this[selectedIndex];
		}
	}
	
	
	const string NULLSTR = "<NULL>";
	public DropdownList() : base() {
		open = false;
		selectedIndex = 0;
	}
	
	public string LabelOfElementAt(int index) {
		if (index < 0) { return NULLSTR; }
		else if (index >= Count) { return NULLSTR; }
		
		T sel = this[index];
		string str = NULLSTR;
		if (sel != null) {
			str = sel.ToString();
		}
		return str;
	}
	
	public void Draw(params GUILayoutOption[] options) {
		
		string label = LabelOfElementAt(selectedIndex);
		//GUI.color = open ? Color.red : Color.blue;
		
		if (GUILayout.Button(label, options)) { open = !open; }
		
		if (open) {
			Rect r = GUILayoutUtility.GetLastRect();
			for (int i = 0; i < Count; i++) {
				string str = LabelOfElementAt(i);
				r = r.MoveDown();
				if (GUI.Button(r, str)) {
					selectedIndex = i;
					open = false;
				}
				
			}
			
			GUI.PushSkin(GUI.blankSkin);
			if (GUI.Button(Screen.all, "")) { open = false; }
			GUI.PopSkin();
		}
		
	}
	
	
	
}