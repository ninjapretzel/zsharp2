using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MassParticles))]
public class MassParticleSystem : MonoBehaviour {
	
	public int maxParticles = 400;
	public int count = 0;
	public int next = 0;
	
	MassParticles massParticles;
	
	void Awake() {
		massParticles = GetComponent<MassParticles>();
		massParticles.sources = new ParticleMaker[maxParticles];
		for (int i = 0; i < maxParticles; i++) {
			massParticles.sources[i] = new DummyParticleMaker();
		}	
	}
	
	public void SetNext(ParticleMaker m) {
		massParticles.sources[next] = m;
		next = (next+1)%maxParticles;
	}
	
	void Start() {
		
	}
	
	void Update() {
		foreach (ParticleMaker p in massParticles.sources) {
			p.Update();
		}
	}
	
}
