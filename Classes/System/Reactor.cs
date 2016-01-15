using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

/// <summary> Reactor. Has a queue of 'Reactions'. Messages are sent into this object, and handled when possible (later). 
/// Intended for use with multi-threaded parts of the game. </summary>
public class Reactor {

	/// <summary> Information for a single 'Reaction' </summary>
	public class Reaction {
		/// <summary> Method to call via Reflection </summary>
		private MethodInfo info;
		/// <summary> Target object to call message on </summary>
		private object target;
		/// <summary> Params to call in reaction </summary>
		private object[] parameters;

		/// <summary> Constructor </summary>
		public Reaction(MethodInfo i, object t, object[] p) {
			info = i;
			target = t;
			parameters = p;
		}

		/// <summary> Invoke the method, on the target, with the given parameters. </summary>
		public void React() { info.Invoke(target, parameters); }
		
	}

	/// <summary> Mutex Lock </summary>
	private object mutex;
	/// <summary> Queue of reactions </summary>
	private Queue<Reaction> reactions;
	

	public Reactor() {
		mutex = new object();
		reactions = new Queue<Reaction>();
	}

	/// <summary> Add a given reaction to the queue </summary>
	public void Add(MethodInfo m, object t, object[] p) {
		lock (mutex) {
			reactions.Enqueue(new Reaction(m, t, p)); 
		}
	}

	/// <summary> Add a reaction to the queue by reflecting the name of the desired function.</summary>
	public void AddReaction(object target, string func, params object[] p) {
		if (target == null) { return; }
		MethodInfo m = ReactorUtils.GetFunction(target, func);
		Add(m, target, p);
	}

	/// <summary> Process all queued events.</summary>
	public void React() {
		while (reactions.Count > 0) {
			Reaction r;
			lock (mutex) {
				r = reactions.Dequeue();
			}
			if (r != null) { r.React(); }
		}
	}
	
}


/// <summary> Helper and extension methods for Reactor </summary>
public static class ReactorUtils {
	/// <summary> BindingFlags that are used. </summary>
	static BindingFlags methodsToGrab = BindingFlags.Public 
								| BindingFlags.NonPublic
								| BindingFlags.Static
								| BindingFlags.Instance;

	/// <summary> Extension method on object to add a reaction to a given Reactor, by reflecting the desired method. </summary>
	public static void AddReaction(this object o, Reactor r, string func, params object[] p) {
		if (o == null) { return; }
		if (r == null) { return; }
		MethodInfo m = GetFunction(o, func);
		if (m == null) { return; }
		
		r.Add(m, o, p);
	}

	/// <summary> Get a function from an object by name </summary>
	internal static MethodInfo GetFunction(object o, string func) {
		MethodInfo m = null;
		if (o.GetType() == typeof(Type)) {
			Type t = o as Type;
			m = t.GetMethod(func, methodsToGrab);
		} else {
			m = o.GetMethod(func, methodsToGrab);
		}
		return m;
	}
	
}
