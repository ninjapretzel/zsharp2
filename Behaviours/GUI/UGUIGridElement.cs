using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class UGUIGrid {
	/// <summary> Width of the grid. Zero/Negative means infinite.  </summary>
	public int itemsWide;
	/// <summary> Height of the grid. Zero/Negative means infinite. </summary>
	public int itemsHigh;
	/// <summary> Pixels of padding on the left/right of the grid. </summary>
	public float hpadding;
	/// <summary> Pixels of padding on the top/bottom of the grid. </summary>
	public float vpadding;

	const float DEFAULT_SIZE = 64;
	/// <summary> Width of each element in the grid. Zero/Negative means calculate based off of parent. </summary>
	public float width = DEFAULT_SIZE;
	/// <summary> Height of each element in the grid. Zero/Negative means calculate based off of parent. </summary>
	public float height = DEFAULT_SIZE;

	/// <summary> Object that the grid exists within, if the objects contained are to resize with the parent. </summary>
	public RectTransform parent = null;

	public UGUIGrid() { itemsWide = 5; itemsHigh = -1; hpadding = 5; vpadding = 5; }
	public UGUIGrid(int wide, int high) : this() { itemsWide = wide; itemsHigh = high; }
	public UGUIGrid(int wide, int high, float hp, float vp) : this(wide, high) { hpadding = hp; vpadding = vp; }
	
	public UGUIGrid(RectTransform p) : this() { parent = p; width = -1; height = -1; }
	public UGUIGrid(RectTransform p, int wide, int high) : this(p) { itemsWide = wide; itemsHigh = high; }
	public UGUIGrid(RectTransform p,int wide, int high, float hp, float vp) : this(p, wide, high) { hpadding = hp; vpadding = vp; }

	/// <summary> Vector representing the Upper-Left corner in UGUI's coordinate system. </summary>
	private static Vector2 UPPER_LEFT = new Vector2(0, 1);

	/// <summary> Pivot of an element in the grid. For consistancy's sake, always Upper-Left </summary>
	public Vector2 pivot { get { return UPPER_LEFT; } } 

	/// <summary> Minimum anchor for all grid elements. For consistancy's sake, always Upper-Left </summary>
	public Vector2 anchorMin { get { return UPPER_LEFT; } } 
	
	/// <summary> Maximum anchor for all grid elements, based on how wide/high the grid is. </summary>
	public Vector2 anchorMax { get { return UPPER_LEFT; } } 

	/// <summary> Width of one element, based either on the width of the 'parent', or a fixed width. </summary>
	public float elementWidth { 
		get {
			//width based on parent's width
			if (width <= 0 && parent != null && itemsWide > 0) {
				float w = parent.rect.width;
				return (w - hpadding) / itemsWide;
			}
			//width based on parent's height
			if (width <= 0 && height <= 0 && parent != null && itemsHigh > 0) {
				float h = parent.rect.height;
				return (h - vpadding) / itemsHigh;
			} 

			//revert to default pixel width
			if (width <= 0) {
				width = DEFAULT_SIZE;
			}

			return width;
		}
	}

	/// <summary> Height of one element, based either on the height of the 'parent', or a fixed height. </summary>
	public float elementHeight {
		get {
			//height based on parent's height
			if (height <= 0 && parent != null && itemsHigh > 0) {
				float h = parent.rect.height;
				return (h - vpadding) / itemsHigh;
			}
			//height based on parent's width
			if (height <= 0 && width <= 0 && parent != null && itemsWide > 0) {
				float w = parent.rect.width;
				return (w - hpadding) / itemsWide;
			}

			//revert to default pixel width
			if (height <= 0) {
				height = DEFAULT_SIZE;
			}

			return height;
		}
	}

	/// <summary> Get the offsetMin vector for a grid element at position (x, y) </summary>
	public Vector2 OffsetMin(int x, int y) {
		float xoff = hpadding + x * elementWidth;
		float yoff = -vpadding - (y+1) * elementHeight;

		return new Vector2(xoff, yoff);
	}
	
	/// <summary> Get the offsetMax vector for a grid element at position (x, y) </summary>
	public Vector2 OffsetMax(int x, int y) {
		float xoff = hpadding + (x+1) * elementWidth;
		float yoff = -vpadding - y * elementHeight;

		return new Vector2(xoff, yoff);
	}


	/// <summary> Sets up some component on this grid. </summary>
	/// <param name="obj">object to set up </param>
	/// <param name="x"> x coordinate </param>
	/// <param name="y"> y coordinate </param>
	public void SetUp(Component obj, int x, int y) {
		var ge = obj.Require<UGUIGridElement>();
		ge.grid = this;
		ge.x = x;
		ge.y = y;
		ge.regrow = true;
	}

	/// <summary> Resets the size of the grid object to ZERO, and allow it to auto set its size on the next frame. </summary>
	public void ResetSize() {
		parent.offsetMin = Vector2.zero;
		parent.offsetMax = Vector2.zero;
	}

	/// <summary> Remove all objects that are under the grid. </summary>
	public void Clear() {
		parent.DeleteAllChildren();
		ResetSize();
	}

	/// <summary> Grows the grid to hold this object if it is larger than the grid </summary>
	/// <param name="rt"></param>
	public void Grow(RectTransform rt) {
		if (rt.offsetMin.y < parent.offsetMin.y) {
			parent.offsetMin = new Vector2(0, rt.offsetMin.y);
		}

		if (rt.offsetMax.y > parent.offsetMax.x) {
			parent.offsetMax = new Vector2(rt.offsetMax.x, 0);
		}
	}
	
}


public class UGUIGridElement : MonoBehaviour {

	public int x;
	public int y;

	public UGUIGrid grid = null;
	int lastWidth;
	int lastHeight; 

	[NonSerialized] public bool regrow = false;

	void Start() {
		lastWidth = -1;
		lastHeight = -1;
	}
	
	void Update() {
		if (lastWidth != Screen.width || lastHeight != Screen.height) { 
			//Pre-Align.
			grid.ResetSize();
			regrow = true;
		} else if (regrow) {
			grid.Grow(transform as RectTransform);
			regrow = false;
		}
	}

	void LateUpdate() {
		if (lastWidth != Screen.width || lastHeight != Screen.height && regrow) { Align(); }
		
		lastWidth = Screen.width;
		lastHeight = Screen.height;
	}
	
	void Align() {
		if (grid == null) {
			Debug.LogWarning("Grid element " + gameObject.name + " does not have a grid assigned.");
			return;
		}

		RectTransform rt = transform as RectTransform;

		rt.pivot = grid.pivot;
		rt.anchorMin = grid.anchorMin;
		rt.anchorMax = grid.anchorMax;
		
		rt.offsetMin = grid.OffsetMin(x, y);
		rt.offsetMax = grid.OffsetMax(x, y);


	}

}
