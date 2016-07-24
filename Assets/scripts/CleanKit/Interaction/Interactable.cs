﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace CleanKit
{
	public partial class Interactable : MonoBehaviour
	{
		public static int LayerMask { get { return 1 << UnityEngine.LayerMask.NameToLayer ("interactable"); } }

		public float kIndicatorSpacing = 2.0f;
		readonly public List<InteractableIndicator> indicators = new List<InteractableIndicator> ();

		Destination destination;
		const float kMinimumDistance = 50.0f;

		void Start ()
		{
			// TODO: Set destination
		}

		public void LayoutIndicators ()
		{
			Vector3 center = RectTransformUtility.WorldToScreenPoint (Camera.main, transform.position);
			int indicatorCount = indicators.Count;

			for (int index = 0; index < indicators.Count; index++) {
				Vector3 position = center;
				InteractableIndicator indicator = indicators [index];
				Rect rect = indicator.GetComponent<RectTransform> ().rect;

				float y = position.y;
				y -= (index * rect.height) + (index * (kIndicatorSpacing * (indicatorCount - 1)));
				y += (rect.height + kIndicatorSpacing) * 0.5f;
					
				position.y = y;

				indicator.gameObject.transform.position = position;
			}
		}

		public void BecomeAvailableForActor (Actor actor)
		{
			foreach (InteractionType interactionType in interactionTypes) {
				InteractableIndicator indicator = InteractableIndicator.Instantiate (name, interactionType.Description ());
				indicators.Add (indicator);
				indicator.OnSelection (() => actor.IndicatorForInteractableSelected (this, interactionType));
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

		public void SetDestination (Destination d)
		{
			destination = d;
			destination.name = name + " (destination)";
		}

		public float Score ()
		{
			float distance = destination.Distance (transform.position);
			float score = (kMinimumDistance - distance) / kMinimumDistance;
			return score;
		}
	}
}

