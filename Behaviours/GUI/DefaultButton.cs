using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class DefaultButton : MonoBehaviour {
	protected virtual void OnEnable() {
		EventSystem.current.SetSelectedGameObject(gameObject);
	}
}
