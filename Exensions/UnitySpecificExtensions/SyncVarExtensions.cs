using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public static class SyncVarExtensions {

	public static void CopyFrom<T>(this SyncList<T> sl, IEnumerable<T> other) {
		sl.Clear();
		foreach(T current in other) {
			sl.Add(current);
		}
	}

}
