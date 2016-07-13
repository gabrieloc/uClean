using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CleanKit
{
	public partial class Bot : MonoBehaviour, Actor
	{
		public Destination destination;

		public bool selected { get; private set; }

		public void SetSelected (bool selected)
		{
			this.selected = selected;
			gameObject.SetSelected (selected);
			if (cell) {
				cell.gameObject.SetSelected (selected);
			}
		}

		public float kRelocatableRadius = 2.0f;
		public float kRelocationSpeed = 20.0f;
		public float kPersonalSpaceRadius = 2.0f;
		public float kRelocationDamping = 20.0f;

		public static Bot Instantiate ()
		{
			GameObject gameObject = Instantiate (Resources.Load ("Bot"), new Vector3 (0, 10, 0), new Quaternion ()) as GameObject;
			gameObject.SetSelected (false);
			gameObject.name = BotNamer.New ();

			Bot bot = gameObject.GetComponent<Bot> ();
			bot.createCell ();
			bot.SetInteraction (InteractionType.None);
			return bot;
		}

		void Update ()
		{
			if (interactable != null) {
				if (canPerformInteraction ()) {
					performInteraction ();
				} else {
					prepareForInteraction ();
				}
			} else {
				if (shouldRelocate ()) {
					destination.Live = true;
					moveTowardsRelocationPoint ();
				} else {
					CancelRelocation ();
				}
			}
		}

		public string Name ()
		{
			return gameObject.name;
		}

		private void moveTowardsRelocationPoint ()
		{
			moveTowardsRelocationPoint (kRelocationSpeed);
		}

		public bool debugDirection = false;
		float distanceFromTarget;
		float speedMultiplier;

		private void moveTowardsRelocationPoint (float speed)
		{
			Debug.DrawLine (transform.position, destination.transform.position, Color.grey);
			Vector3 point = CalculatePersonalSpace (transform.position, destination.transform.position);
			point.y = Mathf.Max (0.0f, point.y);
			lookAtPoint (point);

			distanceFromTarget = Vector3.Distance (destination.transform.position, transform.position);
			float distanceMultiplier = distanceFromTarget / kRelocatableRadius;
			distanceMultiplier = Mathf.Min (1.0f, distanceMultiplier);
			speedMultiplier = Mathf.Pow (distanceMultiplier, 1.5f);
			float distanceDelta = speedMultiplier * speed * Time.deltaTime;
				
			transform.position = Vector3.MoveTowards (transform.position, point, distanceDelta);
		}


		void lookAtPoint (Vector3 point)
		{
			Quaternion rotation = new Quaternion ();

			RaycastHit hit;
			Physics.Raycast (transform.position, Vector3.down, out hit, 1.0f);
//			Debug.DrawRay (hit.point, hit.normal, Color.cyan);

			// TODO: figure out how to align with surface normal
			Vector3 lookPosition = point - transform.position;
			lookPosition.y = 0.0f;
			rotation = Quaternion.LookRotation (lookPosition, hit.normal);
			rotation = Quaternion.Slerp (rotation, transform.rotation, Time.deltaTime * 20.0f);
			transform.rotation = rotation;

			if (debugDirection) {
				Vector3 arrowHead = transform.TransformPoint (new Vector3 (0, 0, 2.0f));
				Debug.DrawLine (transform.position, arrowHead, Color.green);
				Debug.DrawLine (arrowHead, transform.TransformPoint (new Vector3 (-0.5f, 0, 1.5f)), Color.green);
				Debug.DrawLine (arrowHead, transform.TransformPoint (new Vector3 (0.5f, 0, 1.5f)), Color.green);	
			}
		}

		Vector3 CalculatePersonalSpace (Vector3 current, Vector3 desired)
		{
			int actorLayer = 1 << LayerMask.NameToLayer ("Actor");
			int interactableLayer = 1 << LayerMask.NameToLayer ("Interactable");
			int layerMask = actorLayer | interactableLayer;

			Collider[] hits = Physics.OverlapSphere (current, kPersonalSpaceRadius, layerMask);
			if (hits.Length > 0) {
				List<Vector3> opposers = new List<Vector3> ();
				int index = 0;
				while (index < hits.Length) {
					Collider collider = hits [index];
					if (collider.transform.parent.gameObject != gameObject) {
						Vector3 cloPos = Prioritize (collider.ClosestPointOnBounds (current), 1.2f);
						Vector3 cenPos = current + Direction (current, collider.transform.position) * 2.0f;

//						Debug.DrawLine (current, cenPos, Color.cyan);
//						Debug.DrawLine (current, cloPos, Color.cyan);

						Vector3 dDir = Vector3.Lerp (cenPos, cloPos, 0.5f);
						opposers.Add (dDir);
						Debug.DrawLine (current, dDir, Color.blue);
					}
					index++;
				}

				if (opposers.Count > 0) {

					Vector3 opposingVector = new Vector3 ();
					foreach (Vector3 hit in opposers) {
//						Vector3 b = Prioritize (hit, 1.6f);
//						opposingVector += b;
						opposingVector += hit;
					}
					opposingVector /= opposers.Count;
					opposingVector.y = 0.0f; // TODO fuck everything about this

					Vector3 d = Prioritize (desired, 3.0f);
					Vector3 dl = transform.InverseTransformPoint (d);

					Vector3 o = transform.InverseTransformPoint (opposingVector);
					o *= -1.0f;
					Vector3 ol = o;
					o = transform.TransformPoint (o);
					Debug.DrawLine (current, o, Color.red);

					float bd = Vector3.Distance (desired, current);
					float od = Vector3.Distance (desired, o);


					if (dl.magnitude > ol.magnitude) { // We're close enough!
						return current;
					} else if (bd < od) { // Let's go around the obstruction
						Vector3 p1 = Normalize (desired);

						float opposingInfluence = 0.5f;
						Vector3 p2 = Vector3.Lerp (o, d, opposingInfluence); 
						Debug.DrawLine (current, p2, Color.yellow);

						// TODO carve out case for reversing if stuck in a corner
//						Vector3 d1i = Normalize (p1, true);
//						float d1im = Vector3.Distance (d1i, p2);
//						if (d1im < 0.5f) {
//							print (d1im);
//							Vector3 f2 = Vector3.Lerp (transform.TransformPoint (Vector3.back), p2, 0.0f);
//							Debug.DrawLine (current, f2, Color.green);
////							return f2;
//						}

						Vector3 f = Vector3.Lerp (p1, p2, 0.5f);
						f = Normalize (f);
						Debug.DrawLine (current, f, Color.green);
						return f;
					}
				} else {
					Debug.DrawLine (current, Normalize (desired), Color.green);
				}
			}

			return Normalize (desired);
		}

		Vector3 Prioritize (Vector3 point, float radius = 2.0f)
		{
			Vector3 p = transform.InverseTransformPoint (point);
			float distance = Vector3.Distance (point, transform.position);
			float multiplier = Mathf.Pow (radius / distance/* * 0.2f*/, 4.0f);
			p = transform.TransformPoint (p * multiplier);
			return p;
		}

		Vector3 Normalize (Vector3 point, bool invert = false)
		{
			float multiplier = invert ? -1.0f : 1.0f;
			Vector3 p = transform.InverseTransformPoint (point) * multiplier;
			p = transform.TransformPoint (p.normalized);
			return p;
		}

		Vector3 Direction (Vector3 origin, Vector3 destination)
		{
			return	(destination - origin).normalized;	
		}

		public bool kDebugPersonalSpace = false;

		void OnDrawGizmos ()
		{
			if (kDebugPersonalSpace) {
				Gizmos.color = Color.magenta;
				Gizmos.DrawWireSphere (transform.position, kPersonalSpaceRadius);
			}
		}

		private int movementPriorityAmoungActors (List<Actor> actors)
		{
			List<Actor> sortedActors = actors.ToList ();
			sortedActors.Sort ((a1, a2) => a1.DistanceFromDestination ().CompareTo (a2.DistanceFromDestination ()));
			return sortedActors.IndexOf (GetComponentInParent<Actor> ());
		}

		public float DistanceFromDestination ()
		{
			return destination.Distance (transform.position);
		}

		private bool ignoreRelocationPoint;
		private float kMinimumInteractableDistance = 1.0f;

		// Cells
		public ActorCell cell { get; private set; }

		private void createCell ()
		{
			cell = ActorCell.Instantiate ();
			cell.SetName (gameObject.name);
		}

		private void destroyCell ()
		{
			GameObject.Destroy (cell.gameObject);
			cell = null;
		}

		// Swarms

		public Swarm swarm { get; private set; }

		public bool belongsToSwarm {
			get {
				return swarm != null;
			}
		}

		public void JoinSwarm (Swarm swarm)
		{
			destroyCell ();
			this.swarm = swarm;
		}

		public void LeaveSwarm ()
		{
			createCell ();
			this.swarm = null;
		}
	}
}
