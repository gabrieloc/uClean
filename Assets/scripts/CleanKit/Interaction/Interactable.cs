using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace CleanKit
{
	public interface Interactor
	{
		void UseInteractable (Interactable interactable);

		Vector3 ContactPoint ();
	}

	public class Interactable : MonoBehaviour
	{
		public InteractableIndicator indicator { get; private set; }

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

