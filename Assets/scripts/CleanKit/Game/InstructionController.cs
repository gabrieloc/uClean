using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanKit
{
	public interface InstructionDelegate
	{
		void InstructionCellSelected (InstructionController controller, InstructionCell cell);
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

		InstructionCell[] instructionQueue {
			get {
				return cellContainer.GetComponentsInChildren<InstructionCell> ();
			}
		}

		public Instruction[] AvailableInstructions ()
		{
			// TODO consider sorting by priority (lowest number of acors)
			InstructionCell[] unassignedCells = Array.FindAll (instructionQueue, isInstructionForCellAssigned);
			return unassignedCells.Select (cell => cell.instruction).ToArray ();
		}

		public Transform cellContainer;

		public void ClearSelection ()
		{
			InstructionCell[] selectedCells = Array.FindAll (instructionQueue, isCellSelected);
			foreach (InstructionCell cell in selectedCells.ToList()) {
				cell.highlighted = false;
			}
		}

		static bool isCellSelected (InstructionCell cell)
		{
			return cell.highlighted;
		}

		static bool isInstructionForCellAssigned (InstructionCell cell)
		{
			return cell.instruction == null;
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
			Destroy (cell);

			fulfillmentDelegate.InstructionCancelled (this, instruction);

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
