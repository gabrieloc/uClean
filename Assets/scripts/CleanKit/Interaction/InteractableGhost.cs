using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace CleanKit
{
	public class InteractableGhost : MonoBehaviour
	{
		public static InteractableGhost Instantiate (GameObject interactableGameObject)
		{
			GameObject clone = Instantiate (interactableGameObject,
				                   interactableGameObject.transform.position,
				                   interactableGameObject.transform.rotation) as GameObject;
			clone.gameObject.AddComponent<InteractableGhost> ();

			InteractableGhost ghost = clone.GetComponent<InteractableGhost> ();

			clone.GetComponent<Rigidbody> ().isKinematic = true;

			Collider collider = ghost.GetComponent<Collider> ();
			collider.isTrigger = true;

			Destroy (clone.GetComponent<EventTrigger> ());
			Destroy (clone.GetComponent<Interactable> ());

			return ghost;
		}

		public void SetHighlighted (bool highlight)
		{
			Shader ghostShader = Shader.Find ("CleanKit/Ghost");
			Material ghostMaterial = new Material (ghostShader);
			Color color = highlight ? Color.blue : Color.gray;
			color.a = 0.5f;
			ghostMaterial.SetColor ("_color", color);
			Renderer renderer = GetComponent<Renderer> ();

			int materialCount = renderer.materials.Length;
			Material[] ghostMaterials = new Material[materialCount];
			for (int i = 0; i < materialCount; i++) {
				ghostMaterials [i] = ghostMaterial;
			}
			renderer.materials = ghostMaterials;
		}

		public void SetDroppedTransform (Vector3 withPosition)
		{
			transform.position = withPosition;

			// TODO define protocol for Droppable objects, 
			// 		let them provide desired transform
			//		Will need to be per-object
			transform.rotation = new Quaternion ();
		}

		public bool CollisionWithInteractables { get; private set; }

		void OnTriggerStay (Collider otherCollider)
		{
			if (shouldColliderUpdateValidity (otherCollider)) {
				CollisionWithInteractables = true;
			}
		}

		void OnTriggerExit (Collider otherCollider)
		{
			if (shouldColliderUpdateValidity (otherCollider)) {
				CollisionWithInteractables = false;
			}
		}

		bool shouldColliderUpdateValidity (Collider collider)
		{
			GameObject otherGameObject = collider.gameObject;
			bool notParent = otherGameObject.transform.Equals (transform.parent) == false;
			bool isInteractable = otherGameObject.GetComponent<Interactable> () != null;
			return notParent && isInteractable;
		}
	}
}
