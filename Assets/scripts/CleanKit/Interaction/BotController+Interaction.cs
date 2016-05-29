using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public partial class BotController: InteractionDelegate
	{
		private Dictionary<Interactable, List<Interactor>> availableInteractables = new Dictionary<Interactable, List<Interactor>> ();
		private Dictionary<Interactable, Interactor> activeInteractables = new Dictionary<Interactable, Interactor> ();
		private InteractionController interactionController;

		private void setInteractableForInteractor (Interactable interactable, Interactor interactor)
		{
			if (interactableForInteractor (interactor) == interactable) {
				return;
			}

			clearInteractable (interactor);

			if (availableInteractables.ContainsKey (interactable)) {
				List<Interactor> interactors = availableInteractables [interactable];
				interactors.Add (interactor);
				interactionController.SetInteractableAvailable (interactable, interactor, true);
			} else {
				availableInteractables [interactable] = new List<Interactor> ();
				setInteractableForInteractor (interactable, interactor);
			}
		}

		private Interactable interactableForInteractor (Interactor interactor)
		{
			foreach (Interactable interactable in availableInteractables.Keys) {
				if (availableInteractables [interactable].Contains (interactor)) {
					return interactable;
				}
			}
			return null;
		}

		private Interactable activeInteractableForInteractor (Interactor interactor)
		{
			foreach (Interactable interactable in activeInteractables.Keys) {
				if (activeInteractables [interactable].Equals (interactor)) {
					return interactable;
				}
			}
			return null;
		}

		private void clearInteractable (Interactor interactor)
		{
			Interactable interactableForBot = null;

			foreach (Interactable interactable in availableInteractables.Keys) {
				List<Interactor> interactors = availableInteractables [interactable];
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
				interactionController.SetInteractableAvailable (interactableForBot, interactor, interactableAvailable);
			}
		}

		private bool interactableIsAvailable (Interactable interactable)
		{
			return availableInteractables.ContainsKey (interactable);
		}

		// InteractionDelegate

		public void interactionControllerSelectedInteractable (Interactable interactable)
		{
			Interactor currentInteractor = selectionController.currentInteractor;
			currentInteractor.BeginUsingInteractable (interactable);
			activeInteractables [interactable] = currentInteractor; 
		}
	}
}

