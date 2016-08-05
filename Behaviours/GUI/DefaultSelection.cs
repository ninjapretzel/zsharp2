using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Selectable))]
public class DefaultSelection : MonoBehaviour {
	private bool _inited = false;

	protected virtual void Update() {
		if (!_inited && EventSystem.current != null) {
			if (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.activeInHierarchy == false) {
				EventSystem.current.SetSelectedGameObject(gameObject);
			}
		}
		_inited = true;
	}

	protected void OnDisable() {
		_inited = false;
	}
}
