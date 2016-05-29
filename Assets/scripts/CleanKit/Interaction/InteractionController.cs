using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

/*
 * Manages interaction with objects in scene
 * Interprets events and sends elsewhere
 * eg. tells BotController to pick up a liftable object, or to relocate bots
 */

namespace CleanKit
{
	public interface InteractionDelegate
	{
		void interactionControllerSelectedInteractable (Interactable interactable);
	}

	public class InteractionController : MonoBehaviour
	{
		public List<Interactable> allInteractables = new List<Interactable> ();
		private List<Interactable> availableInteractables = new List<Interactable> ();
		public Interactor currentInteractor;
		public InteractionDelegate interactionDelegate;

		void Update ()
		{
			foreach (Interactable interactable in availableInteractables) {
				GameObject indicator = interactable.indicator.gameObject;
				Vector3 position = RectTransformUtility.WorldToScreenPoint (Camera.main, interactable.transform.position);
				indicator.transform.position = position;
			}	
		}

		public void SetInteractableAvailable (Interactable interactable, Interactor interactor, bool available)
		{
			if (availableInteractables.Contains (interactable) == false && available) {
				availableInteractables.Add (interactable);

				currentInteractor = interactor;
				UnityAction onSelection = () => didSelectIndicatorForInteractable (interactable);
				interactable.BecomeAvailableForInteractor (interactor, onSelection);
				interactable.indicator.transform.SetParent (transform);
			} else if (availableInteractables.Contains (interactable) == true && !available) {
				availableInteractables.Remove (interactable);
				interactable.BecomeUnavailable ();
			}
		}

		private void didSelectIndicatorForInteractable (Interactable interactable)
		{
			interactionDelegate.interactionControllerSelectedInteractable (interactable);
		}
	}
}

