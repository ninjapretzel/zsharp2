using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MassParticles : MonoBehaviour {
	
	public ParticleMaker[] sources;
	public bool worldSpace = false;
	public bool alwaysVisible = false;
	
	
	void Start() {
		SetParticles();
		
	}
	
	void Update() {
		SetParticles();
		
		if (alwaysVisible) { 
			Camera cam = Camera.main;
			float dist = (cam.nearClipPlane + cam.farClipPlane) / 2f;
			transform.position = cam.transform.position + cam.transform.forward * dist;
			
		}
		
		
		if (worldSpace) {
			particleSystem.simulationSpace = ParticleSystemSimulationSpace.World;
		} else {
			particleSystem.simulationSpace = ParticleSystemSimulationSpace.Local;
		}
		
	}
	
	void SetParticles() {
		if (sources != null) {
			ParticleSystem.Particle[] parts = new ParticleSystem.Particle[sources.Length];
			
			for (int i = 0; i < sources.Length; i++) {
				parts[i] = sources[i].particle;
				
			}
			
			particleSystem.SetParticles(parts, parts.Length);
		}
	}

	
}
