using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public class InteractableManager
	{
		private Dictionary<GameObject, List<GameObject>> availableInteractables = new Dictionary<GameObject, List<GameObject>> ();

		public void SetInteractableForBot (GameObject interactable, GameObject bot)
		{
			if (availableInteractables.ContainsKey (interactable)) {
				List<GameObject> bots = availableInteractables [interactable];
				bots.Add (bot);
			} else {
				availableInteractables [interactable] = new List<GameObject> ();
				SetInteractableForBot (interactable, bot);
			}
		}

		public GameObject InteractableForBot (GameObject bot)
		{
			foreach (GameObject interactable in availableInteractables.Keys) {
				if (availableInteractables [interactable].Contains (bot)) {
					return interactable;
				}
			}
			return null;
		}

		// Returns interactable
		public GameObject ClearInteractableForBot (GameObject bot)
		{
			foreach (GameObject interactable in availableInteractables.Keys) {
				List<GameObject> botsForInteractable = availableInteractables [interactable];
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

		public bool InteractableIsAvailable (GameObject interactable)
		{
			return availableInteractables.ContainsKey (interactable);
		}
	}
}

