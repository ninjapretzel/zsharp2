using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GUIEvent {

	public static Event evt { get { return Event.current; } }

	public static bool alt { get { return evt.alt; } }
	public static bool shift { get { return evt.shift; } }
	public static bool control { get { return evt.control; } }
	public static bool ctrl { get { return evt.control; } }

	public static int button { get { return evt.button; } }
	public static char character { get { return evt.character; } }


	public static bool leftClickDown { get { return evt.type == EventType.MouseDown && evt.button == 0; } }
	public static bool leftClickUp { get { return evt.type == EventType.MouseUp && evt.button == 0; } }

	public static bool rightClickDown { get { return evt.type == EventType.MouseDown && evt.button == 1; } }
	public static bool rightClickUp { get { return evt.type == EventType.MouseUp && evt.button == 1; } }

	public static void Use() { evt.Use(); }
	


}
