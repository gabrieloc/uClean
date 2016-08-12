using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace CleanKit
{
	public class InstructionController : MonoBehaviour
	{
		List<InstructionCell> instructionQueue {
			get {
				return cellContainer.GetComponentsInChildren<InstructionCell> ().ToList ();
			}
		}

		public Transform cellContainer;

		public void EnqueueInstruction (Instruction instruction)
		{
			GameObject resource = Resources.Load<GameObject> ("UI/InstructionCell");
			GameObject instructionGameObject = GameObject.Instantiate (resource);
			InstructionCell cell = instructionGameObject.GetComponent<InstructionCell> ();
			cell.SetInstruction (instruction);
			cell.GetComponent<Button> ().onClick.AddListener (() => didSelectInstructionCell (cell));
			instructionGameObject.transform.SetParent (cellContainer);
			instructionGameObject.transform.localScale = Vector3.one;

			focusOnInstruction (instruction);

			// TODO animate enqueue
		}

		public void DequeueInstruction (Instruction instruction)
		{
			InstructionCell cell = cellForInstruction (instruction);
			Destroy (cell);

			// TODO animate dequeue
		}

		InstructionCell cellForInstruction (Instruction instruction)
		{
			foreach (InstructionCell existingInstruction in instructionQueue) {
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
			Interactable assignee = instruction.assignee;
			assignee.FocusOnInstruction (instruction);

			Vector3 focusPoint = instruction.destination.position;
			CameraController camera = Camera.main.GetComponent<CameraController> ();
			camera.LookAtPoint (focusPoint);

			// TODO figure out how to have camera center object on screen
		}
	}
}
