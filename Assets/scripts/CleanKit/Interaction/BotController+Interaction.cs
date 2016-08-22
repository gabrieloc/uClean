using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public partial class BotController
	{
		private Dictionary<Interactable, List<Actor>> availableInteractables = new Dictionary<Interactable, List<Actor>> ();
		private Dictionary<Interactable, Actor> activeInteractables = new Dictionary<Interactable, Actor> ();

		private void setInteractableForActor (Interactable interactable, Actor interactor)
		{
			if (interactableForActor (interactor) == interactable) {
				return;
			}

			clearInteractable (interactor);

			if (availableInteractables.ContainsKey (interactable)) {
				List<Actor> interactors = availableInteractables [interactable];
				interactors.Add (interactor);
//				interactionController.SetInteractableAvailable (interactable, interactor, true);
			} else {
				availableInteractables [interactable] = new List<Actor> ();
				setInteractableForActor (interactable, interactor);
			}
		}

		private Interactable interactableForActor (Actor interactor)
		{
			foreach (Interactable interactable in availableInteractables.Keys) {
				if (availableInteractables [interactable].Contains (interactor)) {
					return interactable;
				}
			}
			return null;
		}

		private Interactable activeInteractableForActor (Actor interactor)
		{
			foreach (Interactable interactable in activeInteractables.Keys) {
				if (activeInteractables [interactable].Equals (interactor)) {
					return interactable;
				}
			}
			return null;
		}

		private void clearInteractable (Actor interactor)
		{
			Interactable interactableForBot = null;

			foreach (Interactable interactable in availableInteractables.Keys) {
				List<Actor> interactors = availableInteractables [interactable];
				if (interactors.Contains (interactor)) {
					interactors.Remove (interactor);
					if (interactors.Count == 0) {
						availableInteractables.Remove (interactable);
					}
					interactableForBot = interactable;
					break;
				}
			}

			if (interactableForBot != null) {
				bool interactableAvailable = interactableIsAvailable (interactableForBot);
//				interactionController.SetInteractableAvailable (interactableForBot, interactor, interactableAvailable);
			}
		}

		private bool interactableIsAvailable (Interactable interactable)
		{
			return availableInteractables.ContainsKey (interactable);
		}
	}
}

