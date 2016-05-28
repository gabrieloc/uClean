using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace CleanKit
{
	public class Interactable : MonoBehaviour
	{
		public InteractableIndicator indicator { get; private set; }

		public void BecomeAvailable (UnityAction onSelection)
		{
			if (indicator == null) {
				string identifier = gameObject.GetInstanceID ().ToString ();
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

