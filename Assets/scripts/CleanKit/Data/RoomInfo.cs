using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	[System.Serializable]
	public class RoomInfo
	{
		public string test;
		public List<PropInfo> props;

		public static RoomInfo CreatePlayground ()
		{
			TextAsset jsonAsset = Resources.Load<TextAsset> ("Data/test_props");
			string json = jsonAsset.text;
			RoomInfo roomInfo = JsonUtility.FromJson<RoomInfo> (json);
			return roomInfo;
		}
	}
}
