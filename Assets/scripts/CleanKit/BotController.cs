using UnityEngine;

namespace CleanKit
{
	public class BotController: MonoBehaviour
	{
		public float speed = 15.0f;
		public float relocationRadus = 5.0f;
		public float liftableDetectionRadius = 10.0f;
		public SelectionController selectionController;

		Vector3 contactOrigin;
		Vector3 contactPoint = Vector3.zero;

		void Update ()
		{
			if (Controls.RelocationInputExists ()) {
				Ray ray = Camera.main.ScreenPointToRay (Controls.RelocationInput ());
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, 100)) {
					contactOrigin = ray.origin;
					contactPoint = hit.point;
				}
			}
			Debug.DrawLine (contactOrigin, contactPoint, Color.red);

			float distanceDelta = speed * Time.deltaTime;
			foreach (GameObject bot in GameObjectExtensions.BotObjects()) {
				Vector3 newPosition = contactPoint;
				newPosition.y = 0.5f;
				bool botSelected = selectionController.selectedBots.Contains (bot);
				if (botSelected && Vector3.Distance (newPosition, bot.transform.position) > relocationRadus) {
					bot.transform.position = Vector3.MoveTowards (bot.transform.position, newPosition, distanceDelta);
				}

				// Before looking for liftables, clear the last available one
				selectionController.ClearLiftableForBot (bot);

				foreach (GameObject liftable in GameObjectExtensions.LiftableObjects()) {
					float distance = Vector3.Distance (liftable.transform.position, bot.transform.position);

					// A liftable was available, but select this one if it's closer
					if (selectionController.availableLiftables.ContainsKey (bot)) { 
						float previousDistance = Vector3.Distance (bot.transform.position, selectionController.availableLiftables [bot].transform.position);
						if (distance < previousDistance) {
							selectionController.SetLiftableForBot (liftable, bot);
						}
					} 
					// No liftable was available, select this one if it's close enough
					else if (distance < liftableDetectionRadius) {
						selectionController.SetLiftableForBot (liftable, bot);
					}
				}
				if (selectionController.availableLiftables.ContainsKey (bot)) {
					Debug.DrawLine (bot.transform.position, selectionController.availableLiftables [bot].transform.position, Color.blue);
				}
			}
		}
	}
}