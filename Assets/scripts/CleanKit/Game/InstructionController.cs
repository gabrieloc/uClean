using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CleanKit
{
	public interface InstructionDelegate
	{
		void InstructionCellSelected (InstructionController controller, InstructionCell cell);

		void InstructionCellDestroyed (InstructionController controller, InstructionCell cell);
	}

	public interface FulfillmentDelegate
	{
		void InstructionNeedsFulfillment (InstructionController controller, Instruction instruction);

		void InstructionCancelled (InstructionController controller, Instruction instruction);
	}

	public class InstructionController : MonoBehaviour
	{
		public InstructionDelegate instructionDelegate;

		public FulfillmentDelegate fulfillmentDelegate;

		List<InstructionCell> instructionQueue {
			get {
				return cellContainer.GetComponentsInChildren<InstructionCell> ().ToList ();
			}
		}

		public List<Instruction> AvailableInstructions ()
		{
			// TODO consider sorting by priority (lowest number of acors)
//			List<Actor> unemployedActors { get { return actors.FindAll (a => !a.IsEmployed ()); } }

			List<InstructionCell> unassignedCells = instructionQueue.FindAll (cell => cell.instruction == null);
			return unassignedCells.Select (cell => cell.instruction) as List<Instruction>;
		}

		public Transform cellContainer;

		public void ClearSelection ()
		{
			List<InstructionCell> selectedCells = instructionQueue.FindAll (cell => cell.highlighted);
			selectedCells.ForEach (cell => cell.highlighted = false);
		}

		public void EnqueueInstruction (Instruction instruction)
		{
			Assert.IsNotNull (instruction, "Cannot enqueue null instruction");

			GameObject resource = Resources.Load<GameObject> ("UI/InstructionCell");
			GameObject instructionGameObject = GameObject.Instantiate (resource);
			InstructionCell cell = instructionGameObject.GetComponent<InstructionCell> ();
			cell.SetInstruction (instruction);
			cell.GetComponent<Button> ().onClick.AddListener (() => didSelectInstructionCell (cell));
			cell.name = instruction.name + " (Cell)";
			instructionGameObject.transform.SetParent (cellContainer);
			instructionGameObject.transform.localScale = Vector3.one;

			cell.highlighted = true;
			instructionDelegate.InstructionCellSelected (this, cell);
			fulfillmentDelegate.InstructionNeedsFulfillment (this, instruction);

			// TODO animate enqueue
		}

		public void DequeueInstruction (Instruction instruction)
		{
			Assert.IsNotNull (instruction, "Cannot dequeue null instruction");

			InstructionCell cell = cellForInstruction (instruction);
			instructionDelegate.InstructionCellDestroyed (this, cell);
			fulfillmentDelegate.InstructionCancelled (this, instruction);

			Destroy (cell.gameObject);

			// TODO animate dequeue
		}

		InstructionCell cellForInstruction (Instruction instruction)
		{
			foreach (InstructionCell existingInstruction in instructionQueue.ToList()) {
				if (existingInstruction.instruction.Equals (instruction)) {
					return existingInstruction;
				}
			}
			return null;
		}

		void didSelectInstructionCell (InstructionCell cell)
		{
			instructionDelegate.InstructionCellSelected (this, cell);
		}
	}
}
