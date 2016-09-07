using UnityEngine;
using System.Collections;

namespace CleanKit
{
	[System.Serializable]
	public class PropInfo
	{
		public string name;
		public int[] initial;
		public int[] position;

		public float rw;

		public Vector3 Initial { get { return arrayToVector3 (initial); } }

		public Vector3 Position { get { return arrayToVector3 (position); } }

		public Quaternion Rotation { get { return new Quaternion (0, 1, 0, rw); } }

		public static PropInfo CreateFromJSON (string jsonString)
		{
			return JsonUtility.FromJson<PropInfo> (jsonString);
		}

		static Vector3 arrayToVector3 (int[] array)
		{
			return new Vector3 (array [0], array [1], array [2]);
		}
	}
}
