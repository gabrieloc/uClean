﻿using UnityEngine;
using System.Collections;

namespace CleanKit
{
	public class InteractableGhost : MonoBehaviour
	{
		public static InteractableGhost Instantiate (Interactable interactable)
		{
			GameObject clone = Instantiate (interactable.gameObject,
				                   interactable.gameObject.transform.position,
				                   interactable.gameObject.transform.rotation) as GameObject;

			clone.gameObject.AddComponent<InteractableGhost> ();

			InteractableGhost ghost = clone.GetComponent<InteractableGhost> ();

			clone.GetComponent<Rigidbody> ().isKinematic = true;
			Shader ghostShader = Shader.Find ("CleanKit/Ghost");
			Material ghostMaterial = new Material (ghostShader);
			Renderer renderer = ghost.GetComponent<Renderer> ();

			int materialCount = renderer.materials.Length;
			Material[] ghostMaterials = new Material[materialCount];
			for (int i = 0; i < materialCount; i++) {
				ghostMaterials [i] = ghostMaterial;
			}
			renderer.materials = ghostMaterials;

			Collider collider = ghost.GetComponent<Collider> ();
			collider.isTrigger = true;

			return ghost;
		}

		public void SetDraggingTransform (Vector3 withPosition)
		{
			transform.position = withPosition;
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
