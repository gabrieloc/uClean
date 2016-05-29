using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CleanKit
{
	public class SwarmCell: MonoBehaviour
	{
		public static SwarmCell Instantiate ()
		{
			GameObject gameObject = Instantiate (Resources.Load ("SwarmCell"), new Vector3 (), new Quaternion ()) as GameObject;
			gameObject.SetSelected (false);

			SwarmCell cell = gameObject.GetComponent<SwarmCell> ();
			cell.setCount (0); 
			return cell;
		}

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

		public static List<SwarmCell> AllObjects ()
		{
			List<SwarmCell> objects = new List<SwarmCell> ();
			foreach (SwarmCell cell in GameObject.FindObjectsOfType<SwarmCell> ()) {
				objects.Add (cell.GetComponent<SwarmCell> ());
			}
			objects.Reverse ();
			return objects;
		}
	}
}

