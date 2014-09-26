using UnityEngine;
using System.Collections.Generic;
using System.Collections;

///Fades in or out renderers.
///By default, attempts to use 'Diffuse' and 'Transparent/Diffuse' shaders
///You may need to write your own shaders when using this, as opaque shaders simply can't be made transparent.
///
///The main way of using this behaviour is by using extension methods on a component, FadeIn() and FadeOut()
///It tries to be as magic as possible, but make note of where it doesn't work. 
///If it doesn't look right, it is probably grabbing the 'Transparent/Diffuse' shader, instead of one you want.
public class RendererFader : MonoBehaviour {
	
	public string baseShader = "Diffuse";
	public string transparentShader = "Transparent/Diffuse";
	
	public float fadeTime = 1;
	public bool wantsToBeVisible = true;
	
	float alpha;
	
	public float time = 1;

	
	public float percentage {
		get { return time / fadeTime; }  
	}
	
	void Awake() {
		if (wantsToBeVisible) { time = fadeTime; }
		else { time = 0; }
		//Support for a few shaders that are known can be added here
		if (renderer.material.shader.name == "Vertex Lit") {
			baseShader = "Vertex Lit";
			transparentShader = "Transparent/Vertex Lit";
		}
		
		if (renderer.material.shader.name == "GUI/Text Shader") {
			
			baseShader = "GUI/Text Shader";
			transparentShader = "GUI/Text Shader";
		}
		alpha = renderer.material.color.a;
	}
	
	void Start() {
		
	}
	
	void Update() {
		if (wantsToBeVisible) {
			time = Mathf.Min(fadeTime, time + Time.deltaTime);
			
		} else {
			time = Mathf.Max(0, time - Time.deltaTime);
			
		}
		
		if (time == 0 && renderer.enabled) { renderer.enabled = false; }
		else if (time > 0 && !renderer.enabled) { renderer.enabled = true; }
		
		if (time == fadeTime && renderer.material.shader.name != baseShader) {
			if (alpha * percentage >= 1) { SetBaseShader(); }
			
		} else if (time < fadeTime && renderer.material.shader.name != transparentShader) {
			SetTransparentShader();
			
		}
		
		if (renderer.materials.Length > 1) {
			for (int i = 0; i < renderer.materials.Length; i++) {
				Color c = renderer.materials[i].color;
				c.a = percentage * alpha;
				renderer.materials[i].color = c;
			}
		} else {
			Color c = renderer.material.color;
			c.a = percentage * alpha;
			renderer.material.color = c;
		}
		
	}
	
	
	void SetBaseShader() { SetShader(baseShader); }
	void SetTransparentShader() { SetShader(transparentShader); }
	
	void SetShader(string shader) {
		if (renderer.materials.Length > 1) {
			foreach (Material m in renderer.materials) {
				m.shader = Shader.Find(shader);
			}
		} else {
			renderer.material.shader = Shader.Find(shader);
		}
	}
	
	public void SetTime(float timeToFade) {
		float p = percentage;
		fadeTime = timeToFade;
		time = p * timeToFade;
	}
	
	public void FadeOut() {
		wantsToBeVisible = false;
	}
	
	public void FadeOut(float timeToFade) {
		SetTime(timeToFade);
		wantsToBeVisible = false;
	}
	
	public void FadeOut(float timeToFade, float position) {
		SetTime(timeToFade);
		time = position;
		wantsToBeVisible = false;
	}
	
	public void FadeIn() {
		wantsToBeVisible = true;
	}
	
	public void FadeIn(float timeToFade) {
		SetTime(timeToFade);
		wantsToBeVisible = true;
	}
	
	public void FadeIn(float timeToFade, float position) {
		SetTime(timeToFade);
		time = position;
		wantsToBeVisible = true;
	}
	
	
	
	
}


























