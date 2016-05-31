using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace CleanKit
{
	public interface Interactor
	{
		Vector3 PrimaryContactPoint ();

		void IndicatorForInteractableSelected (Interactable interactable, InteractionType interactionType);

		void RelocateToPosition (Vector3 position);
	}

	public partial class Interactable : MonoBehaviour
	{
		readonly public List<InteractableIndicator> indicators = new List<InteractableIndicator> ();

		public void BecomeAvailableForInteractor (Interactor interactor)
		{
			foreach (InteractionType interactionType in interactionTypes) {
				InteractableIndicator indicator = InteractableIndicator.Instantiate (name, interactionType.Description ());
				indicators.Add (indicator);
				indicator.OnSelection (() => interactor.IndicatorForInteractableSelected (this, interactionType));
			}
		}

		public void BecomeUnavailable ()
		{
			while (indicators.Count > 0) {
				InteractableIndicator indicator = indicators [0];
				Destroy (indicator.gameObject);
				indicators.Remove (indicator);
			}
		}
	}
}

