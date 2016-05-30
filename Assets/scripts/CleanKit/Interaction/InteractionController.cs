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
	}

	public class InteractionController : MonoBehaviour
	{
		public List<Interactable> allInteractables = new List<Interactable> ();
		private List<Interactable> availableInteractables = new List<Interactable> ();
		public Interactor currentInteractor;
		public InteractionDelegate interactionDelegate;

		void Update ()
		{
			if (availableInteractables.Count > 0) {
				foreach (Interactable interactable in availableInteractables) {
					foreach (InteractableIndicator indicator in interactable.indicators) {
						Vector3 position = RectTransformUtility.WorldToScreenPoint (Camera.main, interactable.gameObject.transform.position);
						indicator.gameObject.transform.position = position;
					}
				}	
			}
		}

		public void SetInteractableAvailable (Interactable interactable, Interactor interactor, bool available)
		{
			if (availableInteractables.Contains (interactable) == false && available) {
				availableInteractables.Add (interactable);

				currentInteractor = interactor;
				interactable.BecomeAvailableForInteractor (interactor);
				foreach (InteractableIndicator indicator in interactable.indicators) {
					indicator.gameObject.transform.SetParent (transform);

					// TODO figure out why scale is being affected
					indicator.gameObject.transform.localScale = new Vector3 (1, 1, 1);
				}
			} else if (availableInteractables.Contains (interactable) == true && !available) {
				availableInteractables.Remove (interactable);
				interactable.BecomeUnavailable ();
			}
		}
	}
}

