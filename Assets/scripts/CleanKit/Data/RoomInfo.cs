using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	[System.Serializable]
	public class RoomInfo
	{
		public string test;
		public List<PropInfo> props;

		public static RoomInfo RoomNamed (string name)
		{
			TextAsset jsonAsset = Resources.Load<TextAsset> ("Data/" + name);
			string json = jsonAsset.text;
			RoomInfo roomInfo = JsonUtility.FromJson<RoomInfo> (json);
			return roomInfo;
		}
	}
}
