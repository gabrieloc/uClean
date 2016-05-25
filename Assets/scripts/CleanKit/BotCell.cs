using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CleanKit
{
	// TODO move all this shit to a category or something more sane

	public class BotCell: MonoBehaviour
	{
		public static List<BotCell> AllObjects ()
		{
			List<BotCell> objects = new List<BotCell> ();
			foreach (BotCell cell in GameObject.FindObjectsOfType<BotCell> ()) {
				objects.Add (cell.GetComponent<BotCell> ());
			}
			objects.Reverse ();
			return objects;
		}

		public static BotCell Instantiate (Entity entity)
		{
			GameObject gameObject = Instantiate (Resources.Load ("BotCell"), new Vector3 (), new Quaternion ()) as GameObject;
			gameObject.SetSelected (false);
			BotCell cell = gameObject.GetComponent<BotCell> ();
			cell.name = entity.name + "-cell";
			return cell;
		}

		public void SetGroupCount (int count)
		{
			Text label = gameObject.GetComponentInChildren<Text> ();
			label.text = count.ToString ();
		}
	}
}

