using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace CleanKit
{
	public interface Interactor
	{
		Vector3 PrimaryContactPoint ();

		void BeginUsingInteractable (Interactable interactable);

		void RelocateToPosition (Vector3 position);
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

