using UnityEngine;
using System.Collections;

///Makes a material's color pulse over time.
public class MaterialPulse : MonoBehaviour {
	public int targetMaterial = 0;
	
	private Color baseColor;
	public string targetChannel = "_Emission";
	public Oscillator osc;
	public bool pulseAlpha = false;
	
	void Start() {
		if (!renderer) { Destroy(this);	return; }
		baseColor = renderer.materials[targetMaterial].GetColor(targetChannel);
	}
	
	void Update() {
		float val = osc.Update();
		Color c = baseColor * val;
		if (!pulseAlpha) {
			c.a = baseColor.a;
		}
		renderer.materials[targetMaterial].SetColor(targetChannel, c);
	}
	
	
}