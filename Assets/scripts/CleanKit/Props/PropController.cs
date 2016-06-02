using UnityEngine;
using System.Collections;

namespace CleanKit
{
	public class PropController : MonoBehaviour
	{
		public int initialSpawn = 10;
		public float displacement = 2.0f;

		void Start ()
		{
			InteractionController interactionController = GameObject.Find ("InteractionController").GetComponent<InteractionController> ();

			for (int index = 0; index < initialSpawn; index++) {
				GameObject prop = PropLoader.CreateTestProp ();
				prop.name = "Prop " + index;
				prop.transform.SetParent (transform, false);
				prop.transform.position = new Vector3 ((Random.value + 1) * displacement * (Random.value > 0.5 ? 1 : -1), 10, (Random.value + 1) * displacement * (Random.value > 0.5 ? 1 : -1));

				Interactable interactable = prop.GetComponent<Interactable> ();
				interactable.AddInteractionType (InteractionType.Lift);
				interactable.AddInteractionType (InteractionType.Push);
				interactionController.allInteractables.Add (interactable);
			}
		}
	}
}