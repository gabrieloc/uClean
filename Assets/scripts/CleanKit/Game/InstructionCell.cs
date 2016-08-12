using UnityEngine;
using System.Collections;

namespace CleanKit
{
	public class InstructionCell : MonoBehaviour
	{
		public Instruction instruction { get; private set; }

		public Transform interactableParent;

		public float MaxDimension = 60.0f;

		public void SetInstruction (Instruction instruction)
		{
			Interactable assignee = instruction.assignee;
			GameObject interactableObject = GameObject.Instantiate<GameObject> (assignee.gameObject);
			foreach (Transform child in interactableObject.transform) {
				GameObject.Destroy (child.gameObject);
			}

			interactableObject.transform.SetParent (interactableParent);
			Transform cellInteractable = interactableObject.transform;
			cellInteractable.gameObject.layer = gameObject.layer;

			cellInteractable.localRotation = Quaternion.identity;

			Collider collider = cellInteractable.GetComponent<Collider> ();
			Bounds bounds = collider.bounds;
			Vector3 size = bounds.size;
			float largestD = 0.0f;
			for (int i = 0; i < 3; i++) {
				largestD = size [i] > largestD ? size [i] : largestD;
			}
			float scale = MaxDimension / largestD;
			Vector3 interactableScale = new Vector3 (scale, scale, scale);
			cellInteractable.localScale = interactableScale;
			cellInteractable.localPosition = Vector3.zero;

			Vector3 position = interactableParent.transform.position;
			position.y = bounds.center.y * scale * -1.0f;
			interactableParent.transform.position = position;

			GameObject.Destroy (cellInteractable.GetComponent<Collider> ());
			GameObject.Destroy (cellInteractable.GetComponent<Interactable> ());
			GameObject.Destroy (cellInteractable.GetComponent<Rigidbody> ());

			// TODO consider renderering to image for performance?
		}
	}
}
