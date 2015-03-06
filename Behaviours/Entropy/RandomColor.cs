using UnityEngine;
using System.Collections;

public class RandomColor : MonoBehaviour {
	public Color[] colors;
	public BMM alpha = new BMM(false, .5f, 1);
	public string target;
	
	public bool onAwake = false;
	public bool useSeed = false;
	public static int seed = 1231241;
	
	void Awake() {
		if (onAwake) { SetColor(); }
	}
	
	void Start() {
		SetColor();
	}
	
	void SetColor() {
		if (useSeed) { Random.PushSeed(seed); }
		
		if (GetComponent<Renderer>().material.HasProperty(target)) { 
			Color c = colors.Lerp(Random.value);
			c.a = alpha.value;
			GetComponent<Renderer>().material.SetColor(target, c); 
		}
		
		
		if (useSeed) { Random.PopSeed(); seed++; }
		Destroy(this);
	}
	
}
