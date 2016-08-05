using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	[System.Serializable]
	public class RoomInfo
	{
		public List<PropInfo> props { get; private set; }

		public static RoomInfo CreatePlayground ()
		{
			string config = "test_props";
			TextAsset json = Resources.Load<TextAsset> ("Data/" + config);
			RoomInfo roomInfo = JsonUtility.FromJson<RoomInfo> (json.text);
			return roomInfo;
		}
	}
}
