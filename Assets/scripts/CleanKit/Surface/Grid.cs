using UnityEngine;
using System.Collections;

namespace CleanKit
{
	public class Grid
	{
		public static float CellSize { get { return 1.0f; } }

		public static Vector3 ClosestIntersectingPoint (Vector3 point)
		{
			Vector3 cell = new Vector3 (
				               Mathf.Floor (point.x / CellSize),
				               Mathf.Floor (point.y / CellSize),
				               Mathf.Floor (point.z / CellSize));
			return cell;
		}
	}
}
