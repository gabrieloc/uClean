using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CleanKit
{
	public class InteractionController : MonoBehaviour
	{
		private List<GameObject> availableInteractables = new List<GameObject> ();

		void Update ()
		{
			foreach (GameObject interactable in availableInteractables) {
				GameObject indicator = indicatorForInteractableObject (interactable);
				Vector3 position = RectTransformUtility.WorldToScreenPoint (Camera.main, interactable.transform.position);
				indicator.transform.position = position;
			}	
		}

		public void SetInteractableAvailable (GameObject interactable, bool available)
		{
			if (availableInteractables.Contains (interactable) == false && available) {
				availableInteractables.Add (interactable);
				GameObject interactableIndicator = createInteractableIndicator (stringIdentifierForInteractable (interactable));
				interactableIndicator.transform.SetParent (gameObject.transform);
			} else if (availableInteractables.Contains (interactable) == true && !available) {
				availableInteractables.Remove (interactable);
				GameObject interactableIndicator = indicatorForInteractableObject (interactable);
				Destroy (interactableIndicator);
			}
			Debug.Log (availableInteractables.Count + " available");
		}

		private GameObject createInteractableIndicator (string identifier)
		{
			GameObject indicator = GameObject.Instantiate (Resources.Load ("InteractableIndicator")) as GameObject;
			indicator.name = identifier;
			return indicator;
		}

		private GameObject indicatorForInteractableObject (GameObject interactable)
		{
			List<GameObject> interactableIndicators = GameObjectExtensions.InteractableIndicators ();
			foreach (GameObject indicator in interactableIndicators) {
				if (stringIdentifierForInteractable (interactable) == indicator.name) {
					return indicator;
				}
			}
			return null;
		}

		private string stringIdentifierForInteractable (GameObject interactable)
		{
			return interactable.gameObject.GetInstanceID ().ToString ();
		}
	}
}

