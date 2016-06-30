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
			prop.name = "Random Prop";
			prop.transform.SetParent (transform, false);
			prop.transform.position = new Vector3 ((Random.value + 1) * displacement * (Random.value > 0.5 ? 1 : -1), 20, (Random.value + 1) * displacement * (Random.value > 0.5 ? 1 : -1));
			prop.transform.localScale = new Vector3 (Random.value * kMaxScale, Random.value * kMaxScale, Random.value * kMaxScale);

			Interactable interactable = prop.GetComponent<Interactable> ();
			interactable.AddInteractionType (InteractionType.Move);
			interactionController.allInteractables.Add (interactable);
		}
	}
}