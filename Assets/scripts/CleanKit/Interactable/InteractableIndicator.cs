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

		public static InteractableIndicator Instantiate (string identifier)
		{
			GameObject gameObject = GameObject.Instantiate (Resources.Load ("InteractableIndicator")) as GameObject;
			gameObject.name = identifier;

			InteractableIndicator indicator = gameObject.GetComponent<InteractableIndicator> ();
			return indicator;
		}
	}
}

