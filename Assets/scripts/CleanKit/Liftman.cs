using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public class Liftman
	{
		private Dictionary<GameObject, List<GameObject>> availableLiftables = new Dictionary<GameObject, List<GameObject>> ();

		public void SetLiftableForBot (GameObject liftable, GameObject bot)
		{
			if (availableLiftables.ContainsKey (liftable)) {
				List<GameObject> bots = availableLiftables [liftable];
				bots.Add (bot);
			} else {
				availableLiftables [liftable] = new List<GameObject> ();
				SetLiftableForBot (liftable, bot);
			}
		}

		public GameObject LiftableForBot (GameObject bot)
		{
			foreach (GameObject liftable in availableLiftables.Keys) {
				if (availableLiftables [liftable].Contains (bot)) {
					return liftable;
				}
			}
			return null;
		}

		// Returns liftable
		public GameObject ClearLiftableForBot (GameObject bot)
		{
			foreach (GameObject liftable in availableLiftables.Keys) {
				List<GameObject> botsForLiftable = availableLiftables [liftable];
				if (botsForLiftable.Contains (bot)) {
					botsForLiftable.Remove (bot);
					if (botsForLiftable.Count == 0) {
						availableLiftables.Remove (liftable);
					}
					return liftable;
				}
			}
			return null;
		}

		public bool LiftableIsAvailable (GameObject liftable)
		{
			return availableLiftables.ContainsKey (liftable);
		}
	}
}

