using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace CleanKit
{
	public enum GhostState
	{
		Off,
		Dimmed,
		Bright
	}

	public class InteractableGhost : MonoBehaviour
	{
		public static InteractableGhost Instantiate (GameObject interactableGameObject)
		{
			GameObject clone = Instantiate (interactableGameObject,
				                   interactableGameObject.transform.position,
				                   interactableGameObject.transform.rotation) as GameObject;
			clone.gameObject.AddComponent<InteractableGhost> ();

			InteractableGhost ghost = clone.GetComponent<InteractableGhost> ();
			ghost.state = GhostState.Off;

			clone.GetComponent<Rigidbody> ().isKinematic = true;

			Collider collider = ghost.GetComponent<Collider> ();
			collider.isTrigger = true;

			Destroy (clone.GetComponent<EventTrigger> ());
			Destroy (clone.GetComponent<Interactable> ());

			return ghost;
		}

		GhostState _state;

		public GhostState state {
			get{ return _state; }
			set {
				_state = value;

				Renderer renderer = GetComponent<Renderer> ();
				Material material = materialForState (_state);
				List<Material> materials = new List<Material> ();
				renderer.materials.ToList ().ForEach (m => materials.Add (material));
				renderer.materials = materials.ToArray ();
			}
		}

		Material materialForState (GhostState state)
		{
			Shader ghostShader = Shader.Find ("CleanKit/Ghost");
			Material ghostMaterial = new Material (ghostShader);
			Color color = new Color ();

			switch (state) {
			case GhostState.Off:
				color = Color.clear;
				break;
			case GhostState.Dimmed:
				color = new Color (0, 0, 0, 0.25f);
				break;
			case GhostState.Bright:
				color = new Color (0, 1, 1, 0.5f);
				break;
			}

			ghostMaterial.SetColor ("_color", color);
			return ghostMaterial;
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
