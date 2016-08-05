using UnityEngine;
using System.Collections;

namespace CleanKit
{
	[System.Serializable]
	public class PropInfo
	{
		public string name;
		public Vector3 position;
		public Quaternion rotation;

		public static PropInfo CreateFromJSON (string jsonString)
		{
			return JsonUtility.FromJson<PropInfo> (jsonString);
		}
	}
}
