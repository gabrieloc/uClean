using UnityEngine;

namespace CleanKit
{
	public class Destination: MonoBehaviour
	{
		public bool Live;
		private float radius;

		public InteractableGhost ghost;

		public static Destination Instantiate (Vector3 point, Vector3 normal, InteractableGhost ghost = null)
		{
			Vector3 relativeForward = Vector3.Cross (normal, Vector3.forward);
			Quaternion rotation = Quaternion.LookRotation (relativeForward, normal);
				
			GameObject gameObject = Instantiate (Resources.Load ("Room/Destination"), point, rotation) as GameObject;
			Destination destination = gameObject.GetComponent<Destination> ();

			if (ghost != null) {
				destination.ghost = ghost;
				ghost.transform.SetParent (destination.transform);
			}

			return destination;
		}

		public float Distance (Vector3 referencePoint)
		{
			return Vector3.Distance (transform.position, referencePoint);
		}

		public bool ShouldRelocate (Transform compareTransform)
		{
			// TODO consider comparing rotation too
			bool d = Distance (compareTransform.position) > 0.1f;
			return d;
		}

		void Update ()
		{
			Color color = Live ? Color.blue : Color.gray; 
			float liveRadius = radius > 2.0f ? 1.0f : radius + (2.0f * Time.deltaTime);
			radius = Live ? liveRadius : 2.0f;

			Debug.DrawLine (transform.TransformPoint (new Vector3 (-radius, 0, 0)), transform.TransformPoint (new Vector3 (radius, 0, 0)), color);
			Debug.DrawLine (transform.TransformPoint (new Vector3 (0, 0, -radius)), transform.TransformPoint (new Vector3 (0, 0, radius)), color);
		}
			
		// Ghosts

		public bool IsGhostVisible ()
		{
			return ghost.gameObject.activeInHierarchy;
		}

		public void SetGhostVisible (bool visible, bool highlighted = false)
		{
			ghost.gameObject.SetActive (visible);
			ghost.SetHighlighted (highlighted);
		}

		public bool IsGhostPositionValid ()
		{
			return ghost.CollisionWithInteractables == false;
		}
	}
}

