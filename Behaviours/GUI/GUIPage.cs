using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIPage : MonoBehaviour {

	PageSwitcher _context = null;
	public PageSwitcher context { 
		get {
			if (_context != null) { return _context; }

			var t = transform;
			while (t.parent != null) {
				t = t.parent;
				var c = t.GetComponent<PageSwitcher>();
				if (c != null) { _context = c; break; }
				Debug.Log(c);
			}
			return _context;
		}
	}
	public PageSwitcher parentContext {
		get {
			var t = context.transform;
			while (t.parent != null) {
				t = t.parent;
				var c = t.GetComponent<PageSwitcher>();
				if (c != null) { return c; }
				Debug.Log(c);
			}
			return null;
		}
	}
	public PageSwitcher rootContext {
		get {
			Stack<Transform> ancestry = new Stack<Transform>();
			var t = transform;
			ancestry.Push(t);
			while (t.parent != null) {
				t = t.parent;
				ancestry.Push(t);
			}

			while (ancestry.Count > 0) {
				Transform current = ancestry.Pop();
				PageSwitcher context = current.GetComponent<PageSwitcher>();
				if (context != null) {
					return context;
				}
			}

			return null;
		}
	}

	public void Push(string page) { context.Push(page); }
	public void Push(GameObject next) { context.Push(next); }
	public Transform Pushx(string next) { return context.Pushx(next); }

	public void Pop() { context.Pop(); }
	public GameObject PopG() { return context.PopG(); }
	public void PopAndDestroy() { context.PopAndDestroy(); }

	public void PopTo(GameObject target) { context.PopTo(target); }
	public void PopTo(string target) { context.PopTo(target); }
	public void PopToAndDestroyAll(GameObject target) { context.PopToAndDestroyAll(target); }
	public void PopToAndDestroyAll(string target) { context.PopToAndDestroyAll(target); }
	public void PopToFirstAndDestroy(string targets) { context.PopToFirstAndDestroy(targets); }
	

	public void Switchx(GameObject obj) { context.Switch(obj); }
	public GameObject Switch(GameObject obj) { return context.Switch(obj); }
	public void SwitchAndDestroy(GameObject obj) { context.SwitchAndDestroy(obj); }
	public void SwitchAndDestroy(string str) { context.SwitchAndDestroy(str); }
	public void Switch(string to) { context.Switch(to); }


}
