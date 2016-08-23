using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public partial class BotController: MonoBehaviour, SelectionDelegate, ActorDelegate
	{
		public InstructionController instructionController;
		public SelectionController selectionController;

		public float speed = 15.0f;
		public float interactableDetectionRadius = 10.0f;

		private Vector3 storedContactPoint = Vector3.zero;

		public int BotSpawnCount = 5;
		int spawned;
		int timeSinceLastSpawn = 0;
		int botSpawnInterval = 5;

		Actor[] actors { get { return transform.GetComponentsInChildren<Actor> (); } }

		void Start ()
		{
			spawned = BotSpawnCount;

			selectionController.selectionDelegate = this;
			instructionController.fulfillmentDelegate = this;
		}

		void Update ()
		{
			if (spawned > 0 && timeSinceLastSpawn == 0) {
				spawned--;
				AddBot ();
				timeSinceLastSpawn = botSpawnInterval;
			} else if (BotSpawnCount > 0) {
				timeSinceLastSpawn--;
			}

			updateInteractables (); // consider removing this entirely
		}

		public void RelocateToPosition (Vector3 newPosition, Vector3 normal)
		{
			relocateSelected (newPosition, normal);
		}

		public void SelectBot (Bot bot)
		{
			selectionController.didSelectCellForBot (bot);
		}

		private void relocateSelected (Vector3 contactPoint, Vector3 contactNormal)
		{
			Actor actor = selectionController.currentActor;
			if (contactPoint.Equals (storedContactPoint) == false && actor != null) {
				Destination destination = Destination.Instantiate (contactPoint, contactNormal);
				actor.RelocateToDestination (destination);
				storedContactPoint = contactPoint;
			}
		}

		private void updateInteractables ()
		{
			Actor actor = selectionController.currentActor;
			if (actor == null) {
				return;
			}

			Vector3 contactPoint = actor.PrimaryContactPoint ();
			int layerMask = Interactable.LayerMask;
			Collider[] interactables = Physics.OverlapSphere (contactPoint, interactableDetectionRadius, layerMask);

			foreach (Collider collider in interactables) {
				Interactable interactable = collider.GetComponent<Interactable> ();

				float distance = Vector3.Distance (interactable.transform.position, contactPoint);

				// A interactable was available, but select this one if it's closer
				Interactable existingInteractable = interactableForActor (actor);
				if (existingInteractable != null) {
					float previousDistance = Vector3.Distance (contactPoint, existingInteractable.transform.position);
					if (distance < previousDistance) {
						setInteractableForActor (interactable, actor);
					}
				} 
				// No interactable was available, select this one if it's close enough
				else if (distance < interactableDetectionRadius) {
					setInteractableForActor (interactable, actor);
				}
			}

			Interactable i = interactableForActor (actor);
			if (i != null) {
				// Clear existing interactable if bot is too far away
				float distance = Vector3.Distance (i.transform.position, contactPoint);
				if (distance > interactableDetectionRadius) {
					clearInteractable (actor);
				} else {
					Debug.DrawLine (contactPoint, i.transform.position, Color.gray);
				}
			}
		}

		public void AddBot ()
		{
			Bot bot = Bot.Instantiate ();
			bot.actorDelegate = this;
			bot.transform.SetParent (transform);
			selectionController.DidInsertBot (bot);
		}

		// ActorDelegate

		public void ActorFulfilledInstruction (Actor actor, Instruction instruction)
		{
			instructionController.DequeueInstruction (instruction);
		}
	}
}