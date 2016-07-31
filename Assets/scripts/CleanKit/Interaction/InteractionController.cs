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
		List<Interactable> availableInteractables = new List<Interactable> ();

		public List<Interactable> Interactables { get; private set; }

		Interactable selectedInteractable;

		public InteractionDelegate interactionDelegate;

		void Start ()
		{
			Interactables =	new List<Interactable> (GameObject.FindObjectsOfType<Interactable> ());
		}

		void Update ()
		{
			layoutIndicatorsIfNecessary ();

			Interactable interactable;
			if (panInputExists (out interactable)) {
				
			}
		}

		bool panInputExists (out Interactable interactable)
		{
			interactable = null;
			GameObject selected = null;
			if (Controls.PanInputExists (out selected)) {
				bool selectedInteractable = selected.GetComponent<Interactable> ();
				print (selectedInteractable);
				if (selectedInteractable) {
					interactable = selected.GetComponent<Interactable> ();
				}
				return selectedInteractable;
			}
			return false;
		}

		void layoutIndicatorsIfNecessary ()
		{
			if (availableInteractables.Count > 0) {
				foreach (Interactable interactable in availableInteractables) {
					interactable.LayoutIndicators ();
				}	
			}
		}

		public void SetInteractableAvailable (Interactable interactable, Actor actor, bool available)
		{
			if (availableInteractables.Contains (interactable) == false && available) {
				availableInteractables.Add (interactable);

				interactable.BecomeAvailableForActor (actor);
				foreach (InteractableIndicator indicator in interactable.indicators) {
					indicator.gameObject.transform.SetParent (transform);
				}
			} else if (availableInteractables.Contains (interactable) == true && !available) {
				availableInteractables.Remove (interactable);
				interactable.BecomeUnavailable ();
			}
		}
	}
}

