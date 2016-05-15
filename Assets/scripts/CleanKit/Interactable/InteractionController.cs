using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/*
 * Manages interaction with objects in scene
 * Interprets events and sends elsewhere
 * eg. tells BotController to pick up a liftable object, or to relocate bots
 */

namespace CleanKit
{
	public class InteractionController : MonoBehaviour
	{
		public List<Interactable> allInteractables = new List<Interactable> ();
		private List<Interactable> availableInteractables = new List<Interactable> ();

		void Update ()
		{
			foreach (Interactable interactable in availableInteractables) {
				GameObject indicator = indicatorForInteractableObject (interactable);
				Vector3 position = RectTransformUtility.WorldToScreenPoint (Camera.main, interactable.transform.position);
				indicator.transform.position = position;
			}	
		}

		public void SetInteractableAvailable (Interactable interactable, bool available)
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
		}

		private GameObject createInteractableIndicator (string identifier)
		{
			GameObject indicator = GameObject.Instantiate (Resources.Load ("InteractableIndicator")) as GameObject;
			indicator.name = identifier;
			return indicator;
		}

		private GameObject indicatorForInteractableObject (Interactable interactable)
		{
			List<GameObject> interactableIndicators = GameObjectExtensions.InteractableIndicators ();
			foreach (GameObject indicator in interactableIndicators) {
				if (stringIdentifierForInteractable (interactable) == indicator.name) {
					return indicator;
				}
			}
			return null;
		}

		private string stringIdentifierForInteractable (Interactable interactable)
		{
			return interactable.gameObject.GetInstanceID ().ToString ();
		}
	}
}

