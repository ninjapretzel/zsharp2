using UnityEngine;
using System.Collections;

///Makes a material's color pulse over time.
public class MaterialPulse : MonoBehaviour {
	private Color baseColor;
	public string targetChannel = "_Emission";
	public Oscillator osc;
	public bool pulseAlpha = false;
	
	void Start() {
		if (!renderer) { Destroy(this);	return; }
		baseColor = renderer.material.GetColor(targetChannel);
	}
	
	void Update() {
		float val = osc.Update();
		Color c = baseColor * val;
		if (!pulseAlpha) {
			c.a = baseColor.a;
		}
		renderer.material.SetColor(targetChannel, c);
	}
	
	
}