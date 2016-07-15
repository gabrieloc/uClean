using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CleanKit
{
	public partial class Bot : MonoBehaviour, Actor
	{
		public Destination destination;

		NavMeshAgent agent;

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
			GameObject gameObject = Instantiate (Resources.Load ("Bot"), new Vector3 (), new Quaternion ()) as GameObject;
			gameObject.SetSelected (false);
			gameObject.name = BotNamer.New ();

			Bot bot = gameObject.GetComponent<Bot> ();
			bot.createCell ();
			bot.SetInteraction (InteractionType.None);
			return bot;
		}

		void Start ()
		{
			agent = GetComponent<NavMeshAgent> ();
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
			agent.SetDestination (destination.transform.position);
		}

		public bool debugDirection = false;
		float distanceFromTarget;
		float speedMultiplier;

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
