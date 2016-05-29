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
	}
}