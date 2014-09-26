using UnityEngine;
using System.Collections.Generic;
using System.Collections;

///Pans a material's texture coordinates
///Has an option to clamp the texture's coordinates between [0, 1]
///By default, will affect the mainTexture
///Other target texture2d samplers can be specified using the targets array
public class MaterialPanner : MonoBehaviour {
	public Vector2 speed = new Vector2(1, 0);
	public float scale = 1.0f;
	public bool clamp01 = true;
	
	public bool doMainTexture = true;
	public string[] targets;
	
	public Material material {
		get { return renderer.material; }
		set { renderer.material = value; }
	}
	
	void Awake() {
		if (targets == null) { targets = new string[0]; }
		if (renderer == null) { Destroy(this); return; }
		material = new Material(renderer.material);
	}
	
	void Start() {
		
	}
	
	void Update() {
		UpdateMaterial();
	}
	
	void UpdateMaterial() {
		if (doMainTexture) { Pan("_MainTex"); }
		foreach (string s in targets) { Pan(s); }
	}
	
	void Pan(string target) {
		if (material.HasProperty(target)) {
			Vector2 offset = material.GetTextureOffset(target);
			offset += speed * scale * Time.deltaTime;
			if (clamp01) {
				offset.x %= 1;
				offset.y %= 1;
			}
			material.SetTextureOffset(target, offset);
		}
	}
}
