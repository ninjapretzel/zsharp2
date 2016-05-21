using UnityEngine;

public class HoverRotation : MonoBehaviour {

	private Quaternion initialRotation;
	public Oscillator[] oscis;
	public Vector3[] rotations;
	public bool doLocal;

	public void Start() {
		if (doLocal) { initialRotation = transform.localRotation; } else { initialRotation = transform.rotation; }
	}

	public void Update() {
		int i = 0;
		if (doLocal) { transform.localRotation = initialRotation; } else { transform.rotation = initialRotation; }

		for (i = 0; i < oscis.Length; ++i) {
			transform.Rotate(rotations[i] * oscis[i].Update());
		}

	}

}
