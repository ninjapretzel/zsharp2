using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GUIEvent {

	public static Event evt { get { return Event.current; } }

	public static bool alt { get { return evt.alt; } }
	public static bool shift { get { return evt.shift; } }
	public static bool control { get { return evt.control; } }
	public static bool command { get { return evt.command; } }
	public static bool ctrl { get { return evt.control; } }
	public static bool caps { get { return evt.capsLock; } }
	public static bool capsLock { get { return evt.capsLock; } }
	public static bool numeric { get { return evt.numeric; } }
	
	public static int button { get { return evt.button; } }
	public static char character { get { return evt.character; } }
	public static KeyCode keyCode { get { return evt.keyCode; } }
	
	public static string commandName { get { return evt.commandName; } }

	public static Vector2 delta { get { return evt.delta; } }
	public static Vector2 mousePosition { get { return evt.mousePosition; } }


	public static bool repaint { get { return evt.type == EventType.Repaint; } }
	public static bool layout { get { return evt.type == EventType.Layout; } }
	public static bool ignore { get { return evt.type == EventType.Ignore; } }
	public static bool used { get { return evt.type == EventType.Used; } }

	public static bool mouseDrag { get { return evt.type == EventType.MouseDrag; } }
	public static bool mouseMove { get { return evt.type == EventType.MouseMove; } }
	public static bool mouseDown { get { return evt.type == EventType.MouseDown; } }
	public static bool mouseUp { get { return evt.type == EventType.MouseUp; } }
	public static bool keyUp { get { return evt.type == EventType.KeyUp; } }
	public static bool keyDown { get { return evt.type == EventType.KeyDown; } }
	public static bool scrollWheel { get { return evt.type == EventType.ScrollWheel; } }


	public static bool clickDown { get { return evt.type == EventType.MouseDown; } }
	public static bool clickUp { get { return evt.type == EventType.MouseUp; } }

	public static bool leftClickDown { get { return evt.type == EventType.MouseDown && evt.button == 0; } }
	public static bool leftClickUp { get { return evt.type == EventType.MouseUp && evt.button == 0; } }

	public static bool middleClickDown { get { return evt.type == EventType.MouseDown && evt.button == 2; } }
	public static bool middleClickUp { get { return evt.type == EventType.MouseUp && evt.button == 2; } }

	public static bool rightClickDown { get { return evt.type == EventType.MouseDown && evt.button == 1; } }
	public static bool rightClickUp { get { return evt.type == EventType.MouseUp && evt.button == 1; } }

	public static void Use() { evt.Use(); }
	


}
