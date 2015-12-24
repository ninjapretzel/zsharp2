using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Anchors {
	/// <summary> returns new Rect(0, 0, 1, 1) </summary>
	public static Rect stretchAll { get { return new Rect(0, 0, 1, 1); } }
	/// <summary> returns new Rect(.5f, .5f, .5f, .5f) </summary>
	public static Rect middleCenter { get { return new Rect(.5f, .5f, .5f, .5f); } }


}

#if XtoJSON
public static class UGUIz {

	/// <summary> Current skin being used by all factory functions. </summary>
	public static UGUIzSkin skin = UGUIzSkin.standard;
	/// <summary> Layer that the UI is on </summary>
	const int LAYER = 5;
	/// <summary> Text scale size (larger bitmap text, resized to prevent pixelation) </summary>
	const float textScaling = 4f;
	
	/// <summary> returns new Rect(0, 0, 1, 1) </summary>
	public static Rect stretchAll { get { return Anchors.stretchAll; } }
	/// <summary> returns new Rect(.5f, .5f, .5f, .5f) </summary>
	public static Rect middleCenter { get { return Anchors.middleCenter; } }

	private static Canvas _canvas = null;
	private static Transform _eventSystem = null;
	/// <summary> Gets the canvas object for UGUIz. Creates it if it does not already exist. </summary>
	public static Canvas canvas { 
		get { 
			if (_canvas == null) {
				_canvas = MakeCanvas();
				GameObject.DontDestroyOnLoad(_canvas.gameObject);
				_eventSystem = MakeEventSystem();
				GameObject.DontDestroyOnLoad(_eventSystem.gameObject);
			}
			return _canvas;
		}
	}

	#region initialization

	private static Canvas MakeCanvas() {
		var obj = new GameObject("UGUIz Canvas");
		obj.layer = LAYER;
		obj.AddComponent<RectTransform>();
		Canvas cvs = obj.AddComponent<Canvas>();
		cvs.renderMode = RenderMode.ScreenSpaceOverlay;
		cvs.pixelPerfect = true;
		cvs.sortingOrder = 0;
		cvs.worldCamera = Camera.main;

		CanvasScaler cs = obj.AddComponent<CanvasScaler>();
		cs.referencePixelsPerUnit = 100;
		cs.scaleFactor = 1;

		GraphicRaycaster gr = obj.AddComponent<GraphicRaycaster>();
		gr.ignoreReversedGraphics = true;
		gr.blockingObjects = GraphicRaycaster.BlockingObjects.None;
		
		return cvs;
	}

	private static Transform MakeEventSystem() {
		var obj = new GameObject("Event System");
		obj.layer = LAYER;
		EventSystem es = obj.AddComponent<EventSystem>();
		StandaloneInputModule sim = obj.AddComponent<StandaloneInputModule>();
		//TouchInputModule tim = obj.AddComponent<TouchInputModule>();

		es.firstSelectedGameObject = null;
		es.sendNavigationEvents = true;
		es.pixelDragThreshold = 5;

		sim.horizontalAxis = "Horizontal";
		sim.verticalAxis = "Vertical";
		sim.submitButton = "Submit";
		sim.cancelButton = "Cancel";
		sim.inputActionsPerSecond = 10;
		sim.repeatDelay = .5f;
		sim.forceModuleActive = false;

		//tim.forceModuleActive = false;


		return obj.transform;
	}
	#endregion


	
	/// <summary>
	/// Makes a text object in a given area with a given string inside of it.
	/// </summary>
	/// <param name="area">Rect (from center) where the object is created. </param>
	/// <param name="label">Text to put on the label. </param>
	/// <returns>The transform of the created object. </returns>
	public static RectTransform Label(Rect area, string label = "Text") {
		var textObj = new GameObject("Text");
		textObj.layer = LAYER;

		RectTransform textRect = textObj.AddComponent<RectTransform>().PositionRect(area);
		
		Text text = textObj.AddComponent<Text>();
		text.font = skin.font;
		text.horizontalOverflow = HorizontalWrapMode.Overflow;
		text.verticalOverflow = VerticalWrapMode.Overflow;
		text.alignment = TextAnchor.MiddleCenter;
		text.fontSize = (int)(16 * textScaling);
		textRect.localScale *= 1 / textScaling;
		text.text = label;

		return textRect;
	}

