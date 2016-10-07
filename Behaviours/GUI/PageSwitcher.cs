using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PageSwitcher : MonoBehaviour {

	public static Action pause;

	/// <summary> History of GameObjects that have been visited as menus. </summary>
	private Stack<GameObject> history = new Stack<GameObject>();
	/// <summary> What is the current menu's GameObject </summary>
	private GameObject active { get { return history.Peek(); } }

	public string activeName { get { return active.name; } }

	/// <summary> Path of page resources. </summary>
	public string resourcePath = "Menus";
	string resPath { 
		get {
			if (resourcePath.Length > 0) { return resourcePath + "/"; }
			return "";
		}
	}

	/// <summary> Push a menu GameObject on the stack from a instance context </summary>
	public void Push(GameObject next) {
		if (history.Count > 0) {
			active.SetActive(false);
		}
		next.SetActive(true);
		history.Push(next);
	}
	
	/// <summary> Push a menu GameObject on the stack from Resources </summary>
	public void Push(string next) {
		Pushx(next);
	}

	/// <summary> Push a GameObject on the stack from resources, returns reference to newly created screen. </summary>
	/// <param name="next"></param>
	/// <returns></returns>
	public Transform Pushx(string next) {
		RectTransform t = Resources.Load<RectTransform>(resPath + next);
		if (t != null) {
			if (history.Count > 0) {
				active.SetActive(false);
			}
			RectTransform newObj = CreatePage(t);
			history.Push(newObj.gameObject);
			return newObj;
		}
		return null;
	}

	/// <summary> Instantiate a copy of <paramref name="t"/> as a child of this object, and anchor it to the corners of this object's RectTransform. </summary>
	/// <param name="t">source of page to create</param>
	/// <returns>Copy of <paramref name="t"/> as a child of this object, and anchored to the corners of this object's RectTransform </returns>
	private RectTransform CreatePage(RectTransform t) {
		RectTransform newObj = Instantiate(t) as RectTransform;
		newObj.name = newObj.name.Replace("(Clone)", "");
		newObj.SetParent(transform);
		newObj.anchorMin = new Vector2(0, 0);
		newObj.anchorMax = new Vector2(1, 1);
		newObj.offsetMin = new Vector2(0, 0);
		newObj.offsetMax = new Vector2(0, 0);
		return newObj;
	}

	/// <summary> Pop a menu GameObject from instance context, return nothing. </summary>
	public void Pop() { PopG(); }
	/// <summary> Pop a menu GameObject from instance context, return popped game object. </summary>
	public GameObject PopG() {
		if (history.Count < 1) { return null; }

		GameObject obj = history.Pop();
		obj.SetActive(false);

		if (history.Count >= 1 && active != null) {
			active.SetActive(true);
		}

		return obj;
	}

	/// <summary> Pop once, and destroy the page that was popped. </summary>
	public void PopAndDestroy() { Destroy(PopG()); }

	/// <summary> Pops all objects off of history until a given one is reached. </summary>
	/// <param name="target">Target object to pop to </param>
	public void PopTo(GameObject target) {
		while (history.Contains(target) && active != target) { Pop(); }
	}
	
	/// <summary> Pop to the game object that has a given name, if it exists. </summary>
	/// <param name="target">Name of target game object</param>
	public void PopTo(string target) {
		while (history.Where((g => g.name == target)).Count() > 0 && !(active.name == target)) {
			Pop();
		}
	}
	/// <summary> Pops all objects off of history until a given one is reached. </summary>
	/// <param name="target">Target object to pop to </param>
	public void PopToAndDestroyAll(GameObject target) {
		while (history.Contains(target) && active != target) { PopAndDestroy(); }
	}

	/// <summary> Pop to the game object that has a given name, if it exists. </summary>
	/// <param name="target">Name of target game object</param>
	public void PopToAndDestroyAll(string target) {
		while (history.Where((g => g.name == target) ).Count() > 0 && !(active.name == target)) {
			PopAndDestroy();
		}
	}

	/// <summary> Pop to the first page contained in <paramref name="targets"/> that exists. After one target makes changes, no following targets are used. </summary>
	/// <param name="targets">Comma separated list of names of target GUI pages to potentially pop to. </param>
	public void PopToFirstAndDestroy(string targets) {
		string[] tgs = targets.Split(',');

		int cnt = history.Count;

		foreach (var tg in tgs) {
			PopToAndDestroyAll(tg);
			if (cnt != history.Count) { return; }
		}


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

	/// <summary> Switch to a given menu GameObject from instance context, destroy popped GameObject. </summary>
	public void SwitchAndDestroy(GameObject obj) {
		var popped = PopG();
		Destroy(popped);
		Push(obj);
	}

	/// <summary> Switch to a given menu GameObject from Resources, destroy popped GameObject. </summary>
	public void SwitchAndDestroy(string str) {
		var popped = PopG();
		Destroy(popped);
		Push(str);
	}

	/// <summary> Switch to a given menu GameObject from Resources, destroy popped game object. </summary>
	public void Switch(string to) {
		RectTransform t = Resources.Load<RectTransform>(resPath + to);
		if (t != null) {
			RectTransform newObj = CreatePage(t);
			SwitchAndDestroy(newObj.gameObject);
		}
	}
	
}
