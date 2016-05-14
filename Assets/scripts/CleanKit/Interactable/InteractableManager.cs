using System.Collections.Generic;

namespace CleanKit
{
	public class InteractableManager
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
	}
}