	/// <summary>
	/// Creates a box with an optional label in a given area. 
	/// </summary>
	/// <param name="area">Rect (from center) where the object is created. </param>
	/// <param name="label">Text to put on the label. </param>
	/// <param name="style">Style to use from the current skin</param>
	/// <returns>The transform of the created object. </returns>
	public static RectTransform Box(Rect area, string label = "Panel", string style = "box") {
		var obj = new GameObject("Panel");
		RectTransform rect = obj.AddComponent<RectTransform>().PositionRect(area);
		rect.AddImage(style);
		
		var textObj = Label(area, label);
		textObj.SetParent(rect);

		return rect;
	}

	/// <summary>
	/// Creates a clickable button in the current style
	/// </summary>
	/// <param name="area">Rect (from center) where the object is created.</param>
	/// <param name="label">Text to put on the label</param>
	/// <param name="onClick">Function to execute when clicked</param>
	/// <param name="style">Style to use from the current skin </param>
	/// <returns>Transform of the created object. </returns>
	public static RectTransform Button(Rect area, string label = "Button", UnityAction onClick = null, string style = "button") {
		var obj = new GameObject("Button");
		obj.layer = LAYER;
		RectTransform rect = obj.AddComponent<RectTransform>().PositionRect(area);

		rect.AddImage(style).MakeClickable(onClick, style);
		
		var textObj = Label(area, label);
		textObj.SetParent(rect);

		return rect;
	}


	public static RectTransform MakeScrollView(Rect area, bool scrollHorizontal, bool scrollVertical) {
		var obj = new GameObject("ScrollView");
		obj.layer = LAYER;
		RectTransform rect = obj.transform.PositionRect(area, canvas.transform);
		
		if (scrollHorizontal) {
			Rect hScrollArea = area.BottomLeft(.95f, .05f);
			MakeScrollBar(hScrollArea, true).PositionRect(hScrollArea, obj.transform);
		}

		if (scrollVertical) {
			Rect vScrollArea = area.UpperRight(.05f, .95f);
			MakeScrollBar(vScrollArea, false).PositionRect(vScrollArea, obj.transform);
		}

		var scrollWindow = Box(area, "", "background"); 
		scrollWindow.gameObject.layer = LAYER;

		return rect;
	}


	public static RectTransform MakeScrollBar(Rect area, bool isHorizontal, string style = "background", string handleStyle = "handle", UnityAction<float> onValChange = null) {
		var obj = new GameObject((isHorizontal ? "Horizontal" : "Vertical") + "ScrollBar");
		obj.layer = LAYER;
		RectTransform rect = obj.transform.PositionRect(area);
		rect.AddImage(style);
		
		RectTransform scrollArea = new GameObject("Scroll Area").transform.PositionRect(area, stretchAll, rect);
		RectTransform handle = scrollArea.Box(area, "", handleStyle);

		Scrollbar scrollBar = obj.AddComponent<Scrollbar>();
		scrollBar.interactable = true;
		scrollBar.targetGraphic = handle.GetComponent<Image>();
		scrollBar.transition = Selectable.Transition.ColorTint;
		//ColorBlock cblock = skin.GetColorBlock(handleStyle);
		
		scrollBar.handleRect = handle;
		scrollBar.direction = isHorizontal ? Scrollbar.Direction.LeftToRight : Scrollbar.Direction.TopToBottom;
		scrollBar.value = 0;
		scrollBar.size = .2f;
		scrollBar.numberOfSteps = 0;

		if (onValChange != null) { scrollBar.onValueChanged.AddListener(onValChange); }
		
		return rect;
	}


	#region Extensions
	/// <summary>
	/// Creates a label with the current skin, and attaches it relative to a given object.
	/// </summary>
	/// <param name="t"> Object to attach relative to </param>
	/// <param name="area"> relative area to create the new object </param>
	/// <param name="label"> label text </param>
	/// <returns> Reference to the parent of the new object. </returns>
	public static RectTransform Label(this RectTransform t, Rect area, string label = "Label") {
		Label(area, label).PositionRect(area, t);
		return t;
	}

	/// <summary>
	/// Creates a box with the current skin, attaches it relative to a given object.
	/// </summary>
	/// <param name="t"> Object to attach relative to </param>
	/// <param name="area"> relative area to create the new object </param>
	/// <param name="label"> label text </param>
	/// <returns> Reference to the parent of the new object. </returns>
	public static RectTransform Box(this RectTransform t, Rect area, string label = "", string style = "box") {
		Box(area, label, style).PositionRect(area, t);
		return t;
	}

