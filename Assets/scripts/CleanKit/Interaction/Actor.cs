using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public interface Actor
	{
		Vector3 PrimaryContactPoint ();

		void IndicatorForInteractableSelected (Interactable interactable, InteractionType interactionType);

		void RelocateToDestination (Destination destination);

		float DistanceFromDestination ();
	}
}