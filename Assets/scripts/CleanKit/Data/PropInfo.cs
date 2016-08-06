using UnityEngine;
using System.Collections;

namespace CleanKit
{
	[System.Serializable]
	public class PropInfo
	{
		public string name;
		public float x;
		public float y;
		public float z;

		public float rw;

		public Vector3 position { get { return new Vector3 (x, y, z); } }

		public Quaternion rotation { get { return new Quaternion (0, 1, 0, rw); } }

		public static PropInfo CreateFromJSON (string jsonString)
		{
			return JsonUtility.FromJson<PropInfo> (jsonString);
		}
	}
}
