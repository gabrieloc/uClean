using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public partial class BotController: InteractionDelegate
	{
		private Dictionary<Interactable, List<Bot>> availableInteractables = new Dictionary<Interactable, List<Bot>> ();
		private InteractionController interactionController;

		private void setInteractableForBot (Interactable interactable, Bot bot)
		{
			if (interactableForBot (bot) == interactable) {
				return;
			}

			clearInteractableForBot (bot);

			if (availableInteractables.ContainsKey (interactable)) {
				List<Bot> bots = availableInteractables [interactable];
				bots.Add (bot);
				interactionController.SetInteractableAvailable (interactable, true);
			} else {
				availableInteractables [interactable] = new List<Bot> ();
				setInteractableForBot (interactable, bot);
			}
		}

		private Interactable interactableForBot (Bot bot)
		{
			foreach (Interactable interactable in availableInteractables.Keys) {
				if (availableInteractables [interactable].Contains (bot)) {
					return interactable;
				}
			}
			return null;
		}

		private void clearInteractableForBot (Bot bot)
		{
			Interactable interactableForBot = null;

			foreach (Interactable interactable in availableInteractables.Keys) {
				List<Bot> botsForInteractable = availableInteractables [interactable];
				if (botsForInteractable.Contains (bot)) {
					botsForInteractable.Remove (bot);
					if (botsForInteractable.Count == 0) {
						availableInteractables.Remove (interactable);
					}
					interactableForBot = interactable;
					break;
				}
			}

			if (interactableForBot != null) {
				bool interactableAvailable = interactableIsAvailable (interactableForBot);
				interactionController.SetInteractableAvailable (interactableForBot, interactableAvailable);
			}
		}

		private bool interactableIsAvailable (Interactable interactable)
		{
			return availableInteractables.ContainsKey (interactable);
		}

		// InteractionDelegate

		public void interactionControllerSelectedInteractable (Interactable interactable)
		{
			Debug.Log ("selected " + interactable.name);
		}
	}
}

