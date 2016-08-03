using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Selectable))]
public class DefaultSelection : MonoBehaviour {
	protected virtual void OnEnable() {
		if (EventSystem.current != null) {
			if (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.activeInHierarchy == false) {
				EventSystem.current.SetSelectedGameObject(gameObject);
			}
		}
	}
}
