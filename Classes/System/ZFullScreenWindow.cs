using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZFullScreenWindow : ZWindow {
	
	public override void Draw() {
		Predraw();
		
		if (open) { 
			Window();
		}
	}
	
	
	
	
	
}
