using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CleanKit
{
	public partial class PropController : MonoBehaviour, InteractableDelegate, InstructionDelegate
	{
		public InteractionController interactionController;
		public InstructionController instructionController;

		CameraController cameraController { get { return Camera.main.GetComponent<CameraController> (); } }

		void Start ()
		{
			instructionController.instructionDelegate = this;
		}

		void Update ()
		{
			List<Interactable> interactables = activeInteractables;
			List<GameObject> active = interactables.Select (interactable => interactable.gameObject) as List<GameObject>;
				
			if (Controls.InputExists () && Controls.InteractingWithObjects (active) == false) {
				instructionController.ClearSelection ();
				clearActiveInteractables ();
			}
		}

		public void PrepareProps (List<PropInfo> props)
		{
			foreach (PropInfo propInfo in props) {
				GameObject prop = PropLoader.CreateProp (propInfo, transform);
				Interactable interactable = prop.GetComponent<Interactable> ();
				interactable.interactableDelegate = this;
			}
		}

		List<Interactable> activeInteractables {
			get {
				List<Interactable> interactables = GetComponentsInChildren<Interactable> ().ToList ();
				return interactables.FindAll (i => i.HasSpecifiedDestination ());
			}
		}

		void clearActiveInteractables ()
		{
			activeInteractables.ForEach (i => i.DiscardSpecifiedDestination ());
		}

		void highlightInstruction (Instruction instruction)
		{
			Interactable interactable = instruction.interactable;
			interactable.RevealPreferredDestination ();

			Destination destination = instruction.destination;
			cameraController.FocusOnSubject (destination.ghost.gameObject, ShotSize.CloseUp);
		}

		void enqueueInstructionForDestination (Interactable interactable, Destination destination)
		{
			Instruction instruction = new Instruction ();
			instruction.interactable = interactable;
			instruction.destination = destination;
			instruction.interactionType = InteractionType.Move;

			instructionController.EnqueueInstruction (instruction);
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
			enqueueInstructionForDestination (interactable, destination);
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
			instruction.interactable.DiscardSpecifiedDestination ();
		}
	}
}