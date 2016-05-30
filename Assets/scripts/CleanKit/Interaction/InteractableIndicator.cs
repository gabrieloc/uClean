using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace CleanKit
{
	public class InteractableIndicator : MonoBehaviour
	{
		public void OnSelection (UnityAction selection)
		{
			Button button = gameObject.GetComponent<Button> ();

			button.onClick.AddListener (selection);
		}

		public static InteractableIndicator Instantiate (string interactableName, string interaction)
		{
			GameObject gameObject = GameObject.Instantiate (Resources.Load ("InteractableIndicator")) as GameObject;
			string identifier = interactableName + " (" + interaction + " indicator)";
			gameObject.name = identifier;

			Text titleLabel = gameObject.GetComponentInChildren<Text> ();
			titleLabel.text = interaction;

			InteractableIndicator indicator = gameObject.GetComponent<InteractableIndicator> ();
			return indicator;
		}
	}
}

