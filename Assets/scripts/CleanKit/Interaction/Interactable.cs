using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace CleanKit
{
	public interface Interactor
	{
		//		void UseInteractable (Interactable interactable);
		Vector3 PrimaryContactPoint ();

		void BeginUsingInteractable (Interactable interactable);

		bool CanRelocateInteractable (Interactable interactable);


		void RelocateInteractable (Interactable interactable, Vector3 position, float distanceDelta);

		void RelocateToPosition (Vector3 position, float distanceDelta);

		void PrepareForInteractable (Interactable interactable);
	}

	public class Interactable : MonoBehaviour
	{
		public InteractableIndicator indicator { get; private set; }

		public Bounds undersideBounds {
			get {
				BoxCollider collidor = GetComponent<BoxCollider> ();
				Bounds bounds = collidor.bounds;
//				Bounds underside = 
				return bounds;
			}
		}

		public void BecomeAvailableForInteractor (Interactor interactor, UnityAction onSelection)
		{
			if (indicator == null) {
				string identifier = this.name + " (Indicator)";
				indicator = InteractableIndicator.Instantiate (identifier);
			}
			indicator.OnSelection (onSelection);
		}

		public void BecomeUnavailable ()
		{
			Destroy (indicator.gameObject);
			indicator = null;
		}
	}
}

