using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CleanKit
{
	public partial class Bot : MonoBehaviour, Actor
	{
		Destination destination;

		public ActorDelegate actorDelegate;

		public static int LayerMask { get { return 1 << UnityEngine.LayerMask.NameToLayer ("Actor"); } }

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
			Vector3 position = new Vector3 (Random.Range (-5, 5), 5, Random.Range (-5, 5));
			GameObject gameObject = Instantiate (Resources.Load ("Room/Bot"), position, new Quaternion ()) as GameObject;
			gameObject.SetSelected (false);
			gameObject.name = BotNamer.New ();

			float minScale = 0.1f;
			float maxScale = 1;
			Vector3 scale = new Vector3 (
				                Random.Range (minScale, maxScale),
				                Random.Range (minScale, maxScale), 
				                Random.Range (minScale, maxScale));
			gameObject.transform.localScale = scale;

			NavMeshAgent agent = gameObject.GetComponent<NavMeshAgent> ();
			agent.speed = 5.0f / scale.magnitude;

			Bot bot = gameObject.GetComponent<Bot> ();
			bot.createCell ();
			return bot;
		}

		void Start ()
		{
			agent = GetComponent<NavMeshAgent> ();
		}

		void Update ()
		{
			if (instruction != null) {
				if (canPerformInteraction ()) {
					performInteraction ();
				} else {
					prepareForInteraction ();
				}
			} else if (destination != null) {
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

		void moveTowardsPoint (Vector3 point)
		{
			lookAtPoint (point);
			agent.SetDestination (point);
		}

		private void moveTowardsRelocationPoint ()
		{
			moveTowardsPoint (destination.transform.position);
		}

		void lookAtPoint (Vector3 point)
		{
			Vector3 lookPoint = point - transform.position;
			if (lookPoint.magnitude < 1.0f) {
				return;
			}
			Quaternion rotation = Quaternion.LookRotation (lookPoint);
			transform.rotation = Quaternion.Lerp (transform.rotation, rotation, Time.deltaTime * 5.0f);
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

		// is this still useful? can be calculated based off employment status
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
