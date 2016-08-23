using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CleanKit
{
	public partial class PropController : MonoBehaviour, InteractableDelegate, InstructionDelegate
	{
		public int initialSpawn = 10;
		public float displacement = 2.0f;
		public float kMaxScale = 10.0f;

		public InteractionController interactionController;
		public InstructionController instructionController;

		CameraController cameraController { get { return Camera.main.GetComponent<CameraController> (); } }

		void Start ()
		{
			for (int index = 0; index < initialSpawn; index++) {
				SpawnRandomProp ();
			}

			instructionController.instructionDelegate = this;
		}

		void Update ()
		{
			List<Interactable> interactables = activeInteractables ().ToList ();
			List<GameObject> active = interactables.Select (interactable => interactable.gameObject).ToList ();
				
			if (Controls.InputExists () && Controls.InteractingWithObjects (active) == false) {
				instructionController.ClearSelection ();
				clearActiveInteractables ();
			}
		}

		public void SpawnRandomProp ()
		{
			GameObject prop = PropLoader.CreateTestProp ();
			prop.name = "Prop " + interactionController.Interactables.Count;
			prop.transform.SetParent (transform, false);
			prop.transform.position = new Vector3 (
				(UnityEngine.Random.value + 1) * displacement * (UnityEngine.Random.value > 0.5 ? 1 : -1), 
				20.0f, 
				(UnityEngine.Random.value + 1) * displacement * (UnityEngine.Random.value > 0.5 ? 1 : -1));
			prop.transform.localScale = new Vector3 (
				UnityEngine.Random.value * kMaxScale, 
				UnityEngine.Random.value * kMaxScale, 
				UnityEngine.Random.value * kMaxScale);

			float destinationDispersement = 30.0f;
			Vector3 point = new Vector3 (
				                UnityEngine.Random.value * destinationDispersement,
				                0.0f,
				                UnityEngine.Random.value * destinationDispersement);
			Destination destination = Destination.Instantiate (point, Vector3.up);
			destination.transform.SetParent (transform, false);
				
			Interactable interactable = prop.GetComponent<Interactable> ();
			interactable.SetDestination (destination);
			interactable.AddInteractionType (InteractionType.Move);
			interactionController.Interactables.Add (interactable);
		}

		Interactable[] activeInteractables ()
		{
			Interactable[] interactables = GetComponentsInChildren<Interactable> ();
			Interactable[] activeInteractables = Array.FindAll (interactables, interactableIsActive);
			return activeInteractables;
		}

		void clearActiveInteractables ()
		{
			foreach (Interactable interactable in activeInteractables().ToList()) {
				interactable.SetGhostVisible (false);
			}
		}

		static bool interactableIsActive (Interactable interactable)
		{
			return interactable.IsGhostVisible ();
		}

		void highlightInstruction (Instruction instruction)
		{
			Interactable interactable = instruction.interactable;
			interactable.SetGhostVisible (true, true);

			Destination destination = instruction.destination;
			cameraController.FocusOnSubject (destination.ghost.gameObject, ShotSize.CloseUp);
		}

		// InteractableDelegate

		public void InteractableUpdatedMovement (Interactable interactable, Destination destination)
		{
			cameraController.FocusOnSubject (destination.ghost.gameObject, ShotSize.LongShot);
		}

		public void InteractableCancelledMovement (Interactable interactable)
		{
			cameraController.FocusOnSubject (interactable.gameObject, ShotSize.MidShot);
		}

		public void InteractableConfirmedDestination (Interactable interactable, Destination destination)
		{
			Instruction instruction = new Instruction ();
			instruction.interactable = interactable;
			instruction.destination = destination;
			instruction.interactionType = InteractionType.Move;

			instructionController.EnqueueInstruction (instruction);
		}

		// InstructionDelegate

		public void InstructionCellSelected (InstructionController controller, InstructionCell cell)
		{
			Instruction instruction = cell.instruction;
			highlightInstruction (instruction);
		}

		public void InstructionCellDestroyed (InstructionController controller, InstructionCell cell)
		{
			Instruction instruction = cell.instruction;
			instruction.interactable.SetDestination (null);
		}
	}
}