using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangeLayerWeightOnEnter : StateMachineBehaviour {
	public float targetWeight = 0;
	public float dampening = 3;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

		//animator.SetLayerWeight(layerIndex, targetWeight);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		float weight = animator.GetLayerWeight(layerIndex);
		float diff = (animator.updateMode == AnimatorUpdateMode.AnimatePhysics) ? Time.fixedDeltaTime : Time.deltaTime;
		weight = Mathf.Lerp(weight, targetWeight, diff * dampening);

		animator.SetLayerWeight(layerIndex, weight);
	}
	
}
