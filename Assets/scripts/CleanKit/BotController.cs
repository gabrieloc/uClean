using UnityEngine;

namespace CleanKit
{
	public class BotController: MonoBehaviour
	{
		public float speed = 15.0f;
		public float relocationRadus = 5.0f;
		public float interactableDetectionRadius = 10.0f;
		public SelectionController selectionController;

		Vector3 contactOrigin;
		Vector3 contactPoint = Vector3.zero;

		void Update ()
		{
			if (Controls.RelocationInputExists ()) {
				Ray ray = Camera.main.ScreenPointToRay (Controls.RelocationInput ());
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, 1000)) {
					contactOrigin = ray.origin;
					contactPoint = hit.point;
				}
			}
			Debug.DrawLine (contactOrigin, contactPoint, Color.red);

			float distanceDelta = speed * Time.deltaTime;
			foreach (GameObject bot in GameObjectExtensions.BotObjects()) {
				Vector3 newPosition = contactPoint;
				newPosition.y = 0.5f;
				bool botSelected = selectionController.selectedBots.Contains (bot);
				if (botSelected && Vector3.Distance (newPosition, bot.transform.position) > relocationRadus) {
					bot.transform.position = Vector3.MoveTowards (bot.transform.position, newPosition, distanceDelta);
				}

				// Before looking for interactables, clear the last available one selectionController.ClearInteractableForBot (bot);

				foreach (GameObject interactable in GameObjectExtensions.InteractableObjects()) {
					float distance = Vector3.Distance (interactable.transform.position, bot.transform.position);

					// A interactable was available, but select this one if it's closer
					if (selectionController.InteractableForBot (bot)) {
						GameObject interactableForBot = selectionController.InteractableForBot (bot);
						float previousDistance = Vector3.Distance (bot.transform.position, interactableForBot.transform.position);
						if (distance < previousDistance) {
							selectionController.SetInteractableForBot (interactable, bot);
						}
					} 
					// No interactable was available, select this one if it's close enough
					else if (distance < interactableDetectionRadius) {
						selectionController.SetInteractableForBot (interactable, bot);
					}
				}

				if (selectionController.InteractableForBot (bot)) {
					Debug.DrawLine (bot.transform.position, selectionController.InteractableForBot (bot).transform.position, Color.blue);
				}
			}
		}
	}
}