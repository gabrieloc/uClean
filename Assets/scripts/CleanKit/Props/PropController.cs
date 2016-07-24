using UnityEngine;
using System.Collections;

namespace CleanKit
{
	public class PropController : MonoBehaviour
	{
		public int initialSpawn = 10;
		public float displacement = 2.0f;
		public float kMaxScale = 10.0f;

		InteractionController interactionController;

		void Start ()
		{
			interactionController = GameObject.Find ("InteractionController").GetComponent<InteractionController> ();

			for (int index = 0; index < initialSpawn; index++) {
				SpawnRandomProp ();
			}
		}

		public void SpawnRandomProp ()
		{
			GameObject prop = PropLoader.CreateTestProp ();
			prop.name = "Prop " + interactionController.Interactables.Count;
			prop.transform.SetParent (transform, false);
			prop.transform.position = new Vector3 (
				(Random.value + 1) * displacement * (Random.value > 0.5 ? 1 : -1), 
				20.0f, 
				(Random.value + 1) * displacement * (Random.value > 0.5 ? 1 : -1));
			prop.transform.localScale = new Vector3 (
				Random.value * kMaxScale, 
				Random.value * kMaxScale, 
				Random.value * kMaxScale);

			float destinationDispersement = 30.0f;
			Vector3 point = new Vector3 (
				                Random.value * destinationDispersement,
				                0.0f,
				                Random.value * destinationDispersement);
			Destination destination = Destination.Instantiate (point, Vector3.up);
			destination.transform.SetParent (transform, false);
				
			Interactable interactable = prop.GetComponent<Interactable> ();
			interactable.SetDestination (destination);
			interactable.AddInteractionType (InteractionType.Move);
			interactionController.Interactables.Add (interactable);
		}
	}
}