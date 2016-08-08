using UnityEngine;
using System.Collections;

namespace CleanKit
{
	public class InstructionCell : MonoBehaviour
	{
		public Instruction instruction { get; private set; }

		public Transform interactableParent;

		public void SetInstruction (Instruction instruction)
		{
			Interactable assignee = instruction.assignee;
			GameObject interactableObject = GameObject.Instantiate<GameObject> (assignee.gameObject);
			interactableObject.transform.SetParent (interactableParent);
			Transform cellInteractable = interactableObject.transform;

			cellInteractable.localRotation = Quaternion.identity;
			cellInteractable.localPosition = Vector3.zero;
			cellInteractable.localScale = Vector3.one;

			cellInteractable.gameObject.layer = LayerMask.NameToLayer ("UI");

			GameObject.Destroy (cellInteractable.GetComponent<Collider> ());
			GameObject.Destroy (cellInteractable.GetComponent<Interactable> ());
			GameObject.Destroy (cellInteractable.GetComponent<Rigidbody> ());

			// TODO scale interactable to fill available space
		}
	}
}
