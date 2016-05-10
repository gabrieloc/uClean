using UnityEngine;

public class BotController: MonoBehaviour
{
	public float speed = 15.0f;
	public float relocationRadus = 5.0f;
	public SelectionController selectionController;

	Vector3 contactOrigin;
	Vector3 contactPoint;


	void Update()
	{
		if (Controls.RelocationInputExists()) {
			Ray ray = Camera.main.ScreenPointToRay(Controls.RelocationInput());
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100)) {
				contactOrigin = ray.origin;
				contactPoint = hit.point;
			}
		}
		else if (contactPoint != null) {
			float distanceDelta = speed * Time.deltaTime;
			foreach(GameObject bot in selectionController.selectedBots) {
				Vector3 newPosition = contactPoint;
				newPosition.y = 0.5f;
				float distance = Vector3.Distance(newPosition, bot.transform.position);
				if (distance > relocationRadus) {
					bot.transform.position = Vector3.MoveTowards(bot.transform.position, newPosition, distanceDelta);
				}
			}
		}
		Debug.DrawLine(contactOrigin, contactPoint, Color.red);
	}
}