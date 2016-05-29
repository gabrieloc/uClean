using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public class BotController: MonoBehaviour
	{
		public float speed = 15.0f;
		public float relocationRadus = 5.0f;
		public float interactableDetectionRadius = 10.0f;

		Vector3 contactPoint = Vector3.zero;

		private int botSpawnCount = 5;
		private int timeSinceLastSpawn = 0;

		void Awake ()
		{
			interactionController = GameObject.Find ("InteractionController").GetComponent<InteractionController> ();
			interactionController.interactionDelegate = this;

			selectionController = GameObject.Find ("SelectionController").GetComponent<SelectionController> ();
			selectionController.selectionDelegate = this;
		}

		void Update ()
		{
			if (botSpawnCount > 0 && timeSinceLastSpawn == 0) {
				AddBot ();
				timeSinceLastSpawn = 10;
				botSpawnCount--;
			} else {
				timeSinceLastSpawn--;
			}

			updateContactPoint ();
			relocateToNewContactPoint ();
		}

		private bool canRelocateBot (Bot bot, Vector3 toPosition)
		{
			bool withinRelocatableRadius = Vector3.Distance (toPosition, bot.transform.position) > relocationRadus;
			return contactPointSet () && withinRelocatableRadius;
		}

		private void clearContactPoint ()
		{
			contactPoint = Vector3.zero;
		}

		private bool contactPointSet ()
		{
			return contactPoint.x != 0.0f && contactPoint.z != 0.0f;
		}

		private void updateContactPoint ()
		{
			if (Controls.RelocationInputExists ()) {
				Ray ray = Camera.main.ScreenPointToRay (Controls.RelocationInput ());
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, 1000)) {
					contactPoint = hit.point;
				}
			}

			if (contactPointSet ()) {
				Debug.DrawLine (contactPoint - new Vector3 (2, 0, 0), contactPoint + new Vector3 (2, 0, 0), Color.red);
				Debug.DrawLine (contactPoint - new Vector3 (0, 0, 2), contactPoint + new Vector3 (0, 0, 2), Color.red);
			}
		}

		private void relocateToNewContactPoint ()
		{
			float distanceDelta = speed * Time.deltaTime;
			foreach (Bot bot in selectionController.selectedBots) {
				Vector3 newPosition = contactPoint;
				newPosition.y += 0.5f;

				if (canRelocateBot (bot, newPosition)) {
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

		public void AddBot ()
		{
			Bot bot = Bot.Instantiate ();
			bot.transform.SetParent (transform);
			selectionController.DidInsertBot (bot);
		}

		// TODO replace with proper delegate

		public void selectionControllerSelectedBot (Bot bot)
		{
			bot.gameObject.SetSelected (true);
		}

		public void selectionControllerDeselectedBot (Bot bot)
		{
			bot.gameObject.SetSelected (false);
			clearContactPoint ();
			clearInteractableForBot (bot);
		}

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

				// TODO Only update UI if bot is selected
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

		public void interactionControllerSelectedInteractable (Interactable interactable)
		{
			Debug.Log ("selected " + interactable.name);
		}
	}
}