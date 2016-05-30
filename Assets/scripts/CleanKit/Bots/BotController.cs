using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public partial class BotController: MonoBehaviour, SelectionDelegate
	{
		public float speed = 15.0f;
		public float interactableDetectionRadius = 10.0f;

		private Vector3 storedContactPoint = Vector3.zero;

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

			updateInput ();
			updateInteractables ();
		}

		private void updateInput ()
		{
			if (Controls.RelocationInputExists ()) {
				Ray ray = Camera.main.ScreenPointToRay (Controls.RelocationInput ());
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, 1000)) {
					relocateSelectedToNewContactPoint (hit.point);
				}
			}
		}

		private void relocateSelectedToNewContactPoint (Vector3 contactPoint)
		{
			Interactor interactor = selectionController.currentInteractor;
			if (contactPoint.Equals (storedContactPoint) == false && interactor != null) {
				interactor.RelocateToPosition (contactPoint);
				storedContactPoint = contactPoint;
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