using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CleanKit
{
	public class Scoreboard : MonoBehaviour
	{

		public InteractionController interactionController;

		float evaluationInterval = 0.0f;
		float kEvaluationInterval = 10.0f;

		Text scoreLabel;

		void Start ()
		{
			scoreLabel = GetComponentInChildren<Text> ();
			interactionController = GameObject.Find ("InteractionController").GetComponent<InteractionController> ();
		}

		void Update ()
		{
			if (evaluationInterval > 0.0f) {
				evaluationInterval--;
			} else {
				evaluationInterval = kEvaluationInterval;
				evaluateScore ();
			}
		}

		void evaluateScore ()
		{
			List<Interactable> interactables = interactionController.InstructionedInteractables;
			int count = interactables.Count;
			float score = 0.0f;
			interactables.ForEach (i => score += (1.0f / count) * i.Score ());
			setScore (score);
			setScoreboardVisible (count > 0);
		}

		void setScoreboardVisible (bool visible)
		{
			enabled = visible;
		}

		void setScore (float score)
		{
			float roundScore = Mathf.Round (score * 100);
			scoreLabel.text = roundScore + "%";
		}
	}
}
