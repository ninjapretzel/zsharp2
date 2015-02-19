using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class Reactor {
	
	public class Reaction {
		MethodInfo info;
		object target;
		object[] parameters;
		
		public Reaction(MethodInfo i, object t, object[] p) {
			info = i;
			target = t;
			parameters = p;
		}
		
		public void React() { info.Invoke(target, parameters); }
		
	}
	
	object mutex;
	Queue<Reaction> reactions;
	
	public Reactor() {
		mutex = new object();
		reactions = new Queue<Reaction>();
	}
	
	public void Add(MethodInfo m, object t, object[] p) {
		lock (mutex) {
			reactions.Enqueue(new Reaction(m, t, p)); 
		}
	}
	
	public void React() {
		lock (mutex) {
			Debug.Log("Reacting");
			while (reactions.Count > 0) {
				reactions.Dequeue().React();
			}
		}
	}
	
	
}

public static class ReactorUtils {
	static BindingFlags methodsToGrab = BindingFlags.Public 
								| BindingFlags.NonPublic
								| BindingFlags.Static
								| BindingFlags.Instance;
								
	public static void AddReaction(this object o, Reactor r, string func, params object[] p) {
		if (o == null) { return; }
		if (r == null) { return; }
		MethodInfo m = null;
		if (o.GetType() == typeof(Type)) {
			Type t = o as Type;
			m = t.GetMethod(func, methodsToGrab);
		} else {
			m = o.GetMethod(func, methodsToGrab);
			//
		}
		if (m == null) { return; }
		r.Add(m, o, p);
	}
	
}