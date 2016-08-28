using UnityEngine;
using System.Collections;

namespace CleanKit
{
	public class Grid
	{
		public static float CellSize { get { return 1.0f; } }

		public static Vector3 ClosestIntersectingPoint (Vector3 point)
		{
			Vector3 cell = new Vector3 ();
			for (int i = 0; i < 3; i++) {
				cell [i] = Mathf.Floor ((point [i] + CellSize * 0.5f) / CellSize);
			}
			return cell;
		}

		public static Vector3 ClosestIntersectingCell (Vector3 point)
		{
			Vector3 cell = new Vector3 ();
			for (int i = 0; i < 3; i++) {
				cell [i] = Mathf.Ceil (point [i] * CellSize);
			}
			return cell;
		}
	}
}
