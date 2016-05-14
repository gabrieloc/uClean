using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public class BotController: MonoBehaviour
	{
		public float speed = 15.0f;
		public float relocationRadus = 5.0f;
		public float interactableDetectionRadius = 10.0f;

		Vector3 contactOrigin;
		Vector3 contactPoint = Vector3.zero;

		void Awake ()
		{
			selectionController = GameObject.Find ("SelectionController").GetComponent<SelectionController> ();
			interactionController = GameObject.Find ("InteractionController").GetComponent<InteractionController> ();
		}

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
			foreach (Bot bot in selectionController.allBots) {
				Vector3 newPosition = contactPoint;
				newPosition.y = 0.5f;
				bool botSelected = selectionController.IsBotSelected (bot);
				if (botSelected && Vector3.Distance (newPosition, bot.gameObject.transform.position) > relocationRadus) {
					bot.transform.position = Vector3.MoveTowards (bot.transform.position, newPosition, distanceDelta);
				}

				foreach (Interactable interactable in interactionController.allInteractables) {
					float distance = Vector3.Distance (interactable.transform.position, bot.transform.position);

					// A interactable was available, but select this one if it's closer
					Interactable existingInteractable = interactableForBot (bot);
					if (existingInteractable != null) {
						float previousDistance = Vector3.Distance (bot.transform.position, existingInteractable.transform.position);
						if (distance < previousDistance) {
							setInteractableForBot (interactable, bot);
						}
					} 
					// No interactable was available, select this one if it's close enough
					else if (distance < interactableDetectionRadius) {
						setInteractableForBot (interactable, bot);
					}
				}

				// Clear existing interactable if bot is too far away
				if (interactableForBot (bot) != null) {
					float distance = Vector3.Distance (interactableForBot (bot).transform.position, bot.transform.position);
					if (distance > interactableDetectionRadius) {
						clearInteractableForBot (bot);
					} else {
						Debug.DrawLine (bot.transform.position, interactableForBot (bot).transform.position, Color.blue);
					}
				}
			}
		}

		// Selection

		private SelectionController selectionController;

		// Interactables

		private InteractableManager interMan = new InteractableManager ();
		private InteractionController interactionController;

		private Interactable interactableForBot (Bot bot)
		{
			return interMan.InteractableForBot (bot);
		}

		private void setInteractableForBot (Interactable interactable, Bot bot)
		{
			if (interMan.InteractableForBot (bot) != interactable) {
				clearInteractableForBot (bot);
				interMan.SetInteractableForBot (interactable, bot);
				interactionController.SetInteractableAvailable (interactable, true);
			}
		}

		private void clearInteractableForBot (Bot bot)
		{
			Interactable interactable = interMan.ClearInteractableForBot (bot);
			if (interactable != null) {
				bool interactableAvailable = interMan.InteractableIsAvailable (interactable);
				interactionController.SetInteractableAvailable (interactable, interactableAvailable);
			}
		}
	}
}