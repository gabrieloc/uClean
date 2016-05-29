using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public partial class BotController: InteractionDelegate
	{
		private Dictionary<Interactable, List<Bot>> availableInteractables = new Dictionary<Interactable, List<Bot>> ();

		public void SetInteractableForBot (Interactable interactable, Bot bot)
		{
			if (availableInteractables.ContainsKey (interactable)) {
				List<Bot> bots = availableInteractables [interactable];
				bots.Add (bot);
			} else {
				availableInteractables [interactable] = new List<Bot> ();
				SetInteractableForBot (interactable, bot);
			}
			interactionController.SetInteractableAvailable (interactable, true);
		}

		public Interactable InteractableForBot (Bot bot)
		{
			foreach (Interactable interactable in availableInteractables.Keys) {
				if (availableInteractables [interactable].Contains (bot)) {
					return interactable;
				}
			}
			return null;
		}

		public Interactable ClearInteractableForBot (Bot bot)
		{
			foreach (Interactable interactable in availableInteractables.Keys) {
				List<Bot> botsForInteractable = availableInteractables [interactable];
				if (botsForInteractable.Contains (bot)) {
					botsForInteractable.Remove (bot);
					if (botsForInteractable.Count == 0) {
						availableInteractables.Remove (interactable);
					}
					return interactable;
				}
			}
			return null;
		}

		public bool InteractableIsAvailable (Interactable interactable)
		{
			return availableInteractables.ContainsKey (interactable);
		}

		// Interactables

		private InteractionController interactionController;

		private Interactable interactableForBot (Bot bot)
		{
			return InteractableForBot (bot);
		}

		private void setInteractableForBot (Interactable interactable, Bot bot)
		{
			if (InteractableForBot (bot) != interactable) {
				clearInteractableForBot (bot);
				SetInteractableForBot (interactable, bot);
			}
		}

		private void clearInteractableForBot (Bot bot)
		{
			Interactable interactable = ClearInteractableForBot (bot);
			if (interactable != null) {
				bool interactableAvailable = InteractableIsAvailable (interactable);
				interactionController.SetInteractableAvailable (interactable, interactableAvailable);
			}
		}

		// InteractionDelegate

		public void interactionControllerSelectedInteractable (Interactable interactable)
		{
			Debug.Log ("selected " + interactable.name);
		}
	}
}

