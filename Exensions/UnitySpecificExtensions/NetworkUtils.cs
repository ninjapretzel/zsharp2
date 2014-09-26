using UnityEngine;
using System.Collections;

public static class NetworkUtils {
	
	public static string Details(this NetworkPlayer p) {
		string str = "networkPlayer : {";
		str += "\nid:" + p.ToString();
		str += "\nexternalIP:" + p.externalIP;
		str += "\nexternalPort:" + p.externalPort;
		str += "\nguid:" + p.guid;
		str += "\nipAddress:" + p.ipAddress;
		str += "\nport:" + p.port;
		str += "\n}";
		return str;
	}
	
}
