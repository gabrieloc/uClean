using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CleanKit
{
	public class SceneInteractionController : MonoBehaviour
	{
		private List<GameObject> availableLiftables = new List<GameObject> ();

		void Update ()
		{
			foreach (GameObject liftable in availableLiftables) {
				GameObject indicator = indicatorForLiftableObject (liftable);
				Vector3 position = RectTransformUtility.WorldToScreenPoint (Camera.main, liftable.transform.position);//TODO convert?
				indicator.transform.position = position;
			}	
		}

		public void SetLiftableAvailable (GameObject liftable, bool available)
		{
			if (availableLiftables.Contains (liftable) == false && available) {
				availableLiftables.Add (liftable);
				GameObject liftableIndicator = createLiftableIndicator (stringIdentifierForLiftable (liftable));
				liftableIndicator.transform.SetParent (gameObject.transform);
			} else if (availableLiftables.Contains (liftable) == true && !available) {
				availableLiftables.Remove (liftable);
				GameObject liftableIndicator = indicatorForLiftableObject (liftable);
				Destroy (liftableIndicator);
			}
			Debug.Log (availableLiftables.Count + " available");
		}

		private GameObject createLiftableIndicator (string identifier)
		{
			GameObject indicator = GameObject.Instantiate (Resources.Load ("LiftableIndicator"), new Vector3 (), new Quaternion ()) as GameObject;
			indicator.name = identifier;
			return indicator;
		}

		private GameObject indicatorForLiftableObject (GameObject liftable)
		{
			List<GameObject> liftableIndicators = GameObjectExtensions.LiftableIndicators ();
			foreach (GameObject indicator in liftableIndicators) {
				if (stringIdentifierForLiftable (liftable) == indicator.name) {
					return indicator;
				}
			}
			return null;
		}

		private string stringIdentifierForLiftable (GameObject liftable)
		{
			return liftable.gameObject.GetInstanceID ().ToString ();
		}
	}
}

