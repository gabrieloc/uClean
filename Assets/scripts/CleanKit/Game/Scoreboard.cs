using UnityEngine;
using UnityEngine.UI;

namespace CleanKit
{
	public class Scoreboard : MonoBehaviour
	{

		public InteractionController interactionController;

		float evaluationInterval = 0.0f;
		float kEvaluationInterval = 10.0f;

		Text scoreLabel;

		// Use this for initialization
		void Start ()
		{
			scoreLabel = GetComponentInChildren<Text> ();
			interactionController = GameObject.Find ("InteractionController").GetComponent<InteractionController> ();
		}
		
		// Update is called once per frame
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
			float score = 0.0f;
			int propCount = interactionController.Interactables.Count;
			foreach (Interactable interactable in interactionController.Interactables) {
				float propScore = interactable.Score ();
				score += (1.0f / propCount) * propScore;
			}
			setScore (score);
		}

		void setScore (float score)
		{
			float roundScore = Mathf.Round (score * 100);
			scoreLabel.text = roundScore + "%";
		}
	}
}
