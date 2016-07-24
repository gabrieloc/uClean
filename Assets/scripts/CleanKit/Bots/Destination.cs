using UnityEngine;

namespace CleanKit
{
	public class Destination: MonoBehaviour
	{
		public bool Live;
		private float radius;

		public static Destination Instantiate (Vector3 point, Vector3 normal)
		{
			Vector3 relativeForward = Vector3.Cross (normal, Vector3.forward);
			Quaternion rotation = Quaternion.LookRotation (relativeForward, normal);
				
			GameObject gameObject = Instantiate (Resources.Load ("Destination"), point, rotation) as GameObject;
			Destination Destination = gameObject.GetComponent<Destination> ();
			return Destination;
		}

		public float Distance (Vector3 referencePoint)
		{
			return Vector3.Distance (transform.position, referencePoint);
		}

		void Update ()
		{
			Color color = Live ? Color.blue : Color.gray; 
			float liveRadius = radius > 2.0f ? 1.0f : radius + (2.0f * Time.deltaTime);
			radius = Live ? liveRadius : 2.0f;

			Debug.DrawLine (transform.TransformPoint (new Vector3 (-radius, 0, 0)), transform.TransformPoint (new Vector3 (radius, 0, 0)), color);
			Debug.DrawLine (transform.TransformPoint (new Vector3 (0, 0, -radius)), transform.TransformPoint (new Vector3 (0, 0, radius)), color);
		}
	}
}

