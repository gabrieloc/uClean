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
			} else if (shouldRelocate ()) {
				destination.Live = true;
				moveTowardsRelocationPoint ();
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

		private Vector3 velocity = Vector3.zero;
		public bool debugDirection = false;
		float distanceFromTarget;
		float speedMultiplier;

		private void moveTowardsRelocationPoint (float speed)
		{
			if (shouldRelocate () == false || ignoreRelocationPoint) {
				return;
			}

			Vector3 position = destination.transform.position;
			Debug.DrawLine (transform.position, position, Color.grey);
			position = CalculatePersonalSpace (position);
			position.y = Mathf.Max (0.0f, position.y);

			distanceFromTarget = Vector3.Distance (destination.transform.position, transform.position);
			float distanceMultiplier = distanceFromTarget / kRelocatableRadius;
			distanceMultiplier = Mathf.Min (1.0f, distanceMultiplier);
			speedMultiplier = Mathf.Pow (distanceMultiplier, 2.0f);
			float distanceDelta = speedMultiplier * speed * Time.deltaTime;
				
			transform.position = Vector3.MoveTowards (transform.position, position, distanceDelta);

			if (debugDirection) {
				Vector3 arrowHead = transform.TransformPoint (new Vector3 (0, 0, 2.0f));
				Debug.DrawLine (transform.position, arrowHead, Color.green);
				Debug.DrawLine (arrowHead, transform.TransformPoint (new Vector3 (-0.5f, 0, 1.5f)), Color.green);
				Debug.DrawLine (arrowHead, transform.TransformPoint (new Vector3 (0.5f, 0, 1.5f)), Color.green);	
			}

			if (shouldRelocate ()) {
				Quaternion rotation = new Quaternion ();

				// aligns bot with surface normals. buggy.
//				RaycastHit hit;
//				int layerMask = 1 << LayerMask.NameToLayer ("Surface");
//				Physics.Raycast (transform.position, Vector3.down, out hit, 100, layerMask);
//				rotation = Quaternion.FromToRotation (Vector3.up, hit.normal);

				Vector3 lookPosition = position - transform.position;
				rotation = Quaternion.LookRotation (lookPosition);
				rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * 20);
//				transform.rotation = rotation;
			} else {
				CancelRelocation ();
			}
		}

		Vector3 CalculatePersonalSpace (Vector3 position)
		{
			int layerMask = 1 << LayerMask.NameToLayer ("Actor");
			Collider[] hitColliders = Physics.OverlapSphere (transform.position, kPersonalSpaceRadius, layerMask);
			if (hitColliders.Length > 0) {
			
				List<Actor> opposingActors = new List<Actor> ();
				Vector3 opposingVector = new Vector3 ();
			
				int index = 0;
				while (index < hitColliders.Length) {
					Collider hitCollider = hitColliders [index];
					Bot actor = hitCollider.gameObject.GetComponentInParent<Bot> ();
			
					Vector3 hitPos = actor.gameObject.transform.position;
					opposingActors.Add (actor);
					if (actor.Equals (this) == false) {
						opposingVector += hitPos;
					}
					index++;
				}
			
				// Subtract 1 since this contains the current instance
				int actorCount = opposingActors.Count - 1;
								
				if (actorCount > 0) {
					opposingVector /= actorCount;
			
					Vector3 h1 = opposingVector - transform.position;
					float dis1 = h1.magnitude;
					Vector3 d1 = h1 / dis1 * -2.0f;
					d1 = transform.TransformPoint (d1.normalized);
					Debug.DrawLine (transform.position, d1, Color.red);

					Vector3 d2 = position - transform.position;
					d2 = transform.TransformPoint (d2.normalized);
					Debug.DrawLine (transform.position, d2, Color.black);

					Vector3 d3 = (d1 + d2) / 2.0f;
					d3 = transform.InverseTransformPoint (d3);
					d3 = transform.TransformPoint (d3.normalized);
					Debug.DrawLine (transform.position, d3, Color.green);

					return d3;
				}
			}

			return position;
		}

		void OnDrawGizmos ()
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere (transform.position, kPersonalSpaceRadius);
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

		private void moveTowardsInteractable ()
		{
			RaycastHit hit;
			Vector3 origin = interactable.transform.position;
			int layerMask = 1 << LayerMask.NameToLayer ("Surface");
			Physics.Raycast (origin, Vector3.down, out hit, 100, layerMask);

			Vector3 position = hit.point;
			Debug.DrawLine (origin, hit.point, Color.green);

			float magnitude = Vector3.Distance (transform.position, position);
			bool canInteract = magnitude < kMinimumInteractableDistance;
			ignoreRelocationPoint = !canInteract;

			if (canInteract) {
				return;
			}

			float distanceDelta = kRelocationSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, position, distanceDelta);
			transform.LookAt (new Vector3 (position.x, transform.position.y, position.y));
		}

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
