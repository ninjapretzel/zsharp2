using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///This class uses a MassMarticleSystem and automates the creation of particles.
public class MassParticleEmitter : MonoBehaviour {
	
	public MassParticleSystem target;
	public string targetGameObject;
	
	public bool emit = true;
	public float minEmission = 5;
	public float maxEmission = 10;
	public float minSize = .1f;
	public float maxSize = 1;
	public Vector3 range = Vector3.zero;
	
	public bool randomRotation = false;
	public float angularVelocity = 0;
	public float randAngularVelocity = 0;
	
	
	public float sizeChange = 0;
	public float alphaChange = -.5f;
	public Vector3 velocity = Vector3.zero;
	public Vector3 randVelocity = Vector3.zero;
	
	public AnimationCurve alphaOverTime = new AnimationCurve(new Keyframe(0, 1), new Keyframe(2, 0));
	public Color colorRangeStart = Color.white;
	public Color colorRangeEnd = Color.white;
	
	//float emitTime;
	float timeout = 0;
	
	public virtual ParticleMaker MakeParticle() {
		TimedParticleMaker p = new TimedParticleMaker();
		p.sizeChange = sizeChange;
		p.velocity = velocity + Vector3.Scale(randVelocity, Random.insideUnitSphere);
		p.alphaOverTime = alphaOverTime;
		
		return p;
	}
	
	void Start() {
		if (target == null) {
			GameObject gob = GameObject.Find(targetGameObject);
			target = gob.GetComponent<MassParticleSystem>();
			
		}
	}
	
	
	void Update() {
		if (!emit) { return; }
		
		timeout -= Time.deltaTime;
		
		while (timeout <= 0) {
			Emit();
			timeout += 1.0f / Random.Range(minEmission, maxEmission);
		}
		
	}
	
	void Emit() {
		
		ParticleMaker p = MakeParticle();
		p.position = transform.position + Vector3.Scale(range, Random.insideUnitSphere);
		p.size = Random.Range(minSize, maxSize);
		p.color = Colors.HSVLerp(colorRangeStart, colorRangeEnd, Random.value);
		if (randomRotation) { p.rotation = Random.Range(0f, 360f); }
		p.angularVelocity = angularVelocity + Random.Range(-randAngularVelocity, randAngularVelocity);
		
		//p.color = C
		
		target.SetNext(p);
	
	}
	
}
