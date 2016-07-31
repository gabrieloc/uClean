using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public partial class Interactable : MonoBehaviour
	{
		public static int LayerMask { get { return 1 << UnityEngine.LayerMask.NameToLayer ("interactable"); } }


		void Start ()
		{
			// TODO: Set destination
		}


		// TODO: Refactor all this to use grid-based approach

		const float kMinimumDistance = 50.0f;

		public float Score ()
		{
			// TODO extract this to allow different types of interactables to be scored individually
//			float distance = destination.Distance (transform.position);
//			float score = (kMinimumDistance - distance) / kMinimumDistance;
//			return score;
			return 0.5f;
		}
	}
}

