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
			GameObject resource = Resources.Load<GameObject> ("Props/" + propInfo.name);
			Vector3 position = propInfo.position;
			Quaternion rotation = propInfo.rotation;
			GameObject prop = GameObject.Instantiate (resource, position, rotation) as GameObject;
			prop.transform.localEulerAngles = new Vector3 (-90.0f, 0.0f, 0.0f);

			prop.AddComponent<Interactable> ();
			prop.AddComponent<Rigidbody> ();
			prop.AddComponent<NavMeshObstacle> ();

			MeshCollider collider = prop.AddComponent<MeshCollider> ();
			collider.convex = true;

			prop.name = resource.name;

			return prop;
		}
	}
}