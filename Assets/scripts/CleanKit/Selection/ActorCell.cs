using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CleanKit
{
	public class ActorCell:  MonoBehaviour
	{
		public static ActorCell Instantiate ()
		{
			GameObject gameObject = Instantiate (Resources.Load ("UI/ActorCell"), new Vector3 (), new Quaternion ()) as GameObject;
			gameObject.SetSelected (false);

			ActorCell cell = gameObject.GetComponent<ActorCell> ();
			cell.SetInteraction (InteractionType.None);
			return cell;
		}

		public void SetInteraction (InteractionType interaction)
		{
			GameObject interactionIndicator = transform.Find ("Interaction").gameObject;
			Text label = interactionIndicator.GetComponentInChildren<Text> ();
			label.text = interaction.Description ();
			interactionIndicator.SetActive (label.text.Length > 0);
		}

		public void SetName (string name)
		{
			gameObject.name = name + "-cell";
		}

		// Swarms

		int count = 0;

		private void setCount (int count)
		{
			Text label = gameObject.GetComponentInChildren<Text> ();
			label.text = count.ToString ();
			name = "Swarm (" + count + ")";
		}

		internal void IncrementCount ()
		{
			setCount (++count);
		}

		internal void DecrementCount ()
		{
			setCount (--count);
		}
	}
}
	