using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public partial class BotController: MonoBehaviour, SelectionDelegate
	{
		public float speed = 15.0f;
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
			updateInteractables ();
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
			// Check if a bot or swarm has been selected
			if (selectionController.currentInteractor == null) {
				return;
			}

			Interactor interactor = selectionController.currentInteractor;
			Vector3 newPosition = contactPoint;
			newPosition.y += 0.5f;

			Interactable interactable = activeInteractableForInteractor (interactor);
			float distanceDelta = speed * Time.deltaTime;

			if (interactable != null) {
				if (interactor.CanRelocateInteractable (interactable)) {
					interactor.RelocateInteractable (interactable, newPosition, distanceDelta);
				} else {
					interactor.PrepareForInteractable (interactable);
				}
			} else {
				interactor.RelocateToPosition (newPosition, distanceDelta);
			}
		}

		private void updateInteractables ()
		{
			Interactor interactor = selectionController.currentInteractor;
			if (interactor == null) {
				return;
			}

			Vector3 contactPoint = interactor.PrimaryContactPoint ();

			foreach (Interactable interactable in interactionController.allInteractables) {
				float distance = Vector3.Distance (interactable.transform.position, contactPoint);

				// A interactable was available, but select this one if it's closer
				Interactable existingInteractable = interactableForInteractor (interactor);
				if (existingInteractable != null) {
					float previousDistance = Vector3.Distance (contactPoint, existingInteractable.transform.position);
					if (distance < previousDistance) {
						setInteractableForInteractor (interactable, interactor);
					}
				} 
				// No interactable was available, select this one if it's close enough
				else if (distance < interactableDetectionRadius) {
					setInteractableForInteractor (interactable, interactor);
				}
			}

			Interactable i = interactableForInteractor (interactor);
			if (i != null) {
				// Clear existing interactable if bot is too far away
				float distance = Vector3.Distance (i.transform.position, contactPoint);
				if (distance > interactableDetectionRadius) {
					clearInteractable (interactor);
				} else {
					Debug.DrawLine (contactPoint, i.transform.position, Color.blue);
				}
			}
		}

		public void AddBot ()
		{
			Bot bot = Bot.Instantiate ();
			bot.transform.SetParent (transform);
			selectionController.DidInsertBot (bot);
		}
	}
}