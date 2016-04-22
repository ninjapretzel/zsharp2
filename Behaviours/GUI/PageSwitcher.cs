using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class PageSwitcher : MonoBehaviour {

	public static Action pause;

	/// <summary> History of GameObjects that have been visited as menus. </summary>
	private static Stack<GameObject> history = new Stack<GameObject>();
	/// <summary> What is the current menu's GameObject </summary>
	private static GameObject active { get { return history.Peek(); } }



	/// <summary> Push a menu GameObject on the stack from a instance context </summary>
	public void Push(GameObject next) {
		if (history.Count > 0) {
			active.SetActive(false);
		}
		next.SetActive(true);
		history.Push(next);
	}

	/// <summary> Pop a menu GameObject from instance context, return nothing. </summary>
	public void Pop() { PopG(); }
	/// <summary> Pop a menu GameObject from instance context, return popped game object. </summary>
	public GameObject PopG() {
		if (history.Count <= 1) { return null; }

		GameObject obj = history.Pop();
		obj.SetActive(false);

		if (active != null) {
			active.SetActive(true);
		}

		return obj;
	}

	public void PopTo(GameObject target) {
		while (history.Contains(target) && active != target) { Pop(); }
	}

	/// <summary> Switch to a given menu GameObject from instance context, return nothing. </summary>
	public void Switchx(GameObject obj) {
		Pop();
		Push(obj);
	}

	/// <summary> Switch to a given menu GameObject from instance context, return popped game object. </summary>
	public GameObject Switch(GameObject obj) {
		var popped = PopG();
		Push(obj);
		return popped;
	}


	void Start() {
		
	}
	
	void Update() {

		
	}
	
}
