using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanKit
{
	public class InstructionController : MonoBehaviour
	{
		InstructionCell[] instructionQueue {
			get {
				return cellContainer.GetComponentsInChildren<InstructionCell> ();
			}
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

			focusOnInstruction (instruction);

			// TODO animate enqueue
		}

		public void DequeueInstruction (Instruction instruction)
		{
			Assert.IsNotNull (instruction, "Cannot dequeue null instruction");

			InstructionCell cell = cellForInstruction (instruction);
			Destroy (cell);

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
			focusOnInstruction (cell.instruction);	
		}

		void focusOnInstruction (Instruction instruction)
		{
			InstructionCell cell = cellForInstruction (instruction);
			cell.highlighted = true;

			Interactable assignee = instruction.assignee;
			assignee.SetGhostVisible (true, true);

			Vector3 focusPoint = instruction.destination.transform.position;
			CameraController camera = Camera.main.GetComponent<CameraController> ();
			camera.LookAtPoint (focusPoint);

			// TODO figure out how to have camera center object on screen
		}
	}
}
