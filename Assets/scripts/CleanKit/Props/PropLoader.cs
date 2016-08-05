using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public static class PropLoader
	{
		public static GameObject CreateTestProp ()
		{
			GameObject testProp = GameObject.Instantiate (Resources.Load ("TestProp"), new Vector3 (), new Quaternion ()) as GameObject;
			return testProp;
		}

		public static GameObject CreateProp (PropInfo propInfo)
		{
			GameObject resource = Resources.Load<GameObject> (propInfo.name);
			GameObject prop = GameObject.Instantiate (resource, propInfo.position, propInfo.rotation) as GameObject;
			return prop;
		}
	}
}