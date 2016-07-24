using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CleanKit
{
	public class Scoreboard : MonoBehaviour
	{

		public InteractionController interactionController;

		float evaluationInterval = 0.0f;

		Text scoreLabel;

		// Use this for initialization
		void Start ()
		{
				
		}
		
		// Update is called once per frame
		void Update ()
		{
			if (evaluationInterval > 0.0f) {
				evaluationInterval--;
			} else {
				evaluationInterval = 60.0f;
				evaluateScore ();
			}
		}

		void evaluateScore ()
		{
			float score = 0.0f;
			int propCount = interactionController.allInteractables.Count;
			foreach (Interactable interactable in interactionController.allInteractables) {
				float propScore = interactable.Score()
				score += (1.0f / propCount) * propScore;
			}
			setScore (score);
		}

		void setScore (float score)
		{
			scoreLabel.text = score + "%";
		}
	}
}
