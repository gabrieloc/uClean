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
			moveTowardsRelocationPoint (kRelocationSpeed * Time.deltaTime);
		}

		private void moveTowardsRelocationPoint (float distanceDelta)
		{
			if (shouldRelocate () == false || ignoreRelocationPoint) {
				return;
			}

			Vector3 position = destination.transform.position;
			position = CalculatePersonalSpace (position);
			Debug.DrawLine (transform.position, position, Color.grey);

			transform.position = Vector3.MoveTowards (transform.position, position, distanceDelta);

			Vector3 arrowHead = transform.TransformPoint (new Vector3 (0, 0, 2.0f));
			Debug.DrawLine (transform.position, arrowHead, Color.green);
			Debug.DrawLine (arrowHead, transform.TransformPoint (new Vector3 (-0.5f, 0, 1.5f)), Color.green);
			Debug.DrawLine (arrowHead, transform.TransformPoint (new Vector3 (0.5f, 0, 1.5f)), Color.green);

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
				transform.rotation = rotation;
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
					Actor actor = hitCollider.gameObject.GetComponentInParent<Actor> ();
			
					Vector3 hitPos = hitCollider.transform.position;
					opposingActors.Add (actor);
					if (hitPos.Equals (transform.position) == false) {
						opposingVector += hitPos;
					}
					index++;
				}
			
				// Subtract 1 since this contains "me"
				int actorCount = opposingActors.Count - 1;
								
				if (actorCount > 0) {
					opposingVector /= actorCount;
			
					Vector3 heading = opposingVector - transform.position;
					float distance = heading.magnitude;
					Vector3 direction = heading / distance * -2.0f;
			
//					int rank = movementPriorityAmoungActors (opposingActors);
//					float priority = rank / (float)opposingActors.Count;
//					direction *= priority * 4.0f;
			
					Vector3 directionLocal = transform.TransformPoint (direction);
					Debug.DrawLine (transform.position, directionLocal, Color.red);
			
					return (position + directionLocal) / 2.0f;
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
