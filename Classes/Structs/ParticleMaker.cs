using UnityEngine;
using System.Collections;
using System.Collections.Generic;


///This is a class that has a virtual functions to be overrided to create particles through code.
///This can often be an easier way of creating effects than using the shuriken system's inspector
[System.Serializable]
public class ParticleMaker {

	public Vector3 position = Vector3.zero;
	
	public float size = 1;
	public Color color = Color.white;
	public float angularVelocity = 0;
	public float rotation = 0;
	
	public virtual void Update() {}
	
	public virtual ParticleSystem.Particle particle {
		get {
			ParticleSystem.Particle p = new ParticleSystem.Particle();
			
			p.position = position;
			
			p.startSize = size;
			p.startColor = color;
			p.rotation = rotation + angularVelocity * Time.time;
			
			return p;
		}
	}
	
}

///This is an example ParticleMaker with Update and particle overrided.
public class TimedParticleMaker : ParticleMaker { 
	
	public float lifeTime = 0;
	
	public float sizeChange = 0;
	public Vector3 velocity = Vector3.zero;
	public AnimationCurve alphaOverTime;
	
	public static AnimationCurve baseCurve = new AnimationCurve(
		new Keyframe(0, 1),
		new Keyframe(2, 0));
		
	
	public override void Update() { lifeTime += Time.deltaTime; }
	
	public override ParticleSystem.Particle particle {
		get {
			ParticleSystem.Particle p = base.particle;
			
			p.position += velocity * lifeTime;
			p.startSize += sizeChange * lifeTime;
			if (p.startSize < 0) { p.startSize = 0; }
			
			Color c = p.startColor;
			c.a = alphaOverTime.Evaluate(lifeTime);
			p.startColor = c;
			
			return p;
		}
	}
	
}

///This is a blank particle maker. Creates uncolored particles.
public class DummyParticleMaker : ParticleMaker {
	
	
	public DummyParticleMaker() {
		color = Color.clear;
	}
	
}