	/// <summary>
	/// Creates a button with the current skin, attaches it relative to a given object.
	/// </summary>
	/// <param name="t"> Object to attach relative to </param>
	/// <param name="area"> relative area to create the new object </param>
	/// <param name="label"> label text </param>
	/// <param name="onClick"> action to attach to the created button</param>
	/// <returns> Reference to the parent of the new object. </returns>
	public static RectTransform Button(this RectTransform t, Rect area, string label = "Button", UnityAction onClick = null, string style = "Button") {
		Button(area, label, onClick, style).PositionRect(area, t);
		return t;
	}


	/// <summary>
	/// Adds an UGUI Image to a given GameObject
	/// </summary>
	/// <param name="c">Component on the target GameObject</param>
	/// <param name="style">image style in current skin</param>
	/// <param name="type">image rendering method to use</param>
	/// <returns>The Image component that was added</returns>
	public static Image AddImage(this Component c, string style, Image.Type type = Image.Type.Sliced) {
		Image img = c.gameObject.AddComponent<Image>();
		img.sprite = skin.GetSprite(style);
		img.type = type;
		img.fillCenter = true;
		return img;
	}

	/// <summary>
	/// Adds a UGUI Button component to a given GameObject.
	/// Checks the current skin for a sprite block for the given style.
	/// If a sprite block exists, it applies a sprite transition to the Button.
	/// If not, it applies a color transition to the button.
	/// </summary>
	/// <param name="c">Component on the target GameObject</param>
	/// <param name="onClick">Action for the button to do when clicked</param>
	/// <param name="style">style to use in the current skin</param>
	/// <returns>The Button component that was added</returns>
	public static Button MakeClickable(this Component c, UnityAction onClick = null, string style = "") {
		Button btn = c.AddComponent<Button>();

		if (onClick != null) { btn.onClick.AddListener(onClick); }

		if (style != "" && skin.HasSpriteBlock(style)) {
			btn.transition = Selectable.Transition.SpriteSwap;
			btn.spriteState = skin.GetSpriteBlock(style);
		} else {
			btn.transition = Selectable.Transition.ColorTint;
			btn.colors = skin.GetColorBlock(style);
		}

		return btn;
	}

	/// <summary>
	/// Positions a GameObject with a RectTransform in a given area, relative to some object.
	/// Defaults to position relative to the canvas if no object is passed.
	/// </summary>
	/// <param name="c">Component on the target GameObject</param>
	/// <param name="area">Area to position to. (x,y) relative to the parent object, (width,height) gets set to the sizeDelta of the RectTransform</param>
	/// <param name="parent">Optional parameter to position relative to. Defaults to the UGUIz canvas object </param>
	/// <returns>The RectTransform on the positioned object</returns>
	public static RectTransform PositionRect(this Component c, Rect area, Transform parent = null) {
		return c.PositionRect(area, new Rect(.5f, .5f, .5f, .5f), parent);
	}

	
	public static RectTransform PositionRect(this GameObject g, Rect area, Transform parent = null) {
		return g.transform.PositionRect(area, new Rect(.5f, .5f, .5f, .5f), parent);
	}

	/// <summary>
	/// Positions a GameObject with a RectTransform in a given area, relative to some object.
	/// Defaults to position relative to the canvas if no object is passed.
	/// Sets the anchors to the passed rect where Min = (x,y) and Max = (width,height)
	/// </summary>
	/// <param name="c">Component on the target GameObject</param>
	/// <param name="area">Area to position to. (x,y) relative to the parent object, (width,height) gets set to the sizeDelta of the RectTransform</param>
	/// <param name="parent">Optional parameter to position relative to. Defaults to the UGUIz canvas object </param>
	/// <param name="anchors">Anchors for the rect transform</param>
	/// <returns>The RectTransform on the positioned object</returns>
	public static RectTransform PositionRect(this Component c, Rect area, Rect anchors, Transform parent = null) {
		RectTransform rect = c.Require<RectTransform>();

		Vector3 pos = new Vector3(area.x, area.y, 0);
		Vector2 size = new Vector2(area.width, area.height);

		rect.SetParent(parent == null ? canvas.transform : parent);

		rect.localPosition = pos;
		rect.sizeDelta = size;
		return rect;
	}
	
	

	#endregion


}


#endif
