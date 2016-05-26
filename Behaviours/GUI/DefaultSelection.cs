using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Selectable))]
public class DefaultSelection : MonoBehaviour {
	protected virtual void OnEnable() {
		EventSystem.current.SetSelectedGameObject(gameObject);
	}
}
