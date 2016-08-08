using UnityEngine;
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
			instructionGameObject.transform.SetParent (cellContainer);

			instructionGameObject.transform.localScale = Vector3.one;

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
	}
}
